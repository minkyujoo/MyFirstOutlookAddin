using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MailReceivedEvent
{
    public class MailAIIntent
    {
        WatsonConversationHelper helper = new WatsonConversationHelper("76a465cb-b18a-43a0-9fd3-bf23b516a9c3", "8aa89312-8388-411a-bb59-3c5a097d398e", "TodLttJ1ikx7");

        const string WORDSECTION = "WordSection1"; // no need
        const double importanceLevel = 0.6;

        private string intent = string.Empty;

        /// <summary>
        /// mail item added event handler ****
        /// </summary>
        /// <param name="Item"></param>
        public string GetMailIent(string subject, string body)
        {
            string strReturn = string.Empty;
            string strBody = string.Empty;

            // subject intent
            IntentEntity subjectImportance = IsImportance(subject);
            if (subjectImportance != null)
            {
                return subjectImportance.returnMessage;
            }

            // body intent
            strBody = GetRealBody(body);
            List<string> sentences = GetSentences(strBody);
            List<IntentEntity> bodyIntents = new List<IntentEntity>();

            try
            {
                string strTemp = string.Empty;
                foreach (string sentence in sentences)
                {
                    strTemp = helper.GetAibrilResponse(sentence);
                    if (GetConfidenceFromResponse(strTemp) > importanceLevel && GetIntentFromResponse(strTemp).ToLower() != "other")
                    {
                        bodyIntents.Add(new IntentEntity(true, GetIntentFromResponse(strTemp), GetConfidenceFromResponse(strTemp)));
                    }
                }
            }
            catch (Exception e) // maybe network, timeout etc.
            {
                //Log(e.Message);
                throw e;
            }

            IntentEntity ieBody = IsImportance(bodyIntents);
            if (ieBody != null)
            {
                if (ieBody.confidence > importanceLevel)
                {
                    strReturn = ieBody.returnMessage;
                }
            }

            return strReturn;
        }

        /// <summary>
        /// 한 문장의 intent를 반환하는 method
        /// </summary>
        /// <param name="subject">문장하나</param>
        /// <returns></returns>
        public string GetMailIent(string subject)
        {
            IntentEntity ie = IsImportance(subject);
            if (ie != null)
            {
                return ie.returnMessage;
            }
            else
            {
                return string.Empty;
            }
        }

        #region "IsImportance"
        private IntentEntity IsImportance(string subject, List<IntentEntity> intents)
        {
            if (IsImportance(subject).isImportance) { return IsImportance(subject); }
            return IsImportance(intents);
        }

        private IntentEntity IsImportance(string subject)
        {
            // 1. 중요 말머리 경우
            string[] dicImportanceSubject = { "[중요]", "[Important]", "[긴급]", "[요청]", "[문의]", "[필수]" };
            IntentEntity roIntent = null;
            foreach (string dicImportant in dicImportanceSubject)
            {
                if (subject.StartsWith(dicImportant))
                {
                    roIntent = new IntentEntity(true, "dicImportant", 1, "Important(" + dicImportant + "): 100%");
                }
            }
            // 2. Aibril API
            string strTemp = helper.GetAibrilResponse(subject);
            if (GetConfidenceFromResponse(strTemp) > importanceLevel && GetIntentFromResponse(strTemp).ToLower() != "other")
            {
                roIntent = new IntentEntity(true, GetIntentFromResponse(strTemp), GetConfidenceFromResponse(strTemp));
            }
            return roIntent;
        }

        private IntentEntity IsImportance(List<IntentEntity> intents)
        {
            IntentEntity ie = null;
            // 1. sorting
            intents.Sort();
            if (intents.Count != 0)
            {
                if (intents[0].confidence > importanceLevel)
                {
                    ie = intents[0];
                }
            }
            // 2. sum by intent
            // for multi intents
            return ie;
        }
        #endregion

        #region "Utility"
        private List<string> GetSentences(string realBody)
        {
            List<string> sentences = new List<string>();
            // 정규표현식으로 바꿀 필요가 있음 ("\r\n 사이에 /s 아닌 글자들을 
            // MatchCollection으로 받아서 해당 Collection을 String[]으로 변환
            realBody = realBody.Replace("\r\n\r\n\r\n", "\r\n");
            realBody = realBody.Replace("\r\n\r\n", "\r\n");
            realBody = realBody.Replace("   ", " ");
            realBody = realBody.Replace("  ", " ");

            string[] arrSentences = realBody.Split("\r\n".ToCharArray());
            //string[] arrSentences = Regex.Split(realBody, @"(?<=[\.!\?])\s+");
            
            string strTemp = string.Empty;
            for (int i = 0; i < arrSentences.Length; i++)
            {
                strTemp = arrSentences[i].Trim();
                if (!string.IsNullOrEmpty(strTemp))
                {
                    sentences.Add(strTemp);
                }
            }
            return sentences;
        }

        private string GetIntentFromResponse(string strIntent)
        {
            string[] intConfidence = strIntent.Split(",".ToCharArray());
            string intString = intConfidence[0];
            return intString;
        }
        private double GetConfidenceFromResponse(string strIntent)
        {
            string[] intConfidence = strIntent.Split(",".ToCharArray());
            double decConfidence = Convert.ToDouble(intConfidence[1]);
            return decConfidence;
        }

        public string GetRealBody(string body)
        {
            string realBody = body;
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
        #endregion

    }
}