using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataC
{
    class Upload
    {
        public static Socket socket;

        //public Upload()
        //{
        //    localip = ipsearch();
        //    buildsocket();
        //}

        private static string ipsearch()
        {

            string ethip = null;

            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                //判断是否为以太网卡
                //Wireless80211         无线网卡    Ppp     宽带连接
                //Ethernet              以太网卡   
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    //获取以太网卡网络接口信息
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    //获取单播地址集
                    UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                    foreach (UnicastIPAddressInformation ipadd in ipCollection)
                    {
                        //InterNetwork    IPV4地址      InterNetworkV6        IPV6地址
                        //Max            MAX 位址
                        if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            //判断是否为ipv4
                            ethip = ipadd.Address.ToString();
                            //Console.WriteLine(ethip);
                            if (ethip.StartsWith("10"))
                            {
                                return ethip;
                            }
                            //return ethip;
                        }
                    }
                }
                //if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                //{
                //    //获取以太网卡网络接口信息
                //    IPInterfaceProperties ip = adapter.GetIPProperties();
                //    //获取单播地址集
                //    UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                //    foreach (UnicastIPAddressInformation ipadd in ipCollection)
                //    {
                //        //InterNetwork    IPV4地址      InterNetworkV6        IPV6地址
                //        //Max            MAX 位址
                //        if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                //        {
                //            //判断是否为ipv4
                //            ethip = ipadd.Address.ToString();
                //            Console.WriteLine(ethip);
                //            if (ethip.StartsWith("218"))
                //            {
                //                return ethip;
                //            }
                //        }
                //    }
                //}
            }
            return null;
        }

        public static bool buildsocket()
        {
            string localip = ipsearch();
            //string localip = "192.168.0.66";
            string ipString = Form2.sip;
            //string ipString = "114.215.189.49";
            Int32 port = 8431;//转换为int型
            IPAddress ip = IPAddress.Parse(ipString);//将ip转换为IPAddress
            IPAddress ipl = IPAddress.Parse(localip);
            IPEndPoint IPEPoint = new IPEndPoint(ip, port);//实例化IPEndPoint类
            IPEndPoint IPlocal = new IPEndPoint(ipl, 8431);
            //实例化Socket类以便用于远程连接主机
            socket = new Socket(IPEPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(IPlocal);
            try
            {
                socket.Connect(IPEPoint);
            }
            catch(Exception ex1)
            {
                MessageBox.Show(ex1.Message);
            }
            //socket.Connect(IPEPoint);
            if (socket.Connected)
            {
                return true;
            }
            else
            {
                MessageBox.Show("连接服务器失败", "提示", MessageBoxButtons.OK);
                return false;
            }
        }

        public static bool socketsend(byte[] data)
        {
            if (!socket.Connected)
                return false;

            try
            {
                //for(int j=0;j<48;j++)
                //{
                //    Console.Write(data[j]);
                //    Console.Write(" ");
                //}
                //Console.Write(data[48]);
                socket.Send(data);
                //byte[] a = new byte[4248];
                //for (int j=0;j<4248;j++)
                //{
                //    a[j] = 0x02;
                //}
                //socket.Send(a);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            //return true;
        }

        //public static bool socketconnect()
        //{
        //    return socket.Connected;
        //}
        
    }
}
