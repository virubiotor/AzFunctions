using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Workflows.Models;

namespace Workflows.Functions
{
    public static class OrderStatusFunctions
    {
        [FunctionName("GetOrderStatus")]
        public static async Task<IActionResult> GetOrderStatus(
            [HttpTrigger(AuthorizationLevel.Anonymous,
                "get", Route = "orderstatus/{id}")]HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            [Table(OrderEntity.TableName, OrderEntity.OrderPartitionKey, "{id}", Connection = "AzureWebJobsStorage")] OrderEntity order,
            ILogger log, string id)
        {
            log.LogInformation($"Checking status of order {id}");

            if (order == null)
            {
                return new NotFoundResult();
            }
            var status = await client.GetStatusAsync(order.OrchestrationId);

            var statusObj = new
            {
                status.InstanceId,
                status.CreatedTime,
                status.CustomStatus,
                status.Output,
                status.LastUpdatedTime,
                status.RuntimeStatus,
                order.Items,
                order.Amount,
                PurchaserEmail = order.Email
            };

            return new OkObjectResult(statusObj);
        }

        [FunctionName("DeleteOrder")]
        public static async Task<IActionResult> DeleteOrder(
                [HttpTrigger(AuthorizationLevel.Anonymous,
                "delete", Route = "order/{id}")]HttpRequest req,
                [DurableClient] IDurableOrchestrationClient client,
                [Table(OrderEntity.TableName, OrderEntity.OrderPartitionKey, "{id}", Connection = "AzureWebJobsStorage")] OrderEntity order,
                ILogger log, string id)
        {

            if (order == null)
            {
                log.LogWarning($"Cannot find order {id}");

                return new NotFoundResult();
            }
            log.LogInformation($"Deleting order {id}");

            var status = await client.GetStatusAsync(order.OrchestrationId);
            if (status.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            {
                log.LogWarning($"Cannot find order {id}");

                return new BadRequestResult();

            }
            await client.PurgeInstanceHistoryAsync(order.OrchestrationId);

            return new OkResult();
        }

        [FunctionName("GetAllOrders")]
        public static async Task<IActionResult> GetAllOrders(
            [HttpTrigger(AuthorizationLevel.Anonymous,
                "get", Route = null)]HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            log.LogInformation("getting all orders.");
            var statuses = await client.ListInstancesAsync(new OrchestrationStatusQueryCondition
            {
                CreatedTimeFrom = DateTime.Today.AddHours(-2.0)
            }, CancellationToken.None);

            var orderStatuses = statuses.DurableOrchestrationState.Where(x => x.Name == "O_ProcessOrder").ToArray();

            return new OkObjectResult(orderStatuses);
        }
    }
}
