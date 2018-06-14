using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;

namespace DataC
{
    public partial class Form2 : Form
    {

        public static string ipl = null;
        public static string ipr = null;
        public static ushort port = 0;

        public static string sip = "114.215.189.49";

        public static int v_offset = 0;
        public static int v_length = 0;
        public static int s_offset = 0;
        public static int s_length = 0;

        System.Timers.Timer form_timer;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            #region form_timer

            form_timer = new System.Timers.Timer();
            form_timer.Elapsed += new System.Timers.ElapsedEventHandler(tick_count);
            form_timer.Interval = 100;

            form_timer.Start();

            #endregion
        }

        public void tick_count(object source, System.Timers.ElapsedEventArgs e)
        {

        }

        private void connect_button_Click(object sender, EventArgs e)
        {
            ipl = local_textBox.Text;
            ipr = remote_textBox.Text;
            port = Convert.ToUInt16(port_textBox.Text);

            int ret = HNC_Connect.start_connect(ipr, ipl, port);

            if (ret == 0)
            {
                connect_button.Text = "已连接";
                connect_button.Enabled = false;
            }
            else if (ret == 1)
            {
                MessageBox.Show("请检查适配器", "初始化失败", MessageBoxButtons.OK);
            }
            else if (ret == 2)
            {
                MessageBox.Show("请检查ip", "连接失败", MessageBoxButtons.OK);
            }
        }

        private void cfirm_button_Click(object sender, EventArgs e)
        {
            if (!HNC_Connect.check_connect())
            {
                MessageBox.Show("请先连接机床", "提示", MessageBoxButtons.OK);
                return;
            }

            HNC_Connect.smplperiod();

            HNC_Connect.smplset(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
            Thread.Sleep(10);
            HNC_Connect.smplset(Convert.ToInt32(textBox3.Text), Convert.ToInt32(textBox4.Text));

            HNC_Connect.smplseted = true;

            cfirm_button.Text = "已配置";
            this.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            //HNC_Connect.smploff();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            v_offset = Convert.ToInt32(textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            v_length = Convert.ToInt32(textBox2.Text);
            if (v_length % 2 != 0)
            {
                MessageBox.Show("请检查输入值：\n长度值应为偶数", "错误", MessageBoxButtons.OK);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            s_offset = Convert.ToInt32(textBox3.Text);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            s_length = Convert.ToInt32(textBox4.Text);
            if (s_length % 2 != 0)
            {
                MessageBox.Show("请检查输入值：\n长度值应为偶数", "错误", MessageBoxButtons.OK);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!Upload.socketconnect())
            {
                //button2.Text = "断开";
                try
                {
                    Upload.buildsocket();
                    button2.Text = "断  开";
                }
                catch(Exception ex6)
                {
                    //Upload.socket.r
                }
                //button2.Text = "断  开";
            }
            else
            {
                button2.Text = "连  接";
                Upload.socket.Disconnect(true);
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            sip = textBox5.Text;
        }

        //private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    this.Visible = false;
        //}

        //private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    this.Visible = false;
        //}
    }
}
