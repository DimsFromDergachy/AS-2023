using RestApiClient;

namespace TeamGatherer.Shared.ServerAdapterViewModels
{
    public sealed record EmployeeAdapterViewModel
    {
        public string Id { get; set; }
        public PositionAdapterViewModel Position { get; set; }
        public StaffUnitAdapterViewModel StaffUnit { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public sealed record StaffUnitAdapterViewModel
    {
        public string Id { get; set; }
        public string PositionId { get; set; }
        public string EmployeeId { get; set; }
        public long? CloseTime { get; set; }
        public string Status { get; set; }
    }

    public sealed record PositionAdapterViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
