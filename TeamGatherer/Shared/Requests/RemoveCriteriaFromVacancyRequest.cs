using System.ComponentModel.DataAnnotations;

namespace TeamGatherer.Shared.Requests
{
    public sealed record RemoveCriteriaFromVacancyRequest
    {
        [Required]
        public List<int> CriteriaIds { get; set; }

        [Required]
        public int VacancyId { get; set; }
    }
}

