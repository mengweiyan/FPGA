using Jungo.ft6678_yolo_diag;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ft6678_yolo_diag
{
    public partial class xuanzetupian : Form
    {
        public FT6678_YOLO_diag parent;
        


        public xuanzetupian(FT6678_YOLO_diag parent)
        {
            this.parent = parent;
            InitializeComponent();
            {
                try

                {

                    listView1.View = View.LargeIcon;

                    ImageList imageList1 = new ImageList();

                    imageList1.ImageSize = new System.Drawing.Size(100,100);

                    listView1.LargeImageList = imageList1;
                   

                    imageList1.ColorDepth = ColorDepth.Depth32Bit;

                    string[] files = Directory.GetFiles("f:\\FPGA\\TOH\\data\\","*.bmp");

                    for (int i = 0; i < files.Length; i++)

                    {

                        imageList1.Images.Add(Image.FromFile(files[i]));

                        listView1.Items.Add(Path.GetFileName(files[i]));

                        listView1.Items[i].ImageIndex = i;

                    }

                }

                catch (Exception ex)

                {

                    MessageBox.Show(ex.Message);

                }

            }




        }



        
        private Button btnlog = new Button();
        private void uiButton6_Click(object sender, EventArgs e)
        {
            int m;
            if (this.parent.listView1.Items.Count == 0)
            {
                m = 0;
            }
            else
            {
                m = this.parent.listView1.Items.Count;
            }
            for (int cl = 0; cl < this.listView1.SelectedItems.Count; cl++)   //测试图片循环
            {
                for (int crop = 0; crop < this.uiCheckBoxGroup1.SelectedItems.Count; crop++)     //裁剪大小循环
                {
                    for (int sz = 0; sz < this.uiCheckBoxGroup2.SelectedItems.Count; sz++)     //算子类型循环
                    {
                        m = m + 1;
                        ListViewItem lv = new ListViewItem();
                        lv.Text = "测试" + m.ToString("D2");
                        string s = this.listView1.SelectedItems[cl].ToString();       ///////获取到字符串s
                        lv.SubItems.Add(s.Substring(s.LastIndexOf("{")+1,s.LastIndexOf("}")- s.LastIndexOf("{")-5)); //////获取到{  }字符串之间的字符
                        lv.SubItems.Add(this.uiCheckBoxGroup1.SelectedItems[crop].ToString());
                        lv.SubItems.Add(this.uiCheckBoxGroup2.SelectedItems[sz].ToString());
                        lv.SubItems.Add("未测试");
                        lv.SubItems.Add("无");




                        this.parent.listView1.Items.Add(lv);
                       




                    }


                }
            }
            this.Close();


        }
    }
}
