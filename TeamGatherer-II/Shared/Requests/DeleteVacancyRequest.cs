using System.ComponentModel.DataAnnotations;

namespace TeamGatherer.Shared.Requests
{
    public sealed record DeleteVacancyRequest
    {
        [Required]
        public int Id { get; set; }
    }
}

