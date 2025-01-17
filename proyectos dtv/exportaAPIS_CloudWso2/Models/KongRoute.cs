namespace exportaAPIS_CloudWso2.Models
{
    internal class KongRoute
    {
        public string id { get; set; }
        public string name { get; set; }
        public string[] paths { get; set; }
        public KongService service { get; set; }
    }
}