using System.Threading.Tasks;

namespace StudioX.MqMessages
{
    public class NullMqMessagePublisher : IMqMessagePublisher
    {
        public static NullMqMessagePublisher Instance { get; } = new NullMqMessagePublisher();

        public Task PublishAsync(object mqMessages)
        {
            // Do nothing.
            return Task.FromResult(0);
        }
        
        public void Publish(object mqMessages)
        {
            // Do nothing.
        }
    }
}
