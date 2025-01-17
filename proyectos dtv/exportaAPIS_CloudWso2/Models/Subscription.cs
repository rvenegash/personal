namespace exportaAPIS_CloudWso2.Models
{
    public class Subscription
    {
        public string subscriptionId { get; set; }
        public string tier { get; set; }
        public string apiIdentifier { get; set; }
        public string applicationId { get; set; }
        public string applicationName { get; set; }
        public string status { get; set; }
        public string subscriber { get; set; }
    }


    public class SubscriptionResult
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public Subscription[] list { get; set; }

    }
}