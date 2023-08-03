using System.ComponentModel.DataAnnotations;

namespace TeamGatherer.Shared.ViewModels
{
    public record SkillViewModel
    {
        public int? Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public int? SkillGroupId { get; set; }

        public int MaxScore { get; set; } = 10;

        public bool? IsDeleted { get; set; }
    }
}
