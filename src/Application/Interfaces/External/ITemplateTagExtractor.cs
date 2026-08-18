namespace Application;

public interface ITemplateTagExtractor
{
    /// <summary>Quét file .docx và trả về danh sách tag {{FieldName}} không trùng lặp.</summary>
    IReadOnlyList<string> ExtractTags(Stream docxStream);
}
