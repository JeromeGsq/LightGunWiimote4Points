using LightGunWiimote4Points.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using WiimoteLib;

namespace LightGunWiimote4Points
{
    static class Program
    {
        static IPHostEntry host = Dns.GetHostEntry("localhost");
        static IPAddress ipAddr = host.AddressList[0];
        static IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

        static Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        static Socket clientSocket = null;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Position resolution = new Position(2560, 1440);
            WiimoteCollection mWC = new WiimoteCollection();

            try
            {
                mWC.FindAllWiimotes();

                for (int i = 0; i < mWC.Count; i++)
                {
                    new CoreWiimote(mWC[i], i + 1, resolution);
                }

                listener.Bind(localEndPoint);
                listener.Listen(1);
                Console.WriteLine(localEndPoint.ToString());

                while (true)
                {
                    Console.WriteLine("Waoiting connection ... ");
                    clientSocket = listener.Accept();
                }
            }
            catch (WiimoteNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Wiimote not found error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (WiimoteException ex)
            {
                MessageBox.Show(ex.Message, "Wiimote error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unknown error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                clientSocket?.Shutdown(SocketShutdown.Both);
                clientSocket?.Close();
            }

            // Don't do that
            while (true)
            {
            }
        }

        public static void SendMessage(string data)
        {
            if (clientSocket != null)
            {
                try
                {
                    byte[] message = Encoding.ASCII.GetBytes(data);
                    clientSocket?.Send(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}