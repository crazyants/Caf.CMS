using CAF.Infrastructure.Core.Configuration;

namespace CAF.Infrastructure.Core.IO
{
    public class FileSystemSettings : ISettings
    {
        public string DirectoryName { get; set; }
    }
}