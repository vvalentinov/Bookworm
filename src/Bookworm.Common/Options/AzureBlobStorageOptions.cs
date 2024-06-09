namespace Bookworm.Common.Options
{
    public class AzureBlobStorageOptions
    {
        public const string AzureBlobStorage = nameof(AzureBlobStorage);

        public string AccountKey { get; set; }

        public string AccountName { get; set; }

        public string ContainerName { get; set; }

        public string StorageConnection { get; set; }
    }
}
