using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace Microb.Update {

    class Function: MicrobFunction {

        //--- Methods ---
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public APIGatewayProxyResponse LambdaHandler(APIGatewayProxyRequest request) {
            LambdaLogger.Log(JsonConvert.SerializeObject(request));
            var item = JsonConvert.DeserializeObject<MicrobItem>(request.Body);
            item.id = request.PathParameters["id"];
            try {
                UpdateItem(item.id, item.title, item.content).Wait();
                return new APIGatewayProxyResponse {
                    StatusCode = 200
                };
            }
            catch (Exception e) {
                LambdaLogger.Log($"*** ERROR: {e}");
                return new APIGatewayProxyResponse {
                    Body = e.Message,
                    StatusCode = 500
                };
            }
        }

        private async Task UpdateItem(string id, string title, string content) {
            var item = new Document();
            item["Id"] = id;
            item["Title"] = title;
            item["Content"] = content;
            await _table.UpdateItemAsync(item);
        }
    }
}
