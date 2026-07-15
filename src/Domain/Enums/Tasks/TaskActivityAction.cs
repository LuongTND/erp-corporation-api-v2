namespace Domain;
public enum TaskActivityAction
{
    Created = 1,
    StatusChanged = 2,
    ProgressUpdated = 3,
    Assigned = 4,
    Unassigned = 5,
    DueDateChanged = 6,
    CommentAdded = 7,
    Completed = 8,
    Cancelled = 9,
    SubtaskAdded = 10
}
