using System.Text.Json.Serialization;

record TokenResponse {
    [JsonPropertyName("token_type")]
    
    public string TokenType { get; init; }

    [JsonPropertyName("access_token")]

    public string AccessToken { get; init; }

}