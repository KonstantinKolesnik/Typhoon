using System;
using System.Collections;
using System.Ext.Xml;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.SPOT;

namespace Typhoon.MF.Layouts
{
    public class Layout
    {
        public enum ModifiedAction
        {
            ItemAdded,
            ItemChanged,
            ItemRemoved,
            Cleared
        }

        public delegate void ModifiedEventHandler(object sender, ModifiedAction action, LayoutItem item);

        #region Fields
        private static ExtendedWeakReference ewr;
        private ArrayList items = new ArrayList();
        #endregion

        #region Properties
        public ArrayList Items
        {
            get { return items; }
        }
        public LayoutItem this[int index]
        {
            get
            {
                if (index < 0 || index > items.Count - 1)
                    return null;
                return items[index] as LayoutItem;
            }
            set
            {
                if (index < 0 || index > items.Count - 1)
                    throw new ArgumentOutOfRangeException("index");
                items[index] = value;
            }
        }
        public LayoutItem this[string name]
        {
            get
            {
                foreach (LayoutItem item in items)
                    if (item.Name == name)
                        return item;
                return null;
            }
        }
        #endregion

        #region Events
        public event ModifiedEventHandler Modified;
        #endregion

        #region Serialization
        public static Layout FromXml(string xml)
        {
            Layout res = null;

            byte[] data = Encoding.UTF8.GetBytes(xml);
            if (data != null && data.Length != 0)
            {
                try
                {
                    MemoryStream xmlStream = new MemoryStream(data);

                    XmlReaderSettings ss = new XmlReaderSettings();
                    ss.IgnoreWhitespace = true;
                    ss.IgnoreComments = true;
                    using (XmlReader reader = XmlReader.Create(xmlStream, ss))
                    {
                        while (!reader.EOF)
                        {
                            reader.Read();
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    switch (reader.Name)
                                    {
                                        case "Layout":
                                            res = new Layout();
                                            break;
                                        case "Item":
                                            if (res != null)
                                                res.Add(LayoutItem.FromXml(reader));
                                            break;
                                    }
                                    break;
                                default: break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    res = null;
                }
            }

            return res;
        }
        public string ToXml()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(ms);

            writer.WriteStartElement("Layout");
            foreach (LayoutItem item in items)
            {
                writer.WriteStartElement("Item");
                item.WriteToXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();

            return new string(Encoding.UTF8.GetChars(ms.ToArray()));
        }
        
        public static Layout Load(uint id)
        {
            ewr = ExtendedWeakReference.RecoverOrCreate(typeof(string), id, ExtendedWeakReference.c_SurvivePowerdown | ExtendedWeakReference.c_SurviveBoot);
            ewr.Priority = (int)ExtendedWeakReference.PriorityLevel.System;

            Layout res = null;
            if (ewr.Target != null)
                res = Layout.FromXml((string)ewr.Target);

            return res ?? new Layout();
        }
        public void Save()
        {
            ewr.Target = ToXml();
        }
        #endregion

        #region Public methods
        public void Add(LayoutItem item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                item.PropertyChanged += new PropertyChangedEventHandler(Item_PropertyChanged);
                if (Modified != null)
                    Modified(this, ModifiedAction.ItemAdded, item);
            }
        }
        public void Remove(LayoutItem item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
                if (Modified != null)
                    Modified(this, ModifiedAction.ItemRemoved, item);
            }
        }
        public void Clear()
        {
            items.Clear();
            if (Modified != null)
                Modified(this, ModifiedAction.Cleared, null);
        }
        public bool Contains(LayoutItem item)
        {
            return items.Contains(item);
        }
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }
        #endregion

        #region Event handlers
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Modified != null)
                Modified(this, ModifiedAction.ItemChanged, sender as LayoutItem);
        }
        #endregion
    }
}
