using System.ComponentModel.DataAnnotations;

namespace TeamGatherer.Shared.Requests
{
    public sealed record AddCriteriaToVacancyRequest
    {
        [Required]
        public List<CriteriaRequest> Criterias { get; set; }
        [Required]
        public int VacancyId { get; set; }
    }
}

