namespace Application;

/// <summary>
/// Tra cứu rule phỏng vấn phù hợp nhất cho ứng viên.
/// Dùng trước khi tạo InterviewSchedule.
/// </summary>
public sealed record ResolveInterviewRuleQuery(Guid CandidateId) : IRequest<InterviewRuleConfigResponse?>;
