using System.Collections.Generic;

namespace exportaAPIS_CloudWso2.Models
{
    public class ConsumerTokens
    {
        public string consumer { get; set; }
        public string consumerName { get; set; }
        public List<string> tokens { get; set; }
    }
}