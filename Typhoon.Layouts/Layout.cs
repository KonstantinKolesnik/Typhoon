using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml;
using Typhoon.Layouts.LayoutItems;
using Typhoon.Localization;
using System.Xml.Linq;
using System.Linq;

namespace Typhoon.Layouts
{
    public sealed class Layout : INotifyPropertyChanged
    {
        #region Fields
        private ObservableCollection<LayoutItem> items = new ObservableCollection<LayoutItem>();
        private bool modified = false;
        private string fileName = "";
        private bool isLoading = false;
        #endregion

        #region Properties
        public bool Modified
        {
            get { return modified; }
            private set
            {
                if (modified != value)
                {
                    modified = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Modified"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Title"));
                }
            }
        }
        public string FileName
        {
            get { return fileName; }
            private set
            {
                if (fileName != value)
                {
                    fileName = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("FileName"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Title"));
                }
            }
        }
        public string Title
        {
            get { return fileName + (modified ? "*" : ""); }
        }
        
        public ObservableCollection<LayoutItem> Items
        {
            get { return items; }
        }
        public ObservableCollection<Locomotive> Locomotives
        {
            get
            {
                ObservableCollection<Locomotive> res = new ObservableCollection<Locomotive>();
                foreach (LayoutItem item in Items)
                    if (item.Type == LayoutItemType.Locomotive)
                        res.Add(item as Locomotive);
                
                return res;
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        #endregion

        #region Constructor
        public Layout()
        {
            items.CollectionChanged += Items_CollectionChanged;
            New();
        }
        #endregion

        #region Public methods
        public void New()
        {
            items.Clear();
            FileName = LanguageDictionary.Current.Translate<string>("New", "Text", "New") + ".layout";
            Modified = false;
        }
        public bool LoadFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(fileName);
                    LoadFromXmlDocument(doc);

                    //LoadFromXDocument(XDocument.Load(fileName));
                    
                    
                    FileName = fileName;
                    Modified = false;
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return false;
        }
        public bool LoadFromXml(string xml)
        {
            if (!String.IsNullOrEmpty(xml))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    LoadFromXmlDocument(doc);
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return false;
        }
        public void SaveToFile(string fileName)
        {
            XmlDocument doc = SaveToXmlDocument();
            doc.Save(fileName);
            FileName = fileName;
            Modified = false;
        }
        public string SaveToXml()
        {
            return SaveToXmlDocument().InnerXml;
        }

        public void Import(string fileName)
        {

        }
        public void Export(string fileName, LayoutItem item)
        {

        }
        #endregion

        #region Event handlers
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("Items"));
            OnPropertyChanged(new PropertyChangedEventArgs("Locomotives"));
            Modified = true;
        }
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("Items"));
            OnPropertyChanged(new PropertyChangedEventArgs("Locomotives"));
            Modified = true;
        }
        #endregion

        #region Private methods
        private void LoadFromXmlDocument(XmlDocument doc)
        {
            if (doc.DocumentElement.Name != "Layout")
            {
                string s = LanguageDictionary.Current.Translate<string>("InvalidLayoutFile", "Text", "Invalid layout file!");
                MessageBox.Show(s, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            items.Clear();
            XmlNodeList lst = doc.SelectNodes("/Layout/Item");
            foreach (XmlElement el in lst)
            {
                LayoutItem item = LayoutItem.FromXmlElement(el);
                if (item != null)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                    items.Add(item);
                }
            }
        }
        private XmlDocument SaveToXmlDocument()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Layout/>");
            foreach (LayoutItem item in items)
            {
                XmlElement el = doc.CreateElement("Item");
                item.WriteToXmlElement(el);
                doc.DocumentElement.AppendChild(el);
            }
            return doc;
        }

        private void LoadFromXDocument(XDocument doc)
        {
            if (!doc.Descendants("Layout").Any())
            {
                string s = LanguageDictionary.Current.Translate<string>("InvalidLayoutFile", "Text", "Invalid layout file!");
                MessageBox.Show(s, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                items.Clear();

                var res = from item in doc.Descendants("Item") select LayoutItem.FromXElement(item);
                foreach (LayoutItem item in res)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                    items.Add(item);
                }
            }
        }
        #endregion
    }
}
