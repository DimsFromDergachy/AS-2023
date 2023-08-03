using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGatherer.Shared.ViewModels
{
    public sealed record CriteriaViewModel
    {
        public int Id { get; set; }
        public SkillViewModel Skill { get; set; }
        public int Weight { get; set; }
    }
}
