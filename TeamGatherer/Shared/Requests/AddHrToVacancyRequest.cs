using System.ComponentModel.DataAnnotations;

namespace TeamGatherer.Shared.Requests
{
    public sealed record AddHrToVacancyRequest
    {
        [Required]
        public int VacancyId { get; set; }

        [Required]
        public List<string> HrIds { get; set; }
    }
}

