using Newtonsoft.Json;

namespace Models.Data
{
    public class User
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public User(Guid id, string name) {
            this.Id = id;
            this.Name = name;
        }
    }
}