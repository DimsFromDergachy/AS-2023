using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Interview
{
    [Key]
    [ScaffoldColumn(false)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int VacancyId { get; set; }

    public long StartTimestamp { get; set; }
    public long EndTimestamp { get; set; }
    public string Location { get; set; }

    [Column(TypeName = "jsonb")]
    public required Candidate Candidate { get; set; }
    public required List<string> HrIds { get; set; }
    public required List<string> ExpertIds { get; set; }

    [Column(TypeName = "text")] 
    public InterviewStatus Status { get; set; }
}

public sealed record Candidate
{
    public string FIO { get; set; }
    public string Email { get; set; }
}

public enum InterviewStatus
{
    Created,
    Rejected,
    Finished,
}