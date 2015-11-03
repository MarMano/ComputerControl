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

        private ManualResetEvent tcpCLientConnected = new ManualResetEvent(false);

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
                tcpCLientConnected.Reset();
                Listener.BeginAcceptTcpClient(AcceptTcpClient, Listener);
                tcpCLientConnected.WaitOne();
            }
        }

        private void AcceptTcpClient(IAsyncResult ar)
        {
            var listener = (TcpListener) ar.AsyncState;
            var client = listener.EndAcceptTcpClient(ar);

            HttpProcessor processor = new HttpProcessor(client, this);
            Thread thread = new Thread(processor.process);
            thread.IsBackground = true;
            thread.Start();

            tcpCLientConnected.Set();
        }

        public abstract void handleGETRequest(HttpProcessor p);
        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    }
}
