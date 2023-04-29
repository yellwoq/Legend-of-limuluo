
using UnityEngine;

namespace Fungus
{
    [EventHandlerInfo("Scene",
                   "My Message Received",
                   "The block will execute when the specified message is received from a Send Message command.")]
    [AddComponentMenu("")]
    class MyMessageReceived:MessageReceived
    {
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }
    }
}
