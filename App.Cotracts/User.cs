using System.Net;
using System.Text.Json.Serialization;

namespace App.Contracts
{
    public record User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public IPEndPoint? EndPoint { get; set; }
    }
}


