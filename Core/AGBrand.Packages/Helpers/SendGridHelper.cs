////using AGBrand.Packages.Contracts;
////using AGBrand.Packages.Models;

////namespace AGBrand.Packages.Helpers
////{
////    using System.Collections.Generic;
////    using System.Linq;
////    using System.Threading.Tasks;

////    using SendGrid;
////    using SendGrid.Helpers.Mail;

////    public class SendGridHelper : IMailer
////    {
////        private readonly string _apiKey;

////        public SendGridHelper(string apiKey)
////        {
////            _apiKey = apiKey;
////        }

////        public Task SendAsync(NotificationEmailAddress from, IEnumerable<NotificationEmailAddress> tos, string subject, string body)
////        {
////            var msg = new SendGridMessage();

////            msg.SetFrom(from.Email, from.Name);

////            msg.AddTos(tos.Select(c => new EmailAddress
////            {
////                Email = c.Email,
////                Name = c.Name
////            }).ToList());

////            msg.SetSubject(subject);

////            msg.AddContent(MimeType.Html, body);

////            var client = new SendGridClient(_apiKey);

////            return client.SendEmailAsync(msg);
////        }
////    }
////}
