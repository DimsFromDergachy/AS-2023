using System.ComponentModel.DataAnnotations;

namespace TeamGatherer.Shared.Requests
{
    public sealed record UpdateInterviewRequest
    {
        [Required]
        public int InterviewId { get; set; }
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
        public required string Status { get; set; }
    }
}
