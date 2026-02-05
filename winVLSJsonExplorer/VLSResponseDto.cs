using System.Text.Json.Serialization;

namespace winVLSJsonExplorer
{
    public class VLSResponseDto
    {
        [JsonPropertyName("updateId")]
        public long UpdateId { get; set; }

        [JsonPropertyName("channels")]
        public List<Channel> Channels { get; set; }

        [JsonPropertyName("schedules")]
        public List<Schedule> Schedules { get; set; }

        [JsonPropertyName("programs")]
        public List<ProgramObj> Programs { get; set; }

        [JsonPropertyName("tiers")]
        public List<Tier> Tiers { get; set; }
    }

    public class Channel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("contentChannelId")]
        public int ContentChannelId { get; set; }

        //[JsonPropertyName("sourceId")]
        //public int SourceId { get; set; }

        [JsonPropertyName("vlsChannelId")]
        public string VlsChannelId { get; set; }

        [JsonPropertyName("vcId")]
        public int VcId { get; set; }

        [JsonPropertyName("vcNumber")]
        public int VcNumber { get; set; }

        // [JsonPropertyName("callSign")]
        // public string CallSign { get; set; }

        [JsonPropertyName("shortName")]
        public string ShortName { get; set; }

        // [JsonPropertyName("startDate")]
        // public DateTime StartDate { get; set; }

        // [JsonPropertyName("endDate")]
        // public DateTime EndDate { get; set; }

        [JsonPropertyName("css")]
        public int Css { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("channelType")]
        public string ChannelType { get; set; }
    }

    public class Schedule
    {
        [JsonPropertyName("channelId")]
        public string ChannelId { get; set; }

        [JsonPropertyName("scheduleId")]
        public string ScheduleId { get; set; }

        [JsonPropertyName("sourceId")]
        public int SourceId { get; set; }

        [JsonPropertyName("scheduleDate")]
        public DateTime ScheduleDate { get; set; }

        [JsonPropertyName("events")]
        public List<Event> Events { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("updatedDate")]
        public DateTime UpdatedDate { get; set; }
    }

    public class Event
    {
        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }

        // [JsonPropertyName("updatedDate")]
        // public DateTime UpdatedDate { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("vrioId")]
        public string VrioId { get; set; }

        [JsonPropertyName("numberId")]
        public long NumberId { get; set; }

        [JsonPropertyName("programId")]
        public string ProgramId { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("blackout")]
        public bool Blackout { get; set; }

        [JsonPropertyName("qualifiers")]
        public List<string> Qualifiers { get; set; }

        // [JsonPropertyName("variants")]
        // public List<Variant> Variants { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("purchaseInfo")]
        public PurchaseInfo PurchaseInfo { get; set; }
    }

    public class Variant
    {
        [JsonPropertyName("resolution")]
        public string Resolution { get; set; }

        [JsonPropertyName("variantId")]
        public string VariantId { get; set; }
    }

    public class PurchaseInfo
    {
        [JsonPropertyName("isPurchasable")]
        public bool IsPurchasable { get; set; }

        [JsonPropertyName("ippvId")]
        public int IppvId { get; set; }

        [JsonPropertyName("med")]
        public DateTime Med { get; set; }

        [JsonPropertyName("fed")]
        public DateTime Fed { get; set; }

        [JsonPropertyName("ippvExpireDt")]
        public DateTime IppvExpireDt { get; set; }

        [JsonPropertyName("smsCode")]
        public string SmsCode { get; set; }

        [JsonPropertyName("tierPrice")]
        public string TierPrice { get; set; }
    }

    public class ProgramObj
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("programId")]
        public string programId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("titles")]
        public List<Title> Titles { get; set; }

        [JsonPropertyName("descriptions")]
        public List<Description> Descriptions { get; set; }

        [JsonPropertyName("runTime")]
        public string RunTime { get; set; }

        [JsonPropertyName("originalAudiolang")]
        public string OriginalAudiolang { get; set; }

        // [JsonPropertyName("colorCode")]
        // public string ColorCode { get; set; }

        // [JsonPropertyName("releaseYear")]
        // public int ReleaseYear { get; set; }

        // [JsonPropertyName("images")]
        // public List<Image> Images { get; set; }

        // [JsonPropertyName("credits")]
        // public List<Credit> Credits { get; set; }

        [JsonPropertyName("genres")]
        public List<string> Genres { get; set; }

        [JsonPropertyName("genreDetails")]
        public List<GenreDetail> GenreDetails { get; set; }

        [JsonPropertyName("advisories")]
        public List<string> Advisories { get; set; }

        // [JsonPropertyName("externalRefs")]
        // public List<ExternalRef> ExternalRefs { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("updatedDate")]
        public DateTime UpdatedDate { get; set; }

        [JsonPropertyName("labels")]
        public List<Label> Labels { get; set; }
    }

    public class Title
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("lang")]
        public string Lang { get; set; }

        // [JsonPropertyName("type")]
        // public string Type { get; set; }

        // [JsonPropertyName("length")]
        // public string Length { get; set; }
    }

    public class Description
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("lang")]
        public string Lang { get; set; }

        // [JsonPropertyName("type")]
        // public string Type { get; set; }

        // [JsonPropertyName("length")]
        // public string Length { get; set; }
    }

    public class Image
    {
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("ratio")]
        public string Ratio { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("published")]
        public bool Published { get; set; }
    }

    public class Credit
    {
        [JsonPropertyName("personId")]
        public string PersonId { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("ord")]
        public string Ord { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("characterName")]
        public string CharacterName { get; set; }
    }

    public class GenreDetail
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("names")]
        public List<GenreName> Names { get; set; }
    }

    public class GenreName
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }
    }

    public class ExternalRef
    {
        [JsonPropertyName("system")]
        public string System { get; set; }

        [JsonPropertyName("refName")]
        public string RefName { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class Tier
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("tierName")]
        public string TierName { get; set; }

        [JsonPropertyName("tierInfoList")]
        public TierInfoList TierInfoList { get; set; }
    }

    public class TierInfoList
    {
        [JsonPropertyName("tierInfo")]
        public List<TierInfo> TierInfo { get; set; }
    }

    public class TierInfo
    {
        [JsonPropertyName("cbsId")]
        public int CbsId { get; set; }

        [JsonPropertyName("tierPerCbsId")]
        public int TierPerCbsId { get; set; }

        [JsonPropertyName("country")]
        public Country Country { get; set; }

        [JsonPropertyName("tierPrice")]
        public decimal TierPrice { get; set; }

        [JsonPropertyName("currencyId")]
        public int CurrencyId { get; set; }

        [JsonPropertyName("effectiveDt")]
        public DateTime EffectiveDt { get; set; }

        [JsonPropertyName("endDt")]
        public DateTime EndDt { get; set; }

    }

    public class Country
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Label
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

    }
}
