
namespace Domain;
public class JobLevel : BaseEntity, IAuditable, ICreationTracked, IModificationTracked
{
    public string LevelName { get; private set; } = null!;
    public int LevelOrder { get; private set; }
    public ScopeType DefaultScopeType { get; private set; }
    public string? Description { get; private set; }
    public decimal? BaseSalaryMin { get; private set; }
    public decimal? BaseSalaryMax { get; private set; }

    public bool IsActive { get; set; } = true;
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual ICollection<User> Users { get; private set; } = [];

    private JobLevel() : base()
    {
    }

    public static JobLevel Create(
        string levelName,
        int levelOrder,
        ScopeType defaultScopeType,
        string? description = null,
        decimal? baseSalaryMin = null,
        decimal? baseSalaryMax = null)
    {
        return new JobLevel
        {
            LevelName = levelName,
            LevelOrder = levelOrder,
            DefaultScopeType = defaultScopeType,
            Description = description,
            BaseSalaryMin = baseSalaryMin,
            BaseSalaryMax = baseSalaryMax,
            IsActive = true
        };
    }

    public void Update(
        string levelName,
        int levelOrder,
        ScopeType defaultScopeType,
        string? description = null,
        decimal? baseSalaryMin = null,
        decimal? baseSalaryMax = null)
    {
        LevelName = levelName;
        LevelOrder = levelOrder;
        DefaultScopeType = defaultScopeType;
        Description = description;
        BaseSalaryMin = baseSalaryMin;
        BaseSalaryMax = baseSalaryMax;
    }
}
