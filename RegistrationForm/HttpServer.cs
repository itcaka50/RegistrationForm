using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationForm
{
    public class HttpServer
    {
        private readonly int port;
        private readonly Router router;


        public HttpServer(int port)
        {
            this.port = port;
            this.router = new Router();
        }

        public void Start()
        {
            var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            Console.WriteLine($"Server started on http://localhost:{port}");

            while ( true ) 
            {
                try
                {
                    using var client = listener.AcceptTcpClient();
                    using var stream = client.GetStream();

                    var buffer = new byte[4096];
                    var bytes = stream.Read(buffer, 0, buffer.Length);
                    var requestText = Encoding.UTF8.GetString(buffer, 0, bytes);

                    var response = router.HandleRequest(requestText);

                    var header = "HTTP/1.1 200 OK\r\nContent-Type: text/html; charset=UTF-8\r\nConnection: close\r\n\r\n";
                    var fullResponse = Encoding.UTF8.GetBytes(header + response);
                    stream.Write(fullResponse, 0, fullResponse.Length);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"Failed to start {ex}");
                }
            }
        }

    }
}
