// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ModbusTCPDemo
{
    /// <summary>
    /// 主程序.
    /// </summary>
    internal class Program
    {
        private static readonly string Ip = "127.0.0.1";
        private static readonly int Port = 502;
        private static readonly int SlaveId = 0x01;

        /// <summary>
        /// main.
        /// </summary>
        /// <param name="args">args.</param>
        private static void Main(string[] args)
        {
            ModbusTCPClient client = new ModbusTCPClient();
            bool isConnect = client.ConnectTCP(Ip, Port);
            if (isConnect)
            {
                // Console.WriteLine(client.ReadOutputCoils(SlaveId, 0, 10));
                // Console.WriteLine(client.ReadInputCoils(SlaveId, 0, 10));
               // Console.WriteLine(client.ReadOutputRegister(SlaveId, 0, 10));
            }
            else
            {
                Console.WriteLine("连接失败");
            }
        }
    }
}