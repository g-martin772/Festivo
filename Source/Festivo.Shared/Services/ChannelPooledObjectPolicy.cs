using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

namespace Festivo.Shared.Services;

public class ChannelPooledObjectPolicy(IConnection connection) : IPooledObjectPolicy<IChannel>
{
    public IChannel Create()
    {
        return connection.CreateChannelAsync().Result;
    }

    public bool Return(IChannel obj)
    {
        if (obj.IsOpen)
            return true;
        
        obj?.Dispose();
        return false;
    }
}