namespace SmartEnviMonitoring.API.Repositories;
public interface IGenericRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task DeleteAsync(int id);
    Task<bool> Exists(int id);
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetLastNRecordsAsync(int num);
    Task<T> GetAsync(int? id);
    Task UpdateAsync(T entity);
}