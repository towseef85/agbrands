////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;

////using Microsoft.Extensions.Configuration;

////using Newtonsoft.Json;

////using AGBrand.Packages.Contracts;
////using AGBrand.Packages.Models;

////namespace AGBrand.Packages.Helpers
////{
////    public sealed class EventHubHelper : IEventHubHelper<EventHubClient>
////    {
////        private readonly Dictionary<string, EventHubClient> _clients;

////        public EventHubHelper(IConfiguration configuration)
////        {
////            var configs = new Dictionary<string, EventHubConfig>
////            {
////                {
////                    Constants.EventHubs.DspDemuxerForSigfox,
////                    new EventHubConfig
////                    {
////                        ConnectionString=configuration["Dsp:DemuxerEndpoint"],
////                        EntityPath=configuration["Dsp:DemuxerEntityPath"]
////                    }}
////            };

////            _clients = new Dictionary<string, EventHubClient>();

////            foreach (var (key, value) in configs.Where(c => !string.IsNullOrWhiteSpace(c.Value.ConnectionString) &&
////                                                     !string.IsNullOrWhiteSpace(c.Value.EntityPath) &&
////                                                     !string.IsNullOrWhiteSpace(c.Key)))
////            {
////                var connectionStringBuilder = new EventHubsConnectionStringBuilder(value.ConnectionString)
////                {
////                    EntityPath = value.EntityPath
////                };

////                var client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

////                _clients.TryAdd(key, client);
////            }
////        }

////        public async Task SendAsync(string eventHubName, object message)
////        {
////            var messageString = JsonConvert.SerializeObject(message);

////            using (var eventData = new EventData(Encoding.UTF8.GetBytes(messageString)))
////            {
////                var client = _clients.ContainsKey(eventHubName) ? _clients.GetValueOrDefault(eventHubName, null) : null;

////                if (client != null)
////                {
////                    await client.SendAsync(eventData).ConfigureAwait(false);
////                }
////            }
////        }
////    }
////}
