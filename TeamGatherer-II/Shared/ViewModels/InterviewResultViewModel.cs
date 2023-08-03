namespace TeamGatherer.Shared.ViewModels
{
    public sealed record InterviewResultViewModel
    {
        public int Id { get; set; }
        public EmployeeViewModel Expert { get; set; }
        public int InterviewId { get; set; }

        public required List<SkillEstimationViewModel> Estimations { get; set; } = new();

        public string Comment { get; set; }

        public long OverallScore { get; set; }

    }

    public sealed record SkillEstimationViewModel
    {
        public SkillViewModel Skill { get; set; }
        public int Estimation { get; set; }
    }


}
