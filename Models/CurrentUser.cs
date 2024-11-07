using System.Text.Json.Serialization;

namespace RavelryReimagined.API.Models;

public class CurrentUser
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("username")]
    public string UserName { get; set; }

    [JsonPropertyName("tiny_photo_url")]
    public Uri TinyPhotoUrl { get; set; }

    [JsonPropertyName("small_photo_url")]
    public Uri SmallPhotoUrl { get; set; }

    [JsonPropertyName("photo_url")]
    public Uri PhotoUrl { get; set; }

    [JsonPropertyName("large_photo_url")]
    public Uri LargePhotoUrl { get; set; }

}
