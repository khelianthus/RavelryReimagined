using Newtonsoft.Json;

namespace RavelryReimagined.API.Models;

/// <summary>
/// Model for users full list of projects data
/// </summary>
public class ProjectsData
{
    public List<Project> Projects { get; set; }
}

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }

    [JsonProperty("craft_id")]
    public int CraftId { get; set; }

    [JsonProperty("user_id")]
    public int UserId { get; set; }

    [JsonProperty("made_for")]
    public string MadeFor { get; set; }

    [JsonProperty("made_for_user_id")]
    public int? MadeForUserId { get; set; }

    public int? Rating { get; set; }

    [JsonProperty("tag_names")]
    public List<string>? TagNames { get; set; }

    public string Happiness { get; set; }

    [JsonProperty("start_date")]
    public string StartDate { get; set; }  // Assuming the date is in string format, otherwise use DateTime

    [JsonProperty("end_date")]
    public string EndDate { get; set; }  // Assuming the date is in string format, otherwise use DateTime

    [JsonProperty("pattern_name")]
    public string PatternName { get; set; }

    [JsonProperty("craft_name")]
    public string CraftName { get; set; }

    [JsonProperty("favorites_count")]
    public int FavoritesCount { get; set; }
    public string Size { get; set; }

    [JsonProperty("status_name")]
    public string StatusName { get; set; }

    [JsonProperty("permalink")]
    public string PermaLink { get; set; }
    public string Gauge { get; set; }

    [JsonProperty("row-gauge")]

    public string RowGauge { get; set; }

    [JsonProperty("photos_count")]
    public int PhotosCount { get; set; }
    //public string FirstPhotoUrl { get; set; }  // The URL of the first photo

    [JsonProperty("first_photo")]
    public Photo FirstPhoto { get; set; }  // First photo as a Photo object
    public int? Progress { get; set; }
    public string Completed { get; set; }  // You can use DateTime if necessary
    public string Started { get; set; }  // You can use DateTime if necessary
    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }  // You can use DateTime if necessary

    [JsonProperty("updated_at")]
    public string UpdatedAt { get; set; }  // You can use DateTime if necessary

    [JsonProperty("comments_count")]
    public int CommentsCount { get; set; }
}

public class Photo
{
    public int Id { get; set; }
    public int SortOrder { get; set; }
    public int UserId { get; set; }

    [JsonProperty("x_offset")]
    public int? XOffset { get; set; }

    [JsonProperty("y_offset")]
    public int? YOffset { get; set; }

    [JsonProperty("square_url")]
    public string SquareUrl { get; set; }

    [JsonProperty("medium_url")]
    public string MediumUrl { get; set; }

    [JsonProperty("thumbnail_url")]
    public string ThumbnailUrl { get; set; }

    [JsonProperty("small_url")]
    public string SmallUrl { get; set; }

    [JsonProperty("medium2_url")]
    public string Medium2Url { get; set; }

    [JsonProperty("small2_url")]
    public string Small2Url { get; set; }
    public string Caption { get; set; }

    [JsonProperty("caption_html")]
    public string CaptionHtml { get; set; }

    [JsonProperty("copyright_holder")]
    public string CopyrightHolder { get; set; }
}

