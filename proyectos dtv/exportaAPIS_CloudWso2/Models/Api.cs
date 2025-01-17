using System;
using System.Collections.Generic;
using System.Text;

namespace exportaAPIS_CloudWso2.Models
{
    public class Api
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string context { get; set; }
        public string version { get; set; }
        public string provider { get; set; }
        public string apiDefinition { get; set; }
        public string wsdlUri { get; set; }
        public string status { get; set; }
        public string responseCaching { get; set; }
        public int cacheTimeout { get; set; }
        public string destinationStatsEnabled { get; set; }
        public bool isDefaultVersion { get; set; }
        public string type { get; set; }
        public string[] transport { get; set; }
        public string[] tags { get; set; }
        public string[] tiers { get; set; }
        public MaxTps maxTps { get; set; }
        public string thumbnailUri { get; set; }
        public string visibility { get; set; }
        public string[] visibleRoles { get; set; }
        public string[] visibleTenants { get; set; }
        public string endpointConfig { get; set; }
        public EndpointSecurity endpointSecurity { get; set; }
        public string gatewayEnvironments { get; set; }
        public Sequences[] sequences { get; set; }
        public string subscriptionAvailability { get; set; }
        public string[] subscriptionAvailableTenants { get; set; }
        public BusinessInformation businessInformation { get; set; }
        public CorsConfiguration corsConfiguration { get; set; }

        public string apiLevelPolicy { get; set; }
        public string authorizationHeader { get; set; }
        public string apiSecurity { get; set; }
        public Labels[] labels { get; set; }
        public string accessControl { get; set; }
        public string[] accessControlRoles { get; set; }

    }
    public class Labels
    {
        public string name { get; set; }
        public string description { get; set; }

    }
    public class EndpointConfig
    {
        public string endpoint_type { get; set; }
        public EndpointConfigType production_endpoints { get; set; }
        public EndpointConfigType sandbox_endpoints { get; set; }
    }
    public class EndpointConfigType
    {
        public string url { get; set; }
        public string endpoint_type { get; set; }
        public EndpointConfigConfig config { get; set; }
        public bool template_not_supported { get; set; }
    }
    public class EndpointConfigConfig
    {
        public string format { get; set; }
        public string optimize { get; set; }
        public string actionSelect { get; set; }
        public string actionDuration { get; set; }
        public string suspendDuration { get; set; }
        public string suspendMaxDuration { get; set; }
    }

    public class ApiResult
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public Api[] list { get; set; }
        public Pagination pagination { get; set; }
    }

    public class Pagination
    {
        public int total { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }

    }

    public class BusinessInformation
    {
        public string businessOwner { get; set; }
        public string businessOwnerEmail { get; set; }
        public string technicalOwner { get; set; }
        public string technicalOwnerEmail { get; set; }
    }
    public class CorsConfiguration
    {
        public bool corsConfigurationEnabled { get; set; }
        public string[] accessControlAllowOrigins { get; set; }
        public bool accessControlAllowCredentials { get; set; }
        public string[] accessControlAllowHeaders { get; set; }
        public string[] accessControlAllowMethods { get; set; }

    }

    public class MaxTps
    {
        public int production { get; set; }
        public int sandbox { get; set; }

    }
    public class EndpointSecurity
    {
        public string type { get; set; }
        public string username { get; set; }
        public string password { get; set; }

    }

    public class Sequences
    {
        public string name { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public bool shared { get; set; }
    }
}
