using System.IO;

namespace Sanaclub.Infrastructure.Documents;

public sealed class DocumentStorageOptions
{
    public const string SectionName = "DocumentStorage";

    public string Provider { get; set; } = "Local";
    public LocalDocumentStorageOptions Local { get; set; } = new();
}

public sealed class LocalDocumentStorageOptions
{
    public string? BasePath { get; set; }

    public static string GetDefaultBasePath()
    {
        return Path.Combine(AppContext.BaseDirectory, "private-storage");
    }
}

