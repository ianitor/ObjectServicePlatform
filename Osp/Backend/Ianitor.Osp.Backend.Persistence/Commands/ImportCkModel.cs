using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Backend.Persistence.DataAccess.Internal;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Common.Shared.Exchange;
using NLog;
using AttributeValueTypes = Ianitor.Osp.Backend.Persistence.DatabaseEntities.AttributeValueTypes;
using CkAttribute = Ianitor.Osp.Backend.Persistence.DatabaseEntities.CkAttribute;
using CkEntity = Ianitor.Osp.Backend.Persistence.DatabaseEntities.CkEntity;
using CkEntityAssociation = Ianitor.Osp.Backend.Persistence.DatabaseEntities.CkEntityAssociation;
using CkEntityAttribute = Ianitor.Osp.Backend.Persistence.DatabaseEntities.CkEntityAttribute;
using CkIndexFields = Ianitor.Osp.Backend.Persistence.DatabaseEntities.CkIndexFields;
using Multiplicities = Ianitor.Osp.Backend.Persistence.DatabaseEntities.Multiplicities;
using ScopeIds = Ianitor.Osp.Backend.Persistence.DatabaseEntities.ScopeIds;
using CkSelectionValue = Ianitor.Osp.Backend.Persistence.DatabaseEntities.CkSelectionValue;

namespace Ianitor.Osp.Backend.Persistence.Commands
{
    internal class ImportCkModel
    {
        private readonly IDatabaseContext _databaseContext;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly CkModelValidation _ckModelValidation;
        private readonly TransientCkModel _transientCkModel;

        public ImportCkModel(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
            _ckModelValidation = new CkModelValidation(databaseContext);
            _transientCkModel = new TransientCkModel();
        }

        public async Task ImportText(IOspSession session, string jsonText, ScopeIds scopeId, CancellationToken? cancellationToken = null)
        {
            Logger.Info("Reading CK model....");
            var model = CkSerializer.Deserialize(jsonText);

            await ExecuteImport(session, model, scopeId, cancellationToken);
        }

        public async Task Import(IOspSession session, string filePath, ScopeIds scopeId, CancellationToken? cancellationToken = null)
        {
            Logger.Info("Reading CK model....");
            CkModelRoot model;
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                model = CkSerializer.Deserialize(streamReader);
            }

            Logger.Info("Executing import of CK model....");

            await ExecuteImport(session, model, scopeId, cancellationToken);
            
            Logger.Info("Import of CK model completed.");

        }

        private async Task ExecuteImport(IOspSession session, CkModelRoot model, ScopeIds scopeId, CancellationToken? cancellationToken)
        {
            // Transform to entities
            ProcessCkAttributes(model, scopeId);
            if (CheckCancellation(cancellationToken)) return;

            ProcessCkEntitiesAndAssociations(model, scopeId);
            if (CheckCancellation(cancellationToken)) return;
            
            // ValidateAsync
            await _ckModelValidation.Validate(session, _transientCkModel, scopeId, cancellationToken);

            // Delete the old version
            if (await DeleteOldVersion(session, scopeId, cancellationToken)) return;
            if (CheckCancellation(cancellationToken)) return;

            // ValidateAsync the Model
            if (_transientCkModel.CkAttributes.Any())
            {
                ValidateAndThrow(
                    await _databaseContext.CkAttributes.BulkImportAsync(session,
                        _transientCkModel.CkAttributes.ToArray()));
                if (CheckCancellation(cancellationToken)) return;
            }

            if (_transientCkModel.CkEntities.Any())
            {
                ValidateAndThrow(
                    await _databaseContext.CkEntities.BulkImportAsync(session, _transientCkModel.CkEntities.ToArray()));
                if (CheckCancellation(cancellationToken)) return;
            }

            if (_transientCkModel.CkEntityAssociations.Any())
            {
                ValidateAndThrow(
                    await _databaseContext.CkEntityAssociations.BulkImportAsync(session,
                        _transientCkModel.CkEntityAssociations));
                if (CheckCancellation(cancellationToken)) return;
            }

            if (_transientCkModel.CkEntityInheritances.Any())
            {
                ValidateAndThrow(
                    await _databaseContext.CkEntityInheritances.BulkImportAsync(session,
                        _transientCkModel.CkEntityInheritances));
                if (CheckCancellation(cancellationToken)) return;
            }

            await CreateCollections(session);
            if (CheckCancellation(cancellationToken)) return;
            
            await CreateIndex(session);
        }

        private async Task<bool> DeleteOldVersion(IOspSession session, ScopeIds scopeId, CancellationToken? cancellationToken)
        {
            foreach (var ckAttribute in await _databaseContext.CkAttributes
                .FindManyAsync(session, x => x.ScopeId == scopeId))
            {
                await _databaseContext.CkAttributes.DeleteOneAsync(session, ckAttribute.AttributeId);
            }

            if (CheckCancellation(cancellationToken)) return true;

            foreach (var ckEntity in await _databaseContext.CkEntities
                .FindManyAsync(session, x => x.ScopeId == scopeId))
            {
                await _databaseContext.CkEntities.DeleteOneAsync(session, ckEntity.CkId);
            }

            if (CheckCancellation(cancellationToken)) return true;

            foreach (var ckEntityAssociation in await _databaseContext.CkEntityAssociations
                .FindManyAsync(session, x => x.ScopeId == scopeId))
            {
                await _databaseContext.CkEntityAssociations.DeleteOneAsync(session, ckEntityAssociation.AssociationId);
            }

            if (CheckCancellation(cancellationToken)) return true;

            foreach (var ckEntityInheritance in await _databaseContext.CkEntityInheritances
                .FindManyAsync(session, x => x.ScopeId == scopeId))
            {
                await _databaseContext.CkEntityInheritances.DeleteOneAsync(session, ckEntityInheritance.InheritanceId);
            }

            if (CheckCancellation(cancellationToken)) return true;
            return false;
        }

        private async Task CreateCollections(IOspSession session)
        {
            await _databaseContext.UpdateCollectionsAsync(session);
        }

        private async Task CreateIndex(IOspSession session)
        {
            await _databaseContext.UpdateIndexAsync(session);
        }

        private static bool CheckCancellation(CancellationToken? cancellationToken)
        {
            if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
            {
                return true;
            }

            return false;
        }

        private void ProcessCkAttributes(CkModelRoot model, ScopeIds scopeId)
        {
            foreach (var modelCkAttribute in model.CkAttributes)
            {
                var ckAttribute = new CkAttribute
                {
                    AttributeId = modelCkAttribute.AttributeId,
                    ScopeId = scopeId,
                    AttributeValueType = (AttributeValueTypes) modelCkAttribute.ValueType,
                    SelectionValues = modelCkAttribute.SelectionValues?.Select(sv => new CkSelectionValue
                        {Key = sv.Key, Name = sv.Name}).ToList(),
                    DefaultValue = modelCkAttribute.DefaultValue,
                    DefaultValues = modelCkAttribute.DefaultValues
                };
                _transientCkModel.CkAttributes.Add(ckAttribute);
            }
        }

        private void ProcessCkEntitiesAndAssociations(CkModelRoot model, ScopeIds scopeId)
        {
            var associationRoleDefinitions = model.CkAssociationRoles.ToDictionary(k => k.RoleId, v => v);

            foreach (var entity in model.CkEntities)
            {
                List<CkEntityAttribute> ckEntityAttributes = new List<CkEntityAttribute>();
                foreach (var attribute in entity.Attributes)
                {
                    var ckEntityAttribute = new CkEntityAttribute
                    {
                        AttributeId = attribute.AttributeId,
                        AttributeName = attribute.AttributeName,
                        AutoCompleteFilter = attribute.AutoCompleteFilter,
                        AutoCompleteLimit = attribute.AutoCompleteLimit,
                        IsAutoCompleteEnabled = attribute.IsAutoCompleteEnabled,
                        AutoIncrementReference = attribute.AutoIncrementReference
                    };

                    ckEntityAttributes.Add(ckEntityAttribute);
                }

                List<CkEntityIndex> textSearchDefinitions = new List<CkEntityIndex>();
                foreach (var entityIndexDto in entity.Indexes)
                {
                    var entityIndex = new CkEntityIndex
                    {
                        IndexType = (IndexTypes) entityIndexDto.IndexType,
                        Language = entityIndexDto.Language,
                        Fields = entityIndexDto.Fields
                            .Select(x => new CkIndexFields {Weight = x.Weight, AttributeNames = x.AttributeNames})
                            .ToList()
                    };

                    textSearchDefinitions.Add(entityIndex);
                }


                var ckEntity = new CkEntity
                {
                    CkId = entity.CkId,
                    ScopeId = scopeId,
                    IsFinal = entity.IsFinal,
                    IsAbstract = entity.IsAbstract,
                    Attributes = ckEntityAttributes,
                    Indexes = textSearchDefinitions
                };

                if (!string.IsNullOrWhiteSpace(entity.CkDerivedId))
                {
                    var ckEntityInheritance = new CkEntityInheritance
                    {
                        ScopeId = scopeId,
                        OriginCkId = entity.CkDerivedId,
                        TargetCkId = entity.CkId,
                    };
                    _transientCkModel.CkEntityInheritances.Add(ckEntityInheritance);
                }

                foreach (var association in entity.Associations)
                {
                    var ckEntityAssociation = new CkEntityAssociation
                    {
                        RoleId = association.RoleId,
                        ScopeId = scopeId,
                        InboundMultiplicity = (Multiplicities) association.InboundMultiplicity,
                        OutboundMultiplicity = (Multiplicities) association.OutboundMultiplicity,
                        OriginCkId = ckEntity.CkId,
                        TargetCkId = association.TargetCkId,
                        InboundName = associationRoleDefinitions[association.RoleId].InboundName,
                        OutboundName = associationRoleDefinitions[association.RoleId].OutboundName
                    };
                    _transientCkModel.CkEntityAssociations.Add(ckEntityAssociation);
                }

                _transientCkModel.CkEntities.Add(ckEntity);
            }
        }

        private void ValidateAndThrow(BulkImportResult bulkImportResult)
        {
            if (bulkImportResult.HasError())
            {
                throw new OperationFailedException(
                    $"Write operation was not acknowledged by database.");
            }
        }
    }
}