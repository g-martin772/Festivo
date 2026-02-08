using Microsoft.Extensions.DependencyInjection;

namespace Festivo.Shared.Services;

public static class MessagingExtensions
{
    public static void AddMessaging(this IServiceCollection services, QueueBindingCollection queueBindings)
    {
        services.AddSingleton<EventBus>();
        services.AddHostedService(sp => sp.GetRequiredService<EventBus>());
        services.AddSingleton(queueBindings);
    }
}