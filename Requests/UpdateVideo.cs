namespace IndividualWorkAPI.Requests;

public class UpdateVideo
{
    public int videoId { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public bool privacyStatus { get; set; }
    public string thumbnailUrl { get; set; }
    public string videoUrl { get; set; }
    public DateTime publishDate { get; set; }
    public DateTime updateDate { get; set; }
    public int userId { get; set; }
}