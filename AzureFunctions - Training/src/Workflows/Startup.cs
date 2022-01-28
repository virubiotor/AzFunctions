using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Workflows.Startup))]
namespace Workflows
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            //if (builder == null) throw new ArgumentNullException(nameof(builder));

            //builder.ConfigurationBuilder
            //    .AddJsonFile("secrets.json",
            //                optional: true,
            //                reloadOnChange: false);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            //var configuration = builder.GetContext().Configuration;

            //var applicationInsightsServiceOptions = new ApplicationInsightsServiceOptions
            //{
            //    EnableEventCounterCollectionModule = false,
            //    EnablePerformanceCounterCollectionModule = false,
            //    EnableActiveTelemetryConfigurationSetup = true,
            //    EnableHeartbeat = false,
            //    InstrumentationKey = configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY")
            //};
            //builder.Services.AddApplicationInsightsTelemetry(applicationInsightsServiceOptions);
        }
    }
}
