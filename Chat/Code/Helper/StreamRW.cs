using System;
using System.Linq;
using System.Collections.Generic;
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
            try
            {
                streamReader.BaseStream.ReadTimeout = 2000;
                return streamReader.ReadLine();
            }
            catch(Exception e)
            {
                Log.WriteLine("[StreamRW][{0}] {1}", DateTime.Now, e.InnerException);
                return "";
            }
        }

        /// <summary>
        /// Liest eine Zeile von Zeichen aus dem aktuellen Stream und gibt die Daten als Zeichenfolge zurück, und Timet bereits nach 0.2 Sekunden aus.
        /// </summary>
        /// <returns>Die nächste Zeile des Eingabestreams, bzw. null, wenn das Ende des Eingabestreams erreicht ist.</returns>
        public string ReadLineFastTO()
        {
            try
            {
                streamReader.BaseStream.ReadTimeout = 200;
                return streamReader.ReadLine();
            }
            catch(Exception e)
            {
                Log.WriteLine("[StreamRW][{0}] {1}", DateTime.Now, e.InnerException);
                return "";
            }
        }

        /// <summary>
        /// Schreibt eine Zeichenfolge, gefolgt von einem Zeichen für den Zeilenabschluss, in die Textzeichenfolge oder den Stream.
        /// </summary>
        /// <param name="Value">Die zu schreibende Zeichenfolge. Wenn value null ist, wird nur das Zeichen für den Zeilenabschluss geschrieben.</param>
        public void WriteLine(string Value)
        {
            try
            {
                streamWriter.WriteLine(Value);
            }
            catch (Exception e)
            {
                Log.WriteLine("[StreamRW][{0}] {1}", DateTime.Now, e.InnerException);
            }
        }

        /// <summary>
        /// Schließt die zugrundeliegenden Streams und Reader/Writer.
        /// </summary>
        public void Close()
        {
            streamReader.Close();
            streamWriter.Close();
        }

        /// <summary>
        /// Schreibt alle Daten auf den Stream.
        /// </summary>
        public void Flush()
        {
            streamWriter.Flush();
        }
    }

}