using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Settings
{
    public class EmailSettings
    {
        public string Sender { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
    }
}
