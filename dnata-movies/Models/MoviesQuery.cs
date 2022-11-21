using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DnataMovies.Models
{
    public class MoviesQuery
    {
        [JsonProperty("title")]
        public String? Title { get; set; }

        [JsonProperty("yearOfRelease")]
        public int? YearOfRelease { get; set; }

        [JsonProperty("genres")]
        public List<String> Genres { get; set; }

        public MoviesQuery() {
            this.Genres = new List<String>();
        }
    }
}
