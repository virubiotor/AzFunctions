using System;
using System.Collections.Generic;
using System.Text;

namespace Workflows.Models
{
    public class OrderEntity
    {
        public const string TableName = "Orders";
        public const string OrderPartitionKey = "ORDER";

        public string PartitionKey { get; set; } = OrderPartitionKey;
        public string RowKey { get; set; }
        public string OrchestrationId { get; set; }
        public string Items { get; set; }
        public string Email { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Amount { get; set; }
    }
}
