using System.Text;
using System.Text.Json;
using PlatformService.DTOs;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }
    public async Task SendPlatformToCommand(PlatformReadDTO plat)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(plat),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);


        if (response.IsSuccessStatusCode)
        {
            System.Console.WriteLine("Sync Post to Command Service was OK!");
        }
        else
        {
            System.Console.WriteLine("Bad request to Command");
        }
    }
}
