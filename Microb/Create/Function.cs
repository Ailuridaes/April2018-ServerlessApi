using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace Microb.Create {

    class Function: MicrobFunction {

        //--- Methods ---
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public APIGatewayProxyResponse LambdaHandler(APIGatewayProxyRequest request) {
            LambdaLogger.Log(JsonConvert.SerializeObject(request));
            var body = JsonConvert.DeserializeObject<MicrobItem>(request.Body);
            LambdaLogger.Log(JsonConvert.SerializeObject(body));
            try {
                var item = CreateItem(body.title.ToString(), body.content.ToString()).Result;
                return new APIGatewayProxyResponse {
                    StatusCode = 200,
                    Body = JsonConvert.SerializeObject(item)
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

        private async Task<MicrobItem> CreateItem(string title, string content) {
            var id = Guid.NewGuid().ToString();
            var now = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");
            var item = new Document();
            item["Id"] = id;
            item["Title"] = title;
            item["Content"] = content;
            item["DateCreated"] = now;
            await _table.PutItemAsync(item);
            LambdaLogger.Log($"*** INFO: Created item {id}");
            var response = new MicrobItem {
                id = id,
                title = title,
                content = content,
                date = now
            };
            return response;
        }
    }
}
