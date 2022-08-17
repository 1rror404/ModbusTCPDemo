// <copyright file="ModbusTCPClient.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ModbusTCPDemo
{
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// ModbusTCP客户端.
    /// </summary>
    public class ModbusTCPClient
    {
        private Socket clientSocket;

        /// <summary>
        /// 连接服务器.
        /// </summary>
        /// <param name="ip">服务器ip.</param>
        /// <param name="port">服务器端口.</param>
        /// <returns>是否连接上.</returns>
        public bool ConnectTCP(string ip, int port)
        {
            IPAddress ipAddress = IPAddress.Parse(ip);

            // 初始化Socket.
            this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // 连接服务器.
                this.clientSocket.Connect(new IPEndPoint(ipAddress, port));
            }
            catch (Exception)
            {
                return false;
                throw;
            }

            return true;
        }

        /// <summary>
        /// 断开服务器.
        /// </summary>
        public void DisConnect()
        {
            if (this.clientSocket != null)
            {
                this.clientSocket.Dispose();
            }
        }

        /// <summary>
        /// 读输出线圈. 01.
        /// </summary>
        /// <param name="slaveId">设备标识.</param>
        /// <param name="start">起始地址.</param>
        /// <param name="length">长度.</param>
        /// <returns>线圈值.</returns>
        public string ReadOutputCoils(int slaveId, int start, int length)
        {
            // 1.拼接报文.
            List<byte> sendMsg = new List<byte>();
            sendMsg.Add(0x00);
            sendMsg.Add(0x00);

            sendMsg.Add(0x00);
            sendMsg.Add(0x00);

            sendMsg.Add(0x00);
            sendMsg.Add(0x06); // 长度.

            sendMsg.Add((byte)slaveId); // 设备标识.

            sendMsg.Add(0x01); // 功能码.

            sendMsg.Add((byte)(start / 256));  // 起始地址除256得到高八位
            sendMsg.Add((byte)(start % 256));  // 起始地址模256得到低八位

            sendMsg.Add((byte)(length / 256)); // 线圈数量除256得到高八位
            sendMsg.Add((byte)(length % 256)); // 线圈数量模256得到低八位

            // 2.发送报文.
            Console.WriteLine("发送报文:" + BitConverter.ToString(sendMsg.ToArray()));
            this.clientSocket.Send(sendMsg.ToArray());

            // 3.接收报文.
            byte[] buffer = new byte[1024];
            int count = this.clientSocket.Receive(buffer);
            byte[] result = new byte[count];
            Array.Copy(buffer, 0, result, 0, count);
            Console.WriteLine("接收报文:" + BitConverter.ToString(result));

            // 4.验证报文.
            byte[] temp = new byte[1024];
            if (length % 8 == 0)
            {
                if (result.Length != 9 + (length / 8))
                {
                    return null;
                }

                if (result[8] == length / 8)
                {
                    // 5.解析报文.
                    temp = this.GetChildBytes(buffer, 9, length / 8);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (result.Length != 9 + (length / 8) + 1)
                {
                    return null;
                }

                if (result[8] == (length / 8) + 1)
                {
                    // 5.解析报文.
                    temp = this.GetChildBytes(buffer, 9, (length / 8) + 1);
                }
                else
                {
                    return null;
                }
            }

            // 5.解析报文.
            string resultStr = string.Empty;
            foreach (var item in temp)
            {
                char[] c = Convert.ToString(item, 2).PadLeft(8, '0').ToCharArray();
                Array.Reverse(c);
                string res = new string(c);
                resultStr += res;
            }

            return resultStr.Substring(0, length);
        }

        /// <summary>
        /// 截取字符串数组.
        /// </summary>
        /// <param name="buffer">需要截取的byte数组.</param>
        /// <param name="startindex">起始地址.</param>
        /// <param name="length">长度.</param>
        /// <returns>结果.</returns>
        public byte[] GetChildBytes(byte[] buffer, int startindex, int length)
        {
            if (buffer != null && (startindex + length) <= buffer.Length)
            {
                byte[] res = new byte[length];
                Array.Copy(buffer, startindex, res, 0, length);
                return res;
            }
            else
            {
                return null;
            }
        }
    }
}
