using System.Collections.ObjectModel;
using System.Net.Http.Json;

using Microsoft.Extensions.Options;

using RestApiClient;

using TeamGatherer.Shared.DTOs;
using TeamGatherer.Shared.ServerAdapterViewModels;
using TeamGatherer.Shared.ViewModels;

namespace TeamGatherer.Shared
{
    public class StaffClientAdapter
    {
        private readonly RestApiClient.Client _client;
        private readonly HttpClient _httpClient;
        private readonly StaffConfig _staffConfig;

        public StaffClientAdapter(RestApiClient.Client client, HttpClient httpClient, IOptions<StaffConfig> staffOptions)
        {
            _client = client;
            _httpClient = httpClient;
            _staffConfig = staffOptions.Value;
        }

        public async Task<List<EmployeeAdapterViewModel>> GetAdaptedEmployeesByIdsAsync(params string[] employeeIds)
        {
            var employees = new List<EmployeeAdapterViewModel>();

            employeeIds = employeeIds.Distinct().ToArray();

            foreach (var id in employeeIds)
            {
                var res = await GetAdapedEmployeeById(id);
                employees.Add(res);
            }

            employees = employees.Where(e => e is not null).ToList();

            return employees;
        }

        public async Task<EmployeeAdapterViewModel> GetAdapedEmployeeById(string employeeId)
        {
            try
            {
                var employee = await _client.EmployeesAsync(employeeId);
                var adaptedPosition = await GetAdaptedPositionByIdAsync(employee.PositionId);
                var adaptedStaffUnit = await GetAdaptedStaffUnitByIdAsync(employee.StaffUnitId);

                var result = new EmployeeAdapterViewModel()
                {
                    Email = employee.Email,
                    FirstName = employee.FirstName,
                    Id = employee.Id,
                    LastName = employee.LastName,
                    Position = adaptedPosition,
                    StaffUnit = adaptedStaffUnit
                };

                return result;

            }
            catch (Exception e)
            {
                return null;
            }

        }

        public async Task<PositionAdapterViewModel> GetAdaptedPositionByIdAsync(string positionId)
        {
            try
            {
                var position = await _client.PositionsAsync(positionId);
                if (position is not { })
                    return null;

                return new PositionAdapterViewModel()
                {
                    Id = position.Id,
                    Name = position.Name
                };
            }
            catch (Exception e)
            {

                return null;
            }
        }

        public async Task<StaffUnitAdapterViewModel> GetAdaptedStaffUnitByIdAsync(string staffUnitId)
        {
            try
            {

                var urlBuilder = new System.Text.StringBuilder();
                urlBuilder.Append(_client.BaseUrl != null ? _client.BaseUrl.TrimEnd('/') : "").Append($"/staffUnits/{staffUnitId}");
                var url = urlBuilder.ToString();

                var staffUnit = await _httpClient.GetFromJsonAsync<DTOs.StaffUnit>(url);

                if (staffUnit is not { })
                    return null;

                return new StaffUnitAdapterViewModel()
                {
                    Id = staffUnit.Id,
                    CloseTime = staffUnit?.CloseTime?.Ticks,
                    EmployeeId = staffUnit.EmployeeId,
                    PositionId = staffUnit.PositionId,
                    Status = staffUnit.Status
                };
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                return null;
            }
        }

        public async Task<IReadOnlyCollection<RestApiClient.Employee>> GetEmployeesByPosition(string positionId)
        {
            return new ReadOnlyCollection<RestApiClient.Employee>((await _client.ListAsync(positionId)).ToList());
        }

        public Task<RestApiClient.RegisterEmployeeResult> RegisterEmployee(string email, string staffUnitId, string firstName, string lastName)
        {
            return _client.RegisterAsync(new()
            {
                Email = email,
                StaffUnitId = staffUnitId,
                FirstName = firstName,
                LastName = lastName
            });
        }

        public async Task<List<EmployeeViewModel>> GetEmployeesAsync()
        {
            var employees = await _client.List2Async();
            var result = new List<EmployeeViewModel>();

            foreach (var employee in employees)
            {
                var position = await GetPositionByIdAsync(employee.PositionId);

                result.Add(new EmployeeViewModel
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    Id = employee.Id,
                    Position = new PositionViewModel()
                    {
                        Id = position.Id,
                        Name = position.Name,
                    }
                });
            }

            return result;
        }

        public async Task<RestApiClient.Employee> GetEmployeeByIdAsync(string id)
        {
            return await _client.EmployeesAsync(id);
        }

        public async Task<List<PositionViewModel>> GetPositionsAsync()
        {
            var positions = await _client.List3Async();

            var positionsVMs = new List<PositionViewModel>();
            foreach (var position in positions)
            {
                positionsVMs.Add(new PositionViewModel { Id = position.Id, Name = position.Name });
            }

            return positionsVMs;
        }

        public async Task<List<EmployeeViewModel>> GetHrEmployees()
        {
            var hrs = await GetEmployeesByPosition(_staffConfig.HrKey);

            var hrVms = new List<EmployeeViewModel>();

            foreach (var hr in hrs)
            {
                var position = await GetPositionByIdAsync(hr.PositionId);
                var staffUnit = await GetStaffUnitByIdAsync(hr.StaffUnitId);

                var vm = new EmployeeViewModel()
                {
                    Id = hr.Id,
                    FirstName = hr.FirstName,
                    LastName = hr.LastName,
                    Email = hr.Email,
                    Position = new PositionViewModel { Id = position.Id, Name = position.Name },
                    StaffUnit = new StaffUnitViewModel
                    {
                        Id = staffUnit.Id,
                        PositionId = staffUnit.PositionId,
                        Status = staffUnit.Status,
                        CloseTime = staffUnit.CloseTime?.Ticks,
                        EmployeeId = staffUnit.EmployeeId
                    }
                };

                hrVms.Add(vm);
            }

            return hrVms;
        }

        public async Task<RestApiClient.Position> GetPositionByIdAsync(string id)
        {
            return await _client.PositionsAsync(id);
        }

        public async Task<IReadOnlyCollection<RestApiClient.StaffUnit>> GetStaffUnitsByStatusAsync(RestApiClient.StaffUnitStatus staffUnitStatus)
        {
            return new ReadOnlyCollection<RestApiClient.StaffUnit>((await _client.List4Async(staffUnitStatus.ToString())).ToList());
        }

        public async Task<List<StaffUnitDto>> GetStaffUnitsByStatusAsync(string staffUnitStatus)
        {
            var urlBuilder = new System.Text.StringBuilder();
            urlBuilder.Append(_client.BaseUrl != null ? _client.BaseUrl.TrimEnd('/') : "").Append($"/staffUnits/List/{staffUnitStatus}");
            var url = urlBuilder.ToString();

            var staffUnits = await _httpClient.GetFromJsonAsync<ICollection<DTOs.StaffUnit>>(url);

            var staffDtoList = new List<StaffUnitDto>();

            foreach (var staffUnit in staffUnits)
            {
                var staffDto = new StaffUnitDto
                {
                    Id = staffUnit.Id,
                    Status = "Открыта",
                    Position = await GetPositionByIdAsync(staffUnit.PositionId)
                };

                staffDtoList.Add(staffDto);
            }

            return staffDtoList;
        }

        public async Task<List<StaffUnitDto>> GetStaffUnitsAsync()
        {
            var urlBuilder = new System.Text.StringBuilder();
            urlBuilder.Append(_client.BaseUrl != null ? _client.BaseUrl.TrimEnd('/') : "").Append("/staffUnits/List");
            var url = urlBuilder.ToString();

            var staffUnits = await _httpClient.GetFromJsonAsync<ICollection<DTOs.StaffUnit>>(url);

            var staffDtoList = new List<StaffUnitDto>();

            foreach (var staffUnit in staffUnits)
            {
                var status = string.Empty;
                switch (staffUnit.Status)
                {
                    case "Opened":
                        status = "Открыта";
                        break;
                    case "Closed":
                        status = "Закрыта";
                        break;
                    case "Pending":
                        status = "Вступление в должность";
                        break;
                }

                var staffDto = new StaffUnitDto
                {
                    Id = staffUnit.Id,
                    Status = status,
                    Position = await GetPositionByIdAsync(staffUnit.PositionId)
                };

                staffDtoList.Add(staffDto);
            }

            return staffDtoList;
        }

        public async Task<DTOs.StaffUnit> GetStaffUnitByIdAsync(string id)
        {
            var urlBuilder = new System.Text.StringBuilder();
            urlBuilder.Append(_client.BaseUrl != null ? _client.BaseUrl.TrimEnd('/') : "").Append($"/staffUnits/{id}");
            var url = urlBuilder.ToString();

            var staffUnits = await _httpClient.GetFromJsonAsync<DTOs.StaffUnit>(url);

            return staffUnits;
        }
    }
}
