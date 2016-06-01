using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace Chat
{
    public class MessageDatabase
    {
        static string saveFile = @"Chats.xmldb";
        public List<Message> messageHistory;
        public void Save()
        {
            try
            {
                if(Server.chatHistory == null)
                    messageHistory = new List<Message>();
                else
                    messageHistory = Server.chatHistory;
            }
            catch(Exception e)
            {
                Log.WriteLine("[MessageDatabase][{0}] {1}", DateTime.Now, e.InnerException);
            }

            StreamWriter streamWriter = new StreamWriter(saveFile, false);
            streamWriter.WriteLine(Serializer.Serialize<MessageDatabase>(this, true));
            streamWriter.Close();
        }

        public static MessageDatabase Load()
        {
            if(!File.Exists(saveFile))
            {
                new MessageDatabase()
                {
                    messageHistory = new List<Message>()
                }.Save();
            }
            StreamReader streamReader = new StreamReader(saveFile);
            string history = streamReader.ReadToEnd();
            streamReader.Close();
            return Serializer.Deserialize<MessageDatabase>(history);
        }
    }

}