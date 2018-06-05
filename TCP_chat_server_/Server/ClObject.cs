using System;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class ClObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        string login;
        string password;
        TcpClient client;
        ServObject server;

        public ClObject(TcpClient tcpClient, ServObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                // get username
                string message = GetMessage();
                userName = message;

                message = userName + " go in the chat";

                Console.WriteLine(message);

                SendMessage("Eneter login =>");
                login = GetMessage();
                SendMessage("Enter password =>");
                password = GetMessage();

                // In an infinite loop, we receive messages from the Client

                while (true)
                {
                    try
                    {
                        SendMessage($"{userName} enter command");
                        message = GetMessage().Trim().ToUpper();
                        switch (message)
                        {
                            case "LOGIN":

                                break;
                        }

                        //message = String.Format("{0}: {1}", userName, message);
                        //Console.WriteLine(message);
                    }
                    catch
                    {
                        message = String.Format("{0}: left chat", userName);
                        Console.WriteLine(message);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // In case of an exit from a cycle we close resources

                server.RemoveConnection(this.Id);
                Close();
            }
        }

        private void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            Stream.Write(data, 0, data.Length);
        }

        // Reading an incoming message and converting to a string
        private string GetMessage()
        {
            byte[] data = new byte[64]; // buffer for received data
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // Close Connection
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}