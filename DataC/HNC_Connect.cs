using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Resources;
using System.Timers;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

//HNCAPI
using HNCAPI_INTERFACE;

namespace DataC
{
    class HNC_Connect
    {
        private static HncApi Api = new HncApi();

        public static string progN = null;
        public static string progN1 = null;
        public static string progNp = null;

        public static float pos_X = 1;
        public static float pos_Y = 0;
        public static float pos_Z = 0;
        public static float pos_C = 0;

        public static double speed_F = 0.0;
        public static double speed_S = 0.0;

        //public static int[] series_I;

        private static int cyc_ch = 0;
        private static int used_ch = 12;
        public static int cyc = 0;
        public static int gline = 0;
        public static int gsymbol = 0;
        public static int listnum = 0;
        public static int F_OVERRIDE = 0;
        public static int S_OVERRIDE = 0;
        public static int v_len = 40;
        public static int s_len = 40;

        public static float[] x = new float[4];
        public static float[] y = new float[4];
        public static float[] z = new float[4];
        public static float[] c = new float[4];

        //public static byte[,] xyzc = new byte[4, 16];

        public static string gcode_path = "D:\\NCVM\\Resource\\";

        public static bool start_transfer = false;
        public static bool progch_event = false;
        public static bool load_event = false;
        public static bool cyc_event = false;
        public static bool iscon = false;
        public static bool smplseted = false;
        public static bool issmpl = false;
        public static bool dataready = false;

        public static string ipnr = null;
        public static string ipnl = null;
        public static String temp_l = null;
        public static UInt16 portn = 0;

        public static Mutex getdata = new Mutex();

        public static int[] axisi;
        //public static Int16[] value_v;
        //public static Int16[] value_s;

        public static List<byte>[] temp2 = new List<byte>[4];
        private static List<Int16> value_v1 = new List<short>();
        private static List<Int16> value_s1 = new List<short>();

        public static Queue<Int32> i_que = new Queue<int>();
        public static Queue<Int16> v_que = new Queue<short>();
        public static Queue<Int16> s_que = new Queue<short>();

        private static Socket uploadSocket;
        //public static List<List<Int32>> m_listData = new List<List<int>>();
        //public static List<Int32>[] tmp = new List<Int32>[32];
        //public static int[,] datas = new int[25, 250];
        //public static int[] data = new int[5000];

        public HNC_Connect()
        {
            //getdata = new Mutex(true, "data");
        }

        /// <summary>
        /// HNC接口获取信息线程初始化
        /// </summary>
        private static void thread_init()
        {
            ;
        }

        /// <summary>
        /// HNC接口获取信息线程
        /// </summary>
        public static void thread_connect()
        {
            thread_init();
            Thread connect = new Thread(hnc_con);
            connect.IsBackground = true;
            connect.Start();
        }

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(ref long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(ref long lpFrequency);

        /// <summary>
        /// 循环获取所需HNC状态信息
        /// </summary>
        public static void hnc_con()
        {
            List<List<Int32>> m_listData = new List<List<int>>();
            //List<Int32>[] tmp = new List<Int32>[32];
            long freq = 0;

            if (QueryPerformanceFrequency(ref freq) == false)
            {
                throw new Exception("不支持高精度计时.");
            }
            double count_per_millsec = freq / 1000.0;
            long start_time = 0, stop_time = 0;
            long now_time = 0;

            QueryPerformanceCounter(ref start_time);
            while (true)
            {

                QueryPerformanceCounter(ref now_time);
                while (now_time - start_time < 200 * count_per_millsec)
                {
                    QueryPerformanceCounter(ref now_time);
                }
                Console.WriteLine("等待时间" + ((now_time - start_time) / count_per_millsec).ToString());
                start_time = now_time;

                List<List<Int32>> m_datatemp = new List<List<int>>();

                if (issmpl)
                {
                    //List<List<Int32>> m_datatemp = new List<List<int>>();

                    // 获取数据
                    int ret = Api.HNC_SamplGetData(m_datatemp);
                }

                //getdata.WaitOne();

                if (!iscon)
                {
                    //getdata.ReleaseMutex();
                    QueryPerformanceCounter(ref stop_time);
                    continue;
                }

                //List<List<Int32>> m_datatemp = new List<List<int>>();

                //// 获取数据
                //int ret = Api.HNC_SamplGetData(m_datatemp);

                // 循环获取HNC状态信息
                Api.HNC_FprogGetFullName(0, ref progNp);               // G代码名称
                if (progNp == null)
                {
                    QueryPerformanceCounter(ref stop_time);
                    continue;
                }
                //Console.WriteLine("运行测试2");
                if (progNp[0] == '/')
                {
                    progN = progNp.Remove(0, 13);
                }
                else
                {
                    progN = progNp.Remove(0, 8);
                }
                if (progN1 != progN)
                {
                    progN1 = progN;
                    //progch_event = true;
                    //String temp_r = "mnt/hgfs/hnc/share/HNC8_V1.26.04" + progN;
                    String temp_r = "/h/lnc8/prog/" + progN;
                    temp_l = gcode_path + progN;
                    int retr = getfile(temp_r, temp_l);
                    //int retr = 0;
                    if (retr != 0)
                    {
                        MessageBox.Show("G代码下载失败，错误码：" + retr.ToString());
                    }
                    else
                    {
                        load_event = true;
                    }
                    progch_event = true;
                    gcodeupload();
                }

                //Api.HNC_ChannelGetValue(18, 6, 0, ref cyc);            // 循环启动
                //Api.HNC_ChannelGetValue(32, 7, 0, ref gline);          // G代码行号
                //Api.HNC_ChannelGetValue(47, 8, 0, ref speed_S);        // 主轴转速
                //Api.HNC_ChannelGetValue(6, 9, 0, ref speed_F);         // 进给速度
                //Api.HNC_ChannelGetValue(9, 10, 0, ref F_OVERRIDE);      // 进给修调
                //Api.HNC_ChannelGetValue(49, 11, 0, ref S_OVERRIDE);     // 主轴修调

                Api.HNC_ChannelGetValue(18, 0, 0, ref cyc);            // 循环启动
                Api.HNC_ChannelGetValue(32, 0, 0, ref gline);          // G代码行号
                Api.HNC_ChannelGetValue(47, 0, 0, ref speed_S);        // 主轴转速
                Api.HNC_ChannelGetValue(6, 0, 0, ref speed_F);         // 进给速度
                Api.HNC_ChannelGetValue(9, 0, 0, ref F_OVERRIDE);      // 进给修调
                Api.HNC_ChannelGetValue(49, 0, 0, ref S_OVERRIDE);     // 主轴修调

                //Api.HNC_ChannelGetValue((Int32)HncChannel.HNC_CHAN_CYCLE, 16, 0, ref cyc);

                #region 接口数据转换

                List<byte> lsdata = new List<byte>();

                byte[] cyc1 = BitConverter.GetBytes(cyc);
                lsdata.AddRange(cyc1);

                byte[] gline1 = BitConverter.GetBytes(gline);
                lsdata.AddRange(gline1);

                byte[] speed_S1 = BitConverter.GetBytes(speed_S);
                lsdata.AddRange(speed_S1);

                byte[] speed_F1 = BitConverter.GetBytes(speed_F);
                lsdata.AddRange(speed_F1);

                byte[] F_OVERRIDE1 = BitConverter.GetBytes(S_OVERRIDE);
                lsdata.AddRange(F_OVERRIDE1);

                byte[] S_OVERRIDE1 = BitConverter.GetBytes(F_OVERRIDE);
                lsdata.AddRange(S_OVERRIDE1);

                #endregion

                // 循环启动变化触发事件
                if (cyc_ch != cyc)
                {
                    cyc_ch = cyc;
                    cyc_event = true;
                }

                getdata.WaitOne();

                // 采样是否打开
                if (!issmpl)
                {
                    getdata.ReleaseMutex();
                    QueryPerformanceCounter(ref stop_time);
                    continue;
                }


                try
                {
                    m_listData.Clear();
                }
                catch (Exception ex)
                {
                    //Ignore
                }

                //List<List<Int32>> m_datatemp = new List<List<int>>();

                //// 获取数据
                //int ret = Api.HNC_SamplGetData(m_datatemp);
                //Console.WriteLine("运行测试2：");
                if ((m_datatemp == null) || (m_datatemp.Count == 0))
                {
                    Console.WriteLine("运行测试3：未得到数据");
                    QueryPerformanceCounter(ref stop_time);
                    getdata.ReleaseMutex();
                    continue;
                }
                Console.WriteLine("运行测试3：已得到数据");
                m_listData = m_datatemp;

                int chnum = m_datatemp.Count;
                

                List<Int32>[] tmp = m_listData.ToArray();

                int[] series_X = tmp[0].ToArray();
                int[] series_Y = tmp[1].ToArray();
                int[] series_Z = tmp[2].ToArray();
                int[] series_C = tmp[3].ToArray();
                int[] series_I = tmp[4].ToArray();

                int dag1 = series_X.Length / 4;

                //for()
                temp2[0] = new List<byte>();
                temp2[1] = new List<byte>();
                temp2[2] = new List<byte>();
                temp2[3] = new List<byte>();

                for (int m1 = 0; m1 < 4; m1++)
                {

                    object obj_X = (object)series_X[(m1 + 1) * dag1 - 1];
                    object obj_Y = (object)series_Y[(m1 + 1) * dag1 - 1];
                    object obj_Z = (object)series_Z[(m1 + 1) * dag1 - 1];
                    object obj_C = (object)series_C[(m1 + 1) * dag1 - 1];

                    pos_X = x[m1] = Convert.ToSingle(obj_X) / 100000;
                    pos_Y = y[m1] = Convert.ToSingle(obj_Y) / 100000;
                    pos_Z = z[m1] = Convert.ToSingle(obj_Z) / 100000;
                    pos_C = c[m1] = Convert.ToSingle(obj_C) / 100000;

                    //temp2[m1].AddRange(lsdata);
                    byte[] a = BitConverter.GetBytes(x[m1]);
                    temp2[m1].AddRange(a);
                    a = BitConverter.GetBytes(y[m1]);
                    temp2[m1].AddRange(a);
                    a = BitConverter.GetBytes(z[m1]);
                    temp2[m1].AddRange(a);
                    a = BitConverter.GetBytes(c[m1]);
                    temp2[m1].AddRange(a);
                    temp2[m1].AddRange(lsdata);
                }

                axisi = series_I;

                int dataplist = tmp[0].Count;

                int listc = series_X.Length;

                #region i,v,s等数据转换

                int comp = listc / 4;

                //List<byte>[] i_byte = new List<byte>[4];
                //Queue<Int32> i_que = new Queue<Int32>();
                
                for (int l=0;l<series_I.Length;l++)
                {
                    i_que.Enqueue(series_I[l]);
                }

                //for (int l = 0; l < series_I.Length; l++)
                //{
                //    if (l > comp * 3)
                //    {
                //        byte[] a = BitConverter.GetBytes(series_I[l]);
                //        //i_byte[3].Enqueue()
                //        i_byte[3].AddRange(a);
                //    }
                //    else if (l > comp * 2)
                //    {
                //        byte[] a = BitConverter.GetBytes(series_I[l]);
                //        i_byte[2].AddRange(a);
                //    }
                //    else if (l > comp)
                //    {
                //        byte[] a = BitConverter.GetBytes(series_I[l]);
                //        i_byte[1].AddRange(a);
                //    }
                //    else
                //    {
                //        byte[] a = BitConverter.GetBytes(series_I[l]);
                //        i_byte[0].AddRange(a);
                //    }
                //}

                //int comp = listc / 4;

                //List<byte>[] v_byte = new List<byte>[4];

                //Queue<Int16> v_que = new Queue<Int16>();

                for (int m = 0; m < v_len / 4; m++)
                {
                    tmp[m + 5].Reverse();
                }

                for (int j = 0; j < listc; j++)
                {
                    for (int m = 0; m < v_len / 4; m++)
                    {
                        int temp = tmp[m + 5].Last<Int32>();
                        int temp_2 = tmp[m + 5].Last<Int32>();
                        tmp[m + 5].RemoveAt(tmp[m + 5].Count - 1);

                        Int16 hdata = (Int16)(temp >> 16);
                        Int16 ldata = (Int16)((temp_2 << 16) >> 16);

                        //v_que.Enqueue(ldata);
                        //v_que.Enqueue(hdata);
                        v_que.Enqueue(hdata);
                        v_que.Enqueue(ldata);
                    }
                }

                //for (int j = 0; j < listc; j++)
                //{
                //    for (int m = 0; m < v_len / 4; m++)
                //    {
                //        int temp = tmp[m + 5].Last<Int32>();
                //        tmp[m + 5].RemoveAt(tmp[m + 5].Count-1);

                //        Int16 hdata = (Int16)(temp >> 16);
                //        Int16 ldata = (Int16)((temp << 16) >> 16);

                //        value_v1.Add(hdata);
                //        value_v1.Add(ldata);

                //        if (j > comp*3)
                //        {
                //            byte[] a = BitConverter.GetBytes(hdata);
                //            v_byte[3].AddRange(a);
                //            byte[] b = BitConverter.GetBytes(ldata);
                //            v_byte[3].AddRange(b);
                //        }
                //        else if(j>comp*2)
                //        {
                //            byte[] a = BitConverter.GetBytes(hdata);
                //            v_byte[2].AddRange(a);
                //            byte[] b = BitConverter.GetBytes(ldata);
                //            v_byte[2].AddRange(b);
                //        }
                //        else if(j>comp)
                //        {
                //            byte[] a = BitConverter.GetBytes(hdata);
                //            v_byte[1].AddRange(a);
                //            byte[] b = BitConverter.GetBytes(ldata);
                //            v_byte[1].AddRange(b);
                //        }
                //        else
                //        {
                //            byte[] a = BitConverter.GetBytes(hdata);
                //            v_byte[0].AddRange(a);
                //            byte[] b = BitConverter.GetBytes(ldata);
                //            v_byte[0].AddRange(b);
                //        }
                //    }
                //}

                //List<byte>[] s_byte = new List<byte>[4];

                //Queue<Int16> s_que = new Queue<short>();

                for (int m = 0; m < v_len / 4; m++)
                {
                    tmp[m + 15].Reverse();
                }

                for (int j = 0; j < listc; j++)
                {
                    for (int m = 0; m < s_len / 4; m++)
                    {
                        int temp = tmp[m + 15].Last<Int32>();
                        tmp[m + 15].RemoveAt(tmp[m + 15].Count - 1);

                        Int16 hdata = (Int16)(temp >> 16);
                        Int16 ldata = (Int16)((temp << 16) >> 16);

                        s_que.Enqueue(hdata);
                        s_que.Enqueue(ldata);
                    }

                }

                int a2 = v_que.Count;
              
                if (i_que.Count > 1000)
                    i_que.Clear();
                if (v_que.Count > 10000)
                    v_que.Clear();
                if (s_que.Count > 10000)
                    s_que.Clear();
                //for (int j = 0; j < listc; j++)
                //{
                //    for (int m = 0; m < s_len / 4; m++)
                //    {
                //        int temp = tmp[m + 15].Last<Int32>();
                //        tmp[m + 15].RemoveAt(tmp[m + 15].Count-1);

                //        Int16 hdata = (Int16)(temp >> 16);
                //        Int16 ldata = (Int16)((temp << 16) >> 16);

                //        value_s1.Add(hdata);
                //        value_s1.Add(ldata);

                //        if (j > comp * 3)
                //        {
                //            byte[] a = BitConverter.GetBytes(hdata);
                //            s_byte[3].AddRange(a);
                //            byte[] b = BitConverter.GetBytes(ldata);
                //            s_byte[3].AddRange(b);
                //        }
                //        else if (j > comp * 2)
                //        {
                //            byte[] a = BitConverter.GetBytes(hdata);
                //            s_byte[2].AddRange(a);
                //            byte[] b = BitConverter.GetBytes(ldata);
                //            s_byte[2].AddRange(b);
                //        }
                //        else if (j > comp)
                //        {
                //            byte[] a = BitConverter.GetBytes(hdata);
                //            s_byte[1].AddRange(a);
                //            byte[] b = BitConverter.GetBytes(ldata);
                //            s_byte[1].AddRange(b);
                //        }
                //        else
                //        {
                //            byte[] a = BitConverter.GetBytes(hdata);
                //            s_byte[0].AddRange(a);
                //            byte[] b = BitConverter.GetBytes(ldata);
                //            s_byte[0].AddRange(b);
                //        }
                //    }
                //}

                //value_v = value_v1.ToArray();
                //value_s = value_s1.ToArray();

                #endregion

                //byte[] div = { 0x01, 0x02, 0x03, 0x04 };


                //for(int ki=0;ki<4;ki++)
                //{
                //    datatemp[ki].AddRange(temp2[ki]);
                //    datatemp[ki].AddRange(lsdata);
                //    datatemp[ki].AddRange(div);
                //    datatemp[ki].AddRange(i_byte[ki]);
                //    datatemp[ki].AddRange(v_byte[ki]);
                //    datatemp[ki].AddRange(s_byte[ki]);
                //}
                //datatemp.AddRange(lsdata);
                //datatemp.AddRange(x_byte);
                //datatemp.AddRange(y_byte);
                //datatemp.AddRange(z_byte);
                //datatemp.AddRange(c_byte);
                //datatemp.AddRange(i_byte);
                //datatemp.AddRange(v_byte);
                //datatemp.AddRange(s_byte);

                //byte[] datasend = datatemp.ToArray();

                //Upload.socketsend(datasend);

                lsdata.Clear();
                //x_byte.Clear();
                //y_byte.Clear();
                //z_byte.Clear();
                //c_byte.Clear();
                //i_byte.Clear();
                //v_byte.Clear();
                //s_byte.Clear();
                //datatemp.Clear();

                //value_v = value_v1.ToArray();
                //value_s = value_s1.ToArray();

                //if (!(value_v.Length < 10))
                //{
                dataready = true;
                //}

                getdata.ReleaseMutex();

                QueryPerformanceCounter(ref stop_time);

                Console.WriteLine("通道数目" + chnum.ToString());
                Console.WriteLine("数据长度" + dataplist.ToString());
                Console.WriteLine("使用时间 " + ((stop_time - start_time) / count_per_millsec).ToString());
            }
        }

        public static bool smplset_f()
        {
            //Api.HNC_SamplSetChannel(0, 102, 0, 4, 4);
            //Api.HNC_SamplSetChannel(0, 1, 0, 0, 0);          // X轴位置
            //Api.HNC_SamplSetChannel(1, 1, 1, 0, 0);          // Y轴位置
            //Api.HNC_SamplSetChannel(2, 1, 2, 0, 0);          // Z轴位置
            //Api.HNC_SamplSetChannel(3, 1, 5, 0, 0);          // C轴位置
            //Api.HNC_SamplSetChannel(4, 6, 0, 4, 4);          // 主轴电流

            return true;
        }

        public static bool smplset(int offset, int length)
        {
            int num = length / 4;
            int num_x = length % 4;

            for (int i = 0; i < num; i++)
            {
                Api.HNC_SamplSetChannel(used_ch + i, (Int32)HncSampleType.SAMPL_X_REG, 0, offset + 4 * i, 4);
            }
            if (num_x != 0)
            {
                Api.HNC_SamplSetChannel(used_ch + num, (Int32)HncSampleType.SAMPL_X_REG, 0, offset + 4 * num, num_x);
                used_ch = used_ch + num + 1;
            }
            else
            {
                used_ch = used_ch + num;
            }

            return true;
        }

        public static bool smplsetoff(int offset, int length)
        {
            int num = length / 4;
            int num_x = length % 4;

            for (int i = 0; i < num; i++)
            {
                Api.HNC_SamplRemoveConfig((Int32)HncSampleType.SAMPL_X_REG, 0, offset + 4 * i, 4);
            }
            if (num_x != 0)
            {
                Api.HNC_SamplRemoveConfig((Int32)HncSampleType.SAMPL_X_REG, 0, offset + 4 * num, num_x);
                used_ch = used_ch + num;
            }
            else
            {
                used_ch = used_ch + num - 1;
            }

            return true;
        }

        public static void smplperiod()
        {
            Api.HNC_SamplSetPeriod(1);
            //Api.HNC_SamplSetChannel(1,(Int32)HncSampleType.)
            Api.HNC_SamplSetChannel(1, (Int32)HncSampleType.SAMPL_CMD_POS, 0, 0, 0);           // X指令位置
            Api.HNC_SamplSetChannel(2, (Int32)HncSampleType.SAMPL_CMD_POS, 1, 0, 0);           // Y指令位置
            Api.HNC_SamplSetChannel(3, (Int32)HncSampleType.SAMPL_CMD_POS, 2, 0, 0);           // Z指令位置
            Api.HNC_SamplSetChannel(4, (Int32)HncSampleType.SAMPL_CMD_POS, 5, 0, 0);          // C指令位置
            Api.HNC_SamplSetChannel(5, (Int32)HncSampleType.SAMPL_ACT_TRQ, 0, 0, 0);          // I主轴电流
        }

        public static bool smplon()
        {
            Api.HNC_SamplTriggerOn();
            int ret = Api.HNC_RegSetBit((Int32)HncRegType.REG_TYPE_G, 2960, 12);

            if (ret == 0)
            {
                issmpl = true;
                return true;
            }
            return false;
        }

        public static bool smploff()
        {
            //Api.HNC_SamplRemoveConfig((Int32)HncSampleType.SAMPL_CMD_POS, 0, 0, 0);
            //Api.HNC_SamplRemoveConfig((Int32)HncSampleType.SAMPL_CMD_POS, 1, 0, 0);
            //Api.HNC_SamplRemoveConfig((Int32)HncSampleType.SAMPL_CMD_POS, 2, 0, 0);
            //Api.HNC_SamplRemoveConfig((Int32)HncSampleType.SAMPL_CMD_POS, 5, 0, 0);
            //Api.HNC_SamplRemoveConfig((Int32)HncSampleType.SAMPL_ACT_TRQ, 0, 0, 0);
            //smplsetoff(20, 40);
            //smplsetoff(60, 40);
            //Api.HNC_SamplRemoveConfig((Int32)HncSampleType.SAMPL_X_REG,)
            Api.HNC_SamplTriggerOff();

            return true;
        }

        /// <summary>
        /// 连接机床网络
        /// </summary>
        /// <param name="ipr">
        /// 机床ip
        /// </param>
        /// <param name="ipl">
        /// 本地ip
        /// </param>
        /// <param name="port">
        /// 机床端口
        /// </param>
        /// <returns>
        /// true：连接成功
        /// false：连接失败
        /// </returns>
        public static int start_connect(string ipr, string ipl, UInt16 port)
        {
            ipnl = ipl;
            ipnr = ipr;
            portn = port;
            int ret;

            // 初始化
            ret = Api.HNC_NetInit(ipnl, 9090, "DataC");
            if (ret != 0)
            {
                //MessageBox.Show("初始化失败");
                return 1;
            }

            // 连接
            ret = Api.HNC_NetConnect(ipnr, portn);

            Thread.Sleep(10);

            // 检测是否已连接
            if (Api.HNC_NetIsConnect(ipnr, portn) == 0)
            {
                iscon = true;
                return 0;
            }
            else
            {
                return 2;
            }
        }

        /// <summary>
        /// 检查是否已经连接
        /// </summary>
        /// <returns>
        /// true：已连接
        /// false：未连接
        /// </returns>
        public static bool check_connect()
        {
            if (ipnr == null)
            {
                return false;
            }

            if (Api.HNC_NetIsConnect(ipnr, portn) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据路径从机床获取G代码文件
        /// </summary>
        /// <param name="rpath">
        /// 机床端路径
        /// </param>
        /// <param name="lpath">
        /// 本地路径
        /// </param>
        /// <returns>
        /// 0：成功
        /// 其他：失败
        /// </returns>
        public static Int32 getfile(String rpath, String lpath)
        {
            Int32 ret = FtpApi.DownloadFile(rpath, lpath, ipnr);
            return ret;
        }

        public static byte[] data2byte(int[] data)
        {
            List<byte> temp1 = new List<byte>();
            for(int h = 0; h < data.Length; h++)
            {
                //byte[] a = BitConverter.GetBytes(data[h]);
                temp1.AddRange(BitConverter.GetBytes(data[h]));
            }
            byte[] temp = temp1.ToArray();
            return temp;
        }

        public static byte[] data2byte(Int16[] data)
        {
            List<byte> temp1 = new List<byte>();
            for (int h = 0; h < data.Length; h++)
            {
                temp1.AddRange(BitConverter.GetBytes(data[h]));
                //byte[] a = BitConverter.GetBytes(data[h]);
                //temp1.AddRange(a);
            }
            byte[] temp = temp1.ToArray();
            return temp;
        }

        private static void gcodeupload()
        {
            try
            {
                uploadSocket.Disconnect(true);
            }
            catch(Exception exn)
            {

            }
            Task gcode = new Task(gcodeup);
            gcode.Start();
        }

        private static void gcodeup()
        {
            //创建一个文件对象
            FileInfo fileinfo = new FileInfo(temp_l);
            string targetDir = "wangyushun/connect/fileCode";
            string hostname = "114.215.189.49";
            string username = "FTP_Admin";
            string password = "hustb538";
            string fileName = UploadFile(fileinfo, targetDir, hostname, username, password);

            if (fileName != "")
            {
                //指向远程服务节点
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("114.215.189.49"), 8435);
                //创建套接字
                uploadSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                uploadSocket.Connect(ipep);

                byte[] fileNameByte = System.Text.Encoding.Unicode.GetBytes(fileName);
                byte[] uploadFileName = new byte[fileNameByte.Length + 4];
                byte[] nameSize = BitConverter.GetBytes(fileNameByte.Length);

                for (int i = 0; i < fileNameByte.Length + 4; i++)
                {
                    if (i < 4)
                    {
                        uploadFileName[i] = nameSize[i];
                    }
                    else
                    {
                        uploadFileName[i] = fileNameByte[i - 4];
                    }
                }

                uploadSocket.Send(uploadFileName);

                //uploadChange();
            }
        }

        //void uploadChange()
        //{
        //    if (btnUpload.InvokeRequired)
        //    {
        //        InvokeUpload btnChange = new InvokeUpload(uploadChange);
        //        btnUpload.Invoke(btnChange);
        //    }
        //    else
        //    {
        //        btnUpload.Enabled = true;
        //        //this.richTextBox1.Text = "www";
        //    }
        //}

        private static string UploadFile(FileInfo fileinfo, string targetDir, string hostname, string username, string password)
        {
            //cheack target
            string target;
            if (targetDir.Trim() == "")
            {
                return "";
            }

            target = Guid.NewGuid().ToString() + ".txt";//使用临时文件名

            string URI = "FTP://" + hostname + "/" + targetDir + "/" + target;

            System.Net.FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(URI);

            ftp.Credentials = new NetworkCredential(username, password);

            ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
            //指定文件传输的数据类型
            ftp.UseBinary = true;
            ftp.UsePassive = true;

            //告诉ftp文件的大小
            ftp.ContentLength = fileinfo.Length;

            //缓冲大小设置为2KB
            const int BufferSize = 2048;
            byte[] content = new byte[BufferSize];
            int dataRead;

            //打开一个文件流（System.IO.FileStream)去读上传的文件
            using (FileStream fs = fileinfo.OpenRead())
            {
                try
                {
                    //把上传的文件写入流
                    using (Stream rs = ftp.GetRequestStream())
                    {
                        do
                        {
                            //每次读文件流的2KB
                            dataRead = fs.Read(content, 0, BufferSize);
                            rs.Write(content, 0, dataRead);
                        } while (!(dataRead < BufferSize));

                        rs.Close();
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    fs.Close();
                }
            }

            return target;
        }

    }

}