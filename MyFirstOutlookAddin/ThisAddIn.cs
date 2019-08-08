using System;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Windows.Forms;

namespace MailReceivedEvent
{
    public partial class ThisAddIn
    {
        Outlook.NameSpace outlookNameSpace;
        Outlook.MAPIFolder inbox;
        Outlook.Items items;

        MailAIIntent ai = new MailAIIntent();

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            outlookNameSpace = this.Application.GetNamespace("MAPI");
            inbox = outlookNameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
            items = inbox.Items;

            items.ItemAdd += new Outlook.ItemsEvents_ItemAddEventHandler(items_ItemAdd);
        }

        private void items_ItemAdd(object Item)
        {

            Outlook.MailItem mail = (Outlook.MailItem)Item;

            string subject = mail.Subject;
            string body = mail.Body;
            string realBody = string.Empty;

            realBody = ai.GetRealBody(body);

            string strTemp = ai.GetMailIent(subject, body);
            if (!String.IsNullOrEmpty(strTemp))
            {
                mail.FlagStatus = Outlook.OlFlagStatus.olFlagMarked; 
                mail.Importance = Outlook.OlImportance.olImportanceHigh;
                mail.Categories = strTemp;
                mail.Save();
            }
        }

        private void Log(string message)
        {
            MessageBox.Show(message);
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

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }
        #endregion
    }
}
