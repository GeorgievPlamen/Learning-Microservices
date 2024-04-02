using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;
    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _mapper = mapper;
        _scopeFactory = scopeFactory;
    }
    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;
            default:
                break;
        }
    }

    private void AddPlatform(string platformPublishedMessage)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

            var platformPublishedDTO = JsonSerializer.Deserialize<PlatformPublishedDTO>(platformPublishedMessage);

            try
            {
                var plat = _mapper.Map<Platform>(platformPublishedDTO);
                if (!repo.ExternalPlatformExists(plat.ExternalId))
                {
                    repo.CreatePlatform(plat);
                    repo.SaveChanges();
                    System.Console.WriteLine("Platform added!");
                }
                else
                {
                    System.Console.WriteLine("Platform already exists...");
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Couldnt add platform to DB " + ex);
            }

        }
    }
    private EventType DetermineEvent(string notificationMessage)
    {
        System.Console.WriteLine("Determing Event");
        var eventType = JsonSerializer.Deserialize<GenericEventDTO>(notificationMessage);

        switch (eventType!.Event)
        {
            case "Platform_Published":
                System.Console.WriteLine("Platform Published event detected");
                return EventType.PlatformPublished;
            default:
                System.Console.WriteLine("Couldn't determine event type");
                return EventType.Undetermined;
        }
    }
}

enum EventType
{
    PlatformPublished,
    Undetermined
}
