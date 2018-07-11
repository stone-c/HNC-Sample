using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CefSharp;
using CefSharp.WinForms;

namespace DataC
{
    public partial class Form3 : Form
    {
        public ChromiumWebBrowser browser;

        public void InitBrowser()
        {
            string path = "file:///" + System.IO.Directory.GetCurrentDirectory() + "/RTCP/PubAll.html";
            try
            {
                var settings = new CefSettings();
                settings.RemoteDebuggingPort = 8088;
                settings.CefCommandLineArgs.Add("enable-media-stream", " enable-media-stream");
                settings.IgnoreCertificateErrors = true;
                settings.LogSeverity = LogSeverity.Verbose;
                Cef.Initialize(settings);
                browser = new ChromiumWebBrowser(path);
                this.Controls.Add(browser);
                browser.Dock = DockStyle.Fill;

            }
            catch (Exception x)
            {

            }
        }

        public Form3()
        {
            InitializeComponent();
            InitBrowser();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }
}
