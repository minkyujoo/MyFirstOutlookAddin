using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailReceivedEvent
{

    public class IntentEntity
    {
        public IntentEntity(bool isImportance, string intent, double confidence, string returnMessage)
        {
            this.isImportance = isImportance;
            this.intent = intent;
            this.confidence = confidence;
            this.returnMessage = returnMessage;
        }
        public IntentEntity(bool isImportance, string intent, double confidence)
        {
            this.isImportance = isImportance;
            this.intent = intent;
            this.confidence = confidence;
            this.returnMessage = intent + ": " + (confidence * 100).ToString() + "%"; // 이거는 나중에 tuning 필요
        }

        public IntentEntity(string intent, double confidence)
        {
            this.isImportance = false;
            this.intent = intent;
            this.confidence = confidence;
            this.returnMessage = string.Empty;
        }
        public IntentEntity()
        {
            this.isImportance = false;
            this.intent = string.Empty;
            this.confidence = 0.0d;
            this.returnMessage = string.Empty;
        }

        public bool isImportance { get; set; }
        public string intent { get; set; }
        public double confidence { get; set; }
        public string returnMessage { get; set; }
    }
}