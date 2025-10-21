using RegistrationForm;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;



class Program
{
    static void Main()
    {
        var server = new HttpServer(8080);
        server.Start();
    }
}
