using MFE.Core;
using Microsoft.SPOT;
using System;
using System.Ext.Xml;
using System.Xml;

namespace Typhoon.MF.Layouts
{
    public class LayoutItem
    {
        #region Fields
        private Guid id = Guid.Empty;
        private string name = "";
        private string description = "";
        private object user = null;
        #endregion

        #region Properties
        public Guid ID
        {
            get { return id; }
        }
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    string old = name;
                    name = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Name", old, name));
                }
            }
        }
        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    string old = description;
                    description = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Description", old, description));
                }
            }
        }
        public object User
        {
            get { return user; }
            set
            {
                if (user != value)
                {
                    object old = user;
                    user = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("User", old, user));
                }
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        #endregion

        #region Constructors
        public LayoutItem()
            : this(Guid.NewGuid())
        {
        }
        public LayoutItem(Guid id)
        {
            this.id = id;
            name = "Untitled";
        }
        #endregion

        #region XmlSerialization
        public virtual void WriteToXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("ID", Utils.ToBase64String(id));
            if (!Utils.StringIsNullOrEmpty(name))
                xmlWriter.WriteAttributeString("Name", Utils.ToBase64String(name));
            if (!Utils.StringIsNullOrEmpty(description))
                xmlWriter.WriteAttributeString("Description", Utils.ToBase64String(description));
        }
        public virtual void ReadFromXml(XmlReader xmlReader)
        {
            id = Utils.FromBase64StringToGuid(xmlReader.GetAttribute("ID"));
            if (!Utils.StringIsNullOrEmpty(xmlReader.GetAttribute("Name")))
                name = Utils.FromBase64StringToString(xmlReader.GetAttribute("Name"));
            if (!Utils.StringIsNullOrEmpty(xmlReader.GetAttribute("Description")))
                description = Utils.FromBase64StringToString(xmlReader.GetAttribute("Description"));
        }
        public static LayoutItem FromXml(XmlReader xmlReader)
        {
            LayoutItem item = new LayoutItem();
            item.ReadFromXml(xmlReader);
            return item;
        }
        #endregion

        public override string ToString()
        {
            return name;
        }
    }
}
