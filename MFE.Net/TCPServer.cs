using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;

namespace MFE.Net
{
    public class TCPServer
    {
        #region Fields
        private int port;
        private Socket serverSocket;
        private bool isStopped = true;
        private ArrayList sessions = new ArrayList();
        #endregion

        #region Properties
        public bool IsActive
        {
            get { return serverSocket != null && !isStopped; }
        }
        public ArrayList Sessions
        {
            get { return sessions; }
        }
        #endregion

        #region Events
        public event TCPSessionEventHandler SessionConnected;
        public event TCPSessionDataReceivedEventHandler SessionDataReceived;
        public event TCPSessionEventHandler SessionDisconnected;
        #endregion

        #region Constructor
        public TCPServer(int port)
        {
            this.port = port;
        }
        #endregion

        #region Public methods
        public void Start()
        {
            if (serverSocket == null)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
                serverSocket.Bind(localEP);
                serverSocket.Listen(int.MaxValue);
                
                isStopped = false;

                new Thread(delegate {
                    using (serverSocket)
                    {
                        while (!isStopped)
                        {
                            try
                            {
                                if (serverSocket.Poll(1000 * 10, SelectMode.SelectRead))
                                {
                                    TCPSession session = new TCPSession(serverSocket.Accept());
                                    session.DataReceived += new TCPSessionDataReceivedEventHandler(NetworkClient_DataReceived);
                                    session.Disconnected += new TCPSessionEventHandler(NetworkClient_Disconnected);
                                    sessions.Add(session);
                                    
                                    if (SessionConnected != null)
                                        SessionConnected(session);

                                    session.Start();
                                }
                            }
                            catch (SocketException e)
                            {
                                if (e.ErrorCode != 10035)
                                    break;
                            }
                            catch (Exception e)
                            {
                                Debug.Print(e.Message);
                                break;
                            }
                            Thread.Sleep(0);
                        }
                    }

                    serverSocket.Close();
                    serverSocket = null;
                }).Start();
            }
        }
        public void Stop()
        {
            isStopped = true;
        }
        public void SendToAll(byte[] buffer)
        {
            foreach (TCPSession session in sessions)
                session.Send(buffer);
        }
        #endregion

        #region Event handlers
        private bool NetworkClient_DataReceived(TCPSession session, byte[] data)
        {
            return SessionDataReceived != null ? SessionDataReceived(session, data) : false;
        }
        private void NetworkClient_Disconnected(TCPSession session)
        {
            if (SessionDisconnected != null)
                SessionDisconnected(session);

            if (sessions.Contains(session))
                sessions.Remove(session);
        }
        #endregion
    }
}
