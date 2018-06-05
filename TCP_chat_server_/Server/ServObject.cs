using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class ServObject
    {
        static TcpListener tcpListener; // listening server
        List<ClObject> clients = new List<ClObject>(); // all connections

        protected internal void AddConnection(ClObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            // we obtain by id a closed connection
            ClObject client = clients.FirstOrDefault(c => c.Id == id);
            // and delete it from the list of connections
            if (client != null)
                clients.Remove(client);
        }
        // listening for incoming connections
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 1000);
                tcpListener.Start();
                Console.WriteLine("Server is started. Waiting for connections...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClObject clientObject = new ClObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // disabling all clients
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //server shutdown

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //disconnection of the client
            }
            Environment.Exit(0); //completion of the process
        }
    }
}
