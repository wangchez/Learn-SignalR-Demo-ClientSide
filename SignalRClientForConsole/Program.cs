using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace SignalRClientForConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            p.CallToServer();
        }

        void CallToServer()
        {
            Console.WriteLine("Input your name ==>");
            string inputName = Console.ReadLine();

            var hubConnection = new HubConnection("http://localhost:50515/");
            hubConnection.ConnectionSlow += () => Console.WriteLine("Connection problems.");
            hubConnection.Error += hubConnection_Error;
            IHubProxy consoleHubProxy = hubConnection.CreateHubProxy("HubCenter");
            consoleHubProxy.On("NewMessage", (string name, string message) =>
            {
                Console.WriteLine("{0} say: {1}", name, message);
            });

            hubConnection.Start().Wait();

            string sayWhat;
            Console.WriteLine("Say something.");
            while ((sayWhat = Console.ReadLine()) != "")
            {
                try
                {
                    consoleHubProxy.Invoke("CallFromConsole", new ClientModel { Name = inputName, Message = sayWhat }).Wait();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("SignalR error: {0}", ex.Message);
                }
            }

            hubConnection.Stop();
        }

        void hubConnection_Error(Exception ex)
        {
            Console.WriteLine("SignalR error: {0}", ex.Message);
        }
    }

    public class ClientModel
    {
        public string Name { get; set; }

        public string Message { get; set; }
    }
}
