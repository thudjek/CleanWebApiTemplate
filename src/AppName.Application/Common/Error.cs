using System.Text.Json.Serialization;

namespace AppName.Application.Common;
public class Error
{
    public Error(string error)
    {
        ErrorMessage = error;
    }

    public Error(Dictionary<string, List<string>> validationErrors)
    {
        ErrorMessage = "One or more validation errors occurred";
        ValidationErrors = validationErrors;
    }

    public string ErrorMessage { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, List<string>> ValidationErrors { get; }
}