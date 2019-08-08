using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailReceivedEvent
{
    public class MailIntent
    {
        WatsonConversationHelper helper = new WatsonConversationHelper("76a465cb-b18a-43a0-9fd3-bf23b516a9c3", "8aa89312-8388-411a-bb59-3c5a097d398e", "TodLttJ1ikx7");

        public string GetIntent(string subject, string mailbody)
        {
            string intent = string.Empty;

            //intent = helper.GetAibrilResponse("76a465cb-b18a-43a0-9fd3-bf23b516a9c3", "8aa89312-8388-411a-bb59-3c5a097d398e", "TodLttJ1ikx7", subject);

            return intent;
        }
    }
}
