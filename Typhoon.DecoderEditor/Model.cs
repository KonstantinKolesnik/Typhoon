using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Typhoon.Core;
using Typhoon.Decoders;
using Typhoon.Localization;

namespace Typhoon.DecoderEditor
{
    public class Model : DependencyObject, IDisposable, INotifyPropertyChanged
    {
        #region Fields
        private readonly CommandBindingCollection commandBindings = new CommandBindingCollection();

        private const string optionsFileName = @"\Options.xml";
        private OpenFileDialog dlgOpen = new OpenFileDialog();
        private SaveFileDialog dlgSave = new SaveFileDialog();
        private OptionsContainer optionsContainer = new OptionsContainer();
        private Options options = new Options();
        private string title;
        private bool modified = false;
        private string fileName = "";
        #endregion

        #region Dependency properties
        public static DependencyProperty DecoderProperty = DependencyProperty.Register("Decoder", typeof(Decoder), typeof(Model), new PropertyMetadata(null, new PropertyChangedCallback(OnDecoderChanged)));
        private static void OnDecoderChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Model uc = (Model)sender;
            if (uc.Decoder != null)
                uc.Decoder.PropertyChanged += new PropertyChangedEventHandler(uc.Decoder_PropertyChanged);
        }
        public Decoder Decoder
        {
            get { return (Decoder)GetValue(DecoderProperty); }
            set { SetValue(DecoderProperty, value); }
        }
        #endregion

        #region Properties
        public CommandBindingCollection CommandBindings
        {
            get { return commandBindings; }
        }
        public string Title
        {
            get { return title; }
            private set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Title"));
                }
            }
        }
        public Options Options
        {
            get { return options; }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        #endregion

        #region Constructor
        public Model()
        {
            dlgOpen.DefaultExt = dlgSave.DefaultExt = "decoder";
            dlgOpen.Filter = dlgSave.Filter = LanguageDictionary.Current.Translate<string>("DecoderFilesFilter", "Text", "Decoder files|*.decoder|All files|*.*");

            CommandBindings.Add(new CommandBinding(RoutedCommands.NewDecoder, NewDecoder_Executed, NewDecoder_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.OpenDecoder, OpenDecoder_Executed, OpenDecoder_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.SaveDecoder, SaveDecoder_Executed, SaveDecoder_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.SaveAsDecoder, SaveAsDecoder_Executed, SaveAsDecoder_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.PrintDecoder, PrintDecoder_Executed, PrintDecoder_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.EmailDecoder, EmailDecoder_Executed, EmailDecoder_CanExecute));

            LoadSettings();

            UpdateTitle();
        }
        public void Dispose()
        {
            SaveSettings();

            if (modified)
            {
                string s = LanguageDictionary.Current.Translate<string>("SaveCurrentDecoder", "Text", "Save current decoder?");
                if (MessageBox.Show(s, App.Name, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    RoutedCommands.SaveDecoder.Execute(null, null);
            }
        }
        #endregion

        private void Decoder_PropertyChanged(object sender, EventArgs e)
        {
            modified = true;
            UpdateTitle();
        }

        #region Private methods
        private void LoadSettings()
        {
            if (File.Exists(App.StartupPath + optionsFileName))
            {
                optionsContainer.Load(App.StartupPath + optionsFileName);
                optionsContainer.GetByFields("DecoderEditor", options);
            }
            else
                SaveSettings();
        }
        private void SaveSettings()
        {
            optionsContainer.Create();
            optionsContainer.AddByFields("DecoderEditor", options);
            optionsContainer.Save(App.StartupPath + optionsFileName);
        }
        private void UpdateTitle()
        {
            Title = App.Name + (String.IsNullOrEmpty(fileName) ? "" : " - " + fileName + (modified ? "*" : ""));
        }
        #endregion

        #region Routed commands
        private void NewDecoder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void NewDecoder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (modified)
            {
                string s = LanguageDictionary.Current.Translate<string>("SaveCurrentDecoder", "Text", "Save current decoder?");
                MessageBoxResult res = MessageBox.Show(s, App.Name, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                    RoutedCommands.SaveDecoder.Execute(null, null);
                else if (res == MessageBoxResult.Cancel)
                    return;
            }

            Decoder = new Decoder();
            modified = true;
            fileName = LanguageDictionary.Current.Translate<string>("Untitled", "Text", "Untitled");
            UpdateTitle();
        }

        private void OpenDecoder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void OpenDecoder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null)
            {
                Decoder.LoadFromFile((string)e.Parameter);
                fileName = (string)e.Parameter;
                modified = false;
                UpdateTitle();
            }
            else
            {
                if (dlgOpen.ShowDialog() == true)
                {
                    if (modified)
                    {
                        string s = LanguageDictionary.Current.Translate<string>("SaveCurrentDecoder", "Text", "Save current decoder?");
                        MessageBoxResult res = MessageBox.Show(s, App.Name, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                        if (res == MessageBoxResult.Yes)
                            RoutedCommands.SaveDecoder.Execute(null, null);
                        else if (res == MessageBoxResult.Cancel)
                            return;
                    }

                    Decoder = new Decoder();
                    Decoder.LoadFromFile(dlgOpen.FileName);
                    fileName = dlgOpen.FileName;
                    modified = false;
                    UpdateTitle();
                }
            }
        }

        private void SaveDecoder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = modified;
        }
        private void SaveDecoder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (fileName != LanguageDictionary.Current.Translate<string>("Untitled", "Text", "Untitled"))
            {
                Decoder.SaveToFile(fileName);
                modified = false;
                UpdateTitle();
            }
            else
            {
                if (dlgSave.ShowDialog() == true)
                {
                    Decoder.SaveToFile(dlgSave.FileName);
                    fileName = dlgSave.FileName;
                    modified = false;
                    UpdateTitle();
                }
            }
        }

        private void SaveAsDecoder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Decoder != null;
        }
        private void SaveAsDecoder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (dlgSave.ShowDialog() == true)
            {
                Decoder.SaveToFile(dlgSave.FileName);
                fileName = dlgSave.FileName;
                modified = false;
                UpdateTitle();
            }
        }

        private void PrintDecoder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Decoder != null;
        }
        private void PrintDecoder_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void EmailDecoder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Decoder != null && fileName != LanguageDictionary.Current.Translate<string>("Untitled", "Text", "Untitled");
        }
        private void EmailDecoder_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        #endregion
    }
}
