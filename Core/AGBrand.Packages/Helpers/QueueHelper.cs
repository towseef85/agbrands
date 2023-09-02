////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;

////using Newtonsoft.Json;

////using AGBrand.Packages.Contracts;

////namespace AGBrand.Packages.Helpers
////{
////    using System.Collections.Generic;

////    using Microsoft.Azure.ServiceBus;
////    using Microsoft.Extensions.Configuration;

////    public sealed class QueueHelper : IQueueHelper<QueueClient>
////    {
////        private readonly Dictionary<string, QueueClient> _queueClients;

////        public QueueHelper(IConfiguration configuration)
////        {
////            var queueConfigs = new Dictionary<string, string>
////            {
////                { configuration["Settings:CollateAndProcessQueueName"], configuration["Settings:ServiceBusConnectionString"]},
////                { configuration["PlatformServices:NotificationService:QueueName"], configuration["PlatformServices:NotificationService:QueueConnectionString"]}
////            };

////            _queueClients = new Dictionary<string, QueueClient>();

////            foreach (var (key, value) in queueConfigs.Where(c => !string.IsNullOrWhiteSpace(c.Value) &&
////                                                                !string.IsNullOrWhiteSpace(c.Key)))
////            {
////                var queueClient = new QueueClient(value, key);

////                _queueClients.TryAdd(key, queueClient);
////            }
////        }

////        public async Task SendAsync(string queueName, object message)
////        {
////            var messageSerialized = JsonConvert.SerializeObject(message);

////            var queueMessage = new Message(Encoding.UTF8.GetBytes(messageSerialized));

////            var client = _queueClients.ContainsKey(queueName) ? _queueClients.GetValueOrDefault(queueName, null) : null;

////            if (client != null)
////            {
////                await client.SendAsync(queueMessage).ConfigureAwait(false);
////            }
////        }
////    }
////}
