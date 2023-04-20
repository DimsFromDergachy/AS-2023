using System.Linq.Expressions;
using AS2023Env.Models;

namespace AS2023Env.Data;

public class PositionStorage : IStorage<Position>
{
    private List<Position> _positions;

    public Task Init()
    {
        // список должностей фиксирован, поэтому его хардкодим
        _positions = new List<Position>
        {
            new("hr", "Сотрудник отдела кадров"),
            new("backend-developer", "Бэкенд-разработчик"),
            new("frontend-developer", "Фронтенд-разработчик"),
            new("teamlead", "Тимлид"),
            new("devops", "DevOps инженер"),
            new("qa", "Инженер по тестированию"),
            new("design", "Дизайнер"),
        };
        
        return Task.CompletedTask;
    }

    public Task<List<Position>> GetList(Expression<Func<Position, bool>> filter = null)
    {
        return Task.FromResult(filter == null ? _positions : _positions.Where(filter.Compile()).ToList());
    }

    public Task<Position> ById(string id)
    {
        return Task.FromResult(_positions.FirstOrDefault(p => p.Id == id));
    }

    public Task Add(Position item)
    {
        throw new InvalidOperationException();
    }

    public Task Update(Position item)
    {
        throw new InvalidOperationException();
    }

    public Task Delete(string itemId)
    {
        throw new InvalidOperationException();
    }
}