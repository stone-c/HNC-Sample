using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Runtime.InteropServices;

namespace DataC
{
    public partial class DataC : Form
    {
        System.Timers.Timer form_timer;

        Form setting = new Form2();

        public static Series S1, S2, S3;

        private static int gcode_counter = 0;
        private static int gcode_lines = 0;
        //private static int i_len = 0;
        //private static int v_len = 0;
        //private static int dag_len = 0;
        private static int counter = 0;

        //private static int[] axis_i = new int[500];
        //private static int[] value_vf = new int[10000];
        //private static int[] value_sf = new int[10000];
        private static int[] i_temp = new int[50];
        private static Int16[] v_temp = new short[1000];
        private static Int16[] s_temp = new short[1000];

        private static Mutex Series_update = new Mutex();

        private static Thread senddata;

        public DataC()
        {
            InitializeComponent();
        }

        private void DataC_Load(object sender, EventArgs e)
        {

            #region Series

            S1 = new Series("V");
            S1.ChartType = SeriesChartType.Line;

            S2 = new Series("S");
            S2.ChartType = SeriesChartType.Line;

            S3 = new Series("I");
            S3.ChartType = SeriesChartType.Line;

            S3.Color = Color.FromArgb(255, 255, 255, 0);
            S2.Color = Color.FromArgb(255, 255, 255, 0);
            S1.Color = Color.FromArgb(255, 255, 255, 0);

            for (int i = 0; i < 1000; i++)
            {
                S1.Points.AddXY(i / 10.0, 0);
                S2.Points.AddXY(i / 10.0, 0);
                S3.Points.AddXY(i / 10.0, 0);
            }

            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(112, 255, 255, 255);
            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(112, 255, 255, 255);
            chart1.ChartAreas[0].BackColor = Color.FromArgb(255, 0, 144, 208);
            //chart1.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            //chart1.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;
            this.chart1.ChartAreas[0].AxisY.Maximum = 1.5 * 0.1;
            this.chart1.ChartAreas[0].AxisY.Minimum = -1.5 * 0.1;
            this.chart2.ChartAreas[0].AxisY.Maximum = 1.5 * 0.1;
            this.chart2.ChartAreas[0].AxisY.Minimum = -1.5 * 0.1;
            chart1.ChartAreas[0].AxisX.IntervalOffset = 0.10D;
            this.chart2.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(112, 255, 255, 255);
            this.chart2.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(112, 255, 255, 255);
            chart2.ChartAreas[0].BackColor = Color.FromArgb(255, 0, 144, 208);
            //chart2.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            //chart2.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;
            this.chart3.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(112, 255, 255, 255);
            this.chart3.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(112, 255, 255, 255);
            //chart3.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            //chart3.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;
            chart3.ChartAreas[0].BackColor = Color.FromArgb(255, 0, 144, 208);
            this.chart3.ChartAreas[0].AxisY.Maximum = 100.0 * 0.1;
            this.chart3.ChartAreas[0].AxisY.Minimum = -100.0 * 0.1;

            //chart1.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            //chart1.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;
            //chart2.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            //chart2.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;
            //chart3.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            //chart3.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;

            chart1.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
            chart1.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;
            chart2.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
            chart2.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;
            chart3.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
            chart3.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;

            chart1.Series.Add(S1);
            chart2.Series.Add(S2);
            chart3.Series.Add(S3);

            #endregion

            #region form_timer

            form_timer = new System.Timers.Timer();
            form_timer.Elapsed += new System.Timers.ElapsedEventHandler(tick_count);
            form_timer.Interval = 50;

            form_timer.Start();

            #endregion

            HNC_Connect.thread_connect();

            T_message_send();

        }

        private void setting_button_Click(object sender, EventArgs e)
        {
            if (setting.Visible == false)
            {
                setting.Location = new Point(this.Location.X + 100, this.Location.Y + 50);
                setting.Visible = true;
            }
            else
                setting.Visible = false;
        }

        private void DataC_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void start_button_Click(object sender, EventArgs e)
        {
            HNC_Connect.smplon();
        }


        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(ref long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(ref long lpFrequency);

        public void tick_count(object source, System.Timers.ElapsedEventArgs e)
        {
            long freq = 0;

            long stop_time = 0;
            long now_time = 0;
            if (QueryPerformanceFrequency(ref freq) == false)
            {
                throw new Exception("不支持高精度计时.");
            }
            double count_per_millsec = freq / 1000.0;
            QueryPerformanceCounter(ref now_time);

            //Console.WriteLine("等待时间" + ((now_time - start_time) / count_per_millsec).ToString());

            if (!HNC_Connect.iscon)
            {
                label3.Text = "机床未连接";
                label3.ForeColor = Color.Red;
            }
            else if (HNC_Connect.iscon && (!HNC_Connect.smplseted))
            {
                label3.Text = "采样未设置";
                label3.ForeColor = Color.Red;
            }
            else if (HNC_Connect.smplseted)
            {
                label3.Text = "采样已就绪";
                label3.ForeColor = Color.Green;
            }
            else if(HNC_Connect.issmpl)
            {
                label3.Text = "采样进行中";
                label3.ForeColor = Color.Green;
            }
            if (HNC_Connect.cyc == 1)
            {
                label3.Text = "循环已启动";
                label3.ForeColor = Color.Green;
            }

            if (HNC_Connect.dataready)
            {
                HNC_Connect.dataready = false;
                counter = 0;
                HNC_Connect.getdata.WaitOne();


                if (HNC_Connect.progch_event)
                {
                    while (!read_gcode(HNC_Connect.temp_l)) ;

                    HNC_Connect.progch_event = false;
                }

                //if(HNC_Connect.cyc==1)
                //{
                //    label3.Text = "循环已启动";
                //    label3.ForeColor = Color.Green;
                //}

                try
                {
                    listBox1.SelectedIndex = HNC_Connect.gline;
                }
                catch (Exception ex1)
                {
                    //
                }

                X_label.Text = "X  " + HNC_Connect.pos_X.ToString("000.000") + "  mm";
                Y_label.Text = "Y  " + HNC_Connect.pos_Y.ToString("000.000") + "  mm";
                Z_label.Text = "Z  " + HNC_Connect.pos_Z.ToString("000.000") + "  mm";
                C_label.Text = "C  " + HNC_Connect.pos_C.ToString("000.000") + "  deg";
                F_label.Text = "F  " + HNC_Connect.speed_F.ToString("0000") + "  mm/min";
                S_label.Text = "S  " + HNC_Connect.speed_S.ToString("0000") + "  rpm";
                label1.Text = "主轴倍率： " + HNC_Connect.S_OVERRIDE.ToString() + " %";
                label2.Text = "进给倍率： " + HNC_Connect.F_OVERRIDE.ToString() + " %";

                HNC_Connect.getdata.ReleaseMutex();

                try
                {
                    //i_len = HNC_Connect.axisi.Length;
                    //if(i_len<50)
                    //{
                    //    HNC_Connect.getdata.ReleaseMutex();
                    //    return;
                    //}
                    //v_len = i_len * 20;
                    //dag_len = i_len * 5;

                    chart3.Series.Clear();

                    S3 = new Series("I");

                    int max_i = HNC_Connect.axisi.Max();
                    int min_i = HNC_Connect.axisi.Min();

                    for (int k1 = 0; k1 < 50; k1++)
                    {
                        S3.Points.AddXY(k1, i_temp[k1]);
                    }
                    //for (int k1 = 0; k1 < i_len; k1++)
                    //{
                    //    S3.Points.AddXY(k1, HNC_Connect.axisi[k1]);
                    //}
                    S3.ChartType = SeriesChartType.Line;

                    S3.Color = Color.FromArgb(255, 255, 255, 0);

                    chart3.Series.Add(S3);

                    if (max_i >= 0)
                    {
                        chart3.ChartAreas[0].AxisY.Maximum = max_i * 1.1;
                    }
                    else
                    {
                        chart3.ChartAreas[0].AxisY.Maximum = max_i * 0.9;
                    }
                    if (min_i >= 0)
                    {
                        chart3.ChartAreas[0].AxisY.Maximum = max_i * 0.9;
                    }
                    else
                    {
                        chart3.ChartAreas[0].AxisY.Maximum = max_i * 1.1;
                    }
                    //chart3.ChartAreas[0].AxisY.Minimum = 10;
                    //chart3.ChartAreas[0].AxisY.Maximum = 10;
                    //chart3.ChartAreas[0].AxisY.Maximum = axis_i.Max() * 1.2;

                    //for (int k3 = 0; k3 < v_len; k3++)
                    //{
                    //    value_vf[k3] = HNC_Connect.value_v[k3];
                    //    value_sf[k3] = HNC_Connect.value_s[k3];
                    //}
                    //counter = 0;
                }
                catch
                {

                }

                //v_len = i_len * 20;
                //dag_len = i_len * 5;

                //chart3.Series.Clear();

                //S3.Points.Clear();

                //for (int k1 = 0; k1 < i_len; k1++)
                //{
                //    axis_i[k1] = HNC_Connect.axisi[k1];
                //    S3.Points.AddXY((double)k1, (double)axis_i[k1]);
                //}

                //chart3.Series.Add(S3);

                ////chart3.ChartAreas[0].AxisY.Maximum = 1000;
                ////chart3.ChartAreas[0].AxisY.Maximum = axis_i.Max() * 1.2;

                //for (int k3 = 0; k3 < v_len; k3++)
                //{
                //    value_vf[k3] = HNC_Connect.value_v[k3];
                //    value_sf[k3] = HNC_Connect.value_s[k3];
                //}
                //counter = 0;

                //chart3.Series.Clear();
                //S3.Points.Clear();

                //for (int k = 0; k < i_len; k++)
                //{
                //    S3.Points.AddXY((double)k, (double)axis_i[k]);
                //}
                //chart3.Series.Add(S3);

            }
            else
            {
                //HNC_Connect.getdata.ReleaseMutex();
                return;
            }

            //HNC_Connect.dataready = false;

            //HNC_Connect.getdata.ReleaseMutex();

            //Series_update.WaitOne();

            //if(i_len<20)
            //{

            //}

            //int dag_len = i_len * 4;

            try
            {
                chart1.Series.Clear();
                chart2.Series.Clear();

                S1 = new Series("V");
                S2 = new Series("S");

                S1.ChartType = SeriesChartType.Line;
                S2.ChartType = SeriesChartType.Line;

                S1.Color = Color.FromArgb(255, 255, 255, 0);
                S2.Color = Color.FromArgb(255, 255, 255, 0);

                int max_v = v_temp.Max();
                int max_s = s_temp.Max();
                int min_v = v_temp.Min();
                int min_s = s_temp.Min();

                for (int k2=0;k2<1000;k2++)
                {
                    S1.Points.AddXY(k2, s_temp[k2]);
                    S2.Points.AddXY(k2, v_temp[k2]);
                }

                //for (int k2 = dag_len * counter; k2 < dag_len * (counter + 1); k2++)
                //{
                //    S1.Points.AddXY(k2, value_vf[k2]);
                //    S2.Points.AddXY(k2, value_sf[k2]);
                //}
                //Console.WriteLine("vs点数" + dag_len.ToString());

                chart1.Series.Add(S1);
                chart2.Series.Add(S2);


                if (max_v >= 0)
                {
                    chart1.ChartAreas[0].AxisY.Maximum = max_v * 1.1;
                }
                else
                {
                    chart1.ChartAreas[0].AxisY.Maximum = max_v * 0.9;
                }
                if (min_v >= 0)
                {
                    chart1.ChartAreas[0].AxisY.Maximum = max_v * 0.9;
                }
                else
                {
                    chart1.ChartAreas[0].AxisY.Maximum = max_v * 1.1;
                }

                if (max_s >= 0)
                {
                    chart2.ChartAreas[0].AxisY.Maximum = max_s * 1.1;
                }
                else
                {
                    chart2.ChartAreas[0].AxisY.Maximum = max_s * 0.9;
                }
                if (min_s >= 0)
                {
                    chart2.ChartAreas[0].AxisY.Maximum = max_s * 0.9;
                }
                else
                {
                    chart2.ChartAreas[0].AxisY.Maximum = max_s * 1.1;
                }


                //if (counter <= 4)
                //{
                //    counter++;
                //}

            }
            catch (Exception ex1)
            {
                //Series_update.ReleaseMutex();
                return;
            }

            //chart1.Series.Add(S1);
            //chart2.Series.Add(S2);

            //HNC_Connect.dataready = false;

            //Series_update.ReleaseMutex();

            QueryPerformanceCounter(ref stop_time);

            Console.WriteLine("计时器使用时间" + ((stop_time - now_time) / count_per_millsec).ToString());
        }

        private bool read_gcode(string a)
        {
            try
            {
                listBox1.Items.Clear();
            }
            catch (NullReferenceException ex)
            {
                // Ignore
            }

            if (a == null)
            {
                return false;
            }

            string str;

            gcode_counter = 0;
            StreamReader sr = new StreamReader(a, Encoding.Default);

            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                str = gcode_counter.ToString("d4") + "  " + line;
                listBox1.Items.Add(str);
                gcode_counter++;
            }
            gcode_lines = gcode_counter;
            
            if (listBox1.Items.Count != gcode_counter)
            {
                return false;
            }
            listBox1.SelectedIndex = 0;
            sr.Dispose();
            return true;
        }

        private static void T_message_send()
        {
            senddata = new Thread(m_send);
            senddata.IsBackground = true;
            senddata.Start();
        }

        private static void m_send()
        {
            long freq = 0;

            if (QueryPerformanceFrequency(ref freq) == false)
            {
                throw new Exception("不支持高精度计时.");
            }
            double count_per_millsec = freq / 1000.0;
            long start_time = 0;
            long now_time = 0;

            QueryPerformanceCounter(ref start_time);
            while (true)
            {

                if (!HNC_Connect.issmpl)
                {
                    continue;
                }

                QueryPerformanceCounter(ref now_time);
                while (now_time - start_time < 50 * count_per_millsec)
                {
                    QueryPerformanceCounter(ref now_time);
                }

                bool dataable = (HNC_Connect.i_que.Count > 60) && (HNC_Connect.v_que.Count > 1100) && (HNC_Connect.s_que.Count > 1100);

                if (dataable)
                {
                    List<byte> pack = new List<byte>();

                    for (int p = 0; p < 50; p++)
                    {
                        i_temp[p] = HNC_Connect.i_que.Dequeue();

                        for(int p1 = 0; p < 20; p++)
                        {
                            v_temp[p * 20 + p1] = HNC_Connect.v_que.Dequeue();
                            s_temp[p * 20 + p1] = HNC_Connect.s_que.Dequeue();
                        }
                    }

                    pack.AddRange(HNC_Connect.temp2[counter]);
                    pack.AddRange(HNC_Connect.data2byte(i_temp));
                    pack.AddRange(HNC_Connect.data2byte(v_temp));
                    pack.AddRange(HNC_Connect.data2byte(s_temp));
                    
                    Upload.socketsend(pack.ToArray());
                    Console.WriteLine("发送数据"+counter.ToString());

                    if (counter < 3)
                    {
                        counter++;
                    }
                }
                start_time = now_time;

                //Upload.socketsend(HNC_Connect.datatemp[3].ToArray());
                
            }
        }
    }
}
