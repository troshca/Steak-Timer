using System;
using System.Collections.Generic;
using System.Text;

namespace SteakTimer.Models
{
    public class NotificationEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
