using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Web
{
    public class CCHttpServer : HttpServerBase
    {
        public CCHttpServer(int port = 8080) : base(port)
        {
        }

        public override void handleGETRequest(HttpProcessor p)
        {
            if (p.http_url == "/")
            {
                p.writeSuccess();
                p.outputStream.WriteLine(File.ReadAllText("Website/index.html"));
            }

            if (File.Exists("Website" + p.http_url))
            {
                if(p.http_url.EndsWith(".css"))
                    p.writeSuccess("text/css");
                else
                    p.writeSuccess();
                
                p.outputStream.WriteLine(File.ReadAllText("Website" + p.http_url));
            }
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            throw new NotImplementedException();
        }
    }
}
