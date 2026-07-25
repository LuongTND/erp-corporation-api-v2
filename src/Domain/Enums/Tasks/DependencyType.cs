namespace Domain;

public enum DependencyType
{
    FinishToStart  = 1, // B bắt đầu sau khi A hoàn thành
    StartToStart   = 2, // B bắt đầu sau khi A bắt đầu
    FinishToFinish = 3, // B hoàn thành sau khi A hoàn thành
    StartToFinish  = 4  // B hoàn thành sau khi A bắt đầu
}
