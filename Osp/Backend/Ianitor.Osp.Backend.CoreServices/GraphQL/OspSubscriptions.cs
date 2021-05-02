using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Subscription;
using GraphQL.Types;
using Ianitor.Osp.Backend.CoreServices.GraphQL.Types;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;

namespace Ianitor.Osp.Backend.CoreServices.GraphQL
{
    /// <summary>
    /// Implements GraphQL subscriptions
    /// </summary>
    public class OspSubscriptions : ObjectGraphType<object>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rtEntityDtoTypes">RT Entity types subscriptions are created.</param>
        public OspSubscriptions(IEnumerable<RtEntityDtoType> rtEntityDtoTypes)
        {
            foreach (var rtEntityDtoType in rtEntityDtoTypes)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                AddField(new EventStreamFieldType
                {
                    Name = $"{rtEntityDtoType.Name}Events",
                    Arguments = new QueryArguments(
                        new QueryArgument<OspObjectIdType> {Name = Statics.RtIdArg},
                        new QueryArgument<NonNullGraphType<ListGraphType<UpdateTypesDtoType>>>
                            {Name = Statics.UpdateTypesArg}
                    ),
                    ResolvedType =
                        new DynamicUpdateMessageDtoType<RtEntityUpdateItemDto>(
                            new RtEntityUpdateItemDtoType(rtEntityDtoType)),
                    Resolver = new FuncFieldResolver<DynamicUpdateMessageDto<RtEntityUpdateItemDto>>(ResolveMessage),
                    Subscriber = new EventStreamResolver<object>(Subscribe)
                }).AddMetadata(Statics.CkId, rtEntityDtoType.CkId);
            }
        }

        private IObservable<object> Subscribe(IResolveEventStreamContext context)
        {
            var messageHandlingContext = (MessageHandlingContext) context.UserContext;


            var tenantContext = messageHandlingContext.Get<ITenantContext>(Statics.TenantContext);

            var ckId = context.FieldDefinition.GetMetadata<string>(Statics.CkId);
            var rtId = context.GetArgument<OspObjectId?>(Statics.RtIdArg, null);

            var updateTypeDtoList = context.GetArgument<ICollection<UpdateTypesDto>>(Statics.UpdateTypesArg);
            UpdateTypes updateType = UpdateTypes.Undefined;
            foreach (var updateTypeDto in updateTypeDtoList)
            {
                updateType |= (UpdateTypes) updateTypeDto;
            }

            var updateStreamFilter = new UpdateStreamFilter
            {
                UpdateTypes = updateType,
                RtId = rtId
            };

            var messages = tenantContext.Repository.SubscribeToRtEntities(ckId, updateStreamFilter, context.CancellationToken);

            var observable = messages.GetUpdates().Select(x => new DynamicUpdateMessageDto<RtEntityUpdateItemDto>
            {
                Items = new List<RtEntityUpdateItemDto>
                {
                    new RtEntityUpdateItemDto
                        {UserContext = x.Document, UpdateState = (UpdateTypesDto) x.UpdateType}
                }
            });

            return observable;
        }

        private DynamicUpdateMessageDto<RtEntityUpdateItemDto> ResolveMessage(IResolveFieldContext context)
        {
            var message = context.Source as DynamicUpdateMessageDto<RtEntityUpdateItemDto>;

            return message;
        }
    }
}