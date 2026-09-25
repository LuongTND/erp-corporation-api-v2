namespace Application;

public interface IContractGeneratorService
{
    /// <summary>Trộn data vào file template .docx, trả về stream file đã gen.</summary>
    Stream Generate(Stream templateStream, Dictionary<string, object> data);
}
