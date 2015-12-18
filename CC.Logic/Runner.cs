using System;
using System.Collections.Generic;
using System.Net.Sockets;
using CC.Logic.Commands;
using CC.Metrics;
using CC.Models;
using CC.Web;
using ManoSoftware.WebSocketServer;
using ManoSoftware.WebSocketServer.EventArguments;
using Newtonsoft.Json;

namespace CC.Logic
{
    public class Runner
    {
        private CpuLoad _cpuLoad;
        private RamUsage _ramUsage;
        private DiskActivity _diskActivity;
        private WebSocketServer _webSocket;
        private List<TcpClient> _clients;
        private CCHttpServer _httpServer;

        public void Start()
        {
            _clients = new List<TcpClient>();

            _webSocket = new WebSocketServer(5050);
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
        }

        public void Stop()
        {
            _cpuLoad.Stop();
            _ramUsage.Stop();
            _diskActivity.Stop();
            _webSocket.Stop();
            _httpServer.Stop();
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

        private void _webSocket_NewMessage(object sender, NewMessageEventArgs args)
        {
            var message = JsonConvert.DeserializeObject<Message>(args.Message);
            var type = Type.GetType("CC.Logic.Commands" + "." + message.Type + "Command");
            if (type == null)
                return;
            
            var command = Activator.CreateInstance(type) as BaseCommand;
            if (command == null)
                return;

            if (typeof (InitCommand) == command.GetType())
                _clients.Add(args.Client);

            var returnContent = command.Handle(message.Arguments);

            if(!string.IsNullOrEmpty(returnContent))
                _webSocket.SendMessage(args.Client, returnContent);
        }

        private void _webSocket_ClientDisconnected(object sender, ClientConnectionEventArgs args)
        {
            _clients.Remove(args.Client);
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
