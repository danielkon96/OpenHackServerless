// RatingModel myDeserializedClass = JsonConvert.DeserializeObject<RatingModel>(myJsonResponse); 
using Newtonsoft.Json;

public class RatingModel    {
        [JsonProperty("id")]
        public string id { get; set; } 
        [JsonProperty("userId")]
        public string userId { get; set; } 
        [JsonProperty("productId")]
        public string productId { get; set; } 
        [JsonProperty("timestamp")]
        public string timestamp { get; set; } 
        [JsonProperty("locationName")]
        public string locationName { get; set; } 
        [JsonProperty("rating")]
        public int rating { get; set; } 
        [JsonProperty("userNotes")]
        public string userNotes { get; set; } 
    }