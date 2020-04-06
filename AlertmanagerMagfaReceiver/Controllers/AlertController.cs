using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AlertmanagerMagfaReceiver.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly SmsService _smsService;
        private readonly IOptionsMonitor<MagfaConfigs> _optionsMonitor;

        public AlertController(SmsService smsService, IOptionsMonitor<MagfaConfigs> optionsMonitor)
        {
            _smsService = smsService;
            _optionsMonitor = optionsMonitor;
        }

        [HttpGet("test")]
        public async Task<ActionResult> TestSms(string receiver, string message)
        {
            await _smsService.SendSms(message, new[] { receiver });
            return Ok();
        }

        [HttpPost]
        public async Task Alert([FromBody] WebhookPayload payload)
        {
            var tasks = new List<Task>();

            foreach (var payloadAlert in payload.Alerts)
            {
                var message =
                    $"{payloadAlert.Status}{Environment.NewLine}{payloadAlert.Annotations["summary"]}:{Environment.NewLine}{payloadAlert.Annotations["description"]}";
                tasks.Add(_smsService.SendSms(message, _optionsMonitor.CurrentValue.RecipientNumbers.ToArray()));
            }

            await Task.WhenAll(tasks);
        }
    }
}
