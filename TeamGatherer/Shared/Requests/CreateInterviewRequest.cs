using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGatherer.Shared.Requests
{
    public sealed record CreateInterviewRequest
    {
        [Required]
        public int VacancyId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public string Location { get; set; }

        [Required]
        public required CandidateRequest Candidate { get; set; }
        [Required]
        public required List<string> HrIds { get; set; }
        [Required]
        public required List<string> ExpertIds { get; set; }

    }

    public sealed record CandidateRequest
    {
        [Required]
        public string FIO { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
