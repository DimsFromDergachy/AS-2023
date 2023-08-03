using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGatherer.Shared.Requests
{
    public sealed record SkillRequest
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        
        [Required]
        public int? SkillGroupId { get; set; }
        public int MaxScore { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
