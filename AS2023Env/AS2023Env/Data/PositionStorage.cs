using System.Linq.Expressions;
using System.Text.Json;
using AS2023Env.Models;

namespace AS2023Env.Data;

public class PositionStorage : IStorage<Position>
{
    private List<Position> _positions;

    public Task Init()
    {
        string dataRaw = Environment.GetEnvironmentVariable("AS23_POSITIONS")
                         ?? throw new Exception("No AS23_POSITIONS environment variable set");

        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(dataRaw);

        // список должностей фиксирован, поэтому его хардкодим
        _positions = data.Select(d => new Position(d.Key, d.Value)).ToList();
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