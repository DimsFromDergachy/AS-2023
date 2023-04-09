using System.Linq.Expressions;

namespace AS2023Env;

public interface IStorage<TItem>
{
    public Task Init();
    public Task<List<TItem>> GetList(Expression<Func<TItem, bool>> filter = null);
    public Task Add(TItem item);
    public Task Update(TItem item);
    public Task Delete(string itemId);
}