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
            Microsoft.Office.Interop.Outlook.MailItem mail = (Microsoft.Office.Interop.Outlook.MailItem) Item;
            //var mail = (Outlook.OlItemType.olMailItem)Item;
            string subject = mail.Subject;

            //mail.BodyFormat = Outlook.OlBodyFormat.olFormatPlain;
            // MailItem에서 Body를 못 가져옵니다. // Office 2016 pro plus의 오류. ㅜㅡ; ****
            string body = mail.Body;
            string realBody = string.Empty;
            realBody = ai.GetRealBody(body);

            string strTemp = ai.GetMailIent(subject, realBody);
            if (!String.IsNullOrEmpty(strTemp))
            {
                mail.FlagStatus = Outlook.OlFlagStatus.olFlagMarked; 
                mail.Importance = Outlook.OlImportance.olImportanceHigh;
                mail.Categories = strTemp;
                mail.Save();

                // 여기서 좀 더 확장가능
                // Reqeust: To-do에 등록
                // Meeting Request: 일정에 등록 또는 "일정확정" 으로 to-do에 등록. 
                // Reply: 
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
