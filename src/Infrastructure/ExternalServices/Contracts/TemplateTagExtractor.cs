using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Infrastructure;

[RegisterService(typeof(ITemplateTagExtractor))]
public sealed class TemplateTagExtractor : ITemplateTagExtractor
{
    private static readonly Regex TagPattern = new(@"\{\{(\w+)\}\}", RegexOptions.Compiled);

    public IReadOnlyList<string> ExtractTags(Stream docxStream)
    {
        var tags = new HashSet<string>(StringComparer.Ordinal);

        using var doc = WordprocessingDocument.Open(docxStream, isEditable: false);
        var body = doc.MainDocumentPart?.Document?.Body
            ?? throw new InvalidOperationException("Invalid .docx: no document body.");

        // Reconstruct full paragraph text trước khi regex
        // — tránh lỗi split runs ({{Ho trong run 1, Ten}} trong run 2)
        foreach (var para in body.Descendants<Paragraph>())
        {
            var text = string.Concat(para.Descendants<Text>().Select(t => t.Text));
            foreach (Match m in TagPattern.Matches(text))
                tags.Add(m.Groups[1].Value);
        }

        // Quét cả header/footer nếu có
        if (doc.MainDocumentPart?.HeaderParts is { } headers)
            foreach (var hp in headers)
                foreach (var para in hp.Header.Descendants<Paragraph>())
                {
                    var text = string.Concat(para.Descendants<Text>().Select(t => t.Text));
                    foreach (Match m in TagPattern.Matches(text))
                        tags.Add(m.Groups[1].Value);
                }

        return [.. tags];
    }
}
