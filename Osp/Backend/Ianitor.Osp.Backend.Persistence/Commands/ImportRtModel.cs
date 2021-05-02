﻿using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Common.Shared.Exchange;
using MongoDB.Bson;
using RtAssociation = Ianitor.Osp.Backend.Persistence.DatabaseEntities.RtAssociation;
using RtEntity = Ianitor.Osp.Backend.Persistence.DatabaseEntities.RtEntity;

namespace Ianitor.Osp.Backend.Persistence.Commands
{
    internal class ImportRtModel
    {
        private readonly ITenantContextInternal _tenantContext;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly HashSet<ObjectId> _entityImportIds;
        private int _associationsCount;

        private readonly ConcurrentQueue<RtEntity> _importEntityQueue;
        private readonly ConcurrentQueue<RtAssociation> _importAssociationQueue;

        private int _entityProgressCount;
        private const int Max = 5000;
        
        public ImportRtModel(ITenantContextInternal tenantContext)
        {
            _tenantContext = tenantContext;

            _entityImportIds = new HashSet<ObjectId>();
            _importEntityQueue = new ConcurrentQueue<RtEntity>();
            _importAssociationQueue = new ConcurrentQueue<RtAssociation>();
        }

        public async Task ImportText(IOspSession session, string jsonText, CancellationToken? cancellationToken = null)
        {
            Logger.Info("Importing RT entities using text started.");
            
            using (var stream = new StreamReader(jsonText))
            {
                await RtSerializer.DeserializeAsync(stream, x=> ImportEntity(session, x), cancellationToken);
            }
            
            // Finish the last entities
            await ImportToDatabase(session);

            Logger.Info($"{_entityImportIds.Count} entities, {_associationsCount} associations imported.");
        }

        public async Task Import(IOspSession session, string filePath, CancellationToken? cancellationToken = null)
        {
            Logger.Info("Importing RT entities using file started.");
            
            using (var stream = File.OpenText(filePath))
            {
                await RtSerializer.DeserializeAsync(stream, x=> ImportEntity(session, x), cancellationToken);
            }

            // Finish the last entities
            await ImportToDatabase(session);

            Logger.Info($"{_entityImportIds.Count} entities, {_associationsCount} associations imported.");
        }

        private async Task ImportEntity(IOspSession session, Common.Shared.Exchange.RtEntity modelRtEntity)
        {
            var progress = Interlocked.Increment(ref _entityProgressCount);
            if (progress > Max)
            {
                Logger.Info($"{_importEntityQueue.Count} entities (total imports of {_entityImportIds.Count}) imported.");
                Interlocked.Exchange(ref _entityProgressCount, 1);

                await ImportToDatabase(session);
            }

            var entityCacheItem = _tenantContext.CkCache.GetEntityCacheItem(modelRtEntity.CkId);

            var rtEntity = _tenantContext.Repository.CreateTransientRtEntity(modelRtEntity.CkId);
            rtEntity.RtId = modelRtEntity.Id.ToObjectId();
            rtEntity.WellKnownName = modelRtEntity.WellKnownName;

            if (_entityImportIds.Contains(rtEntity.RtId))
            {
                Logger.Error($"{rtEntity.RtId} already imported.");
                return;
            }

            _entityImportIds.Add(rtEntity.RtId);

            foreach (var modelAttribute in modelRtEntity.Attributes)
            {
                var attributeCacheItem = entityCacheItem.Attributes.Values.FirstOrDefault(a => a.AttributeId == modelAttribute.Id);
                if (attributeCacheItem == null)
                {
                    Logger.Error($"'{modelAttribute.Id}' does not exit on type $'{entityCacheItem.CkId}'");
                    return;
                }

                rtEntity.SetAttributeValue(attributeCacheItem.AttributeName, attributeCacheItem.AttributeValueType, modelAttribute.Value);
            }

            _importEntityQueue.Enqueue(rtEntity);

            if (modelRtEntity.Associations != null && modelRtEntity.Associations.Count > 0)
            {
                var originId = rtEntity.RtId;

                foreach (var association in modelRtEntity.Associations)
                {
                    var rtAssociation = new RtAssociation
                    {
                        AssociationRoleId = association.RoleId,
                        OriginRtId = originId,
                        OriginCkId = rtEntity.CkId,
                        TargetRtId = association.TargetRtId.ToObjectId(),
                        TargetCkId = association.TargetCkId
                    };
                    _importAssociationQueue.Enqueue(rtAssociation);
                    Interlocked.Increment(ref _associationsCount);
                }
            }
        }

        private async Task ImportToDatabase(IOspSession session)
        {
            Logger.Info($"Importing {_importEntityQueue.Count} to database.");

            try
            {
                var importEntities = new List<RtEntity>();
                var importAssociations = new List<RtAssociation>();

                var entityMax = _importEntityQueue.Count;
                var associationsMax = _importAssociationQueue.Count;

                for (int i = 0; i < entityMax; i++)
                {
                    if (_importEntityQueue.TryDequeue(out RtEntity tmp))
                    {
                        importEntities.Add(tmp);
                    }
                    else
                    {
                        break;
                    }
                }

                for (int i = 0; i < associationsMax; i++)
                {
                    if (_importAssociationQueue.TryDequeue(out RtAssociation tmp))
                    {
                        importAssociations.Add(tmp);
                    }
                    else
                    {
                        break;
                    }
                }

                if (importEntities.Any())
                {
                    Logger.Info("Adding entities...");
                    await _tenantContext.InternalRepository.BulkInsertRtEntitiesAsync(session, importEntities);
                }

                if (importAssociations.Any())
                {
                    Logger.Info("Adding associations...");
                    await _tenantContext.InternalRepository.BulkRtAssociationsAsync(session, importAssociations);
                }
                

                Logger.Info("Add to database completed.");

            }
            catch (Exception e)
            {
                throw new ModelImportException("Import of model failed during import to database.", e);
            }
        }
    }
}