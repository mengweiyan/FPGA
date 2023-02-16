using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace open_bin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

                StringBuilder str = new StringBuilder();

                binchar = binreader.ReadBytes(file_len);

                foreach (byte j in binchar)
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
                }
                textBox1.Text = str.ToString();
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
    }
}
