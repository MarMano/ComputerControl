using System;
using System.Collections.Generic;
using System.Net.Sockets;
using CC.Logic.Commands;
using CC.Metrics;
using CC.Models;
using CC.Web;
using Newtonsoft.Json;

namespace CC.Logic
{
    public class Runner
    {
        private CpuLoad _cpuLoad;
        private RamUsage _ramUsage;
        private DiskActivity _diskActivity;
        private WebSocketServer.WebSocketServer _webSocket;
        private List<TcpClient> _clients;
        private CCHttpServer _httpServer;

        public void Start()
        {
            _clients = new List<TcpClient>();

            _webSocket = new WebSocketServer.WebSocketServer(5050);
            _webSocket.ClientConnected += _webSocket_ClientConnected;
            _webSocket.ClientDisconnected += _webSocket_ClientDisconnected;
            _webSocket.NewMessage += _webSocket_NewMessage;

            _webSocket.Start();

            _cpuLoad = new CpuLoad();
            _cpuLoad.Update += _cpuLoad_Update;

            _ramUsage = new RamUsage();
            _ramUsage.Update += _ramUsage_Update;

            _diskActivity = new DiskActivity();
            _diskActivity.Update += _diskActivity_Update;

            _httpServer = new CCHttpServer();

            Console.WriteLine("Server is ready. Press Q to exit."); 
        }

        public void Stop()
        {
            _cpuLoad.Stop();
            _ramUsage.Stop();
            _diskActivity.Stop();
            _webSocket.Stop();
            _httpServer.Stop();
            Console.WriteLine("\nSystem shutting down!");
        }

        private void _diskActivity_Update(object sender, Models.EventArguments.DiskUpdateEvent args)
        {
            foreach (var tcpClient in _clients)
            {
                _webSocket.SendMessage(tcpClient, JsonConvert.SerializeObject(new { Type = "DiskInfo", args.Data }));
            }
        }

        private void _ramUsage_Update(object sender, Models.EventArguments.RamUpdateEventArgs args)
        {
            foreach (var tcpClient in _clients)
            {
                _webSocket.SendMessage(tcpClient, JsonConvert.SerializeObject(new { Type = "RamInfo", Data = new {args.Avaliable, args.Total } }));
            }
        }

        private void _webSocket_NewMessage(object sender, WebSocketServer.EventArguments.NewMessageEventArgs args)
        {
            var message = JsonConvert.DeserializeObject<Message>(args.Message);
            var type = Type.GetType("CC.Logic.Commands" + "." + message.Type + "Command");
            if (type == null)
                return;
            
            var command = Activator.CreateInstance(type) as BaseCommand;
            if (command == null)
                return;

            _webSocket.SendMessage(args.Client, command.Handle(message.Arguments));

            _clients.Add(args.Client);
        }

        private void _webSocket_ClientDisconnected(object sender, WebSocketServer.EventArguments.ClientConnectionEventArgs args)
        {
            Console.WriteLine("Client Disconnected");
            _clients.Remove(args.Client);
        }

        private void _webSocket_ClientConnected(object sender, WebSocketServer.EventArguments.ClientConnectionEventArgs args)
        {
            Console.WriteLine("Client Connected");
            
        }

        private void _cpuLoad_Update(object sender, Models.EventArguments.CpuUpdateEventArgs args)
        {
            foreach (var tcpClient in _clients)
            {
                _webSocket.SendMessage(tcpClient, JsonConvert.SerializeObject(new { Type = "CpuInfo", args.Data }));
            }
        }
    }
}
