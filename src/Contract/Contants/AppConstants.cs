namespace Contract;

public static class AppConstants
{
    public const int DefaultPageTop = 10;
    public const int DefaultPageSkip = 0;
    public const bool DefaultNeedTotalCount = false;
    public const int MaxPageSize = 100;
    public const int DefaultPage = 1;
    public const string DefaultOrderBy = "CreatedAt";
    public const string DefaultOrderType = "DESC";

    public const int DefaultMinLength = 1;

    // public const string FrontendUrl = "http://localhost:3000";
    public const string FrontendUrl = "https://sswms-fe.vercel.app";

    // Default max length 
    public const int MaxLengthName = 300;
    public const int MaxLengthPermision = 1000;
    public const int MaxLengthEmail = 320;
    public const int MaxLengthDescription = 1000;
    public const int MaxLengthUrl = 2000;
    public const int MaxLengthSearchQuery = 150;

    // Default media file size
    public const long MaxBlobSize = 1024 * 1024 * 50;
    public const string BlobContainer = "bahungerp";
    public static readonly string DatabaseName = "BaHungERP";
    public static readonly string DefaultUri = "";
    public static readonly string DefaultUserPicture = "";
}