using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGatherer.Shared.ViewModels
{
    public class VacancyViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public PositionViewModel Position { get; set; }
        public StaffUnitViewModel StaffUnit { get; set; }

        [Required]
        public string Requrements { get; set; }

        [Required]
        public string Responsibilities { get; set; }

        [Required]
        public string WorkingConditions { get; set; }

        public List<EmployeeViewModel> Hrs { get; set; }

        public List<CriteriaViewModel> Criteria { get; set; }
        public bool IsClosed { get; set; }

    }

    public class VacancyListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsClosed { get; set; }
    }

    public class EmployeeViewModel
    {
        public string Id { get; set; }
        public PositionViewModel Position { get; set; }
        public StaffUnitViewModel StaffUnit { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class StaffUnitViewModel
    {
        public string Id { get; set; }
        public string PositionId { get; set; }
        public string EmployeeId { get; set; }
        public long? CloseTime { get; set; }
        public string Status { get; set; }

    }

    public sealed record PositionViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
