using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MailReceivedEvent
{
    public partial class ThisAddIn
    {
        Outlook.NameSpace outlookNameSpace;
        Outlook.MAPIFolder inbox;
        Outlook.Items items;
        WatsonConversationHelper helper = new WatsonConversationHelper("76a465cb-b18a-43a0-9fd3-bf23b516a9c3", "8aa89312-8388-411a-bb59-3c5a097d398e", "TodLttJ1ikx7");

        const string WORDSECTION = "WordSection1";
        const double importanceLevel = 0.6;

        private string intent = string.Empty;
        private double confidence = 0.0d;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            //outlookNameSpace = this.Application.GetNamespace("MAPI");
            //inbox = outlookNameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
            //items = inbox.Items;

            // mail ItemAddEventHandler
            items.ItemAdd += new Outlook.ItemsEvents_ItemAddEventHandler(items_ItemAdd);
        }

        /// <summary>
        /// mail item added event handler ****
        /// </summary>
        /// <param name="Item"></param>
        private void items_ItemAdd(object Item)
        {
            Outlook.MailItem mail = (Outlook.MailItem)Item;

            string subject = mail.Subject;
            string bodyHTML = mail.Body;
            string realBody = string.Empty;

            realBody = GetRealBody(bodyHTML);

            // sentence array from sentence tokenizing
            string[] sentencesBody = GetSentences(realBody);
            string[] intentsBody = new String[sentencesBody.Length];
            string sentenceSubject = mail.Subject;
            string intentSubject = string.Empty;

            int i = 0;
            string intent = string.Empty;
            try
            {
                intentSubject = helper.GetAibrilResponse(sentenceSubject);

                foreach (string sentence in sentencesBody)
                {
                    // format: strIntent, decimalConfidence
                    intentsBody[i] = helper.GetAibrilResponse(sentence);
                    i++;
                }
            }catch (Exception e) // maybe network, timeout etc.
            {
                Log(e.Message);
            }

            // 중요한 메일일 경우 flag set, FlagIcon, Importance, Category, 후 save. ****
            if (IsImportance(intentSubject, intentsBody))
            {
                mail.FlagStatus = Outlook.OlFlagStatus.olFlagMarked; //ok
                //mail.FlagIcon = Outlook.OlFlagIcon.olYellowFlagIcon;
                //mail.FlagRequest = "중요함(FlagRequest)";
                mail.Importance = Outlook.OlImportance.olImportanceHigh;
                mail.Categories = this.intent + ": "+this.confidence.ToString();// intent 결과 표시.
                mail.Save();

                // 추가로 할일, 요청, 답변 --> to do task. 
                // 모임 요청 --> ???
            }
        }

        private bool IsImportance(string intentSubject, string[] intentsBody)
        {
            bool isImportance = false;
            // subject importance first
            // body importance 2nd
            if (IsImportance(intentSubject)) { return true; }
            if (IsImportance(intentsBody)) { isImportance = true; }
            return isImportance;
        }

        private bool IsImportance(string intentSubject)
        {
            string[] dicImportanceSubject = { "[중요]", "[Important]", "[긴급]", "[요청]", "[문의]", "[필수]" };
            bool isImportance = false;
            foreach (string dicImportant in dicImportanceSubject) {
                if (intentSubject.StartsWith(dicImportant)){ return true; }
            }
            return isImportance;
        }

        private bool IsImportance(string[] intents)
        {
            bool isImportance = false;
            foreach (string intent in intents)
            {
                if ((double)GetConfidenceFromResponse(intent) > importanceLevel && GetIntentFromResponse(intent) != "Other")
                {
                    isImportance = true;
                    this.intent = GetIntentFromResponse(intent);
                    this.confidence = (double)GetConfidenceFromResponse(intent);
                    break;
                }
            }
            return isImportance;
        }

        private string[] GetSentences(string realBody)
        {
            // 정규표현식으로 바꿀 필요가 있음 ("\r\n 사이에 /s 아닌 글자들을 
            // MatchCollection으로 받아서 해당 Collection을 String[]으로 변환
            realBody = realBody.Replace("\r\n\r\n\r\n", "\r\n");
            realBody = realBody.Replace("\r\n\r\n", "\r\n");
            realBody = realBody.Replace("   ", " ");
            realBody = realBody.Replace("  ", " ");

            string[] sentences = realBody.Split("\r\n".ToCharArray());
            // intents from aibril conversation
            string[] intents = new String[sentences.Length];

            for (int i= 0; i < intents.Length; i++) { intents[i] = intents[i].Trim(); }
            return intents;
        }

        private void Log(string message)
        {
            MessageBox.Show(message);
        }

        private string GetIntentFromResponse(string strIntent)
        {
            string[] intConfidence = strIntent.Split(",".ToCharArray());
            string intString = intConfidence[0];
            return intString;
        }
        private decimal GetConfidenceFromResponse(string strIntent)
        {
            string[] intConfidence = strIntent.Split(",".ToCharArray());
            decimal decConfidence = Convert.ToDecimal(intConfidence[1]);
            return decConfidence;
        }


        private string GetRealBody(string bodyHTML)
        {
            string realBody = bodyHTML;
            // to be implemented
            // 내용만 추출하는 방법. (구현필요)
            // 1. WordSection1을 찾는다.
            // 2. 감사합니다. 위 부분을 자른다.
            // 3. 가로줄 테그를 찾아서 자른다.
            //________________________________
            // 4. 잘라낸 text에서 테그를 제거한다.

            //realBody = "답변 요청 드립니다." +
            //"줄 넘김을 인식할지 모르겠네요. " +
            //"명시적으로 줄 넘김을 넣어 줘야 할 수도 있겠습니다. " +
            //"이렇게 아무말 써 놓고, 저 위의 핵심 문장을 이해할 수 있을지 모르겠네요. " +
            //"중요하지 않을 수도 있습니다." +
            //"말머리를 인식해야 할 수도 있겠네요.";

            return realBody;
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO에서 생성한 코드
        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        #endregion
    }
}
