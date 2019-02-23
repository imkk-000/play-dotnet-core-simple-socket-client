using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint endPoint = null;
            Socket socket = null;
            byte[] buffer = new byte[65536];
            byte[] data = null;
            string rawCommand = "";
            string[] commands = null;

            while (!rawCommand.Equals("0"))
            {
                Console.Write("Send Command: ");
                rawCommand = Console.ReadLine();
                rawCommand = rawCommand.Trim();
                commands = rawCommand.Split(' ');

                try
                {
                    if (commands[0].Equals("connect"))
                    {
                        if (commands.Length != 3)
                        {
                            Console.WriteLine("use(string ip, int port)");
                            continue;
                        }
                        if (socket == null)
                        {
                            endPoint = new IPEndPoint(IPAddress.Parse(commands[1]), Convert.ToInt32(commands[2]));
                            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                        }
                        if (!socket.Connected)
                        {
                            socket.Connect(endPoint);
                            Console.WriteLine(socket.Connected);
                        }
                    }
                    else if (commands[0].Equals("disconnect"))
                    {
                        if (socket != null)
                        {
                            socket.Close();
                            socket = null;
                            Console.WriteLine(socket);
                        }
                    }
                    else if (commands[0].Equals("send"))
                    {
                        if (socket == null)
                        {
                            Console.WriteLine("socket not connect");
                            continue;
                        }
                        else if (!socket.Connected)
                        {
                            Console.WriteLine("socket not connect");
                            continue;
                        }
                        else if (commands.Length != 2)
                        {
                            Console.WriteLine("use(string data)");
                            continue;
                        }

                        socket.Send(HexStringToByteArray(commands[1].ToUpper()));
                        int receiveLength = socket.Receive(buffer);
                        data = ResizeBufferToData(buffer, receiveLength);
                        Console.WriteLine(ByteToHexString(data));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e);
                }
            }
            if (socket != null)
            {
                socket.Close();
                socket = null;
                Console.WriteLine(socket);
            }
            Console.WriteLine("Bye Bye!");
        }

        static byte[] ResizeBufferToData(byte[] buffer, int length)
        {
            byte[] data = new byte[length];
            Array.Copy(buffer, 0, data, 0, length);
            return data;
        }

        static string ByteToHexString(byte[] bytes, string seperateBy = "")
        {
            string result = "";
            for (uint i = 0; i < bytes.Length; i++)
            {
                result += bytes[i].ToString("x2");
                if (i < bytes.Length - 1)
                {
                    result += seperateBy;
                }
            }
            return result.ToUpper();
        }

        static byte[] HexStringToByteArray(string HexString)
        {
            string text = "";
            foreach (char char_ in HexString)
            {
                if (IsHexChar(char_))
                {
                    text += char_.ToString();
                }
            }
            if (text.Length % 2 != 0)
            {
                text = text.Substring(0, text.Length - 1);
            }
            byte[] array = new byte[text.Length / 2];
            int num = 0;
            for (int j = 0; j < array.Length; j++)
            {
                string string_ = new string(new char[] { text[num], text[num + 1] });
                array[j] = ConvertHexNumberStringToByte(string_);
                num += 2;
            }
            return array;
        }

        static bool IsHexChar(char hexChar)
        {
            int num = Convert.ToInt32('A');
            int num2 = Convert.ToInt32('0');
            hexChar = char.ToUpper(hexChar);
            int num3 = Convert.ToInt32(hexChar);
            return (num3 >= num && num3 < num + 6) || (num3 >= num2 && num3 < num2 + 10);
        }

        static byte ConvertHexNumberStringToByte(string HexNumber)
        {
            if (HexNumber.Length > 2 || HexNumber.Length <= 0)
            {
                return 0;
            }
            return byte.Parse(HexNumber, NumberStyles.HexNumber);
        }
    }
}
