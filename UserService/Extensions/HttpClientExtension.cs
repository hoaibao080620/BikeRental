namespace UserService.Extensions;

public static class HttpClientExtension
{
    public static void AddOktaAuthorizationHeader(this HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Add("Authorization", "SSWS 00Vd08EGEgzkzDBJhVtkKAI3n7wsrWiZNgqwzWyb0j");
    }
}