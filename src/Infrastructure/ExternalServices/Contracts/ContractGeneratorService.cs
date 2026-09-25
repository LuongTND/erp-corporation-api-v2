using MiniSoftware;

namespace Infrastructure;

[RegisterService(typeof(IContractGeneratorService))]
public sealed class ContractGeneratorService : IContractGeneratorService
{
    public Stream Generate(Stream templateStream, Dictionary<string, object> data)
    {
        // MiniWord yêu cầu file path — dùng temp files, cleanup trong finally
        var tempTemplate = Path.GetTempFileName() + ".docx";
        var tempOutput = Path.GetTempFileName() + ".docx";

        try
        {
            using (var fs = File.Create(tempTemplate))
                templateStream.CopyTo(fs);

            MiniWord.SaveAsByTemplate(tempOutput, tempTemplate, data);

            return new MemoryStream(File.ReadAllBytes(tempOutput));
        }
        finally
        {
            if (File.Exists(tempTemplate)) File.Delete(tempTemplate);
            if (File.Exists(tempOutput)) File.Delete(tempOutput);
        }
    }
}
