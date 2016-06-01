using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Chat
{
    public class Message
    {
        public Content content;

        public string sender;

        public DateTime sendTime;

        public Message()
        {
        }
        public Message(Content content, string sender, DateTime sendTime)
        {
            this.content = content;
            this.sender = sender;
            this.sendTime = sendTime;
        }
        public override string ToString()
        {
            return Serializer.Serialize(this, false);
        }
    }

    public class Content
    {
        public ContentType type;
        public string[] parameter;

        public Content()
        {
        }
        public Content(ContentType Command, params string[] Parameter)
        {
            parameter = Parameter;
            type = Command;
        }
    }

    public enum ContentType
    {
        Register,
        RequestChat,
        Login,
        Message,
        Kicked,
        Disconnect
    }
}