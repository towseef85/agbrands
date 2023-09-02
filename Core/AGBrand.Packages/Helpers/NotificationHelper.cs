////using AGBrand.Packages.Contracts.Notification;

////namespace AGBrand.Packages.Helpers
////{
////    using Contracts;
////    using Microsoft.Extensions.Configuration;
////    using Models;
////    using System.Collections.Generic;
////    using System.Linq;
////    using System.Threading.Tasks;

////    public sealed class NotificationHelper : INotificationHelper
////    {
////        private readonly IConfiguration _configuration;
////        private readonly IQueueHelper<QueueClient> _queueHelper;

////        public NotificationHelper(IQueueHelper<QueueClient> queueHelper, IConfiguration configuration)
////        {
////            _queueHelper = queueHelper;
////            _configuration = configuration;
////        }

////        public async Task EmailAsync(IEnumerable<NotificationEmailAddress> tos, IEnumerable<NotificationEmailAddress> bccs, string body, string subject)
////        {
////            var emailNotification = new NotificationMessage<NotificationEmail>
////            {
////                NotificationType = NotificationType.Email,
////                Payload = new NotificationEmail
////                {
////                    From = _configuration["Settings:AppEmail"],
////                    Name = _configuration["Settings:AppName"],
////                    Content = body,
////                    Subject = subject,
////                    Tos = tos.Select(c => c.Email).Distinct().ToList(),
////                    Bccs = bccs.Select(c => c.Email).Distinct().ToList()
////                }
////            };

////            if (_queueHelper != null)
////            {
////                await _queueHelper.SendAsync(_configuration["PlatformServices:NotificationService:QueueName"], emailNotification).ConfigureAwait(false);
////            }
////        }

////        public async Task NotifyBccsAsync(NotificationAddresses notificationAddresses, string emailBody, string smsBody, string subject)
////        {
////            var notificationMessage = new NotificationMessage<dynamic>
////            {
////                NotificationType = NotificationType.All,
////                Payload = new NotificationAll
////                {
////                    Email = new NotificationEmail
////                    {
////                        From = _configuration["Settings:AppEmail"],
////                        Name = _configuration["Settings:AppName"],
////                        Content = emailBody,
////                        Subject = subject,
////                        Bccs = notificationAddresses.Emails.Distinct().ToList(),
////                        Tos = Enumerable.Empty<string>().ToList()
////                    },
////                    Sms = new NotificationSms
////                    {
////                        Content = smsBody,
////                        Tos = notificationAddresses.Mobiles.Distinct().ToList()
////                    }
////                }
////            };

////            if (_queueHelper != null)
////            {
////                await _queueHelper.SendAsync(_configuration["PlatformServices:NotificationService:QueueName"], notificationMessage).ConfigureAwait(false);
////            }
////        }

////        public Task EmailAsync(IEnumerable<NotificationEmailAddress> tos, string body, string subject)
////        {
////            return EmailAsync(tos, Enumerable.Empty<NotificationEmailAddress>().ToList(), body, subject);
////        }

////        public Task EmailBccAsync(IEnumerable<NotificationEmailAddress> bccs, string body, string subject)
////        {
////            return EmailAsync(Enumerable.Empty<NotificationEmailAddress>().ToList(), bccs, body, subject);
////        }

////        public async Task SmsAsync(List<string> mobiles, string sms)
////        {
////            var smsNotification = new NotificationMessage<NotificationSms>
////            {
////                NotificationType = NotificationType.Email,
////                Payload = new NotificationSms
////                {
////                    Content = sms,
////                    Tos = mobiles.Distinct().ToList()
////                }
////            };

////            if (_queueHelper != null)
////            {
////                await _queueHelper.SendAsync(_configuration["PlatformServices:NotificationService:QueueName"], smsNotification).ConfigureAwait(false);
////            }
////        }
////    }
////}
