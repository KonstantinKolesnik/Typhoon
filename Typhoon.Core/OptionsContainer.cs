using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Typhoon.Core
{
    public class OptionsContainer
    {
        private XmlDocument doc = null;

        public void Create()
        {
            doc = new XmlDocument();
            doc.LoadXml("<Options/>");
        }
        public void Load(string fileName)
        {
            doc = new XmlDocument();
            if (File.Exists(fileName))
                doc.Load(fileName);
        }
        public void Save(string fileName)
        {
            doc.Save(fileName);
        }

        public void AddString(string name, string value)
        {
            XmlElement el = doc.CreateElement(name);
            el.InnerText = value;
            doc.DocumentElement.AppendChild(el);
        }
        public void AddByFields(string sectionName, object obj)
        {
            SaveFields(doc, doc.DocumentElement, sectionName, obj);
        }
        public void AddByProperties(string sectionName, object obj)
        {
            SaveProps(doc, doc.DocumentElement, sectionName, obj);
        }

        public string GetString(string name)
        {
            XmlElement el = (XmlElement)doc.SelectSingleNode("/Options/" + name);
            return el == null ? null : el.InnerText;
        }
        public void GetByFields(string sectionName, object obj)
        {
            LoadFields(doc, doc.DocumentElement, sectionName, obj);
        }
        public void GetByProperties(string sectionName, object obj)
        {
            LoadProps(doc, doc.DocumentElement, sectionName, obj);
        }

        // Helper methods **********************************************************************

        private static void SaveProps(XmlDocument doc, XmlElement parent, string nodeName, object obj)
        {
            XmlElement el = doc.CreateElement(nodeName);
            ExportProps(el, obj);
            parent.AppendChild(el);
        }
        private static void SaveFields(XmlDocument doc, XmlElement parent, string nodeName, object obj)
        {
            XmlElement el = doc.CreateElement(nodeName);
            ExportFields(el, obj);
            parent.AppendChild(el);
        }
        private static void ExportProps(XmlElement el, object obj)
        {
            if (el != null && obj != null)
                foreach (PropertyInfo pi in obj.GetType().GetProperties())
                {
                    string val = ObjectToString(pi.GetValue(obj, null));
                    if (!String.IsNullOrEmpty(val))
                        el.SetAttribute(pi.Name, val);
                }
        }
        private static void ExportFields(XmlElement el, object obj)
        {
            if (el != null && obj != null)
                foreach (FieldInfo fi in obj.GetType().GetFields())
                {
                    string val = ObjectToString(fi.GetValue(obj));
                    if (!String.IsNullOrEmpty(val))
                        el.SetAttribute(fi.Name, val);
                }
        }

        private static void LoadProps(XmlDocument doc, XmlElement parent, string nodeName, object obj)
        {
            XmlElement el = (XmlElement)parent.SelectSingleNode(nodeName);
            ImportProps(el, obj);
        }
        private static void LoadFields(XmlDocument doc, XmlElement parent, string nodeName, object obj)
        {
            XmlElement el = (XmlElement)parent.SelectSingleNode(nodeName);
            ImportFields(el, obj);
        }
        private static void ImportProps(XmlElement el, object obj)
        {
            if (el != null && obj != null)
                foreach (PropertyInfo pi in obj.GetType().GetProperties())
                {
                    object val = StringToObject(el.GetAttribute(pi.Name), pi.PropertyType);
                    if (val != null)
                        pi.SetValue(obj, val, null);
                }
        }
        private static void ImportFields(XmlElement el, object obj)
        {
            if (el != null && obj != null)
                foreach (FieldInfo fi in obj.GetType().GetFields())
                {
                    object val = StringToObject(el.GetAttribute(fi.Name), fi.FieldType);
                    if (val != null)
                        fi.SetValue(obj, val);
                }
        }

        private static string ObjectToString(object value)
        {
            if (value == null)
                return "";
            else if (value is String)
                return value as string;
            else if (value is Guid)
                return ((Guid)value).ToString();
            else if (value is bool || value is int || value is float)
                return value.ToString();
            else if (value is Enum)
                return Enum.GetName(value.GetType(), value); //((int)value).ToString();
            //else if (value is Image)
            //{
            //    MemoryStream stream = new MemoryStream();
            //    BinaryFormatter bf = new BinaryFormatter();
            //    try { bf.Serialize(stream, value as Image); }
            //    catch (SerializationException ex) { throw new DSException(ex.Message, ex); }
            //    return Convert.ToBase64String(stream.GetBuffer(), Base64FormattingOptions.None);
            //}
            else
                return "";
        }
        private static object StringToObject(string value, Type t)
        {
            if (String.IsNullOrEmpty(value))
                return null;
            else if (t == typeof(String))
                return value;
            else if (t == typeof(Guid))
                return new Guid(value);
            else if (t == typeof(bool))
                return bool.Parse(value);
            else if (t == typeof(int))
                return int.Parse(value);
            else if (t == typeof(float))
                return float.Parse(value);
            else if (t == typeof(Enum))
            {
                //int a = int.Parse(value);
                return Enum.Parse(t, value);


                //return Activator.
            }
            //else if (t == typeof(Image))
            //{
            //    MemoryStream stream = new MemoryStream(Convert.FromBase64String(value));
            //    BinaryFormatter bf = new BinaryFormatter();
            //    try { return bf.Deserialize(stream) as Image; }
            //    catch (SerializationException ex) { throw new DSException(ex.Message, ex); }
            //}
            else
                return null;
        }
    }
}
