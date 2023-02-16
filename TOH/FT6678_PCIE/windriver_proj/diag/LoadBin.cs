using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Jungo.wdapi_dotnet;
using Jungo.ft6678_yolo_lib;
using wdc_err = Jungo.wdapi_dotnet.WD_ERROR_CODES;
using DWORD = System.UInt32;
using BOOL = System.Boolean;
using BYTE = System.Byte;



namespace Jungo.ft6678_yolo_diag
{
    public partial class LoadBin : Form
    {
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private FT6678_YOLO_Device m_device;
        public static byte[] binchar_q = new byte[] { };
        public static int length_q = 0 ;
        private System.ComponentModel.Container components = null;

        public LoadBin(FT6678_YOLO_Device dev)
        {
            InitializeComponent();

            m_device = dev;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
         /* private System.ComponentModel.Container components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }*/

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(3, 12);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(477, 21);
            this.textBox2.TabIndex = 5;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(3, 42);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(566, 258);
            this.textBox1.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(484, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "打开\r\n";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // LoadBin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 307);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "LoadBin";
            this.Text = "LoadBin";
            this.Load += new System.EventHandler(this.LoadBin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public bool GetInput()
        {
            DialogResult result = DialogResult.Retry;

            while ((result = ShowDialog()) == DialogResult.Retry) ;

            return true;
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            //  openFileDialog1.InitialDirectory = "C:\\";
            openFileDialog1.Filter = "*.bin|*.BIN";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = openFileDialog1.FileName;


                //改进版，读取上百kb的文件执行时间可以接受，StringBuilder执行效率远大于Mytext += j.ToString("X2")。
                int file_len;
                int addr = 0;
                int count = 0;
                byte[] binchar = new byte[] { };

                FileStream Myfile = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                BinaryReader binreader = new BinaryReader(Myfile);

                file_len = (int)Myfile.Length;//获取bin文件长度
                length_q = file_len;

                StringBuilder str = new StringBuilder();

                binchar = binreader.ReadBytes(file_len);

                binchar_q = binchar;   //全局变量赋值
                
              /*  
                foreach (byte j in binchar)  //显示权重数据
                {
                    if (count % 16 == 0)
                    {

                        count = 0;
                        if (addr > 0)
                            str.Append("\r\n");
                        str.Append(addr.ToString("x8") + "      ");

                        addr++;
                    }

                    str.Append(j.ToString("X2") + " ");
                    if (count == 8)
                        str.Append("  ");
                    count++;
                }*/
                //textBox1.Text = str.ToString();
                 binreader.Close();
            }
            //原版,打开几十kb的文件时间还行，上百kb之后时间就慢的卡住，主要是Mytext += j.ToString("X2");处理时间长，牵扯到垃圾回收机制
            //string Mytext = "";
            //int file_len;
            //byte[] binchar = new byte[] { };

            //FileStream Myfile = new FileStream("test.bin", FileMode.Open, FileAccess.Read);
            //BinaryReader binreader = new BinaryReader(Myfile);

            //file_len = (int)Myfile.Length;//获取bin文件长度
            //binchar = binreader.ReadBytes(file_len);

            //foreach (byte j in binchar)
            //{
            //    Mytext += j.ToString("X2");
            //    Mytext += " ";               
            //}

            //textBox1.Text = Mytext;
            //binreader.Close();
        }

        private void LoadBin_Load(object sender, EventArgs e)
        {

        }

    }
}
