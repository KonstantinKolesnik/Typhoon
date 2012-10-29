using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Typhoon.Net
{
    public class NetworkMessage
    {
        #region Fields
        private string id;
        private Dictionary<string, string> parameters = new Dictionary<string, string>();
        #endregion

        #region Properties
        public string ID
        {
            get { return id; }
        }
        public string this[string parameterName]
        {
            get
            {
                string val;
                return parameters.TryGetValue(parameterName, out val) ? val : null;
            }
            set
            {
                if (parameters.ContainsKey(parameterName))
                    parameters[parameterName] = value;
                else
                    parameters.Add(parameterName, value);
            }
        }
        #endregion

        #region Constructors
        public NetworkMessage(string id)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentNullException("NetworkMessage ID not specified");

            this.id = id;
        }
        #endregion

        #region Public Methods
        public string ToXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<MSG/>");
            doc.DocumentElement.SetAttribute("ID", id);

            foreach (KeyValuePair<string, string> kvp in parameters)
            {
                XmlElement el = doc.CreateElement("P");
                el.SetAttribute("Name", kvp.Key);
                el.InnerText = kvp.Value;

                doc.DocumentElement.AppendChild(el);
            }
            return doc.InnerXml;
        }
        public string ToText()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append("ID=" + id + ";");
            foreach (KeyValuePair<string, string> kvp in parameters)
                sb.Append(kvp.Key + "=" + kvp.Value + ";");

            return sb.ToString();
        }

        public static NetworkMessage FromXml(string xml)
        {
            NetworkMessage msg = null;

            if (!String.IsNullOrEmpty(xml))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    if (doc.DocumentElement.Name == "MSG" && doc.DocumentElement.HasAttribute("ID"))
                    {
                        msg = new NetworkMessage(doc.DocumentElement.GetAttribute("ID"));
                        foreach (XmlElement el in doc.DocumentElement.ChildNodes)
                        {
                            if (el.Name == "P" && el.HasAttribute("Name"))
                                msg[el.GetAttribute("Name")] = el.InnerText;
                        }
                    }
                }
                catch (Exception)
                {
                    msg = null;
                }
            }

            return msg;
        }
        public static NetworkMessage FromText(string txt)
        {
            NetworkMessage msg = null;
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            try
            {
                string[] pairs = txt.Split(new Char[] { ';' });
                foreach (string pair in pairs)
                {
                    if (!String.IsNullOrEmpty(pair))
                    {
                        string[] s = pair.Split(new Char[] { '=' });
                        parameters.Add(s[0], s[1]);
                    }
                }

                if (parameters.ContainsKey("ID"))
                {
                    msg = new NetworkMessage(parameters["ID"]);
                    foreach (string key in parameters.Keys)
                        if (key != "ID")
                            msg[key] = parameters[key];
                }
            }
            catch { }

            return msg;
        }

        public byte[] PackXml()
        {
            return Encoding.UTF8.GetBytes(
                NetworkMessageDelimiters.BOM +
                ToXml() +
                NetworkMessageDelimiters.EOM);
        }
        public byte[] PackText()
        {
            return Encoding.UTF8.GetBytes(
                NetworkMessageDelimiters.BOM +
                ToText() +
                NetworkMessageDelimiters.EOM);
        }
        #endregion
    }
}
