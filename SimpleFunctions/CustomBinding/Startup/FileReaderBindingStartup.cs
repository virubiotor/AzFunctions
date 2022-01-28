using CustomBinding;
using CustomBinding.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(FileReaderBindingStartup))]
namespace CustomBinding
{
    public class FileReaderBindingStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddFileReaderBinding();
        }
    }
}
