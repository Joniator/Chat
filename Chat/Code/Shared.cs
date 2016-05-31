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
    /// Log-Klasse um eine geordnete Ausgabe von Protokollierung bereitzustellen.
    /// </summary
    public static class Log
    {
        public static void WriteLine(string Message, params object[] param)
        {
            string message = string.Format(Message, param);
            Console.WriteLine(message);
        }
    }

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

        internal void Close()
        {
            streamReader.Close();
            streamWriter.Close();
        }

        public void Flush()
        {
            streamWriter.Flush();
        }
    }

    public static class Serializer
    {
        /// <summary>
        /// Serialisiert das aktuelle Objekt und gibt das Ergebnis als String zurück.
        /// </summary>
        /// <param name="indent">True wenn Zeilenumbrüche und Einrückungen beachtet werden sollen.</param>
        public static string Serialize<T>(T toSerialize, bool indent)
        {
            string serializedObject = null;
            if (toSerialize != null)
            {
                // Überprüft ob Zeilen
                if (indent)
                {
                    StringWriter stringWriter = new StringWriter();
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stringWriter, toSerialize);
                    serializedObject = stringWriter.ToString();

                    // Aufräumarbeiten
                    stringWriter.Close();
                }
                else
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
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(xmlWriter, toSerialize);
                    serializedObject = stringWriter.ToString();

                    // Aufräumarbeiten.
                    xmlWriter.Close();
                    stringWriter.Close();
                }
                return serializedObject;
            }
            return null;
        }

        /// <summary>
        /// Deserialisiert ein Objekt welches übergeben wird und gibt das Objekt zurück.
        /// </summary>
        /// <param name="toDeserialize">Das Serialisiert Objekt als string.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string toDeserialize)
        {
            T deserializedObject = default(T);

            try
            {
                // Erstellt einen StringReader, der das vom serializer deserialisierte Objekt enthält.
                StringReader reader = new StringReader(toDeserialize);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                deserializedObject = (T)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fehler beim Deserialisieren von {0}...: {1}", toDeserialize, e.Message);
            }
            return deserializedObject;

        }
    }

    public class Message
    {
        public Content content;

        public string sender;

        public DateTime sendTime;

        public Message() { }
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

        public Content() { }
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
        Image,
        Disconnect
    }
}
