using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace FunctionTests
{
    public class Tests
    {
        [Fact]
        public void Should_Execute_Timer_Function()
        {
            var logger = new Mock<ILogger>();
            TimerSchedule schedule = new DailySchedule("7:00:00");
            var result = Functions.Functions.RunTimer(new TimerInfo(schedule, new ScheduleStatus()), logger.Object);
            result.Should().Contain("Timer function executed at");
        }

        [Fact]
        public void Should_Execute_Http_Function()
        {
            var logger = new Mock<ILogger>();
            var context = new DefaultHttpContext();
            var functionId = 1234;
            var result = Functions.Functions.RunHttp(context.Request, functionId, logger.Object);
            result.Should().Be($"C# HTTP trigger function processed a request with id {functionId}");
        }

        [Fact]
        public void Should_Execute_ServiceBus_Function()
        {
            var logger = new Mock<ILogger>();

            var queueItem = "testItem";
            var deliveryCount = 5;
            var messageId = "testId";
            var result = Functions.Functions.RunServiceBus(queueItem,deliveryCount,DateTime.Now, messageId, logger.Object);
            result.Should().Be(queueItem);
        }
    }
}
