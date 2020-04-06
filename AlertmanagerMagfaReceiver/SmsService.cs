using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using Magfa;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlertmanagerMagfaReceiver
{
    public class SmsService
    {
        private readonly IOptionsMonitor<MagfaConfigs> _optionsMonitor;
        private readonly ILogger<SmsService> _logger;

        private SoapSmsQueuableImplementationClient _smsQueueClient;

        public SmsService(IOptionsMonitor<MagfaConfigs> optionsMonitor, ILogger<SmsService> logger)
        {
            _optionsMonitor = optionsMonitor;
            _logger = logger;

            _optionsMonitor.OnChange(initClient);
            initClient(_optionsMonitor.CurrentValue);
        }

        private void initClient(MagfaConfigs configs)
        {
            _smsQueueClient = new SoapSmsQueuableImplementationClient();
            _smsQueueClient.ClientCredentials.UserName.UserName = configs.Username;
            _smsQueueClient.ClientCredentials.UserName.Password = configs.Password;
        }

        public async Task SendSms(string text, string[] recipients)
        {
            var request = new enqueueRequest()
            {
                domain = _optionsMonitor.CurrentValue.Domain,
                senderNumbers = new[] { _optionsMonitor.CurrentValue.SenderNumber },
                recipientNumbers = recipients,
                messageBodies = new[] { text },
                checkingMessageIds = new[] { _optionsMonitor.CurrentValue.CheckingMessageId }
            };

            var resultCode = (await _smsQueueClient.enqueueAsync(request)).enqueueReturn[0];
            ensureSuccessResultCode(resultCode);
        }

        private static void ensureSuccessResultCode(long code)
        {
            if (code >= 1000)
            {
                return;
            }
            switch (code)
            {
                case -1:
                    throw new MagfaServiceException("The target of report is not available(e.g. no message is associated with entered IDs)");
                case 1:
                    throw new MagfaServiceException("the Strings You presented as recipient numbers are not valid phone numbers, please check them again");
                case 2:
                    throw new MagfaServiceException("the Strings You presented as sender numbers(3000-blah blah blahs) are not valid numbers, please check them again");
                case 3:
                    throw new MagfaServiceException("are You sure You've entered the right encoding for this message? You can try other encodings to bypass this error code");
                case 4:
                    throw new MagfaServiceException("entered MessageClass is not valid. for a normal MClass, leave this entry empty");
                case 6:
                    throw new MagfaServiceException("entered UDH is invalid. in order to send a simple message, leave this entry empty");
                case 12:
                    throw new MagfaServiceException("you're trying to use a service from another account??? check your UN/Password/NumberRange again");
                case 13:
                    throw new MagfaServiceException("check the text of your message. it seems to be null.");
                case 14:
                    throw new MagfaServiceException("Your credit's not enough to send this message. you might want to buy some credit.call ");
                case 15:
                    throw new MagfaServiceException("something bad happened on server side, you might want to call MAGFA Support about this:");
                case 16:
                    throw new MagfaServiceException("Your account is not active right now, call -- to activate it");
                case 17:
                    throw new MagfaServiceException("looks like Your account's reached its expiration time, call -- for more information");
                case 18:
                    throw new MagfaServiceException("the combination of entered Username/Password/Domain is not valid. check them again");
                case 19:
                    throw new MagfaServiceException("You're not entering the correct combination of Username/Password");
                case 20:
                    throw new MagfaServiceException("check the service type you're requesting. we don't get what service you want to use. your sender number might be wrong, too.");
                case 22:
                    throw new MagfaServiceException("your current number range doesn't have the permission to use Webservices");
                case 23:
                    throw new MagfaServiceException("Sorry, Server's under heavy traffic pressure, try testing another time please");
                case 24:
                    throw new MagfaServiceException("entered message-id seems to be invalid, are you sure You entered the right thing?");
                case 106:
                    throw new MagfaServiceException("array of recipient numbers must have at least one member");
                case 107:
                    throw new MagfaServiceException("the maximum number of recipients per message is 90");
                case 108:
                    throw new MagfaServiceException("array of sender numbers must have at least one member");
                case 103:
                    throw new MagfaServiceException("This error happens when you have more than one " +
                            "sender-number for message. when you have more than one sender number, for each sender-number you must " +
                            "define a recipient number...");
                case 101:
                    throw new MagfaServiceException("when you have N > 1 texts to send, you have to define N recipient-numbers...");
                case 104:
                    throw new MagfaServiceException("this happens when you try to define UDHs for your messages. in this case you must define one recipient number for each udh");
                case 102:
                    throw new MagfaServiceException("this happens when you try to define MClasses for your messages. in this case you must define one recipient number for each MClass");
                case 109:
                    throw new MagfaServiceException("this happens when you try to define encodings for your messages. in this case you must define one recipient number for each Encoding");
                case 110:
                    throw new MagfaServiceException("this happens when you try to define checking-message-ids for your messages. in this case you must define one recipient number for each checking-message-id");
                default:
                    throw new MagfaServiceException($"unknown error happened. code: {code}");
            }
        }
    }

    class MagfaServiceException : Exception
    {
        public MagfaServiceException(string message) : base(message)
        {

        }
    }
}
