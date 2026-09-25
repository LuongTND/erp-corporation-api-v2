namespace Application;

public sealed class InterviewScheduleMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.InterviewSchedule, InterviewScheduleResponse>()
            .Map(dest => dest.Location, src => src.Location.ToString())
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.CandidateName, src => src.Candidate != null ? src.Candidate.FullName : string.Empty)
            .Map(dest => dest.InterviewerName, src => src.Interviewer != null ? src.Interviewer.FullName : string.Empty);
    }
}
