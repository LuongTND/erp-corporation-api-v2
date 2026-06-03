namespace Domain.Enums;

public enum UserStatus
{
    Active = 1,
    Probation = 2,
    Resigned = 3,
    Terminated = 4,
    Suspended = 5,
    MaternityLeave = 6
}
// Mapping to IsActive:
// Active, Probation, MaternityLeave -> true (MaternityLeave can be true or false, we default to true to allow system login / access)
// Suspended, Resigned, Terminated -> false
