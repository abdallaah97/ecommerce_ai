namespace EcommerceAPI_AI.Entities
{
    public class DigitalProductProfile
    {
        public int Id { get; set; }
        public string DownloadUrl { get; set; } = string.Empty;
        public double FileSizeMB { get; set; }
        public string FileFormat { get; set; } = string.Empty;
    }
}
