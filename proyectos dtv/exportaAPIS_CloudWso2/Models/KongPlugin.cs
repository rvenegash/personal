namespace exportaAPIS_CloudWso2.Models
{
    internal class KongPlugin
    {
        public string id { get; set; }
        public string name { get; set; }
        public string instance_name { get; set; }
        public KongPluginConfig config { get; set; }
        public bool enabled { get; set; }
        public long updated_at { get; set; }
        public long created_at { get; set; }
        public string[] tags { get; set; }
        public string[] protocols { get; set; }
        public KongRoute route { get; set; }
    }

     internal class KongPluginConfig
    {
        public string[] allow { get; set; }
        public string[] deny { get; set; }
        public bool hide_groups_header { get; set; }
    }
}
