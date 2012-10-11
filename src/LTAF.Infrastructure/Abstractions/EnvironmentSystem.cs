using System;
using System.Net;
using System.Net.Sockets;

namespace LTAF.Infrastructure.Abstractions
{
    internal class EnvironmentSystem : IEnvironmentSystem
    {
        public string ExpandEnvironmentVariables(string environmentVariable)
        {
            return Environment.ExpandEnvironmentVariables(environmentVariable);
        }

        public int GetNextAvailablePort(int usedPort = 0)
        {
            int lowerAddressRangeBoundary = 2048;
            int higherAddressRangeBoundary = 3072;
            Random r = new Random(unchecked((int)DateTime.Now.Ticks));
            int port = 0;

            do
            {
                port = r.Next(lowerAddressRangeBoundary, higherAddressRangeBoundary);

            } while (IsLocalTcpPortInUse(port)  || port == usedPort);

            return port;
        }

        public Version OSVersion 
        {
            get
            {
                return Environment.OSVersion.Version;
            }
        }

        private bool IsLocalTcpPortInUse(int port)
        {
            bool result = true;

            Socket socket = null;
            IPAddress ipAddress = Dns.GetHostEntry("127.0.0.1").AddressList[0];
            IPEndPoint ipe = new IPEndPoint(ipAddress, (int)port);
            socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(ipe);
                result = socket.Connected;
            }
            catch (SocketException)
            {
                result = false;
            }
            finally
            {
                if (socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
            }

            return result;
        }
    }
}
