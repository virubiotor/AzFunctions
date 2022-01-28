using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using System.IO;

namespace CustomBinding
{
    [Extension("FileReaderBinding")]
    public class FileReaderBinding : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            var rule = context.AddBindingRule<FileReaderBindingAttribute>();
            rule.BindToInput(BuildItemFromAttribute);
        }

        private FileReaderModel BuildItemFromAttribute(FileReaderBindingAttribute arg)
        {
            string content = string.Empty;
            if (File.Exists(arg.Location))
            {
                content = File.ReadAllText(arg.Location);
            }

            return new FileReaderModel
            {
                FullFilePath = arg.Location,
                Content = content
            };
        }
    }
}
