using System.ComponentModel.DataAnnotations;

using TeamGatherer.Shared.ViewModels;

namespace TeamGatherer.Shared.Requests
{
    public sealed record CreateVacancyRequest
    {
        [Required]
        public string PositionId { get; set; }
        [Required]
        public string StateUnitId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Requrements { get; set; }
        [Required]
        public string Responsibilities { get; set; }
        [Required]
        public string WorkingConditions { get; set; }

        public List<string> HRs { get; set; } = new();

        public List<CriteriaRequest> Criterias { get; set; } = new();
    }

    public sealed record CriteriaRequest
    {
        [Required]
        public SkillRequest Skill { get; set; }

        [Required]
        public int Weight { get; set; }
    }
}

