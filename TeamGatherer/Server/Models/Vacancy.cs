using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamGatherer.Server.Models
{
    public class Vacancy
    {
        [Key]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string PositionId { get; set; }
        public required string StaffUnitId { get; set; }
        public required string Name { get; set; }
        public required string Requrements { get; set; }
        public required string Responsibilities { get; set; }
        public required string WorkingConditions { get; set; }

        public required List<string> HrIds { get; set; } = new();
        public required List<int> Criterias { get; set; }
        public bool IsClosed { get; set; }
    }
}
