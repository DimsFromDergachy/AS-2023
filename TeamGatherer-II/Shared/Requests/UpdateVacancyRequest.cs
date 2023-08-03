using System.ComponentModel.DataAnnotations;

namespace TeamGatherer.Shared.Requests
{
    public sealed record UpdateVacancyRequest
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string PositionId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Requrements { get; set; }
        [Required]
        public string Responsibilities { get; set; }
        [Required]
        public string WorkingConditions { get; set; }
    }
}

