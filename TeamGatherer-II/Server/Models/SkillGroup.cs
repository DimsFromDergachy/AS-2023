using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TeamGatherer.Server.Models;

public class SkillGroup
{
    [Key]
    [ScaffoldColumn(false)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required List<Skill> Skills { get; set; } = new();

    public bool IsDeleted { get; set; }
}

