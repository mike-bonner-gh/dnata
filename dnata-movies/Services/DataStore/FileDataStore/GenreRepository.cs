using Models.Data;

namespace Services.DataStore.FileDataStore
{
    public class GenreRepository : IRepository<Genre>
    {
        private List<Genre> genres;

        public GenreRepository() {
            this.genres = new List<Genre>();
        }

        public void Add(Genre genre) {
            this.genres.Add(genre);
        }

        public void Delete(Genre genre) {
            this.genres.RemoveAll(g => g.Id == genre.Id);
        }

        public Genre Get(Guid id) {
            if (this.genres.Where(g => g.Id == id).Count() == 1) {
                return this.genres.Where(g => g.Id == id).First();
            }
            else {
                return null;
            }
        }

        public IEnumerable<Genre> GetAll() {
            return this.genres;
        }

        public void Update(Genre genre) {
            if (this.genres.Where(g => g.Id == genre.Id).Count() == 0) {
                this.genres.Add(genre);

            }
            else {
                this.genres.RemoveAll(g => g.Id == genre.Id);
                this.genres.Add(genre);
            }
        }
    }
}