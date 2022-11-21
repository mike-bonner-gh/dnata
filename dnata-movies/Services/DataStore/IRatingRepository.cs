using Models.Data;

namespace Services.DataStore
{
    public interface IRatingRepository<T> : IRepository<T>
    {
        public void setRating(Guid userId, Guid movieId, int rating);
    }
}
