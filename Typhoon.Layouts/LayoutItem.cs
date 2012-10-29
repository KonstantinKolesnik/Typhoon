using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Xml;
using Typhoon.Core;
using Typhoon.Layouts.LayoutItems;
using Typhoon.Localization;
using System.Xml.Linq;

namespace Typhoon.Layouts
{
    public abstract class LayoutItem : INotifyPropertyChanged
    {
        #region Fields
        private Guid id = Guid.Empty;
        private LayoutItemType type;
        private string name = "";
        private string description = "";
        private Image image = null;
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
                    name = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Name"));
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
                    description = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Description"));
                }
            }
        }

        public Image Image
        {
            get { return image; }
            set
            {
                if (image != value)
                {
                    image = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Image"));
                    OnPropertyChanged(new PropertyChangedEventArgs("ImageSource"));
                }
            }
        }
        public BitmapSource ImageSource
        {
            get { return (image != null ? Helpers.BitmapSourceFromImage(image) : null); }
            set
            {
                if (value != null)
                {
                    Image = Image.FromStream(Helpers.BitmapSourceToStream(value));
                    OnPropertyChanged(new PropertyChangedEventArgs("ImageSource"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Image"));
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
            name = LanguageDictionary.Current.Translate<string>("Untitled", "Text", "Untitled");
        }
        #endregion

        #region XmlSerialization
        public virtual void WriteToXmlElement(XmlElement el)
        {
            el.SetAttribute("ID", Helpers.ToBase64String(id));
            el.SetAttribute("Type", ((int)type).ToString());
            if (!String.IsNullOrEmpty(name))
                el.SetAttribute("Name", Helpers.ToBase64String(name));
            if (!String.IsNullOrEmpty(description))
                el.SetAttribute("Description", Helpers.ToBase64String(description));
            if (image != null)
                el.SetAttribute("Image", Helpers.ImageToString(image));
        }
        public virtual void ReadFromXmlElement(XmlElement el)
        {
            if (el.HasAttribute("ID"))
                id = Helpers.FromBase64StringToGuid(el.GetAttribute("ID"));
            if (el.HasAttribute("Name"))
                name = Helpers.FromBase64StringToString(el.GetAttribute("Name"));
            if (el.HasAttribute("Description"))
                description = Helpers.FromBase64StringToString(el.GetAttribute("Description"));
            if (el.HasAttribute("Image"))
                image = Helpers.ImageFromString(el.GetAttribute("Image"));
        }

        public static LayoutItem FromXmlElement(XmlElement el)
        {
            LayoutItem item = null;

            LayoutItemType t = (LayoutItemType)(int.Parse(el.GetAttribute("Type")));
            switch (t)
            {
                case LayoutItemType.Locomotive: item = new Locomotive(); break;
                case LayoutItemType.Consist: item = new Consist(); break;
                //case LayoutItemType.Turnout: item = new Turnout(); break;
                //case LayoutItemType.Signal: item = new Signal(); break;
                //case LayoutItemType.Turntable: item = new Turntable(); break;
                //case LayoutItemType.AccessoryGroup: item = new AccessoryGroup(); break;
                default: break;
            }
            if (item != null)
                item.ReadFromXmlElement(el);

            return item;
        }

        //-------------------------------

        public virtual void WriteToXElement(XElement el)
        {
            el.Attribute("ID").Value = Helpers.ToBase64String(id);
            el.Attribute("Type").Value = ((int)type).ToString();
            if (!String.IsNullOrEmpty(name))
                el.Attribute("Name").Value =  Helpers.ToBase64String(name);
            if (!String.IsNullOrEmpty(description))
                el.Attribute("Description").Value = Helpers.ToBase64String(description);
            if (image != null)
                el.Attribute("Image").Value = Helpers.ImageToString(image);
        }
        public virtual void ReadFromXElement(XElement el)
        {
            if (el.Attribute("ID") != null)
                id = Helpers.FromBase64StringToGuid(el.Attribute("ID").Value);
            if (el.Attribute("Name") != null)
                name = Helpers.FromBase64StringToString(el.Attribute("Name").Value);
            if (el.Attribute("Description") != null)
                description = Helpers.FromBase64StringToString(el.Attribute("Description").Value);
            if (el.Attribute("Image") != null)
                image = Helpers.ImageFromString(el.Attribute("Image").Value);
        }
        public static LayoutItem FromXElement(XElement el)
        {
            LayoutItem item = null;

            LayoutItemType t = (LayoutItemType)(int.Parse(el.Attribute("Type").Value));
            switch (t)
            {
                case LayoutItemType.Locomotive: item = new Locomotive(); break;
                case LayoutItemType.Consist: item = new Consist(); break;
                //case LayoutItemType.Turnout: item = new Turnout(); break;
                //case LayoutItemType.Signal: item = new Signal(); break;
                //case LayoutItemType.Turntable: item = new Turntable(); break;
                //case LayoutItemType.AccessoryGroup: item = new AccessoryGroup(); break;
                default: break;
            }
            if (item != null)
                item.ReadFromXElement(el);

            return item;
        }
        #endregion
    }
}
