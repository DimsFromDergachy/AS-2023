using System.Data;
using System.Linq.Expressions;
using AS2023Env.Models;
using Bogus;
using Bogus.DataSets;

namespace AS2023Env.Data;

public class EmployeeStorage : IStorage<Employee>
{
    private readonly IStorage<Position> _positionStorage;
    private readonly List<Employee> _employees = new();

    public EmployeeStorage(IStorage<Position> positionStorage)
    {
        _positionStorage = positionStorage;
    }
    
    public async Task Init()
    {
        var faker = new Faker("ru");
        var rnd = new Random();
        bool needAddMock = Constants.IsAdmin;

        List<Position> positions = await _positionStorage.GetList();
        foreach (Position position in positions)
        {
            int employeeCount = rnd.Next(
                Constants.InitialEmployeePerPositionMin,
                Constants.InitialEmployeePerPositionMax + 1
            );

            if (position.Id == Constants.MockPosition && needAddMock)
            {
                employeeCount--;
                var mockEmp = new Employee
                {
                    Id = Constants.MockGuid.ToString(),
                    PositionId = position.Id,
                    FirstName = "Полиграф",
                    LastName = "Полиграфов",
                    Email = "ihatecats@domain.com"
                };
                _employees.Add(mockEmp);
                needAddMock = false;
            }

            while (employeeCount-- > 0)
            {
                Name.Gender gender = rnd.Next(2) == 0 ? Name.Gender.Female : Name.Gender.Male;
                string first = faker.Name.FirstName(gender);
                string last = faker.Name.LastName(gender);
                var employee = new Employee
                {
                    Id = Guid.NewGuid().ToString(),
                    PositionId = position.Id,
                    FirstName = first,
                    LastName = last,
                    Email = faker.Internet.Email(first, last)
                };

                _employees.Add(employee);
            }
        }
    }

    public Task<List<Employee>> GetList(Expression<Func<Employee, bool>> filter = null)
    {
        return Task.FromResult(filter == null ? _employees : _employees.Where(filter.Compile()).ToList());
    }

    public Task<Employee> ById(string id)
    {
        return Task.FromResult(_employees.FirstOrDefault(e => e.Id == id));
    }

    public Task Add(Employee item)
    {
        _employees.Add(item);
        return Task.CompletedTask;
    }

    public Task Update(Employee item)
    {
        Employee found = _employees.FirstOrDefault(e => e.Id == item.Id);
        if (found == null)
        {
            throw new DataException($"Сотрудник с id = {item.Id} не найден");
        }

        int index = _employees.IndexOf(found);
        _employees.RemoveAt(index);
        _employees.Insert(index, item);
        
        return Task.CompletedTask;
    }

    public Task Delete(string itemId)
    {
        _employees.RemoveAll(e => e.Id == itemId);
        return Task.CompletedTask;
    }
}