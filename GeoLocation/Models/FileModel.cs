namespace GeoLocation.Models
{
    public class FileModel
    {
        public int    Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string UrlPath { get; set; } = string.Empty;
        public string Extension { get; set; }= string.Empty;
        public string Size { get; set; } = string.Empty;
    }
}
