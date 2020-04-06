using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AlertmanagerMagfaReceiver.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly SmsService _smsService;

        public AlertController(SmsService smsService)
        {
            _smsService = smsService;
        }

        [HttpGet("test")]
        public async Task<ActionResult> TestSms(string receiver, string message)
        {
            await _smsService.SendSms(message, new[] { receiver });
            return Ok();
        }
    }
}
