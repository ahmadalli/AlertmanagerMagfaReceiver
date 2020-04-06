using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertmanagerMagfaReceiver
{
    public class WebhookPayload
    {
        public string Status { get; set; }
        public string Receiver { get; set; }
        public List<Alert> Alerts { get; set; }
    }

    public class Alert
    {
        public string Status { get; set; }
        public Dictionary<string, string> Labels { get; set; }
        public Dictionary<string, string> Annotations { get; set; }
    }
}
