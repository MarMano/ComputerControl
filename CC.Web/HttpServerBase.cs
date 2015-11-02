using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CC.Web
{
    public abstract class HttpServerBase
    {
        private readonly int _port;
        protected TcpListener Listener;
        protected bool IsActive = true;

        protected HttpServerBase(int port)
        {
            _port = port;
        }

        public void Listen()
        {
            Listener = new TcpListener(IPAddress.Any, _port);
            Listener.Start();
            while (IsActive)
            {
                try
                {
                    TcpClient s = Listener.AcceptTcpClient();
                    HttpProcessor processor = new HttpProcessor(s, this);
                    Thread thread = new Thread(processor.process);
                    thread.Start();
                    Thread.Sleep(1);
                }
                catch (SocketException)
                {}
                
            }
        }

        public abstract void handleGETRequest(HttpProcessor p);
        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    }
}
