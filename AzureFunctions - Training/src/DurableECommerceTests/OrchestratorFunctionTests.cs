using FluentAssertions;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Workflows.Functions;
using Workflows.Models;

namespace DurableECommerceTests
{
    public class OrchestratorFunctionTests
    {
        private ILogger mockLogger;

        [SetUp]
        public void Setup()
        {
            mockLogger = Mock.Of<ILogger>();
        }

        [Test]
        public async Task CanSuccessfullyProcessAnOrder()
        {
            var order = CreateTestOrder(false);
            var context = CreateMockDurableOrchestrationContext(order, null);

            var orderResult = await OrchestratorFunctions.ProcessOrder(context.Object, mockLogger);

            context.Verify(c => c.CallActivityAsync("A_SaveOrderToDatabase", order), Times.Once);
            context.Verify(c => c.CallActivityAsync("A_RequestOrderApproval", order), Times.Never);
            context.Verify(c => c.CallActivityAsync<string>("A_CreatePersonalizedPdf", It.IsAny<object>()), Times.Once);

            context.Verify(c => c.CallActivityWithRetryAsync("A_SendOrderConfirmationEmail",
                It.IsAny<RetryOptions>(),
                It.IsAny<object>()), Times.Once);
            orderResult.Should().BeEquivalentTo(new OrderResult { Status = "Success", Downloads = new[] { "example.pdf" } });
        }

        [Test]
        public async Task CanSuccessfullyProcessAnOrderWithApproval()
        {
            var order = CreateTestOrder(true);
            var context = CreateMockDurableOrchestrationContext(order, "Approved");

            var orderResult = await OrchestratorFunctions.ProcessOrder(context.Object, mockLogger);

            context.Verify(c => c.CallActivityAsync("A_SaveOrderToDatabase", order), Times.Once);
            context.Verify(c => c.CallActivityAsync("A_RequestOrderApproval", order), Times.Once);
            context.Verify(c => c.CallActivityAsync<string>("A_CreatePersonalizedPdf", It.IsAny<object>()), Times.Once);

            context.Verify(c => c.CallActivityWithRetryAsync("A_SendOrderConfirmationEmail",
                It.IsAny<RetryOptions>(),
                It.IsAny<object>()), Times.Once);

            orderResult.Should().BeEquivalentTo(new OrderResult { Status = "Success", Downloads = new[] { "example.pdf" } });
        }


        [Test]
        public async Task CanNotifyAnOrderNotApproved()
        {
            var order = CreateTestOrder(true);
            var context = CreateMockDurableOrchestrationContext(order, "Rejected");

            var orderResult = await OrchestratorFunctions.ProcessOrder(context.Object, mockLogger);

            context.Verify(c => c.CallActivityAsync("A_SaveOrderToDatabase", order), Times.Once);
            context.Verify(c => c.CallActivityAsync("A_RequestOrderApproval", order), Times.Once);
            context.Verify(c => c.CallActivityAsync("A_SendNotApprovedEmail", order), Times.Once);

            orderResult.Should().BeEquivalentTo(new OrderResult { Status = "NotApproved" });
        }

        [Test]
        public async Task SendsProblemEmailOnFailureToTranscode()
        {
            var order = CreateTestOrder(false);
            var context = CreateMockDurableOrchestrationContext(order, null, true);

            var orderResult = await OrchestratorFunctions.ProcessOrder(context.Object, mockLogger);

            context.Verify(c => c.CallActivityAsync("A_SaveOrderToDatabase", order), Times.Once);
            context.Verify(c => c.CallActivityAsync("A_RequestOrderApproval", order), Times.Never);
            context.Verify(c => c.CallActivityWithRetryAsync("A_SendProblemEmail", It.IsAny<RetryOptions>(), order), Times.Once);

            orderResult.Should().BeEquivalentTo(new OrderResult { Status = "Problem" });
        }

        private static Order CreateTestOrder(bool requiresApproval)
        {
            var order = new Order
            {
                Id = "102030",
                OrchestrationId = "100200",
                Items = new[]
                {
                    new OrderItem
                    {
                        Amount = requiresApproval ? 12345 : 50,
                        ProductId = "Prod1",
                    }
                },
                Date = DateTime.Now,
                PurchaserEmail = "test@example.com"
            };
            return order;
        }

        private static Mock<IDurableOrchestrationContext> CreateMockDurableOrchestrationContext(Order order, string approvalResult, bool throwErrorInPdf = false)
        {
            var context = new Mock<IDurableOrchestrationContext>();
            context.Setup(c => c.GetInput<Order>()).Returns(order);
            context.SetupGet(c => c.InstanceId).Returns("12345");
            if (throwErrorInPdf)
            {
                context.Setup(c => c.CallActivityAsync<string>("A_CreatePersonalizedPdf", It.IsAny<object>())).ThrowsAsync(new InvalidOperationException("Failed to create PDF"));
            }
            else
            {
                context.Setup(c => c.CallActivityAsync<string>("A_CreatePersonalizedPdf", It.IsAny<object>())).ReturnsAsync("example.pdf");
            }
            context.Setup(c => c.WaitForExternalEvent<string>("OrderApprovalResult", It.IsAny<TimeSpan>(), null, It.IsAny<CancellationToken>())).ReturnsAsync(approvalResult);

            return context;
        }
    }
}