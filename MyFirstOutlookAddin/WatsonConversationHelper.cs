using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace MailReceivedEvent
{
    class WatsonConversationHelper
    {
        private readonly string _Server;
        private readonly NetworkCredential _NetCredential;

        public WatsonConversationHelper(string workSpaceID, string userID, string password)
        {
            _Server = string.Format("https://gateway.aibril-watson.kr/assistant/api/v1/workspaces/{0}/message?version={1}", workSpaceID, DateTime.Today.ToString("yyyy-MM-dd"));
            _NetCredential = new NetworkCredential(userID, password);
        }

        public async Task<string> GetResponse(string input, string context = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            string req = null;

            if (string.IsNullOrEmpty(context)) req = "{\"input\": {\"text\": \"" + input + "\"}, \"alternate_intents\": true}";
            else req = "{\"input\": {\"text\": \"" + input + "\"}, \"alternate_intents\": true}, \"context\": \"" + context + "\"";

            using (var handler = new HttpClientHandler
            {
                Credentials = _NetCredential
            })

            using (var client = new HttpClient(handler))
            {
                var cont = new HttpRequestMessage();
                cont.Content = new StringContent(req.ToString(), Encoding.UTF8, "application/json");
                var result = await client.PostAsync(_Server, cont.Content);
                return await result.Content.ReadAsStringAsync();
            }
        }

        public string GetAibrilResponse(string workSpaceID, string userID, string password, string input)
        {
            //URL
            string strUri = string.Format("https://gateway.aibril-watson.kr/assistant/api/v1/workspaces/{0}/message?version={1}", workSpaceID, DateTime.Today.ToString("yyyy-MM-dd"));

            //보낼 데이터(사용자 input)
            StringBuilder dataParams = new StringBuilder();
            dataParams.Append("{\"input\": {\"text\": \"" + input + "\"}, \"alternate_intents\": true}");

            //String ==> Byte 변환
            byte[] byteDataParams = UTF8Encoding.UTF8.GetBytes(dataParams.ToString());
            HttpWebRequest requst = (HttpWebRequest)WebRequest.Create(strUri);
            requst.Method = "POST";
            requst.ContentType = "application/json";
            requst.ContentLength = byteDataParams.Length;
            requst.Credentials = new NetworkCredential(userID, password);

            //Byte ==> Stream 변환
            Stream stDataParams = requst.GetRequestStream();
            stDataParams.Write(byteDataParams, 0, byteDataParams.Length);
            stDataParams.Close();

            //request, response
            HttpWebResponse response = (HttpWebResponse)requst.GetResponse();

            //response stream 읽기
            Stream stReadData = response.GetResponseStream();
            StreamReader srReadData = new StreamReader(stReadData, Encoding.UTF8);

            //stream ==> string 변환
            string strResult = srReadData.ReadToEnd();

            AibrilResponseClass.RootObject responseToJson = JsonConvert.DeserializeObject<AibrilResponseClass.RootObject>(strResult);

            string intent = responseToJson.intents[0].intent;
            decimal confidence = responseToJson.intents[0].confidence;

            return intent + "," + confidence.ToString(); //modified
        }

        internal string GetAibrilResponse(string sentence)
        {
            // 입력 값들은 나중에 받아
            return GetAibrilResponse("76a465cb-b18a-43a0-9fd3-bf23b516a9c3", "8aa89312-8388-411a-bb59-3c5a097d398e", "TodLttJ1ikx7", sentence);
        }

    }
}


