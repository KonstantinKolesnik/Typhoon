using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Typhoon.Core;

namespace Typhoon.Net
{
    public class TCPClient : INotifyPropertyChanged
    {
        #region Fields
        private Socket socket = null;
        private byte[] buffer;
        private bool isStarted = false;
        private string receivedString;
        #endregion

        #region Properties
        public bool IsStarted
        {
            get { return isStarted; }
            private set
            {
                if (isStarted != value)
                {
                    isStarted = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsStarted"));
                }
            }
        }
        public bool IsConnected
        {
            get { return IsSocketConnected(socket); }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        public event EventHandler Started;
        public event EventHandler Stopped;
        public event ErrorHandler Error;
        public event EventHandler ServerDisconnected;
        public event NetworkMessageEventHandler NetworkMessageProcessor;
        #endregion

        #region Public Methods
        public void Start(string host, int port)
        {
            IPHostEntry ipHostInfo = Dns.Resolve(host);
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint ep = new IPEndPoint(ipAddress, port);
            
            Start(ep);
        }
        public void Start(IPEndPoint serverEP)
        {
            if (!IsStarted)
            {
                try
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    buffer = new byte[socket.ReceiveBufferSize];
                    socket.Connect(serverEP);
                    IsStarted = true;

                    Receive();

                    if (Started != null)
                        Started(this, EventArgs.Empty);
                }
                catch (Exception e)
                {
                    IsStarted = false;
                    socket = null;

                    if (Error != null)
                        Error(this, e);
                }
            }
        }
        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
                socket.Close();
                socket.Dispose();
                socket = null;

                if (Stopped != null)
                    Stopped(this, EventArgs.Empty);
            }
        }

        public void SendXml(NetworkMessage msg)
        {
            if (msg != null)
                Send(msg.PackXml());
        }
        public void SendText(NetworkMessage msg)
        {
            if (msg != null)
                Send(msg.PackText());
        }
        #endregion

        #region Private Methods
        private static bool IsSocketConnected(Socket socket)
        {
            if (socket == null)
                return false;

            // This is how you can determine whether a socket is still connected.
            bool res = false;
            bool oldBlockingState = socket.Blocking;
            try
            {
                byte[] tmp = new byte[1];
                socket.Blocking = false;
                socket.Send(tmp, 0, 0);
                res = true;
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK
                if (e.NativeErrorCode.Equals(10035))
                {
                    //Console.WriteLine("Still Connected, but the Send would block");
                }
                else
                {
                    //Console.WriteLine("Disconnected: error code {0}!", e.NativeErrorCode);
                    res = false;
                }
            }
            finally
            {
                socket.Blocking = oldBlockingState;
            }
            return res;
        }

        private void Send(byte[] data)
        {
            //if (IsStarted && data != null && data.Length > 0)
            //    socket.Send(data, data.Length, 0);


            if (IsStarted && data != null && data.Length > 0)
            {
                try
                {
                    socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(OnSend), socket);
                }
                catch (Exception e)
                {
                    if (Error != null)
                        Error.Invoke(this, e);
                }
            }
        }
        private void Receive()
        {
            try
            {
                socket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(OnReceive), null);
            }
            catch (SocketException)
            {
                Stop();
            }
            catch (Exception e)
            {
                if (Error != null)
                    Error.Invoke(this, e);
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            try
            {
                int a = ((Socket)ar.AsyncState).EndSend(ar);
            }
            catch (Exception e)
            {
                if (Error != null)
                    Error(this, e);
            }
        }
        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                int bytesRead = socket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    byte[] bb = new byte[bytesRead];
                    Array.Copy(buffer, bb, bytesRead);

                    // test
                    //string s = new string(Encoding.UTF8.GetChars(b));

                    receivedString += new string(Encoding.UTF8.GetChars(bb));

                    string data = null;
                    do
                    {
                        data = GetPayload(ref receivedString);
                        if (data != null)
                        {
                            //NetworkMessage msg = NetworkMessage.FromXml(data);
                            NetworkMessage msg = NetworkMessage.FromText(data);

                            if (msg != null && NetworkMessageProcessor != null)
                                //SendXml(NetworkMessageProcessor(msg));
                                SendText(NetworkMessageProcessor(msg));
                        }
                    }
                    while (data != null);
                }

                Receive();
            }
            catch (ObjectDisposedException)
            {
                // exception when client was manually closed; ignore
                int a = 0;
            }
            catch (SocketException)
            {
                // exception when server closes
                Stop(); // !!!! needed
                if (ServerDisconnected != null)
                    ServerDisconnected(this, EventArgs.Empty);
            }
            catch (NullReferenceException)
            {
                // exception: socket = null
                // exception when socket was manually closed;
                Stop();
            }
            catch (Exception e)
            {
                if (Error != null)
                    Error(this, e);
            }
        }
        private string GetPayload(ref string s)
        {
            int a = s.IndexOf(NetworkMessageDelimiters.BOM);
            int b = s.IndexOf(NetworkMessageDelimiters.EOM);

            if (a != -1 && b != -1) // there's a msg inside of s
            {
                // ccccc<BOM>ccccccccc<EOM>ccccc
                //      a             b

                string data = s.Substring(a + NetworkMessageDelimiters.BOM.Length, b - a - NetworkMessageDelimiters.BOM.Length);
                s = s.Substring(b + NetworkMessageDelimiters.EOM.Length);

                return data;
            }

            return null;
        }
        #endregion
    }
}
