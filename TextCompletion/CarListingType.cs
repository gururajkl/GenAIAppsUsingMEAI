using System.Text.Json.Serialization;

namespace TextCompletion;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CarListingType { Sale, Lease }
