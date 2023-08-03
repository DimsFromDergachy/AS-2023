using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamGatherer.Server.Models;

public class Skill
{
    [Key]
    [ScaffoldColumn(false)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Name { get; set; }
    public int SkillGroupId { get; set; }
    public int MaxScore { get; set; }
    public bool IsDeleted { get; set; }

    public bool IsBinary()
    {
        return MaxScore == 1;
    }
}

