using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml;
using Typhoon.Core;
using System.Xml.Linq;
using System.Linq;

namespace Typhoon.Layouts.LayoutItems
{
    public class ConsistItem : INotifyPropertyChanged
    {
        #region Fields
        private Guid locomotiveId = Guid.Empty;
        private bool forward = true;
        #endregion

        #region Properties
        public Guid LocomotiveID
        {
            get { return locomotiveId; }
            set
            {
                if (locomotiveId != value)
                {
                    locomotiveId = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("LocomotiveID"));
                }
            }
        }
        public bool Forward
        {
            get { return forward; }
            set
            {
                if (forward != value)
                {
                    forward = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Forward"));
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

        #region XmlSerialization
        public void WriteToXmlElement(XmlElement el)
        {
            if (locomotiveId != Guid.Empty)
            {
                XmlElement el2 = el.OwnerDocument.CreateElement("ConsistItem");
                el2.SetAttribute("LocomotiveID", Helpers.ToBase64String(locomotiveId));
                el2.SetAttribute("Forward", forward.ToString());

                el.AppendChild(el2);
            }
        }
        public void ReadFromXmlElement(XmlElement el)
        {
            if (el.HasAttribute("LocomotiveID"))
                locomotiveId = Helpers.FromBase64StringToGuid(el.GetAttribute("LocomotiveID"));
            forward = bool.Parse(el.GetAttribute("Forward"));
        }

        public static ConsistItem FromXmlElement(XmlElement el)
        {
            ConsistItem item = new ConsistItem();
            item.ReadFromXmlElement(el);
            return item;
        }
        #endregion
    }

    public class Consist : LayoutItem
    {
        #region Fields
        private ObservableCollection<ConsistItem> items = new ObservableCollection<ConsistItem>();
        #endregion

        #region Properties
        public ObservableCollection<ConsistItem> Items
        {
            get { return items; }
        }
        #endregion

        #region Constructor
        public Consist() : base(LayoutItemType.Consist)
        {
            items.CollectionChanged += Items_CollectionChanged;
        }
        #endregion

        #region Private methods
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("Items"));
        }
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("Items"));
        }
        #endregion

        #region XmlSerialization
        public override void WriteToXmlElement(XmlElement el)
        {
            base.WriteToXmlElement(el);

            // remove null locomotives
            for (int i = 0; i < items.Count; i++)
                if (items[i] == null || items[i].LocomotiveID == Guid.Empty)
                {
                    items.RemoveAt(i);
                    i--;
                }

            foreach (ConsistItem item in items)
                item.WriteToXmlElement(el);
        }
        public override void ReadFromXmlElement(XmlElement el)
        {
            base.ReadFromXmlElement(el);

            foreach (XmlElement el2 in el.ChildNodes)
            {
                if (el2.Name == "ConsistItem")
                {
                    ConsistItem itm = ConsistItem.FromXmlElement(el2);
                    items.Add(itm);
                    itm.PropertyChanged += Item_PropertyChanged;
                }
            }
        }

        public override void ReadFromXElement(XElement el)
        {
            base.ReadFromXElement(el);

            //var res = from ci in el.Descendants("ConsistItem") select ConsistItem.FromXElement(ci);
            //foreach (ConsistItem ci in res)
            //{
            //    ci.PropertyChanged += Item_PropertyChanged;
            //    items.Add(ci);
            //}
        }

        #endregion
    }
}
