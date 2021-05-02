using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Ianitor.Osp.Frontend.Client;
using Ianitor.Osp.Frontend.Client.Tenants;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool.Commands.Implementations
{
    internal class TestCommand : OspCommand
    {
        private readonly ITenantClient _tenantClient;

        public TestCommand(IOptions<OspToolOptions> options, ITenantClient tenantClient)
            : base("Test", "Test", options)
        {
            _tenantClient = tenantClient;
        }

        public class ContactDto
        {
            public string RtId { get; set; }
            public string Revision { get; set; }
            public string ServiceNumber { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string CompanyName { get; set; }
            public string AdditionalLine { get; set; }
            public int NotificationType { get; set; }
            public string EMailAddress { get; set; }
            public string PhoneNumberMobile { get; set; }
        
            // public SubscriptionTypes SubscriptionType { get; set; }
            // public AccountingTypes AccountingType { get; set; }
            //
            // public QlItemsContainer<AccountingItemDto> Billings { get; set; }

        }
        
        public override async Task Execute()
        {
            Logger.Info("TestCommand");

            string contactRtId = "b39699fe-43b4-4220-b7bd-a1080a0cb421";
            
            var query = new GraphQLRequest
            {
                Query = @"query GetContact($rtIds: [String]!) {
                  paketServiceContactConnection(rtIds: $rtIds) {
                    items {
                      rtId
                      revision
                      servic eNumber
                      firstName
                      lastName
                      companyName
                      subscriptionType
                      accountingType
                      freeParcelShipmentCount
                      billings {
                        totalCount
                        items {
                          rtId
                          revision
                          amount
                          price
                          chargeType
                          metaData
                        }
                      }
                    }
                  }
                }
                ",
                Variables = new {rtIds = new[] {contactRtId}}
            };

            try
            {
                var result = await _tenantClient.SendQueryAsync<ContactDto>(query);

                var t = result.Items.FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        

            // var query = _dataSourceAccess.ServiceClient.CreateConstructionKitQuery()
            //     .AddField(c => c.Items,
            //         sq => sq
            //             .AddField(e => e.CkId)
            //             .AddField(e => e.TypeName)
            //             .AddField(e => e.ScopeId)
            //             .AddField(e => e.Revision)
            //             .AddField(e => e.IsFinal)
            //             .AddField(e => e.IsAbstract)
            //             .AddField(e => e.Attributes, aq =>
            //                     aq
            //                         .AddField(a => a.Items,
            //                             aiq => aiq
            //                                 .AddField(x => x.AttributeId)
            //                                 .AddField(x=> x.AttributeName)
            //                                 .AddField(x=> x.AttributeValueType))
            //                         .AddField(a => a.TotalCount)));
            //
            // var request = new GraphQLRequest(query.Build());
            // var result = await _dataSourceAccess.ServiceClient.SendQueryAsync<CkEntity>(request);
            //
            // Logger.Info(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}