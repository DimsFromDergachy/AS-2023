using System.ComponentModel.DataAnnotations;

namespace TeamGatherer.Shared.Requests
{
    public sealed record CreateInterviewResultRequest
    {
        [Required]
        public int InterviewId { get; set; }

        [Required]
        public string ExpertId { get; set; }

        [Required]
        public List<SkillEstimationRequest> Estimations { get; set; }

        public string Comment { get; set; }

    }

    public record SkillEstimationRequest
    {
        [Required]
        public int SkillId { get; set; }
        [Required]
        public int Estimation { get; set; }

        public string Name { get; set; }
        public int MaxScore { get; set; }
    }
}
