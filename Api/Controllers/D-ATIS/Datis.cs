namespace Api.Controllers.DAtis
{
    internal class DAtis
    {
        [JsonPropertyName("airport")]
        public string Airport { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("datis")]
        public string Datis { get; set; }
    }
}
