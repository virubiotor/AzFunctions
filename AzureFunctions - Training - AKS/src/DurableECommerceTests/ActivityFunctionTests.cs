using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Workflows.Models;
using Moq;
using NUnit.Framework;
using Workflows.Functions;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using FluentAssertions;

namespace DurableECommerceTests
{
    public class ActivityFunctionTests
    {
        private ILogger mockLogger;

        [SetUp]
        public void Setup()
        {
            mockLogger = Mock.Of<ILogger>();
        }

        [Test]
        public async Task CanSaveOrderToDatabase()
        {
            var order = CreateTestOrder();
            var collector = new Mock<IAsyncCollector<OrderEntity>>();
            OrderEntity entity = null;
            collector.Setup(c => c.AddAsync(It.IsAny<OrderEntity>(), It.IsAny<CancellationToken>()))
                .Callback((OrderEntity oe, CancellationToken ct) => entity = oe)
                .Returns(Task.CompletedTask);

            await ActivityFunctions.SaveOrderToDatabase(order, collector.Object, mockLogger);

            entity.Should().NotBeNull();
            entity.OrchestrationId.Should().Be(order.OrchestrationId);
            entity.OrderDate.Should().Be(order.Date);
            entity.Amount.Should().Be(order.Total());
            entity.Email.Should().Be(order.PurchaserEmail);
            entity.RowKey.Should().Be(order.Id);
            entity.PartitionKey.Should().Be(OrderEntity.OrderPartitionKey);
        }

        private static Order CreateTestOrder()
        {
            var order = new Order
            {
                Id = "102030",
                OrchestrationId = "100200",
                Items = new []
                {
                    new OrderItem {ProductId = "Prod1", Amount = 1234 }
                },
                Date = DateTime.Now,
                PurchaserEmail = "test@example.com"
            };
            return order;
        }
    }
}