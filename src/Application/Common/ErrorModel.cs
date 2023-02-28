using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Common;
public class ErrorModel
{
    public ErrorModel()
    {
        Error = "Something went wrong, please try again.";
    }

    public ErrorModel(string error)
    {
        Error = error;
    }

    public ErrorModel(string error, Dictionary<string, string[]> validationErrors)
    {
        Error = error;
        ValidationErrors = validationErrors;
    }

    public string Error { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string[]> ValidationErrors { get; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}