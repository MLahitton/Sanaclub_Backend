namespace Sanaclub.Domain.Catalogs;

public sealed class DocumentType : CatalogEntity
{
    private DocumentType()
    {
    }

    public DocumentType(string code, string name, string? description = null, int sortOrder = 0)
        : base(code, name, description, sortOrder)
    {
    }
}
