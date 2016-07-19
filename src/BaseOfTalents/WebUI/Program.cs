﻿using System;
using System.Net.Http;
using Microsoft.Owin.Hosting;

namespace BaseOfTalents.WebUI
{
    public class Program
    {

        static void Main()
        {
#if !RELEASE
            int port = 54537;
            string options = $"http://localhost:{port}/";

#else
            int port = 9000;
            StartOptions options = new StartOptions($"http://+:{port}")
            {
                ServerFactory = "Microsoft.Owin.Host.HttpListener"
            };

            //StartOptions options = new StartOptions();
            //options.Urls.Add($"http://localhost:{port}");
            //options.Urls.Add($"http://127.0.0.1:{port}");
            //options.Urls.Add($"http://192.168.0.102:{port}");
            //options.Urls.Add($"http://{Environment.MachineName}:{port}");

#endif
            using (WebApp.Start<ApiStartup>(options))
            {
                Console.WriteLine("Server started...");
                Console.WriteLine($"Machine name: {Environment.MachineName}");
#if DEBUG
                HttpClient client = new HttpClient();
                var res = client.GetAsync($"http://localhost:{port}/api/tag").Result;
                Console.WriteLine($"{res.Content.ReadAsStringAsync().Result}");
#endif
                Console.WriteLine("Press ENTER to stop server");
                Console.ReadLine();
            }
            Console.WriteLine("End");
        }
    }
}