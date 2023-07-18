using System.ComponentModel.DataAnnotations;

namespace TeamGatherer.Shared.ViewModels
{
    public record SkillGroupViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
