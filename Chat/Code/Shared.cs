using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Chat
{
    /// <summary>
    /// Kombinierter StreamReader und StreamWriter mit vereinfachter Funktionalität.
    /// </summary>
    public class StreamRW
    {
        StreamReader streamReader;
        StreamWriter streamWriter;

        /// <summary>
        /// Ruft einen Wert ab, der angibt, ob der StreamWriter nach jedem Aufruf
        /// von StreamRW.WriteLine(System.String) den Puffer in den zugrunde liegenden
        /// Stream wegschreibt, oder legt diesen fest.
        /// </summary>
        public bool AutoFlush
        {
            set
            {
                streamWriter.AutoFlush = value;
            }
            get
            {
                return streamWriter.AutoFlush;
            }
        }

        public StreamRW(Stream stream)
        {
            streamReader = new StreamReader(stream);
            streamWriter = new StreamWriter(stream);
            AutoFlush = true;
        }

        /// <summary>
        /// Liest eine Zeile von Zeichen aus dem aktuellen Stream und gibt die Daten als Zeichenfolge zurück.
        /// </summary>
        /// <returns>Die nächste Zeile des Eingabestreams, bzw. null, wenn das Ende des Eingabestreams erreicht ist.</returns>
        public string ReadLine()
        {
            return streamReader.ReadLine();
        }

        /// <summary>
        /// Schreibt eine Zeichenfolge, gefolgt von einem Zeichen für den Zeilenabschluss, in die Textzeichenfolge oder den Stream.
        /// </summary>
        /// <param name="Value">Die zu schreibende Zeichenfolge. Wenn value null ist, wird nur das Zeichen für den Zeilenabschluss geschrieben.</param>
        public void WriteLine(string Value)
        {
            streamWriter.WriteLine(Value);
        }

        public void Flush()
        {
            streamWriter.Flush();
        }
    }

    public class Message
    {
        public Command content;

        public string sender;

        public DateTime sendTime;

        public Message() { }
        public Message(Command content, string sender, DateTime sendTime)
        {
            this.content = content;
            this.sender = sender;
            this.sendTime = sendTime;
        }

        public override string ToString()
        {
            return MessageSerializer.Serialize(this);
        }
    }

    static class MessageSerializer
    {
        /// <summary>
        /// Serialisiert das aktuelle Objekt und gibt das Ergebnis als String zurück.
        /// </summary>
        /// <returns></returns>
        public static string Serialize(Message toSerialize)
        {
            string serializedObject;
            if (toSerialize != null)
            {
                    // Erstellt einen StringWriter der später die Ausgabe des Objektes übernimmt.
                    StringWriter stringWriter = new StringWriter();

                    // Erstellt einen XMLWriter, der den XML-Code formatiert und Zeilenumbrüche entfernt,
                    // und gibt den neuen Code an den StringWriter weiter.
                    XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings()
                    {
                        Indent = false,
                        NewLineHandling = NewLineHandling.None,
                        NewLineOnAttributes = false
                    });

                    // Erstellt einen Serializer, der das Objekt serialisiert und an den XMLWriter übergibt, der sich um die Formatierung kümmert.
                    XmlSerializer serializer = new XmlSerializer(typeof(Message));
                    serializer.Serialize(xmlWriter, toSerialize);
                    serializedObject = stringWriter.ToString();

                    // Aufräumarbeiten.
                    xmlWriter.Close();
                    stringWriter.Close();

                return serializedObject;
            }
            return null;
        }

        /// <summary>
        /// Deserialisiert ein Objekt welches übergeben wird und gibt das Objekt zurück.
        /// </summary>
        /// <param name="toDeserialize">Das Serialisiert Objekt als string.</param>
        /// <returns></returns>
        public static Message Deserialize(string toDeserialize)
        {
            Message deserializedObject;

            try
            {
                // Erstellt einen StringReader, der das vom serializer deserialisierte Objekt enthält.
                StringReader reader = new StringReader(toDeserialize);
                XmlSerializer serializer = new XmlSerializer(typeof(Message));
                deserializedObject = (Message)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fehler beim Deserialisieren von {0}...: {1}", toDeserialize, e.Message);
                return null;
            }

            return deserializedObject;

        }
    }

    public class Command
    {
        public CommandType type;
        public string[] parameter;

        public Command() { }
        public Command(CommandType Command, params string[] Parameter)
        {
            parameter = Parameter;
            type = Command;            
        }
    }

    public enum CommandType
    {
        Login,
        Message,
        Disconnect
    }
}
