namespace Application;

public interface IPosRegionReader
{
    Task<IEnumerable<PosRegionResponse>> GetAllRegionsAsync(CancellationToken ct = default);
}
