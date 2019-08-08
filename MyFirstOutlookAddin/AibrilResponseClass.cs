using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailReceivedEvent
{
    class AibrilResponseClass : IDisposable
    {
        public class Intent
        {
            public string intent { get; set; }
            public decimal confidence { get; set; }
        }

        public class Input
        {
            public string text { get; set; }
        }

        public class LogMessage
        {
            public string level { get; set; }
            public string msg { get; set; }
        }

        public class Output
        {
            public List<object> generic { get; set; }
            public List<object> text { get; set; }
            public List<object> nodes_visited { get; set; }
            public string warning { get; set; }
            public List<LogMessage> log_messages { get; set; }
        }

        public class DialogStack
        {
            public string dialog_node { get; set; }
        }

        public class System
        {
            public bool initialized { get; set; }
            public List<DialogStack> dialog_stack { get; set; }
            public int dialog_turn_counter { get; set; }
            public int dialog_request_counter { get; set; }
        }

        public class Context
        {
            public string conversation_id { get; set; }
            public System system { get; set; }
        }

        public class RootObject
        {
            public List<Intent> intents { get; set; }
            public List<object> entities { get; set; }
            public Input input { get; set; }
            public Output output { get; set; }
            public Context context { get; set; }
            public bool alternate_intents { get; set; }
        }

        public virtual void Dispose()
        {

        }
    }
}
