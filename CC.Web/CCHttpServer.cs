using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CC.Web
{
    public class CCHttpServer : HttpServerBase
    {
        private Thread _listener;
        public CCHttpServer(int port = 8080) : base(port)
        {
            _listener = new Thread(Listen);
            _listener.IsBackground = true;
            _listener.Start();
        }

        public override void handleGETRequest(HttpProcessor p)
        {
            if (p.http_url == "/")
            {
                p.writeSuccess();
                p.outputStream.WriteLine(File.ReadAllText("Website/index.html"));
                return;
            }

            if (!File.Exists("Website" + p.http_url))
            {
                p.writeFailure();
                return;
            }

            if(p.http_url.EndsWith(".css"))
                p.writeSuccess("text/css");
            else
                p.writeSuccess();
                
            p.outputStream.WriteLine(File.ReadAllText("Website" + p.http_url));
        }

        public void Stop()
        {
            IsActive = false;
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            throw new NotImplementedException();
        }
    }
}
