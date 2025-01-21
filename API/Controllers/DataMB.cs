using System.Text.Json.Serialization;

namespace API.Controllers
{
    public class LotoData
    {
        public int top_number { get; set; }
        public string last_number { get; set; }
    }

    public class LotoLast
    {
        public int last_number { get; set; }
        public string top_number { get; set; }
    }

    public class PrizeData
    {
        public int keyId { get; set; }
        public string value { get; set; }
    }

    public class DataPrize
    {
        public int Number { get; set; }
        public string Text { get; set; }
        public List<PrizeData> Data { get; set; }
    }

    public class DataEntry
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public List<LotoData> Loto_top { get; set; }
        public List<LotoLast> Loto_last { get; set; }
        public List<DataPrize> DataPrize { get; set; }
    }

    public class DataMB
    {
        public string Date { get; set; }
        public string Type { get; set; }
        public List<DataEntry> Datas { get; set; }
    }
    public class ResultData
    {
        [JsonPropertyName("Data")]
        public object Data { get; set; }

        [JsonPropertyName("Message")]
        public string Message { get; set; }

        [JsonPropertyName("Success")]
        public bool Success { get; set; }
    }
}
