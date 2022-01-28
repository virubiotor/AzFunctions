using Microsoft.Azure.WebJobs;
using System;

namespace CustomBinding.Extensions
{
    public static class MyFileReaderBindingExtension
    {
        public static IWebJobsBuilder AddFileReaderBinding(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<FileReaderBinding>();
            return builder;
        }
    }
}
