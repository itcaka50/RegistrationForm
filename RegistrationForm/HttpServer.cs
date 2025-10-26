using RegistrationForm.Utils;
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

                    var buffer = new byte[8192];
                    var requestBuilder = new StringBuilder();
                    var bytes = stream.Read(buffer, 0, buffer.Length);
                    requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
                    System.Threading.Thread.Sleep(50);

                    while (stream.DataAvailable)
                    {
                        bytes = stream.Read(buffer, 0, buffer.Length);
                        if (bytes > 0) requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
                    }
                    var requestText = requestBuilder.ToString();

                    var sessionId = ExtractSessionId(requestText);

                    var response = router.HandleRequest(requestText, sessionId,out string newSessionId);
                    var contentType = "text/html";

                    if (requestText.Contains(".css")) contentType = "text/css";

                    if (requestText.Contains("GET /captcha.png"))
                    {
                        HandleCaptchaImage(stream);
                        continue;
                    }


                    var header = "HTTP/1.1 200 OK\r\n" +
                                 $"Content-Type: {contentType}; charset=UTF-8\r\n";

                    if (!string.IsNullOrEmpty(newSessionId))
                    {
                        header += $"Set-Cookie: sessionId={newSessionId}; Path=/; HttpOnly\r\n";
                    }

                    header += "Connection: close\r\n\r\n";
                    var fullResponse = Encoding.UTF8.GetBytes(header + response);
                    stream.Write(fullResponse, 0, fullResponse.Length);
                    
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"Failed to start {ex}");
                }
            }
        }

        private void HandleCaptchaImage(NetworkStream stream)
        {
            try
            {
                var (code, imageBytes) = Captcha.GenerateCaptcha();
                CaptchaStorage.CurrentCode = code;

                var header = "HTTP/1.1 200 OK\r\n" +
                             "Content-Type: image/png\r\n" +
                             $"Content-Length: {imageBytes.Length}\r\n" +
                             "Connection: close\r\n\r\n";

                var headerBytes = Encoding.UTF8.GetBytes(header);
                stream.Write(headerBytes, 0, headerBytes.Length);
                stream.Write(imageBytes, 0, imageBytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CAPTCHA error: {ex.Message}");
                var errorHeader = "HTTP/1.1 500 Internal Server Error\r\n\r\n";
                var errorBytes = Encoding.UTF8.GetBytes(errorHeader);
                stream.Write(errorBytes, 0, errorBytes.Length);
            }
        }

        private string ExtractSessionId(string request)
        {
            var lines = request.Split(new[] { "\r\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (line.StartsWith("Cookie:"))
                {
                    var cookies = line.Substring(7).Trim();
                    var parts = cookies.Split(';');
                    foreach (var part in parts)
                    {
                        var keyValue = part.Trim().Split('=');
                        if (keyValue.Length == 2 && keyValue[0] == "sessionId")
                        {
                            return keyValue[1];
                        }
                    }
                }
            }
            return null;
        }

    }
}
