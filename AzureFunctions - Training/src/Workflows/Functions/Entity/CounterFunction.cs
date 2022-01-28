using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Workflows.Functions.Old
{
    public static class CounterFunction
    {
        [FunctionName("CounterOld")]
        public static void CounterOld([EntityTrigger] IDurableEntityContext ctx)
        {
            switch (ctx.OperationName.ToLowerInvariant())
            {
                case "add":
                    ctx.SetState(ctx.GetState<int>() + ctx.GetInput<int>());
                    break;
                case "reset":
                    ctx.SetState(0);
                    break;
                case "get":
                    ctx.Return(ctx.GetState<int>());
                    break;
            }
        }
    }
}
