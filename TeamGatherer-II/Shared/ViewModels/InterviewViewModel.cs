using System.ComponentModel.DataAnnotations.Schema;

using TeamGatherer.Shared.Requests;

namespace TeamGatherer.Shared.ViewModels
{
    public sealed record class InterviewViewModel
    {
        public int Id { get; set; }
        public int VacancyId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public required CandidateViewModel Candidate { get; set; }
        public required List<EmployeeViewModel> Hrs { get; set; }
        public required List<EmployeeViewModel> Experts { get; set; }
        public string Status { get; set; }
    }

    public sealed record CandidateViewModel
    {
        public string FIO { get; set; }
        public string Email { get; set; }
    }

}
