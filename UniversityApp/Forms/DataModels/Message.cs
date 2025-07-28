using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApp.Forms.DataModels
{
    internal class Message
    {
        // Atributes
        private Person sender;
        private bool starred;
        private string messageText;
        private DateTime dateSent;
        // Constructor
        public Message(Person sender, string messageText, DateTime dateSent)
        {
            this.sender = sender;
            this.messageText = messageText;
            this.starred = false;
            this.dateSent = dateSent;
        }
        // Getters, Setters, and ToString()
        // You can't change the identity of whoever sent the message, the text of the message, or the date the messagwe was sent after the message is sent
        public Person GetSender()
        {
            return sender;
        }
        public bool IsStarred()
        {
            return starred;
        }
        public void SetStarred(bool starred)
        {
            this.starred = starred;
        }
        public string GetMessageText()
        {
            return messageText;
        }
        public DateTime GetDateSent()
        {
            return dateSent;
        }
        public override string ToString()
        {
            return $"From: {sender.GetName()}\nDate: {dateSent}\nMessage: {messageText}\nStarred: {starred}";
        }
    }
}
