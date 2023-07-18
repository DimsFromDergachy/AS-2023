using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class InterviewResult
{
    [Key]
    [ScaffoldColumn(false)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string ExpertId { get; set; }
    public required int InterviewId { get; set; }

    [Column(TypeName = "jsonb")]
    public required List<SkillEstimation> Estimations { get; set; } = new();

    public string Comment { get; set; }
}

public sealed record SkillEstimation
{
    public int SkillId { get; set; }
    public int Estimation { get; set; }
}
