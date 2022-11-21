using Newtonsoft.Json;
using Models.Data;

namespace Services.DataStore.FileDataStore
{
    public class UserRepository : IRepository<User>
    {
        private List<User> users;

        public UserRepository() {
            this.users = new List<User>();
        }

        public void Add(User user) {
            this.users.Add(user);
        }

        public void Delete(User user) {
            this.users.RemoveAll(us => us.Id == user.Id);
        }

        public User Get(Guid id) {
            if (this.users.Where(us => us.Id == id).Count() == 1) {
                return this.users.Where(us => us.Id == id).First();
            }
            else {
                return null;
            }
        }

        public IEnumerable<User> GetAll() { 
            return this.users;
        }

        public void Update(User user) {
            if (this.users.Where(us => us.Id == user.Id).Count() == 0) {
                this.users.Add(user);

            }
            else {
                this.users.RemoveAll(us => us.Id == user.Id);
                this.users.Add(user);
            }
        }
    }
}