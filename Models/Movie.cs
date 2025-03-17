
using System.Text.Json.Serialization;

namespace wj_api.Models
{
    public class MovieResponse
    {
        public List<Movie>? Movies { get; set; }
    }

    public class Movie
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("year")]
        public string? Year { get; set; }

        [JsonPropertyName("rated")]
        public string? Rated { get; set; }

        [JsonPropertyName("released")]
        public string? Released { get; set; }

        [JsonPropertyName("runtime")]
        public string? Runtime { get; set; }

        [JsonPropertyName("genre")]
        public string? Genre { get; set; }

        [JsonPropertyName("director")]
        public string? Director { get; set; }

        [JsonPropertyName("writer")]
        public string? Writer { get; set; }

        [JsonPropertyName("actors")]
        public string? Actors { get; set; }

        [JsonPropertyName("plot")]
        public string? Plot { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("awards")]
        public string? Awards { get; set; }

        [JsonPropertyName("poster")]
        public string? Poster { get; set; }

        [JsonPropertyName("metascore")]
        public string? Metascore { get; set; }

        [JsonPropertyName("rating")]
        public string? Rating { get; set; }

        [JsonPropertyName("votes")]
        public string? Votes { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("price")]
        public string? PriceString { get; set; }

        [JsonIgnore]
        public decimal? Price => PriceString != null && decimal.TryParse(PriceString, out var price) ? price : null;

        public string? Provider { get; set; }
    }

    public class MovieComparison
    {
        public string? Title { get; set; }
        public Movie? CinemaWorldMovie { get; set; }
        public Movie? FilmWorldMovie { get; set; }
        public decimal? CheapestPrice => GetCheapestPrice();
        
        private decimal? GetCheapestPrice()
        {
            if (CinemaWorldMovie?.Price == null && FilmWorldMovie?.Price == null)
                return null;
            
            if (CinemaWorldMovie?.Price == null)
                return FilmWorldMovie.Price;
            
            if (FilmWorldMovie?.Price == null)
                return CinemaWorldMovie.Price;
            
            return Math.Min(CinemaWorldMovie.Price.Value, FilmWorldMovie.Price.Value);
        }
    }
}