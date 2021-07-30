using System;
using System.Collections.Generic;
using System.Web.Http;
//using camp.health.local.Controllers.MVC;
using MessageMedia.Messages;
using MessageMedia.Messages.Controllers;
using MessageMedia.Messages.Exceptions;
using MessageMedia.Messages.Models;


namespace MessageMediaApiProxy.Controllers
{
    public class MessageReceived
    {
        // the phone number of the sms message destination
        public string PhoneNumber { get; set; }

        // the content of the sms message body
        public string MessageContent { get; set; }
    }


    public class MessageMediaProxyController : ApiController
    {
        readonly string API_KEY = "";       // replace with the MessageMedia API Key
        readonly string API_SECRET = "";    // replace with the MessageMedia API Secret
        //readonly string API_KEY = SystemSettingsController.GetSystemSetting("CAMP", "MessageMediaApiKey");
        //readonly string API_SECRET = SystemSettingsController.GetSystemSetting("CAMP", "MessageMediaApiSecreat");
        readonly Boolean HMAC = false;

        // POST: api/MessageMediaProxy
        [HttpPost]
        public string Post(string smsMessageCode, [FromBody] List<string> phoneNumbers)
        {
            MessageMediaMessagesClient client = new MessageMediaMessagesClient(API_KEY, API_SECRET, HMAC);

            MessagesController messages = client.Messages;

            string SMSContent = "";     // replace with the sms message content template
            //string SMSContent = SMSMessagesController.GetSMSMessageContent(smsMessageCode);
            SMSContent = SMSContent.Replace("{SystemDateTime}", DateTime.Now.ToString());

            SendMessagesRequest body = new SendMessagesRequest();
            body.Messages = new List<Message>();

            
            // loop through the phoneNumbers list to populate the Messages object
            for (int i = 0; i < phoneNumbers.Count; i++)
            {
                Message body_messages = new Message();
                body_messages.Content = SMSContent;
                body_messages.DestinationNumber = phoneNumbers[i];
                body.Messages.Add(body_messages);
            }

            try
            {
                // use the MessageMedia VS Extension to call the MessageMedia API POST method to send messages
                SendMessagesResponse result = messages.SendMessagesAsync(body).Result;
                return "Messages successfully sent.";
            }
            catch (APIException e)
            {
                return e.Message + e.ResponseCode + e.HttpContext.ToString();
            }
        }
    }
}