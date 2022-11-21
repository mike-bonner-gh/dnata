using Models.Data;

namespace Services.DataStore
{
    public interface IRatingRepository<T> : IRepository<T>
    {
        public List<T> GetTop5Movies();
        public List<MovieRating> GetTop5MoviesByUser(Guid userId);
        public void setRating(Guid userId, Guid movieId, int rating);
    }
}
