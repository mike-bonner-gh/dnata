
namespace Services.DataStore
{
    public interface IRepository<T>
    {
        public void Add(T item);
        public T Get(Guid id);
        public void Update(T item);
        public void Delete(T item);
        public IEnumerable<T> GetAll();
    }
}
