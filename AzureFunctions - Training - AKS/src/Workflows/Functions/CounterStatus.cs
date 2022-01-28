using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Workflows.Functions
{
    public class CounterStatus
    {
        [FunctionName("GetCounter")]
        public static async Task<IActionResult> GetCounter(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Counter/{entityKey}")] HttpRequestMessage req,
        [DurableClient] IDurableEntityClient client,
        string entityKey)
        {
            var entityId = new EntityId("Counter", entityKey);
            var state = await client.ReadEntityStateAsync<Counter>(entityId);
            return new OkObjectResult(state);
        }

        [FunctionName("DeleteCounter")]
        public static async Task<HttpResponseMessage> DeleteCounter(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Counter/{entityKey}")] HttpRequestMessage req,
        [DurableClient] IDurableEntityClient client,
        string entityKey)
        {
            var entityId = new EntityId("Counter", entityKey);
            await client.SignalEntityAsync<ICounter>(entityId, proxy => proxy.Delete());

            // If only a single class implements entity whe can use:
            // await client.SignalEntityAsync<ICounter>(entityKey, proxy => proxy.Delete());

            return req.CreateResponse(HttpStatusCode.Accepted);
        }
    }
}
