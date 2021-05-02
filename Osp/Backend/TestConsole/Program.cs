using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Ianitor.Osp.Backend.DistributedCache;
using Ianitor.Osp.Backend.Persistence;
using Ianitor.Osp.Backend.Persistence.DataAccess;
using Ianitor.Osp.Backend.Persistence.DatabaseEntities;
using Ianitor.Osp.Backend.Persistence.SystemStores;
using Ianitor.Osp.Common.Shared;
using Ianitor.Osp.Common.Shared.DataTransferObjects;
using IdentityModel.Jwk;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;

namespace TestConsole
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static async Task Main(string[] args)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            
            NLog.LogManager.Configuration = config;

            // while (true)
            // {
            //     var xz = OspObjectId.GenerateNewId();
            //     Logger.Info(xz.ToString());
            // }

            Logger.Info("Hello World!");

            try
            {
                //HttpClient httpClient = new HttpClient();

                //var request = new HttpRequestMessage(HttpMethod.Post,
                //    "http://localhost:8529/_db/TestOsp/_api/import?type=documents&collection=CkEntities&waitForSync=False&complete=True&details=True");
                //request.Headers.Add("Authorization", "Basic cm9vdDpSaWFrc2k1Nw==");
                //var result = httpClient.SendAsync(request);

                string ds = "paketservice";
                
                
                var systemContext = new SystemContext(new OptionsWrapper<OspSystemConfiguration>(new OspSystemConfiguration
                {
                    DatabaseUserPassword = "Riaksi57",
                    AdminUserPassword = "Riaksi57",
                    
                }), new DistributedWithPubSubCache(new OptionsWrapper<DistributeCacheWithPubSubOptions>(new DistributeCacheWithPubSubOptions())));
                
                // if (await systemContext.IsDataSourceExistingAsync(ds))
                // {
                //     await systemContext.DropDataSourceAsync(ds);
                // }
                // if (await systemContext.IsSystemDatabaseExistingAsync())
                // {
                //     await systemContext.DropSystemDatabaseAsync();
                // }
                //
                // await systemContext.CreateSystemDatabaseAsync();
                
                
//                 await systemContext.CreateDataSourceAsync(ds, ds);
//                 
//                 await systemContext.ImportCkModelAsync(ds, ScopeIds.Application,
//                     "/Users/gerald/RiderProjects/PaketService/Backend/Persistence/PaketServiceConstructionKit.json",
//                     CancellationToken.None);
//                 
//                 await systemContext.ImportRtModelAsync(ds, "/Users/gerald/RiderProjects/rtmodel.json", CancellationToken.None);
// //
             //  var dataSourceContext = await systemContext.CreateOrGetDataSourceContext(ds);
               
               EntityNotificationRepository x = new EntityNotificationRepository(systemContext);
               var re = await x.GetPendingMessagesAsync("paketservice", NotificationTypesDto.EMail);
               var t = await x.StoreNotificationMessages("paketservice", re.List);
               
               
               // var x = await dataSourceContext.Repository.ExtractAutoCompleteValuesAsync("PaketService.ParcelShipment", "Sender",
               //     "^[A-Z]+-[0-9]+ .+$", 100);
               
               // var ckResult = await dataSourceContext.Repository.GetCkAttributesAsync(null, new DataQueryOperation
               // {
               //     FieldFilters = new[] {new FieldFilter("AttributeValueType", FieldFilterOperator.Equals, "BOOL")}
               // }, 0, 10);
               //
               //
               //
               //  var result = await dataSourceContext.Repository.GetRtEntitiesByTypeAsync("PaketService.ParcelShipment", new DataQueryOperation
               //  {
               //      FieldFilters = new []{new FieldFilter("State", FieldFilterOperator.Equals, "Eingelagert")}
               //  });
               //
               // Logger.Info( JsonConvert.SerializeObject(result));
               //
               // var result1 = await dataSourceContext.Repository.get(new []{"System.Entity"}, new DataQueryOperation
               // {
               //     Language = "de",
               //   //  AttributeSearchFilter = new AttributeSearchFilter(new []{"IsAbstract", "CkId"}, "item")
               //     
               //   //  FieldFilters = new []{new FieldFilter("CreationDateTime", FieldFilterOperator.GreaterEqualThan, "@startOfDay(-1)")}
               // }, null, 10);
               //
               //
               //
               // var result2 = await dataSourceContext.Repository.GetRtEntitiesByTypeAsync("PaketService.Contact", new DataQueryOperation
               // {
               //     FieldFilters = new []{new FieldFilter("LastName", FieldFilterOperator.Like, "*Loch*")}
               // });
               //
               //Logger.Info( JsonConvert.SerializeObject(result1));

               
               // var resultAssocs = await dataSourceContext.Repository.GetRtAssociationTargetsAsync(result2.Result.First().RtId,
               //     "System.ParentChild", "PaketService.ParcelShipment", GraphDirections.Outbound, new DataQueryOperation(), null, null);
               
             //  Logger.Info( JsonConvert.SerializeObject(resultAssocs));

               PerfManager.Instance.WriteLogging(Logger);
//
//                var databaseContext = new DatabaseContext("http://localhost:8529", "testosp");
//                var attributeName = "PaketService.Contacts.City";
//                
//                var test = databaseContext.Query<CkEntity>().Filter(entity => AQL.Last(entity.Properties).AttributeId == attributeName)
//                    
//                    //.Where(s => s.Properties.Count > 0 && s.Properties.Last().AttributeId == attributeName)
//                    .Skip(0).Take(10)
//                    .ToArray();
//                
//                
//                Logger.CkBaseTypeInfo("done");


//                await databaseContext.DropDatabase();
//                await databaseContext.CreateDatabase();

//                var ckEntity = await databaseContext.CkEntities.DocumentAsync(databaseContext.GetId<CkEntity>("PaketService.ParcelShipment"));
//                var result = await databaseContext.GetCkEntityAttributeInheritanceChainAsync(ckEntity);
//                var result2 = await databaseContext.GetCkEntityAttributeInheritanceChainAsync(ckEntity);


                //var attributeInheritanceChain = databaseContext.GetCkEntityAttributeInheritanceChain(ckEntity);

//                using (StreamReader streamReader = File.OpenText(@"D:\Development\ObjectServicePlatform\Backend\Ianitor.Osp.Backend.Persistence\CKModel.json"))
                //                {
                //                    databaseContext.ImportCkModel(streamReader.BaseStream).Wait();
                //                }
//                using (StreamReader streamReader = File.OpenText(@"D:\Development\ToolRent\Backend\Persistence\PaketServiceConstructionKit.json"))
//                {
//                    await databaseContext.ImportCkModel(streamReader.BaseStream);
//                }

//                for (int i = 0; i < 200; i++)
//                {
//                    var t = await databaseContext.CkEntities.DocumentAsync("CkEntities/PaketService.ParcelShipment");
//                }
//                
//                
//                using (StreamReader streamReader = File.OpenText(@"D:\rtmodel.json"))
//                {
//                    await databaseContext.ImportRtModel(streamReader.BaseStream);
//                }
                //var associationInheritanceChain =  databaseContext.GetCkEntityAssociationInheritanceChain(ckEntity);
                //var runtimeEntity =  databaseContext.CreateRtEntityFromCk(ckEntity);

                //databaseContext.RtEntities.Insert(runtimeEntity);


            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
