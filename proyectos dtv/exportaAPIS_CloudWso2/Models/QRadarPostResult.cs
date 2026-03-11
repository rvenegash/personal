using System.Collections.Generic;

namespace exportaAPIS_CloudWso2.Models
{
    internal class QRadarPostResult
    {
        public string search_id { get; set; }
        public string status { get; set; }
        public bool isValid { get; set; }
        public string errorDetail { get; set; }
    }

    internal class QRadarResults
    {
        public List<QRadarResultsEvent> events { get; set; }
    }

/// <summary>
/// \"Route Name\" AS 'Route_Name', \"userName\" AS 'Username', \"Sender Country\" AS 'Sender_Country', \"Metadata System ID\" AS 'Metadata_System_ID', \"Response Code\" AS 'Response_Code', \"date\" AS 'date'
/// </summary>
    internal class QRadarResultsEvent
    {
        public string Route_Name { get; set; }
        public string Username { get; set; }
        public string Sender_Country { get; set; }
        public string Metadata_System_ID { get; set; }
        public string Response_Code { get; set; }
        public string date { get; set; }
        public int Count { get; set; }
    }
}
