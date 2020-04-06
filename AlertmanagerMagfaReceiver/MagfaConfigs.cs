using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertmanagerMagfaReceiver
{
    public class MagfaConfigs
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string SenderNumber { get; set; }
        public string CheckingMessageId { get; set; }
        public List<string> RecipientNumbers { get; set; } = new List<string>();
    }
}
