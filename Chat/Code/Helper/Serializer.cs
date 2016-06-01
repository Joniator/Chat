using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Chat
{
    public static class Serializer
    {
        /// <summary>
        /// Serialisiert das aktuelle Objekt und gibt das Ergebnis als String zurück.
        /// </summary>
        /// <param name="indent">True wenn Zeilenumbrüche und Einrückungen beachtet werden sollen.</param>
        public static string Serialize<T>(T toSerialize, bool indent)
        {
            string serializedObject = null;
            if(toSerialize != null)
            {
                /// Überprüft ob Zeilenumbrüche und Einrückungen eingefügt werden sollen.
                /// Die Einrückung würde StreamReader.ReadLine() nur die erste Zeile auslesen lassen, was nicht ausreicht 
                /// und zu Fehlern führen kann, weil eventuell Zeilen verloren gehen, weswegen sie für das Versenden deaktiviert werden können,
                /// die Dateien der Übersichtlichkeit halber aber formatiert werden.
                if(indent)
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
            /// Da die ReadLine-Methode bei einer Fehlermeldung einen Leeren String zurückgibt wird für diesen Fall 
            /// ein default-Message-Objekt 
            if(toDeserialize != "")
            {
                try
                {
                    // Erstellt einen StringReader, der das vom serializer deserialisierte Objekt enthält.
                    StringReader reader = new StringReader(toDeserialize);
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    deserializedObject = (T)serializer.Deserialize(reader);
                }
                catch(Exception e)
                {
                    Console.WriteLine("[Serializer][{0}] Fehler beim Deserialisieren von {1}...: {2}",DateTime.Now, toDeserialize, e.Message);
                }
            }
            return deserializedObject;
        }
    }

}