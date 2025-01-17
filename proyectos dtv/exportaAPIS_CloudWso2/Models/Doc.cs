namespace exportaAPIS_CloudWso2.Models
{
    public class Doc
    {
        public string documentId { get; set; }
        public string summary { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string sourceUrl { get; set; }
        public string sourceType { get; set; }
        public string visibility { get; set; }
    }


    public class DocResult
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public Doc[] list { get; set; }
    }
}
