using System;
using System.Ext.Xml;
using System.Xml;
using MFE.Utilities;
using Microsoft.SPOT;
using Typhoon.Layouts;
using Typhoon.MF.Layouts.LayoutItems;

namespace Typhoon.MF.Layouts
{
    public class LayoutItem
    {
        #region Fields
        private Guid id = Guid.Empty;
        private LayoutItemType type;
        private string name = "";
        private string description = "";
        //private Image image = null;
        private object user = null;
        #endregion

        #region Properties
        public Guid ID
        {
            get { return id; }
        }
        public LayoutItemType Type
        {
            get { return type; }
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
        //public Image Image
        //{
        //    get { return image; }
        //    set
        //    {
        //        if (image != value)
        //        {
        //            image = value;
        //            OnPropertyChanged(new PropertyChangedEventArgs("Image"));
        //            OnPropertyChanged(new PropertyChangedEventArgs("ImageSource"));
        //        }
        //    }
        //}
        //public BitmapSource ImageSource
        //{
        //    get { return (image != null ? Helpers.BitmapSourceFromImage(image) : null); }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            Image = Image.FromStream(Helpers.BitmapSourceToStream(value));
        //            OnPropertyChanged(new PropertyChangedEventArgs("ImageSource"));
        //            OnPropertyChanged(new PropertyChangedEventArgs("Image"));
        //        }
        //    }
        //}
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
        public LayoutItem(LayoutItemType type)
            : this(Guid.NewGuid(), type)
        {
        }
        public LayoutItem(Guid id, LayoutItemType type)
        {
            this.id = id;
            this.type = type;

            name = "Untitled";// LanguageDictionary.Current.Translate<string>("Untitled", "Text", "Untitled");
        }
        #endregion

        #region XmlSerialization
        public virtual void WriteToXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("ID", Utils.ToBase64String(id));
            xmlWriter.WriteAttributeString("Type", ((int)type).ToString());
            if (!Utils.IsStringNullOrEmpty(name))
                xmlWriter.WriteAttributeString("Name", Utils.ToBase64String(name));
            if (!Utils.IsStringNullOrEmpty(description))
                xmlWriter.WriteAttributeString("Description", Utils.ToBase64String(description));
        }
        public virtual void ReadFromXml(XmlReader xmlReader)
        {
            id = Utils.FromBase64StringToGuid(xmlReader.GetAttribute("ID"));
            if (!Utils.IsStringNullOrEmpty(xmlReader.GetAttribute("Name")))
                name = Utils.FromBase64StringToString(xmlReader.GetAttribute("Name"));
            if (!Utils.IsStringNullOrEmpty(xmlReader.GetAttribute("Description")))
                description = Utils.FromBase64StringToString(xmlReader.GetAttribute("Description"));
            //image = Helpers.ImageFromString(el.GetAttribute("Image"));
        }
        public static LayoutItem FromXml(XmlReader xmlReader)
        {
            LayoutItem item = null;
            
            LayoutItemType t = (LayoutItemType)(int.Parse(xmlReader.GetAttribute("Type")));
            switch (t)
            {
                case LayoutItemType.Locomotive: item = new Locomotive(); break;
                //case LayoutItemType.Consist: item = new Consist(); break;
                //case LayoutItemType.Turnout: item = new Turnout(); break;
                //case LayoutItemType.Signal: item = new Signal(); break;
                //case LayoutItemType.Turntable: item = new Turntable(); break;
                //case LayoutItemType.AccessoryGroup: item = new AccessoryGroup(); break;
                default: break;
            }
            if (item != null)
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
