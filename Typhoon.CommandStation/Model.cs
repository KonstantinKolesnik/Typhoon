using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using Typhoon.API;
using Typhoon.Core;
using Typhoon.Decoders;
using Typhoon.Layouts;
using Typhoon.Localization;
using Typhoon.Net;

namespace Typhoon.CommandStation
{
    public class Model : DependencyObject, IDisposable, INotifyPropertyChanged
    {
        #region Fields
        private readonly CommandBindingCollection commandBindings = new CommandBindingCollection();

        private const string optionsFileName = @"\Options.xml";
        private const string decodersFolder = @"\Decoders";
        private OpenFileDialog dlgOpen = new OpenFileDialog();
        private SaveFileDialog dlgSave = new SaveFileDialog();
        private OptionsContainer optionsContainer = new OptionsContainer();
        private Options options = new Options();

        private bool stationPower = false;
        private bool mainBoosterIsActive = false;
        private bool mainBoosterIsOverloaded = false;
        private string mainBoosterCurrent = "0 mA";
        private bool progBoosterIsActive = false;
        private bool progBoosterIsOverloaded = false;
        private string progBoosterCurrent = "0 mA";
        #endregion

        #region Dependency properties
        public static DependencyProperty LayoutProperty = DependencyProperty.Register("Layout", typeof(Layout), typeof(Model), new PropertyMetadata(null));
        public Layout Layout
        {
            get { return (Layout)GetValue(Model.LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        public static DependencyProperty TCPClientProperty = DependencyProperty.Register("TCPClient", typeof(TCPClient), typeof(Model), new PropertyMetadata(null));
        public TCPClient TCPClient
        {
            get { return (TCPClient)GetValue(TCPClientProperty); }
            set { SetValue(TCPClientProperty, value); }
        }

        public static DependencyProperty ServerFinderProperty = DependencyProperty.Register("ServerFinder", typeof(ServerFinder), typeof(Model), new PropertyMetadata(null));
        public ServerFinder ServerFinder
        {
            get { return (ServerFinder)GetValue(ServerFinderProperty); }
            set { SetValue(ServerFinderProperty, value); }
        }

        public static DependencyProperty LocomotiveDecoderReferenceCollectionProperty = DependencyProperty.Register("LocomotiveDecoderReferenceCollection", typeof(DecoderReferenceCollection), typeof(Model), new PropertyMetadata(null));
        public DecoderReferenceCollection LocomotiveDecoderReferenceCollection
        {
            get { return (DecoderReferenceCollection)GetValue(LocomotiveDecoderReferenceCollectionProperty); }
            set { SetValue(LocomotiveDecoderReferenceCollectionProperty, value); }
        }

        public static DependencyProperty SoundDecoderReferenceCollectionProperty = DependencyProperty.Register("SoundDecoderReferenceCollection", typeof(DecoderReferenceCollection), typeof(Model), new PropertyMetadata(null));
        public DecoderReferenceCollection SoundDecoderReferenceCollection
        {
            get { return (DecoderReferenceCollection)GetValue(SoundDecoderReferenceCollectionProperty); }
            set { SetValue(SoundDecoderReferenceCollectionProperty, value); }
        }

        public static DependencyProperty AccessoryDecoderReferenceCollectionProperty = DependencyProperty.Register("AccessoryDecoderReferenceCollection", typeof(DecoderReferenceCollection), typeof(Model), new PropertyMetadata(null));
        public DecoderReferenceCollection AccessoryDecoderReferenceCollection
        {
            get { return (DecoderReferenceCollection)GetValue(AccessoryDecoderReferenceCollectionProperty); }
            set { SetValue(AccessoryDecoderReferenceCollectionProperty, value); }
        }
        #endregion

        #region Properties
        public CommandBindingCollection CommandBindings
        {
            get { return commandBindings; }
        }
        public Options Options
        {
            get { return options; }
        }
        public string Title
        {
            get { return App.Name + " - " + Layout.Title; }
        }

        public bool StationPower
        {
            get { return stationPower; }
            private set
            {
                if (stationPower != value)
                {
                    stationPower = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("StationPower"));
                }
            }
        }

        public bool MainBoosterIsActive
        {
            get { return mainBoosterIsActive; }
            private set
            {
                if (mainBoosterIsActive != value)
                {
                    mainBoosterIsActive = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("MainBoosterIsActive"));
                }
            }
        }
        public bool MainBoosterIsOverloaded
        {
            get { return mainBoosterIsOverloaded; }
            private set
            {
                if (mainBoosterIsOverloaded != value)
                {
                    mainBoosterIsOverloaded = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("MainBoosterIsOverloaded"));
                }
            }
        }
        public string MainBoosterCurrent
        {
            get { return mainBoosterCurrent; }
            private set
            {
                if (mainBoosterCurrent != value)
                {
                    mainBoosterCurrent = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("MainBoosterCurrent"));
                }
            }
        }
        
        public bool ProgBoosterIsActive
        {
            get { return progBoosterIsActive; }
            private set
            {
                if (progBoosterIsActive != value)
                {
                    progBoosterIsActive = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ProgBoosterIsActive"));
                }
            }
        }
        public bool ProgBoosterIsOverloaded
        {
            get { return progBoosterIsOverloaded; }
            private set
            {
                if (progBoosterIsOverloaded != value)
                {
                    progBoosterIsOverloaded = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ProgBoosterIsOverloaded"));
                }
            }
        }
        public string ProgBoosterCurrent
        {
            get { return progBoosterCurrent; }
            private set
            {
                if (progBoosterCurrent != value)
                {
                    progBoosterCurrent = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ProgBoosterCurrent"));
                }
            }
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
            LoadSettings();

            Layout = new Layout();
            Layout.PropertyChanged += new PropertyChangedEventHandler(Layout_PropertyChanged);
            LocomotiveDecoderReferenceCollection = new DecoderReferenceCollection(App.StartupPath + decodersFolder, DecoderType.Locomotive);
            SoundDecoderReferenceCollection = new DecoderReferenceCollection(App.StartupPath + decodersFolder, DecoderType.Sound);
            AccessoryDecoderReferenceCollection = new DecoderReferenceCollection(App.StartupPath + decodersFolder, DecoderType.Accessory);

            dlgOpen.DefaultExt = dlgSave.DefaultExt = "layout";
            dlgOpen.Filter = dlgSave.Filter = LanguageDictionary.Current.Translate<string>("LayoutFilesFilter", "Text", "Layout files|*.layout|All files|*.*");

            TCPClient = new TCPClient();
            TCPClient.Started += new EventHandler(TCPClient_Started);
            TCPClient.Stopped += new EventHandler(TCPClient_Stopped);
            TCPClient.Error += new ErrorHandler(TCPClient_Error);
            TCPClient.ServerDisconnected += TCPClient_ServerDisconnected;
            TCPClient.NetworkMessageProcessor += new NetworkMessageEventHandler(TCPClient_NetworkMessageProcessor);

            ServerFinder = new ServerFinder(Options.UDPServerPort, "TyphoonCentralStation");
            ServerFinder.ServerFound += new EventHandler(ServerFinder_ServerFound);
            ServerFinder.ServerLost += new EventHandler(ServerFinder_ServerLost);
            ServerFinder.Start();

            CommandBindings.Add(new CommandBinding(RoutedCommands.NewLayout, NewLayout_Executed, NewLayout_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.OpenLayout, OpenLayout_Executed, OpenLayout_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.SaveLayout, SaveLayout_Executed, SaveLayout_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.SaveAsLayout, SaveAsLayout_Executed, SaveAsLayout_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.PrintLayout, PrintLayout_Executed, PrintLayout_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.EmailLayout, EmailLayout_Executed, EmailLayout_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.ImportLayout, ImportLayout_Executed, ImportLayout_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.ExportLayout, ExportLayout_Executed, ExportLayout_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.StationPower, Power_Executed, Power_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.StationRailcom, Railcom_Executed, Railcom_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.AllBrake, AllBrake_Executed, AllBrake_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.AllStop, AllStop_Executed, AllStop_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.AllReset, AllReset_Executed, AllReset_CanExecute));
        }
        public void Dispose()
        {
            if (ServerFinder != null)
                ServerFinder.Stop();
            if (TCPClient != null)
                TCPClient.Stop();
            
            SaveSettings();

            if (Layout.Modified)
            {
                string s = LanguageDictionary.Current.Translate<string>("SaveCurrentLayout", "Text", "Save current layout?");
                if (MessageBox.Show(s, App.Name, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    RoutedCommands.SaveLayout.Execute(null, null);
            }
        }
        #endregion

        #region Event handlers
        
        #region ServerFinder
        private void ServerFinder_ServerFound(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((ThreadStart)delegate()
            {
                TCPClient.Start(new IPEndPoint(ServerFinder.ServerEP.Address, options.IPServerPort));
                CommandManager.InvalidateRequerySuggested();
            }, DispatcherPriority.Normal);
        }
        private void ServerFinder_ServerLost(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((ThreadStart)delegate()
            {
                TCPClient.Stop();
                CommandManager.InvalidateRequerySuggested();
            }, DispatcherPriority.Normal);
        }
        #endregion

        #region TCP Client
        private void TCPClient_Started(object sender, EventArgs e)
        {
            // get station power status
            MessageSender.GetPower();
        }
        private void TCPClient_Stopped(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            if (ServerFinder.IsServerAvailable)
                TCPClient.Start(new IPEndPoint(ServerFinder.ServerEP.Address, options.IPServerPort));
        }
        private void TCPClient_Error(object sender, Exception e)
        {
            //Dispatcher.BeginInvoke((ThreadStart)delegate()
            //{
            //    MessageBox.Show(e.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
            //}, DispatcherPriority.Normal);
        }
        private void TCPClient_ServerDisconnected(object sender, EventArgs e)
        {
            TCPClient.Stop();
        }
        private NetworkMessage TCPClient_NetworkMessageProcessor(NetworkMessage msg)
        {
            return ProcessNetworkMessage(msg);
        }
        #endregion

        #region Layout
        private void Layout_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("Title"));
        }
        #endregion

        #endregion

        #region Private methods
        private void LoadSettings()
        {
            if (File.Exists(App.StartupPath + optionsFileName))
            {
                optionsContainer.Load(App.StartupPath + optionsFileName);
                optionsContainer.GetByFields("CommandStation", options);
            }
            else
                SaveSettings();
        }
        private void SaveSettings()
        {
            optionsContainer.Create();
            optionsContainer.AddByFields("CommandStation", options);
            optionsContainer.Save(App.StartupPath + optionsFileName);
        }
        private bool SaveModifiedLayoutAndProceed()
        {
            if (Layout.Modified)
            {
                string s = LanguageDictionary.Current.Translate<string>("SaveCurrentLayout", "Text", "Save current layout?");
                MessageBoxResult res = MessageBox.Show(s, App.Name, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                    RoutedCommands.SaveLayout.Execute(null, null);
                else if (res == MessageBoxResult.Cancel)
                    return false;
            }

            return true;
        }
        #endregion

        #region Routed commands
        private void NewLayout_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void NewLayout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (SaveModifiedLayoutAndProceed())
                Layout.New();
        }

        private void OpenLayout_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void OpenLayout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null)
                Layout.LoadFromFile((string)e.Parameter);
            else
            {
                if (dlgOpen.ShowDialog() == true)
                {
                    if (SaveModifiedLayoutAndProceed())
                        Layout.LoadFromFile(dlgOpen.FileName);
                }
            }
        }

        private void SaveLayout_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Layout.Modified;
        }
        private void SaveLayout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (File.Exists(Layout.FileName))
                Layout.SaveToFile(Layout.FileName);
            else
            {
                dlgSave.FileName = Layout.FileName;
                if (dlgSave.ShowDialog() == true)
                    Layout.SaveToFile(dlgSave.FileName);
            }
        }

        private void SaveAsLayout_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void SaveAsLayout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (dlgSave.ShowDialog() == true)
                Layout.SaveToFile(dlgSave.FileName);
        }

        private void PrintLayout_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void PrintLayout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void EmailLayout_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = File.Exists(Layout.FileName) && !Layout.Modified;
        }
        private void EmailLayout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void ImportLayout_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TCPClient.IsStarted;
        }
        private void ImportLayout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (SaveModifiedLayoutAndProceed())
                MessageSender.GetLayout();
        }

        private void ExportLayout_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TCPClient.IsStarted;
        }
        private void ExportLayout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (SaveModifiedLayoutAndProceed())
                MessageSender.SetLayout(Layout);
        }

        private void Power_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TCPClient.IsStarted;
        }
        private void Power_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageSender.SetPower(!StationPower);
        }

        private void Railcom_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TCPClient.IsStarted;
        }
        private void Railcom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void AllBrake_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TCPClient.IsStarted && MainBoosterIsActive;
        }
        private void AllBrake_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageSender.BroadcastBrake();
        }

        private void AllStop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TCPClient.IsStarted && MainBoosterIsActive;
        }
        private void AllStop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageSender.BroadcastStop();
        }

        private void AllReset_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TCPClient.IsStarted && MainBoosterIsActive;
        }
        private void AllReset_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageSender.BroadcastReset();
        }
        #endregion

        #region Network message processing
        private NetworkMessage ProcessNetworkMessage(NetworkMessage msg)
        {
            NetworkMessage response = null;

            switch (msg.ID)
            {
                case NetworkMessageID.OK:
                    Dispatcher.BeginInvoke((ThreadStart)delegate()
                    {
                        string s = LanguageDictionary.Current.Translate<string>("OperationOK", "Text", "Operation completed successfully!");
                        if (!String.IsNullOrEmpty(msg["Msg"]))
                            s = msg["Msg"];

                        MessageBox.Show(s, App.Name, MessageBoxButton.OK, MessageBoxImage.Information);
                    }, DispatcherPriority.Normal);
                    break;

                case NetworkMessageID.Information:
                    Dispatcher.BeginInvoke((ThreadStart)delegate()
                    {
                        MessageBox.Show(msg["Msg"], App.Name, MessageBoxButton.OK, MessageBoxImage.Information);
                    }, DispatcherPriority.Normal);
                    break;

                case NetworkMessageID.Warning:
                    Dispatcher.BeginInvoke((ThreadStart)delegate()
                    {
                        MessageBox.Show(msg["Msg"], App.Name, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }, DispatcherPriority.Normal);
                    break;

                case NetworkMessageID.Error:
                    Dispatcher.BeginInvoke((ThreadStart)delegate()
                    {
                        string s = LanguageDictionary.Current.Translate<string>("OperationError", "Text", "Operation failed!");
                        if (!String.IsNullOrEmpty(msg["Msg"]))
                            s = msg["Msg"];

                        MessageBox.Show(s, App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    }, DispatcherPriority.Normal);
                    break;

                case NetworkMessageID.Layout:
                    Dispatcher.BeginInvoke((ThreadStart)delegate()
                    {
                        string s;
                        if (Layout.LoadFromXml(Helpers.FromBase64StringToString(msg["Layout"])))
                            s = LanguageDictionary.Current.Translate<string>("OperationOK", "Text", "Operation completed successfully!");
                        else
                            s = LanguageDictionary.Current.Translate<string>("OperationError", "Text", "Operation failed!");

                        MessageBox.Show(s, App.Name, MessageBoxButton.OK, MessageBoxImage.Information);
                    }, DispatcherPriority.Normal);
                    break;

                case NetworkMessageID.Power:
                    Dispatcher.BeginInvoke((ThreadStart)delegate()
                    {
                        StationPower = bool.Parse(msg["Power"]);
                        MainBoosterIsActive = bool.Parse(msg["MainActive"]);
                        MainBoosterIsOverloaded = bool.Parse(msg["MainOverload"]);
                        ProgBoosterIsActive = bool.Parse(msg["ProgActive"]);
                        ProgBoosterIsOverloaded = bool.Parse(msg["ProgOverload"]);
                    }, DispatcherPriority.Normal);
                    break;

                case NetworkMessageID.BoostersCurrent:
                    Dispatcher.BeginInvoke((ThreadStart)delegate()
                    {
                        MainBoosterCurrent = msg["Main"] + " mA";
                        ProgBoosterCurrent = msg["Prog"] + " mA";
                    }, DispatcherPriority.Normal);
                    break;

                default:
                    break;
            }

            return response;
        }
        #endregion
    }
}
