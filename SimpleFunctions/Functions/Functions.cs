using CustomBinding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;

namespace Functions
{
    public static class Functions
    {
        [FunctionName("HttpTrigger")]
        [return: Queue("outputQueue", Connection = "TABLE_CONNECTION_STRING")]
        public static string RunHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{id:int}")] HttpRequest req,
            [Bind] int id,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request with id {id}");

            return $"C# HTTP trigger function processed a request with id {id}";
        }

        [FunctionName("Timer")]
        [return: Queue("outputQueue", Connection = "TABLE_CONNECTION_STRING")]
        public static string RunTimer(
            [TimerTrigger("*/2 7 * * *")] TimerInfo timer, ILogger log)
        {
            if (timer.IsPastDue)
            {
                log.LogInformation($"Timer running late");
            }
            log.LogInformation($"Timer function executed at {DateTime.UtcNow}");
            log.LogInformation($"Next executions will be at {timer.FormatNextOccurrences(10)}");

            return $"Timer function executed at {DateTime.UtcNow}";
        }

        [FunctionName("ServiceBus")]
        [return: Queue("outputQueue", Connection = "TABLE_CONNECTION_STRING")]
        public static string RunServiceBus(
            [ServiceBusTrigger("myqueue", Connection = "ServiceBusConnection")]
            string myQueueItem,
            int deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId, 
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            log.LogInformation($"EnqueuedTimeUtc={enqueuedTimeUtc}");
            log.LogInformation($"DeliveryCount={deliveryCount}");
            log.LogInformation($"MessageId={messageId}");

            return myQueueItem;
        }

        [FunctionName("ServiceBus2")]
        public static void RunServiceBus2(
            [ServiceBusTrigger("myqueue", Connection = "ServiceBusConnection")]
            string myQueueItem,
            int deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId,
            ILogger log,
            [Queue("outputQueue", Connection = "TABLE_CONNECTION_STRING")] out string queueItem
            )
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            log.LogInformation($"EnqueuedTimeUtc={enqueuedTimeUtc}");
            log.LogInformation($"DeliveryCount={deliveryCount}");
            log.LogInformation($"MessageId={messageId}");

            queueItem = myQueueItem;
        }


        [FunctionName("CustomBinding")]
        public static IActionResult RunCustomBinding(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "custombinding/{name}")]
            HttpRequest req,
            ILogger log,
            string name,
            [FileReaderBinding(Location = "%FilePath%\\{name}")]
            FileReaderModel fileReaderModel)
        {
            return new OkObjectResult(fileReaderModel.Content);
        }

        [FunctionName("Queue")]
        public static void RunQueue(
        [QueueTrigger("input-queue")] string myQueueItem,
        [Queue("output-queue")] ICollector<string> myDestinationQueue,
        ILogger log)
        {
            log.LogInformation($"C# function processed: {myQueueItem}");
            myDestinationQueue.Add($"Copy 1: {myQueueItem}");
            myDestinationQueue.Add($"Copy 2: {myQueueItem}");
        }
    }
}
