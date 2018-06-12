//using System;
//using System.Windows.Forms.VisualStyles;

namespace DataC
{
    partial class DataC
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.setting_button = new System.Windows.Forms.Button();
            this.start_button = new System.Windows.Forms.Button();
            this.X_label = new System.Windows.Forms.Label();
            this.Y_label = new System.Windows.Forms.Label();
            this.Z_label = new System.Windows.Forms.Label();
            this.C_label = new System.Windows.Forms.Label();
            this.F_label = new System.Windows.Forms.Label();
            this.S_label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart3)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Location = new System.Drawing.Point(12, 12);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(560, 160);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // chart2
            // 
            chartArea2.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea2);
            this.chart2.Location = new System.Drawing.Point(12, 184);
            this.chart2.Name = "chart2";
            series2.ChartArea = "ChartArea1";
            series2.Name = "Series1";
            this.chart2.Series.Add(series2);
            this.chart2.Size = new System.Drawing.Size(560, 160);
            this.chart2.TabIndex = 1;
            this.chart2.Text = "chart2";
            // 
            // chart3
            // 
            chartArea3.Name = "ChartArea1";
            this.chart3.ChartAreas.Add(chartArea3);
            this.chart3.Location = new System.Drawing.Point(12, 357);
            this.chart3.Name = "chart3";
            series3.ChartArea = "ChartArea1";
            series3.Name = "Series1";
            this.chart3.Series.Add(series3);
            this.chart3.Size = new System.Drawing.Size(560, 160);
            this.chart3.TabIndex = 2;
            this.chart3.Text = "chart3";
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Items.AddRange(new object[] {
            "Waiting For Gcode"});
            this.listBox1.Location = new System.Drawing.Point(593, 248);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(179, 259);
            this.listBox1.TabIndex = 3;
            // 
            // setting_button
            // 
            this.setting_button.Location = new System.Drawing.Point(601, 529);
            this.setting_button.Name = "setting_button";
            this.setting_button.Size = new System.Drawing.Size(81, 20);
            this.setting_button.TabIndex = 4;
            this.setting_button.Text = "设  置";
            this.setting_button.UseVisualStyleBackColor = true;
            this.setting_button.Click += new System.EventHandler(this.setting_button_Click);
            // 
            // start_button
            // 
            this.start_button.Location = new System.Drawing.Point(691, 529);
            this.start_button.Name = "start_button";
            this.start_button.Size = new System.Drawing.Size(81, 20);
            this.start_button.TabIndex = 5;
            this.start_button.Text = "开始采样";
            this.start_button.UseVisualStyleBackColor = true;
            this.start_button.Click += new System.EventHandler(this.start_button_Click);
            // 
            // X_label
            // 
            this.X_label.AutoSize = true;
            this.X_label.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.X_label.Location = new System.Drawing.Point(596, 29);
            this.X_label.Name = "X_label";
            this.X_label.Size = new System.Drawing.Size(150, 26);
            this.X_label.TabIndex = 6;
            this.X_label.Text = "X  000.000  mm";
            // 
            // Y_label
            // 
            this.Y_label.AutoSize = true;
            this.Y_label.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Y_label.Location = new System.Drawing.Point(596, 55);
            this.Y_label.Name = "Y_label";
            this.Y_label.Size = new System.Drawing.Size(149, 26);
            this.Y_label.TabIndex = 7;
            this.Y_label.Text = "Y  000.000  mm";
            // 
            // Z_label
            // 
            this.Z_label.AutoSize = true;
            this.Z_label.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Z_label.Location = new System.Drawing.Point(596, 81);
            this.Z_label.Name = "Z_label";
            this.Z_label.Size = new System.Drawing.Size(148, 26);
            this.Z_label.TabIndex = 8;
            this.Z_label.Text = "Z  000.000  mm";
            // 
            // C_label
            // 
            this.C_label.AutoSize = true;
            this.C_label.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.C_label.Location = new System.Drawing.Point(596, 108);
            this.C_label.Name = "C_label";
            this.C_label.Size = new System.Drawing.Size(147, 26);
            this.C_label.TabIndex = 9;
            this.C_label.Text = "C  000.000  deg";
            // 
            // F_label
            // 
            this.F_label.AutoSize = true;
            this.F_label.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.F_label.Location = new System.Drawing.Point(596, 160);
            this.F_label.Name = "F_label";
            this.F_label.Size = new System.Drawing.Size(157, 26);
            this.F_label.TabIndex = 10;
            this.F_label.Text = "F  0000 mm/min";
            // 
            // S_label
            // 
            this.S_label.AutoSize = true;
            this.S_label.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.S_label.Location = new System.Drawing.Point(596, 191);
            this.S_label.Name = "S_label";
            this.S_label.Size = new System.Drawing.Size(116, 26);
            this.S_label.TabIndex = 11;
            this.S_label.Text = "S  0000 rpm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(263, 527);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 22);
            this.label1.TabIndex = 12;
            this.label1.Text = "主轴倍率： 100 %";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(424, 527);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 22);
            this.label2.TabIndex = 13;
            this.label2.Text = "进给倍率： 100 %";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(20, 527);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 22);
            this.label3.TabIndex = 14;
            this.label3.Text = "机床未连接";
            // 
            // DataC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.S_label);
            this.Controls.Add(this.F_label);
            this.Controls.Add(this.C_label);
            this.Controls.Add(this.Z_label);
            this.Controls.Add(this.Y_label);
            this.Controls.Add(this.X_label);
            this.Controls.Add(this.start_button);
            this.Controls.Add(this.setting_button);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.chart3);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.chart1);
            this.Name = "DataC";
            this.Text = "DataC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DataC_FormClosing);
            this.Load += new System.EventHandler(this.DataC_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart3;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button setting_button;
        private System.Windows.Forms.Button start_button;
        private System.Windows.Forms.Label X_label;
        private System.Windows.Forms.Label Y_label;
        private System.Windows.Forms.Label Z_label;
        private System.Windows.Forms.Label C_label;
        private System.Windows.Forms.Label F_label;
        private System.Windows.Forms.Label S_label;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

