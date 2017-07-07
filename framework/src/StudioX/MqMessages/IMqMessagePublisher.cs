using System.Threading.Tasks;

namespace StudioX.MqMessages
{
    public interface IMqMessagePublisher
    {
        void Publish(object mqMessages);

        Task PublishAsync(object mqMessages);
    }
}
