using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;


using Jungo.wdapi_dotnet;
using Jungo.ft6678_yolo_lib;
using wdc_err = Jungo.wdapi_dotnet.WD_ERROR_CODES;
using DWORD = System.UInt32;
using WORD = System.UInt16;
using BYTE = System.Byte;
using BOOL = System.Boolean;
using UINT32 = System.UInt32;
using UINT64 = System.UInt64;
using WDC_DEVICE_HANDLE = System.IntPtr;
using WDC_ADDR_SIZE = System.UInt32;
using HANDLE = System.IntPtr;
using System.Text;
using System.IO;
using System.Timers;
using System.Linq;
using Sunny.UI;
using ft6678_yolo_diag;







namespace Jungo.ft6678_yolo_diag
{
    public enum RW
    {
        READ = 0,
        WRITE = 1,
        READ_ALL = 2
    }

    public enum TRANSFER_TYPE
    {
        BLOCK = 0,
        NONBLOCK = 1
    }

    public enum ACTION_TYPE
    {
        CFG = 0,
        RT = 1,
    }







    public partial class FT6678_YOLO_diag : System.Windows.Forms.Form

    {
        private IContainer components;
        private FT6678_YOLO_DeviceList pciDevList;
        private Log log;
        private DWORD WIDTH = 640;    //图像长
        private DWORD HEIGHT = 512;   //图像宽
        private int pcie_index = 0;
        private Exception m_excp;
        UINT64 RCaddr;
        UInt32 EPaddr;
        UINT64 RCSize;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuAddrSpaces;
        private System.Windows.Forms.MenuItem menuEvents;

        private System.Windows.Forms.MenuItem menuRegisterEvent;

        private System.Windows.Forms.MenuItem menuCfgSpace;
        private System.Windows.Forms.MenuItem menuRTRegs;
        private System.Windows.Forms.MenuItem menuCfgOffset;
        private System.Windows.Forms.MenuItem menuCfgReg;
        private System.Windows.Forms.MenuItem menuAddrRW;
        private System.Windows.Forms.MenuItem menuRTRegsRW;
        private System.Windows.Forms.MenuItem menuInterrupts;

        private System.Windows.Forms.MenuItem menuEnableInt;
        DWORD DMA_NUM_OFFSET = 0x978;
        DWORD DMA_WREN_OFFSET = 0x97c;
        DWORD DMA_WRDOORBELL_OFFSET = 0x980;
        DWORD DMA_RDEN_OFFSET = 0x99c;
        DWORD DMA_RDDOORBELL_OFFSET = 0x9a0;

        DWORD DMA_WR_INTERRUPT_STA = 0x9BC;
        DWORD DMA_WR_INTERRUPT_MSK = 0x9C4;
        DWORD DMA_WR_INTERRUPT_CLR = 0x9C8;

        DWORD DMA_WR_DONE_IMWR_LOW = 0x9d0;  //write IMWr low 32bits
        DWORD DMA_WR_DONE_IMWR_HIGH = 0x9d4; //write IMWr high 32bits
        DWORD DMA_WR_ABORT_IMWR_LOW = 0x9d8;
        DWORD DMA_WR_ABORT_IMWR_HIGH = 0x9dc;
        DWORD DMA_WR_IMWR_DATA = 0x9e0;
        DWORD RC_DMA_WRITE = 1;
        DWORD RC_DMA_READ = 0;
        DWORD DMA_RD_INTERRUPT_STA = 0xA10;
        DWORD DMA_RD_INTERRUPT_MSK = 0xA18;
        DWORD DMA_RD_INTERRUPT_CLR = 0xA1C;

        DWORD DMA_RD_DONE_IMWR_LOW = 0xA3c;  //read IMWr low  32bits
        DWORD DMA_RD_DONE_IMWR_HIGH = 0xA40;  //read IMWr high 32bits
        DWORD DMA_RD_ABORT_IMWR_LOW = 0xA44;
        DWORD DMA_RD_ABORT_IMWR_HIGH = 0xA48;
        DWORD DMA_RD_IMWR_DATA = 0xA4C;

        DWORD DMA_INDEX_OFFSET = 0xA6C;
        DWORD DMA_CTRL1_OFFSET = 0xA70;
        DWORD DMA_SIZE_OFFSET = 0xA78;
        DWORD DMA_SRC_LOW_OFFSET = 0xA7C;
        DWORD DMA_SRC_HIGH_OFFSET = 0xA80;
        DWORD DMA_DST_LOW_OFFSET = 0xA84;
        private TabPage tabPage1;
        private TabPage tabPage2;

        private UITextBox textBoxEP;
        private UITextBox textBoxsize;

        private UIButton qxButton;
        private UITextBox uiTextBox2;
        private UIButton tpopenButton;
        private UIButton qzopenButton;
        private UITextBox uiTextBox1;
        private UIListBox lstBxDevices;
        private UILabel uiLabel3;
        private UILabel uiLabel2;
        private UILabel uiLabel1;
        private UIButton btDevice;
        private UITableLayoutPanel uiTableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private UIMarkLabel label2;
        private UIMarkLabel label1;
        private UICheckBoxGroup uiCheckBoxGroup1;
        private UICheckBoxGroup uiCheckBoxGroup2;
        private UIButton uiButton1;
        private UIButton uiButton4;
        private UIButton uiButton6;
        private UIButton uiButton5;
        private UICheckBoxGroup uiCheckBoxGroup5;
        private UICheckBoxGroup uiCheckBoxGroup4;
        private UICheckBoxGroup uiCheckBoxGroup3;
        private UICheckBoxGroup uiCheckBoxGroup6;
        private UICheckBoxGroup uiCheckBoxGroup7;
        private UITabControl uiTabControl1;
        private TabPage tabPage3;
        private TabPage tabPage7;
        private UITabControlMenu uiTabControlMenu1;
        private UITabControlMenu uiTabControlMenuceshi;
        private TabPage tabPage4;
        public ListView listView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader2;
        private UIButton uiButton3;
        private UISymbolButton uiButton2;
        private UISymbolButton zj;
        private UIButton load_weight;
        private TabPage tabPage6;

        private UITextBox textBoxfeature;
        private UIButton button1;
        private UIMarkLabel label11;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private UIButton uiButton9;
        private UIButton uiButton8;
        DWORD DMA_DST_HIGH_OFFSET = 0xA88;
        public FT6678_YOLO_diag()
        {
            InitializeComponent();

            // log = new Log(new Log.TRACE_LOG(TraceLog),
            //   new Log.ERR_LOG(ErrLog));
            pciDevList = FT6678_YOLO_DeviceList.TheDeviceList();



            int count = this.Controls.Count * 2 + 2;
            float[] nature = new float[count];
            int i = 0;
            nature[i++] = Size.Width;
            nature[i++] = Size.Height;
            foreach (Control ctrl in this.Controls)
            {
                nature[i++] = ctrl.Location.X / (float)Size.Width;
                nature[i++] = ctrl.Location.Y / (float)Size.Height;
                ctrl.Tag = ctrl.Size;
            }
            Tag = nature;



        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Windows Form Designer generated code
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FT6678_YOLO_diag));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuAddrSpaces = new System.Windows.Forms.MenuItem();
            this.menuAddrRW = new System.Windows.Forms.MenuItem();
            this.menuInterrupts = new System.Windows.Forms.MenuItem();
            this.menuEnableInt = new System.Windows.Forms.MenuItem();
            this.menuEvents = new System.Windows.Forms.MenuItem();
            this.menuRegisterEvent = new System.Windows.Forms.MenuItem();
            this.menuCfgSpace = new System.Windows.Forms.MenuItem();
            this.menuCfgOffset = new System.Windows.Forms.MenuItem();
            this.menuCfgReg = new System.Windows.Forms.MenuItem();
            this.menuRTRegs = new System.Windows.Forms.MenuItem();
            this.menuRTRegsRW = new System.Windows.Forms.MenuItem();
            this.uiTabControl1 = new Sunny.UI.UITabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.uiButton9 = new Sunny.UI.UIButton();
            this.uiButton8 = new Sunny.UI.UIButton();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.uiButton3 = new Sunny.UI.UIButton();
            this.uiButton2 = new Sunny.UI.UISymbolButton();
            this.zj = new Sunny.UI.UISymbolButton();
            this.load_weight = new Sunny.UI.UIButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.uiTabControlMenu1 = new Sunny.UI.UITabControlMenu();
            this.uiTabControlMenuceshi = new Sunny.UI.UITabControlMenu();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.textBoxfeature = new Sunny.UI.UITextBox();
            this.button1 = new Sunny.UI.UIButton();
            this.label11 = new Sunny.UI.UIMarkLabel();
            this.btDevice = new Sunny.UI.UIButton();
            this.lstBxDevices = new Sunny.UI.UIListBox();
            this.uiTableLayoutPanel2 = new Sunny.UI.UITableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label2 = new Sunny.UI.UIMarkLabel();
            this.label1 = new Sunny.UI.UIMarkLabel();
            this.uiCheckBoxGroup7 = new Sunny.UI.UICheckBoxGroup();
            this.uiCheckBoxGroup6 = new Sunny.UI.UICheckBoxGroup();
            this.uiCheckBoxGroup5 = new Sunny.UI.UICheckBoxGroup();
            this.uiCheckBoxGroup4 = new Sunny.UI.UICheckBoxGroup();
            this.uiCheckBoxGroup3 = new Sunny.UI.UICheckBoxGroup();
            this.uiButton6 = new Sunny.UI.UIButton();
            this.uiButton5 = new Sunny.UI.UIButton();
            this.uiButton4 = new Sunny.UI.UIButton();
            this.uiButton1 = new Sunny.UI.UIButton();
            this.uiCheckBoxGroup2 = new Sunny.UI.UICheckBoxGroup();
            this.uiCheckBoxGroup1 = new Sunny.UI.UICheckBoxGroup();
            this.textBoxEP = new Sunny.UI.UITextBox();
            this.textBoxsize = new Sunny.UI.UITextBox();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.uiLabel3 = new Sunny.UI.UILabel();
            this.uiTextBox1 = new Sunny.UI.UITextBox();
            this.qzopenButton = new Sunny.UI.UIButton();
            this.tpopenButton = new Sunny.UI.UIButton();
            this.uiTextBox2 = new Sunny.UI.UITextBox();
            this.qxButton = new Sunny.UI.UIButton();
            this.uiTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.uiTableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuAddrSpaces,
            this.menuInterrupts,
            this.menuEvents,
            this.menuCfgSpace,
            this.menuRTRegs});
            // 
            // menuAddrSpaces
            // 
            this.menuAddrSpaces.Index = 0;
            this.menuAddrSpaces.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuAddrRW});
            this.menuAddrSpaces.Text = "Address Spaces";
            this.menuAddrSpaces.Click += new System.EventHandler(this.menuAddrSpaces_Click);
            // 
            // menuAddrRW
            // 
            this.menuAddrRW.Index = 0;
            this.menuAddrRW.Text = "Read/Write Address Space";
            this.menuAddrRW.Click += new System.EventHandler(this.menuAddrRW_Click);
            // 
            // menuInterrupts
            // 
            this.menuInterrupts.Index = 1;
            this.menuInterrupts.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuEnableInt});
            this.menuInterrupts.Text = "Interrupts";
            this.menuInterrupts.Select += new System.EventHandler(this.menuInterrupts_Select);
            // 
            // menuEnableInt
            // 
            this.menuEnableInt.Index = 0;
            this.menuEnableInt.Text = "Enable Interrupts";
            this.menuEnableInt.Click += new System.EventHandler(this.menuEnableInt_Click);
            // 
            // menuEvents
            // 
            this.menuEvents.Index = 2;
            this.menuEvents.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuRegisterEvent});
            this.menuEvents.Text = "Events";
            this.menuEvents.Select += new System.EventHandler(this.menuEvents_Select);
            // 
            // menuRegisterEvent
            // 
            this.menuRegisterEvent.Index = 0;
            this.menuRegisterEvent.Text = "Regsiter Events";
            this.menuRegisterEvent.Click += new System.EventHandler(this.menuRegisterEvent_Click);
            // 
            // menuCfgSpace
            // 
            this.menuCfgSpace.Index = 3;
            this.menuCfgSpace.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuCfgOffset,
            this.menuCfgReg});
            this.menuCfgSpace.Text = "Configuration Space";
            // 
            // menuCfgOffset
            // 
            this.menuCfgOffset.Index = 0;
            this.menuCfgOffset.Text = "By Offset ";
            this.menuCfgOffset.Click += new System.EventHandler(this.menuCfgOffset_Click);
            // 
            // menuCfgReg
            // 
            this.menuCfgReg.Index = 1;
            this.menuCfgReg.Text = "By Register";
            this.menuCfgReg.Click += new System.EventHandler(this.menuCfgReg_Click);
            // 
            // menuRTRegs
            // 
            this.menuRTRegs.Index = 4;
            this.menuRTRegs.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuRTRegsRW});
            this.menuRTRegs.Text = "RunTime Registers";
            this.menuRTRegs.Click += new System.EventHandler(this.menuRTRegs_Click);
            // 
            // menuRTRegsRW
            // 
            this.menuRTRegsRW.Index = 0;
            this.menuRTRegsRW.Text = "Read/Write RT Registers";
            this.menuRTRegsRW.Click += new System.EventHandler(this.menuRTRegsRW_Click);
            // 
            // uiTabControl1
            // 
            this.uiTabControl1.Controls.Add(this.tabPage1);
            this.uiTabControl1.Controls.Add(this.tabPage2);
            this.uiTabControl1.Controls.Add(this.tabPage3);
            this.uiTabControl1.Controls.Add(this.tabPage7);
            this.uiTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControl1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTabControl1.Frame = null;
            this.uiTabControl1.ItemSize = new System.Drawing.Size(200, 40);
            this.uiTabControl1.Location = new System.Drawing.Point(0, 0);
            this.uiTabControl1.MainPage = "";
            this.uiTabControl1.Name = "uiTabControl1";
            this.uiTabControl1.SelectedIndex = 0;
            this.uiTabControl1.Size = new System.Drawing.Size(1312, 706);
            this.uiTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.uiTabControl1.TabIndex = 0;
            this.uiTabControl1.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTabControl1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.uiButton9);
            this.tabPage1.Controls.Add(this.uiButton8);
            this.tabPage1.Controls.Add(this.listView1);
            this.tabPage1.Controls.Add(this.uiButton3);
            this.tabPage1.Controls.Add(this.uiButton2);
            this.tabPage1.Controls.Add(this.zj);
            this.tabPage1.Controls.Add(this.load_weight);
            this.tabPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPage1.Location = new System.Drawing.Point(0, 40);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(1312, 666);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "算子测试";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // uiButton9
            // 
            this.uiButton9.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton9.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton9.Location = new System.Drawing.Point(891, 608);
            this.uiButton9.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton9.Name = "uiButton9";
            this.uiButton9.Radius = 35;
            this.uiButton9.Size = new System.Drawing.Size(100, 35);
            this.uiButton9.TabIndex = 61;
            this.uiButton9.Text = "全不选";
            this.uiButton9.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton9.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton9.Click += new System.EventHandler(this.uiButton9_Click);
            // 
            // uiButton8
            // 
            this.uiButton8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton8.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton8.Location = new System.Drawing.Point(759, 608);
            this.uiButton8.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton8.Name = "uiButton8";
            this.uiButton8.Radius = 35;
            this.uiButton8.Size = new System.Drawing.Size(100, 35);
            this.uiButton8.TabIndex = 60;
            this.uiButton8.Text = "全选";
            this.uiButton8.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton8.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton8.Click += new System.EventHandler(this.uiButton8_Click);
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.SystemColors.Window;
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader2,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listView1.Cursor = System.Windows.Forms.Cursors.No;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.OwnerDraw = true;
            this.listView1.Size = new System.Drawing.Size(1312, 577);
            this.listView1.TabIndex = 58;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView1_DrawColumnHeader);
            this.listView1.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView1_DrawItem);
            this.listView1.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView1_DrawSubItem);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "序号";
            this.columnHeader1.Width = 154;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "测试用例";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 184;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "图像大小";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 250;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "算子类型";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 235;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "测试进展";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader5.Width = 206;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "日志";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader6.Width = 188;
            // 
            // uiButton3
            // 
            this.uiButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton3.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.uiButton3.Location = new System.Drawing.Point(220, 608);
            this.uiButton3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton3.Name = "uiButton3";
            this.uiButton3.Radius = 35;
            this.uiButton3.Size = new System.Drawing.Size(100, 35);
            this.uiButton3.TabIndex = 57;
            this.uiButton3.Text = "移除";
            this.uiButton3.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton3.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton3.Click += new System.EventHandler(this.uiButton3_Click);
            // 
            // uiButton2
            // 
            this.uiButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton2.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.uiButton2.Location = new System.Drawing.Point(335, 607);
            this.uiButton2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton2.Name = "uiButton2";
            this.uiButton2.Radius = 35;
            this.uiButton2.Size = new System.Drawing.Size(100, 35);
            this.uiButton2.StyleCustomMode = true;
            this.uiButton2.Symbol = 61453;
            this.uiButton2.TabIndex = 56;
            this.uiButton2.Text = "清除";
            this.uiButton2.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton2.Click += new System.EventHandler(this.uiButton2_Click);
            // 
            // zj
            // 
            this.zj.BackColor = System.Drawing.Color.White;
            this.zj.Cursor = System.Windows.Forms.Cursors.Hand;
            this.zj.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.zj.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.zj.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.zj.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.zj.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.zj.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.zj.Location = new System.Drawing.Point(23, 608);
            this.zj.MinimumSize = new System.Drawing.Size(1, 1);
            this.zj.Name = "zj";
            this.zj.Padding = new System.Windows.Forms.Padding(28, 0, 0, 0);
            this.zj.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.zj.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.zj.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.zj.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.zj.Size = new System.Drawing.Size(161, 35);
            this.zj.Style = Sunny.UI.UIStyle.Orange;
            this.zj.Symbol = 61543;
            this.zj.TabIndex = 54;
            this.zj.Text = "新建测试计划";
            this.zj.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.zj.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.zj.Click += new System.EventHandler(this.uiSymbolButton1_Click);
            // 
            // load_weight
            // 
            this.load_weight.Cursor = System.Windows.Forms.Cursors.Hand;
            this.load_weight.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.load_weight.Location = new System.Drawing.Point(1169, 607);
            this.load_weight.MinimumSize = new System.Drawing.Size(1, 1);
            this.load_weight.Name = "load_weight";
            this.load_weight.Radius = 35;
            this.load_weight.Size = new System.Drawing.Size(100, 35);
            this.load_weight.StyleCustomMode = true;
            this.load_weight.TabIndex = 41;
            this.load_weight.Text = "测试";
            this.load_weight.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.load_weight.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.load_weight.Click += new System.EventHandler(this.load_weight_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabPage2.Location = new System.Drawing.Point(0, 40);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(200, 60);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "神经网络测试";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(0, 40);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(200, 60);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "自动化基准测试";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage7
            // 
            this.tabPage7.Location = new System.Drawing.Point(0, 40);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(200, 60);
            this.tabPage7.TabIndex = 3;
            this.tabPage7.Text = "硬件可靠性测试";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // uiTabControlMenu1
            // 
            this.uiTabControlMenu1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControlMenu1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTabControlMenu1.Location = new System.Drawing.Point(0, 0);
            this.uiTabControlMenu1.Name = "uiTabControlMenu1";
            this.uiTabControlMenu1.SelectedIndex = 0;
            this.uiTabControlMenu1.Size = new System.Drawing.Size(450, 270);
            this.uiTabControlMenu1.TabIndex = 0;
            this.uiTabControlMenu1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiTabControlMenuceshi
            // 
            this.uiTabControlMenuceshi.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControlMenuceshi.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTabControlMenuceshi.Location = new System.Drawing.Point(0, 0);
            this.uiTabControlMenuceshi.Name = "uiTabControlMenuceshi";
            this.uiTabControlMenuceshi.SelectedIndex = 0;
            this.uiTabControlMenuceshi.Size = new System.Drawing.Size(450, 270);
            this.uiTabControlMenuceshi.TabIndex = 0;
            this.uiTabControlMenuceshi.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(0, 0);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(200, 100);
            this.tabPage4.TabIndex = 0;
            // 
            // tabPage6
            // 
            this.tabPage6.Location = new System.Drawing.Point(0, 0);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(200, 100);
            this.tabPage6.TabIndex = 0;
            // 
            // textBoxfeature
            // 
            this.textBoxfeature.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.textBoxfeature.ButtonWidth = 40;
            this.textBoxfeature.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxfeature.DoubleValue = 1D;
            this.textBoxfeature.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxfeature.IntValue = 1;
            this.textBoxfeature.Location = new System.Drawing.Point(369, 112);
            this.textBoxfeature.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxfeature.MinimumSize = new System.Drawing.Size(1, 16);
            this.textBoxfeature.Name = "textBoxfeature";
            this.textBoxfeature.ShowText = false;
            this.textBoxfeature.Size = new System.Drawing.Size(255, 38);
            this.textBoxfeature.TabIndex = 51;
            this.textBoxfeature.Text = "1";
            this.textBoxfeature.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.textBoxfeature.Watermark = "";
            this.textBoxfeature.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.textBoxfeature.TextChanged += new System.EventHandler(this.textBoxfeature_TextChanged);
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(461, 104);
            this.button1.MinimumSize = new System.Drawing.Size(1, 1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(119, 46);
            this.button1.TabIndex = 50;
            this.button1.Text = "获取指定层特征";
            this.button1.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label11.Font = new System.Drawing.Font("宋体", 10F);
            this.label11.Location = new System.Drawing.Point(233, 123);
            this.label11.Name = "label11";
            this.label11.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label11.Size = new System.Drawing.Size(129, 27);
            this.label11.TabIndex = 52;
            this.label11.Text = "指定特征层：0x";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label11.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.label11.Click += new System.EventHandler(this.label11_Click);
            // 
            // btDevice
            // 
            this.btDevice.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btDevice.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btDevice.Location = new System.Drawing.Point(366, 460);
            this.btDevice.MinimumSize = new System.Drawing.Size(1, 1);
            this.btDevice.Name = "btDevice";
            this.btDevice.Radius = 1;
            this.btDevice.Size = new System.Drawing.Size(99, 35);
            this.btDevice.TabIndex = 49;
            this.btDevice.Text = "打开设备";
            this.btDevice.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btDevice.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.btDevice.Click += new System.EventHandler(this.btDevice_Click);
            // 
            // lstBxDevices
            // 
            this.lstBxDevices.FillColor = System.Drawing.Color.White;
            this.lstBxDevices.Font = new System.Drawing.Font("宋体", 10F);
            this.lstBxDevices.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.lstBxDevices.ItemHeight = 17;
            this.lstBxDevices.ItemSelectForeColor = System.Drawing.Color.White;
            this.lstBxDevices.Location = new System.Drawing.Point(93, 460);
            this.lstBxDevices.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstBxDevices.MinimumSize = new System.Drawing.Size(1, 1);
            this.lstBxDevices.Name = "lstBxDevices";
            this.lstBxDevices.Padding = new System.Windows.Forms.Padding(2);
            this.lstBxDevices.Radius = 1;
            this.lstBxDevices.ShowText = false;
            this.lstBxDevices.Size = new System.Drawing.Size(223, 31);
            this.lstBxDevices.Style = Sunny.UI.UIStyle.Custom;
            this.lstBxDevices.TabIndex = 27;
            this.lstBxDevices.Text = null;
            this.lstBxDevices.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.lstBxDevices.DoubleClick += new System.EventHandler(this.lstBxDevices_DoubleClicked);
            this.lstBxDevices.SelectedIndexChanged += new System.EventHandler(this.lstBxDevices_SelectedIndexChanged);
            // 
            // uiTableLayoutPanel2
            // 
            this.uiTableLayoutPanel2.ColumnCount = 1;
            this.uiTableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel2.Controls.Add(this.tableLayoutPanel3);
            this.uiTableLayoutPanel2.Location = new System.Drawing.Point(5, 15);
            this.uiTableLayoutPanel2.Name = "uiTableLayoutPanel2";
            this.uiTableLayoutPanel2.RowCount = 1;
            this.uiTableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTableLayoutPanel2.Size = new System.Drawing.Size(701, 355);
            this.uiTableLayoutPanel2.TabIndex = 1;
            this.uiTableLayoutPanel2.TagString = null;
            this.uiTableLayoutPanel2.Paint += new System.Windows.Forms.PaintEventHandler(this.uiTableLayoutPanel2_Paint);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.77951F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.22049F));
            this.tableLayoutPanel3.Controls.Add(this.pictureBox1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.pictureBox2, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.66667F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.51282F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 37.38318F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(695, 349);
            this.tableLayoutPanel3.TabIndex = 45;
            this.tableLayoutPanel3.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel3_Paint);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 82);
            this.pictureBox1.Name = "pictureBox1";
            this.tableLayoutPanel3.SetRowSpan(this.pictureBox1, 3);
            this.pictureBox1.Size = new System.Drawing.Size(346, 264);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 34;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Location = new System.Drawing.Point(355, 82);
            this.pictureBox2.Name = "pictureBox2";
            this.tableLayoutPanel3.SetRowSpan(this.pictureBox2, 3);
            this.pictureBox2.Size = new System.Drawing.Size(337, 264);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 35;
            this.pictureBox2.TabStop = false;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("宋体", 10F);
            this.label2.Location = new System.Drawing.Point(355, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label2.Size = new System.Drawing.Size(176, 79);
            this.label2.TabIndex = 37;
            this.label2.Text = "识别后";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("宋体", 10F);
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(346, 79);
            this.label1.TabIndex = 36;
            this.label1.Text = "识别前";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // uiCheckBoxGroup7
            // 
            this.uiCheckBoxGroup7.ColumnCount = 2;
            this.uiCheckBoxGroup7.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiCheckBoxGroup7.Items.AddRange(new object[] {
            "有",
            "无"});
            this.uiCheckBoxGroup7.ItemSize = new System.Drawing.Size(130, 35);
            this.uiCheckBoxGroup7.Location = new System.Drawing.Point(57, 294);
            this.uiCheckBoxGroup7.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiCheckBoxGroup7.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiCheckBoxGroup7.Name = "uiCheckBoxGroup7";
            this.uiCheckBoxGroup7.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiCheckBoxGroup7.SelectedIndexes = ((System.Collections.Generic.List<int>)(resources.GetObject("uiCheckBoxGroup7.SelectedIndexes")));
            this.uiCheckBoxGroup7.Size = new System.Drawing.Size(700, 68);
            this.uiCheckBoxGroup7.Style = Sunny.UI.UIStyle.Custom;
            this.uiCheckBoxGroup7.TabIndex = 59;
            this.uiCheckBoxGroup7.Text = "偏置";
            this.uiCheckBoxGroup7.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiCheckBoxGroup7.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiCheckBoxGroup6
            // 
            this.uiCheckBoxGroup6.ColumnCount = 3;
            this.uiCheckBoxGroup6.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiCheckBoxGroup6.Items.AddRange(new object[] {
            "1",
            "2"});
            this.uiCheckBoxGroup6.ItemSize = new System.Drawing.Size(130, 35);
            this.uiCheckBoxGroup6.Location = new System.Drawing.Point(57, 528);
            this.uiCheckBoxGroup6.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiCheckBoxGroup6.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiCheckBoxGroup6.Name = "uiCheckBoxGroup6";
            this.uiCheckBoxGroup6.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiCheckBoxGroup6.SelectedIndexes = ((System.Collections.Generic.List<int>)(resources.GetObject("uiCheckBoxGroup6.SelectedIndexes")));
            this.uiCheckBoxGroup6.Size = new System.Drawing.Size(700, 68);
            this.uiCheckBoxGroup6.Style = Sunny.UI.UIStyle.Custom;
            this.uiCheckBoxGroup6.TabIndex = 62;
            this.uiCheckBoxGroup6.Text = "池化步长";
            this.uiCheckBoxGroup6.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiCheckBoxGroup6.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiCheckBoxGroup6.ValueChanged += new Sunny.UI.UICheckBoxGroup.OnValueChanged(this.uiCheckBoxGroup6_ValueChanged);
            // 
            // uiCheckBoxGroup5
            // 
            this.uiCheckBoxGroup5.ColumnCount = 2;
            this.uiCheckBoxGroup5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiCheckBoxGroup5.Items.AddRange(new object[] {
            "2*2",
            "3*3"});
            this.uiCheckBoxGroup5.ItemSize = new System.Drawing.Size(130, 35);
            this.uiCheckBoxGroup5.Location = new System.Drawing.Point(57, 450);
            this.uiCheckBoxGroup5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiCheckBoxGroup5.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiCheckBoxGroup5.Name = "uiCheckBoxGroup5";
            this.uiCheckBoxGroup5.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiCheckBoxGroup5.SelectedIndexes = ((System.Collections.Generic.List<int>)(resources.GetObject("uiCheckBoxGroup5.SelectedIndexes")));
            this.uiCheckBoxGroup5.Size = new System.Drawing.Size(700, 68);
            this.uiCheckBoxGroup5.Style = Sunny.UI.UIStyle.Custom;
            this.uiCheckBoxGroup5.TabIndex = 61;
            this.uiCheckBoxGroup5.Text = "池化尺寸";
            this.uiCheckBoxGroup5.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiCheckBoxGroup5.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiCheckBoxGroup4
            // 
            this.uiCheckBoxGroup4.ColumnCount = 4;
            this.uiCheckBoxGroup4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiCheckBoxGroup4.Items.AddRange(new object[] {
            "relu",
            "leakyRelu",
            "非线性",
            "部分非线性"});
            this.uiCheckBoxGroup4.ItemSize = new System.Drawing.Size(130, 35);
            this.uiCheckBoxGroup4.Location = new System.Drawing.Point(57, 372);
            this.uiCheckBoxGroup4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiCheckBoxGroup4.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiCheckBoxGroup4.Name = "uiCheckBoxGroup4";
            this.uiCheckBoxGroup4.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiCheckBoxGroup4.SelectedIndexes = ((System.Collections.Generic.List<int>)(resources.GetObject("uiCheckBoxGroup4.SelectedIndexes")));
            this.uiCheckBoxGroup4.Size = new System.Drawing.Size(700, 68);
            this.uiCheckBoxGroup4.Style = Sunny.UI.UIStyle.Custom;
            this.uiCheckBoxGroup4.TabIndex = 60;
            this.uiCheckBoxGroup4.Text = "激活";
            this.uiCheckBoxGroup4.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiCheckBoxGroup4.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiCheckBoxGroup4.ValueChanged += new Sunny.UI.UICheckBoxGroup.OnValueChanged(this.uiCheckBoxGroup4_ValueChanged);
            // 
            // uiCheckBoxGroup3
            // 
            this.uiCheckBoxGroup3.ColumnCount = 2;
            this.uiCheckBoxGroup3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiCheckBoxGroup3.Items.AddRange(new object[] {
            "1",
            "2"});
            this.uiCheckBoxGroup3.ItemSize = new System.Drawing.Size(130, 35);
            this.uiCheckBoxGroup3.Location = new System.Drawing.Point(57, 216);
            this.uiCheckBoxGroup3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiCheckBoxGroup3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiCheckBoxGroup3.Name = "uiCheckBoxGroup3";
            this.uiCheckBoxGroup3.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiCheckBoxGroup3.SelectedIndexes = ((System.Collections.Generic.List<int>)(resources.GetObject("uiCheckBoxGroup3.SelectedIndexes")));
            this.uiCheckBoxGroup3.Size = new System.Drawing.Size(700, 68);
            this.uiCheckBoxGroup3.Style = Sunny.UI.UIStyle.Custom;
            this.uiCheckBoxGroup3.TabIndex = 60;
            this.uiCheckBoxGroup3.Text = "卷积步长";
            this.uiCheckBoxGroup3.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiCheckBoxGroup3.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiButton6
            // 
            this.uiButton6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton6.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.uiButton6.Location = new System.Drawing.Point(879, 93);
            this.uiButton6.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton6.Name = "uiButton6";
            this.uiButton6.Size = new System.Drawing.Size(95, 35);
            this.uiButton6.Style = Sunny.UI.UIStyle.Custom;
            this.uiButton6.TabIndex = 66;
            this.uiButton6.Text = "全不选";
            this.uiButton6.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton6.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton6.Click += new System.EventHandler(this.uiButton6_Click);
            // 
            // uiButton5
            // 
            this.uiButton5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton5.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.uiButton5.Location = new System.Drawing.Point(764, 93);
            this.uiButton5.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton5.Name = "uiButton5";
            this.uiButton5.Size = new System.Drawing.Size(100, 35);
            this.uiButton5.Style = Sunny.UI.UIStyle.Custom;
            this.uiButton5.TabIndex = 65;
            this.uiButton5.Text = "全选";
            this.uiButton5.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton5.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton5.Click += new System.EventHandler(this.uiButton5_Click);
            // 
            // uiButton4
            // 
            this.uiButton4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton4.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.uiButton4.Location = new System.Drawing.Point(879, 160);
            this.uiButton4.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton4.Name = "uiButton4";
            this.uiButton4.Size = new System.Drawing.Size(95, 35);
            this.uiButton4.Style = Sunny.UI.UIStyle.Custom;
            this.uiButton4.TabIndex = 64;
            this.uiButton4.Text = "全不选";
            this.uiButton4.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton4.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton4.Click += new System.EventHandler(this.uiButton4_Click);
            // 
            // uiButton1
            // 
            this.uiButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton1.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.uiButton1.Location = new System.Drawing.Point(764, 160);
            this.uiButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton1.Name = "uiButton1";
            this.uiButton1.Size = new System.Drawing.Size(100, 35);
            this.uiButton1.Style = Sunny.UI.UIStyle.Custom;
            this.uiButton1.TabIndex = 63;
            this.uiButton1.Text = "全选";
            this.uiButton1.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton1.Click += new System.EventHandler(this.uiButton1_Click);
            // 
            // uiCheckBoxGroup2
            // 
            this.uiCheckBoxGroup2.ColumnCount = 4;
            this.uiCheckBoxGroup2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiCheckBoxGroup2.Items.AddRange(new object[] {
            "test1",
            "test2",
            "test3",
            "test4"});
            this.uiCheckBoxGroup2.ItemSize = new System.Drawing.Size(130, 35);
            this.uiCheckBoxGroup2.Location = new System.Drawing.Point(57, 60);
            this.uiCheckBoxGroup2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiCheckBoxGroup2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiCheckBoxGroup2.Name = "uiCheckBoxGroup2";
            this.uiCheckBoxGroup2.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiCheckBoxGroup2.SelectedIndexes = ((System.Collections.Generic.List<int>)(resources.GetObject("uiCheckBoxGroup2.SelectedIndexes")));
            this.uiCheckBoxGroup2.Size = new System.Drawing.Size(700, 68);
            this.uiCheckBoxGroup2.Style = Sunny.UI.UIStyle.Custom;
            this.uiCheckBoxGroup2.TabIndex = 62;
            this.uiCheckBoxGroup2.Text = "测试用例";
            this.uiCheckBoxGroup2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiCheckBoxGroup2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiCheckBoxGroup2.ValueChanged += new Sunny.UI.UICheckBoxGroup.OnValueChanged(this.uiCheckBoxGroup2_ValueChanged);
            // 
            // uiCheckBoxGroup1
            // 
            this.uiCheckBoxGroup1.AutoSize = true;
            this.uiCheckBoxGroup1.ColumnCount = 5;
            this.uiCheckBoxGroup1.ColumnInterval = 1;
            this.uiCheckBoxGroup1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiCheckBoxGroup1.Items.AddRange(new object[] {
            "conv1_1",
            "conv2_2",
            "conv3_3",
            "conv4_4",
            "conv5_5"});
            this.uiCheckBoxGroup1.ItemSize = new System.Drawing.Size(130, 35);
            this.uiCheckBoxGroup1.Location = new System.Drawing.Point(52, 138);
            this.uiCheckBoxGroup1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiCheckBoxGroup1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiCheckBoxGroup1.Name = "uiCheckBoxGroup1";
            this.uiCheckBoxGroup1.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiCheckBoxGroup1.SelectedIndexes = ((System.Collections.Generic.List<int>)(resources.GetObject("uiCheckBoxGroup1.SelectedIndexes")));
            this.uiCheckBoxGroup1.Size = new System.Drawing.Size(700, 68);
            this.uiCheckBoxGroup1.Style = Sunny.UI.UIStyle.Custom;
            this.uiCheckBoxGroup1.TabIndex = 61;
            this.uiCheckBoxGroup1.Text = "卷积核";
            this.uiCheckBoxGroup1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiCheckBoxGroup1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiCheckBoxGroup1.ValueChanged += new Sunny.UI.UICheckBoxGroup.OnValueChanged(this.uiCheckBoxGroup1_ValueChanged);
            // 
            // textBoxEP
            // 
            this.textBoxEP.ButtonWidth = 40;
            this.textBoxEP.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxEP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxEP.DoubleValue = 90000000D;
            this.textBoxEP.Font = new System.Drawing.Font("宋体", 10F);
            this.textBoxEP.IntValue = 90000000;
            this.textBoxEP.Location = new System.Drawing.Point(30, 5);
            this.textBoxEP.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxEP.MinimumSize = new System.Drawing.Size(1, 16);
            this.textBoxEP.Name = "textBoxEP";
            this.textBoxEP.Radius = 1;
            this.textBoxEP.ShowText = false;
            this.textBoxEP.Size = new System.Drawing.Size(140, 32);
            this.textBoxEP.TabIndex = 41;
            this.textBoxEP.Text = "90000000";
            this.textBoxEP.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.textBoxEP.Watermark = "";
            this.textBoxEP.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // textBoxsize
            // 
            this.textBoxsize.ButtonWidth = 40;
            this.textBoxsize.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxsize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxsize.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxsize.Location = new System.Drawing.Point(30, 5);
            this.textBoxsize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxsize.MinimumSize = new System.Drawing.Size(1, 16);
            this.textBoxsize.Name = "textBoxsize";
            this.textBoxsize.Radius = 1;
            this.textBoxsize.ShowText = false;
            this.textBoxsize.Size = new System.Drawing.Size(140, 24);
            this.textBoxsize.TabIndex = 45;
            this.textBoxsize.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.textBoxsize.Watermark = "";
            this.textBoxsize.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.Location = new System.Drawing.Point(25, 57);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(100, 23);
            this.uiLabel1.TabIndex = 1;
            this.uiLabel1.Text = "设备ID";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel2
            // 
            this.uiLabel2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel2.Location = new System.Drawing.Point(25, 111);
            this.uiLabel2.Name = "uiLabel2";
            this.uiLabel2.Size = new System.Drawing.Size(100, 23);
            this.uiLabel2.TabIndex = 2;
            this.uiLabel2.Text = "权重文件";
            this.uiLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel3
            // 
            this.uiLabel3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel3.Location = new System.Drawing.Point(25, 171);
            this.uiLabel3.Name = "uiLabel3";
            this.uiLabel3.Size = new System.Drawing.Size(100, 23);
            this.uiLabel3.TabIndex = 3;
            this.uiLabel3.Text = "测试图片";
            this.uiLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel3.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiTextBox1
            // 
            this.uiTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox1.Location = new System.Drawing.Point(123, 111);
            this.uiTextBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTextBox1.Name = "uiTextBox1";
            this.uiTextBox1.Padding = new System.Windows.Forms.Padding(2);
            this.uiTextBox1.ShowText = false;
            this.uiTextBox1.Size = new System.Drawing.Size(223, 35);
            this.uiTextBox1.Style = Sunny.UI.UIStyle.Custom;
            this.uiTextBox1.TabIndex = 50;
            this.uiTextBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox1.Watermark = "";
            this.uiTextBox1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // qzopenButton
            // 
            this.qzopenButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.qzopenButton.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.qzopenButton.Location = new System.Drawing.Point(344, 111);
            this.qzopenButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.qzopenButton.Name = "qzopenButton";
            this.qzopenButton.Size = new System.Drawing.Size(100, 35);
            this.qzopenButton.TabIndex = 51;
            this.qzopenButton.Text = "打开权重";
            this.qzopenButton.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.qzopenButton.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // tpopenButton
            // 
            this.tpopenButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tpopenButton.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tpopenButton.Location = new System.Drawing.Point(343, 168);
            this.tpopenButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.tpopenButton.Name = "tpopenButton";
            this.tpopenButton.Size = new System.Drawing.Size(100, 35);
            this.tpopenButton.TabIndex = 52;
            this.tpopenButton.Text = "打开图片";
            this.tpopenButton.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tpopenButton.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiTextBox2
            // 
            this.uiTextBox2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox2.Location = new System.Drawing.Point(125, 171);
            this.uiTextBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTextBox2.Name = "uiTextBox2";
            this.uiTextBox2.Padding = new System.Windows.Forms.Padding(2);
            this.uiTextBox2.ShowText = false;
            this.uiTextBox2.Size = new System.Drawing.Size(221, 32);
            this.uiTextBox2.Style = Sunny.UI.UIStyle.Custom;
            this.uiTextBox2.TabIndex = 53;
            this.uiTextBox2.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox2.Watermark = "";
            this.uiTextBox2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // qxButton
            // 
            this.qxButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.qxButton.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.qxButton.Location = new System.Drawing.Point(343, 267);
            this.qxButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.qxButton.Name = "qxButton";
            this.qxButton.Size = new System.Drawing.Size(113, 35);
            this.qxButton.TabIndex = 54;
            this.qxButton.Text = "取消";
            this.qxButton.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.qxButton.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // FT6678_YOLO_diag
            // 
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Beige;
            this.ClientSize = new System.Drawing.Size(1312, 706);
            this.Controls.Add(this.uiTabControl1);
            this.Menu = this.mainMenu1;
            this.Name = "FT6678_YOLO_diag";
            this.Text = " ";
            this.Closed += new System.EventHandler(this.FT6678_YOLO_diag_Closing);
            this.Load += new System.EventHandler(this.FT6678_YOLO_diag_Load);
            this.Resize += new System.EventHandler(this.FT6678_YOLO_diag_Resize);
            this.uiTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.uiTableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.Graphics.FillRectangle(Brushes.DarkGray, e.Bounds);   //采用特定颜色绘制标题列,这里我用的灰色
                e.DrawText();   //采用默认方式绘制标题文本
            }

            else if (e.ColumnIndex == 1)
            {
                e.Graphics.FillRectangle(Brushes.DarkGray, e.Bounds);   //采用特定颜色绘制标题列,这里我用的灰色
                e.DrawText();   //采用默认方式绘制标题文本
            }

            else if (e.ColumnIndex == 2)
            {
                e.Graphics.FillRectangle(Brushes.DarkGray, e.Bounds);   //采用特定颜色绘制标题列,这里我用的灰色
                e.DrawText();   //采用默认方式绘制标题文本
            }
            else if (e.ColumnIndex == 3)
            {
                e.Graphics.FillRectangle(Brushes.DarkGray, e.Bounds);   //采用特定颜色绘制标题列,这里我用的灰色
                e.DrawText();   //采用默认方式绘制标题文本
            }
            else if (e.ColumnIndex == 4)
            {
                e.Graphics.FillRectangle(Brushes.DarkGray, e.Bounds);   //采用特定颜色绘制标题列,这里我用的灰色
                e.DrawText();   //采用默认方式绘制标题文本
            }
            else if (e.ColumnIndex == 5)
            {
                e.Graphics.FillRectangle(Brushes.DarkGray, e.Bounds);   //采用特定颜色绘制标题列,这里我用的灰色
                e.DrawText();   //采用默认方式绘制标题文本
            }
            else if (e.ColumnIndex == 6)
            {
                e.Graphics.FillRectangle(Brushes.DarkGray, e.Bounds);   //采用特定颜色绘制标题列,这里我用的灰色
                e.DrawText();   //采用默认方式绘制标题文本
            }
            else if (e.ColumnIndex == 7)
            {
                e.Graphics.FillRectangle(Brushes.DarkGray, e.Bounds);   //采用特定颜色绘制标题列,这里我用的灰色
                e.DrawText();   //采用默认方式绘制标题文本
            }
            else if (e.ColumnIndex == 8)
            {
                e.Graphics.FillRectangle(Brushes.DarkGray, e.Bounds);   //采用特定颜色绘制标题列,这里我用的灰色
                e.DrawText();   //采用默认方式绘制标题文本
            }
            else if (e.ColumnIndex == 9)
            {
                e.Graphics.FillRectangle(Brushes.DarkGray, e.Bounds);   //采用特定颜色绘制标题列,这里我用的灰色
                e.DrawText();   //采用默认方式绘制标题文本
            }
        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;

        }

        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        /// The main entry point for the application.
        [STAThread]
        static void Main()
        {
            Application.Run(new FT6678_YOLO_diag());
        }

        //20220818 wm 写FT6678的PCIe配置寄存器
        //入参：
        //      FT6678_YOLO_Device device       设备指针
        //      DWORD Offset                    寄存器偏移量（基地址为0x21801000）
        //      byte[] buff                     设置参数（固定32位）,以一个字节为单位，低字节在低位，高字节在高位
        //出参：
        //      bool num                        返回1为写入参数成功；返回0为写入参数失败
        private bool SetPCIeReg(DWORD Offset, byte[] buff)
        {
            FT6678_YOLO_Device device = pciDevList.Get(lstBxDevices.SelectedIndex);
            DWORD Bytes = 4;    //单次读写字节数
            IntPtr pData = Marshal.AllocHGlobal((int)Bytes);
            byte[] bufftest = new byte[Bytes];    //检验值
            DWORD dwStatus;

            //写配置
            Marshal.Copy(buff, 0, pData, (int)Bytes);
            dwStatus = wdc_lib_decl.WDC_PciWriteCfg(device.Handle, Offset, pData, Bytes);   //注意！需要先device.Open()，才可以使用
            //读配置
            dwStatus = wdc_lib_decl.WDC_PciReadCfg(device.Handle, Offset, pData, Bytes);    //注意！需要先device.Open()，才可以使用
            if (dwStatus == (DWORD)wdc_err.WD_STATUS_SUCCESS)
                Marshal.Copy(pData, bufftest, 0, (int)Bytes);
            //确认参数已经写入
            if (Enumerable.SequenceEqual(buff, bufftest))
                return true;
            return false;
        }
        private void writePCIeReg(DWORD Offset, DWORD data)
        {
            FT6678_YOLO_Device device = pciDevList.Get(lstBxDevices.SelectedIndex);
            DWORD Bytes = 4;    //单次读写字节数
            IntPtr pData = Marshal.AllocHGlobal((int)Bytes);
            DWORD dwStatus;
            byte[] buff = new byte[Bytes];    //传输值
            int i;
            for (i = (int)Bytes - 1; i >= 0; i--)
                buff[i] = (byte)(data >> i * 8);

            //写配置
            Marshal.Copy(buff, 0, pData, (int)Bytes);
            dwStatus = wdc_lib_decl.WDC_PciWriteCfg(device.Handle, Offset, pData, Bytes);   //注意！需要先device.Open()，才可以使用
            //读配置
            dwStatus = wdc_lib_decl.WDC_PciReadCfg(device.Handle, Offset, pData, Bytes);    //注意！需要先device.Open()，才可以使用

        }
        private DWORD readPCIeReg(DWORD Offset)
        {

            FT6678_YOLO_Device device = pciDevList.Get(lstBxDevices.SelectedIndex);
            DWORD Bytes = 4;    //单次读写字节数
            IntPtr pData = Marshal.AllocHGlobal((int)Bytes);
            DWORD dwStatus;
            byte[] buff = new byte[Bytes];    //传输值
            int i;
            DWORD data = 0;

            //读配置
            dwStatus = wdc_lib_decl.WDC_PciReadCfg(device.Handle, Offset, pData, Bytes);    //注意！需要先device.Open()，才可以使用
            if (dwStatus == (DWORD)wdc_err.WD_STATUS_SUCCESS)
                Marshal.Copy(pData, buff, 0, (int)Bytes);
            for (i = (int)Bytes - 1; i >= 0; i--)
                data += (DWORD)(buff[i] << i * 8);
            return data;
        }
        private void ChangeEpAddr(DWORD EPaddr)
        {
            FT6678_YOLO_Device device = pciDevList.Get(lstBxDevices.SelectedIndex);
            DWORD Offset = 0;    //偏移地址
            DWORD Bytes = 4;    //单次读写字节数
            byte[] buff = new byte[Bytes];    //传输值
            int i;
            Offset = 0x918;
            for (i = (int)Bytes - 1; i >= 0; i--)
                buff[i] = (byte)(EPaddr >> i * 8);
            if (!SetPCIeReg(Offset, buff))
            {
                //Log.TraceLog("FT6678_PCIe_IATU_LMR_TARGET_ADDR_REG(0x918) error.");
                //throw m_excp;
            }
            //SET1_REG
            Offset = 0x904;
            for (i = 0; i < 4; i++)
                buff[i] = 0;
            if (!SetPCIeReg(Offset, buff))
            {
                Log.TraceLog("FT6678_PCIe_SET1_REG(0x904) error.");
                throw m_excp;
            }
            //SET2_REG
            Offset = 0x908;
            for (i = 0; i < 3; i++)
                buff[i] = 0;
            buff[3] = 0x80;
            if (!SetPCIeReg(Offset, buff))
            {
                Log.TraceLog("FT6678_PCIe_SET2_REG(0x908) error.");
                throw m_excp;
            }
        }
        private void dma_transfer_data(DWORD ep_addr, DWORD size, DWORD direction)
        {
            FT6678_YOLO_Device device = pciDevList.Get(lstBxDevices.SelectedIndex);
            DWORD Offset = 0;    //偏移地址
            DWORD Bytes = 4;    //单次读写字节数
            DWORD data = 0;
            DWORD pAdd;
            byte[] buff = new byte[Bytes];    //传输值
            int i;
            if (direction == RC_DMA_WRITE)
            {
                writePCIeReg(DMA_RD_INTERRUPT_CLR, 1);
            }
            else
            {
                writePCIeReg(DMA_WR_INTERRUPT_CLR, 1);
            }

            if (direction == RC_DMA_WRITE)//实现RC本地存储空间到EP远程存储空间的数据搬移

            {
                //*(unsigned int *) (base + DMA_RDEN_OFFSET) = 1;
                writePCIeReg(DMA_RDEN_OFFSET, 1);


                pAdd = readPCIeReg(DMA_RD_INTERRUPT_STA);


                // *(unsigned int *) (base + DMA_INDEX_OFFSET) = (unsigned int)0x1<<31;
                writePCIeReg(DMA_INDEX_OFFSET, 0x80000000);

                //  *(unsigned int *) (base + DMA_SRC_LOW_OFFSET) = rc_addr;
                writePCIeReg(DMA_SRC_LOW_OFFSET, (DWORD)RCaddr);

                // *(unsigned int *) (base + DMA_SRC_HIGH_OFFSET) = 0;
                writePCIeReg(DMA_SRC_HIGH_OFFSET, (DWORD)(RCaddr >> 32));

                // *(unsigned int *) (base + DMA_DST_LOW_OFFSET) = ep_addr;
                writePCIeReg(DMA_DST_LOW_OFFSET, ep_addr);

                // *(unsigned int *) (base + DMA_DST_HIGH_OFFSET) = 0;
                writePCIeReg(DMA_DST_HIGH_OFFSET, 0);

                // *(unsigned int *) (base + DMA_RD_INTERRUPT_MSK) = 0;
                writePCIeReg(DMA_RD_INTERRUPT_MSK, 0);

                //  *(unsigned int *)(base + DMA_CTRL1_OFFSET) = 0x00000018;
                writePCIeReg(DMA_CTRL1_OFFSET, 0x00000018);
                //  *(unsigned int *) (base + DMA_SIZE_OFFSET) = size;
                writePCIeReg(DMA_SIZE_OFFSET, size);
                //  *(unsigned int *) (base + DMA_RDDOORBELL_OFFSET) = 0;
                writePCIeReg(DMA_RDDOORBELL_OFFSET, 0);
            }
            else
            {//RC_DMA_READ EP的EDMA将实现EP远程存储空间到RC本地存储空间的数据搬移；

                //    *(unsigned int *) (base + DMA_WREN_OFFSET) = 1;
                writePCIeReg(DMA_INDEX_OFFSET, 0x80000000);

                //    pAdd = (unsigned int *) (base + DMA_WR_INTERRUPT_STA);

                //   *(unsigned int *) (base + DMA_INDEX_OFFSET) = 0;
                writePCIeReg(DMA_INDEX_OFFSET, 0);

                //   *(unsigned int *) (base + DMA_WR_INTERRUPT_MSK) = 0;
                writePCIeReg(DMA_WR_INTERRUPT_MSK, 0);

                //  *(unsigned int *) (base + DMA_SRC_LOW_OFFSET) = ep_addr;//0x0c300 0000
                writePCIeReg(DMA_SRC_LOW_OFFSET, ep_addr);

                //  *(unsigned int *) (base + DMA_SRC_HIGH_OFFSET) = 0;
                writePCIeReg(DMA_SRC_HIGH_OFFSET, 0);

                //  *(unsigned int *) (base + DMA_DST_LOW_OFFSET) = rc_addr;//0x0c20 0000
                writePCIeReg(DMA_DST_LOW_OFFSET, (DWORD)RCaddr);

                //   *(unsigned int *) (base + DMA_DST_HIGH_OFFSET) = 0;
                writePCIeReg(DMA_DST_HIGH_OFFSET, 0);

                //   *(unsigned int *)(base + DMA_CTRL1_OFFSET) = 0x00000018;
                writePCIeReg(DMA_CTRL1_OFFSET, 0x00000018);

                //    *(unsigned int *) (base + DMA_SIZE_OFFSET) = size;
                writePCIeReg(DMA_SIZE_OFFSET, size);

                //    *(unsigned int *) (base + DMA_WRDOORBELL_OFFSET) = 0;
                writePCIeReg(DMA_WRDOORBELL_OFFSET, 0);

            }

            //	        while (((*pAdd) & 0x1) != 0x1);


            //	        printf("DMA finished \n");*/

        }
        //20220818 wm 设置EP端PCIe寄存器所需值
        private void Init6678PCIe()
        {
            //设置EP端BAR0映射
            FT6678_YOLO_Device device = pciDevList.Get(lstBxDevices.SelectedIndex);
            DWORD Offset = 0;    //偏移地址
            DWORD Bytes = 4;    //单次读写字节数
            byte[] buff = new byte[Bytes];    //传输值
            int i;
            m_excp = new Exception("输入的数据无效或有误. " + "请再次修改后尝试 (hex)");
            //获取RC端地址及有效长度
            /*
            RCaddr = device.RCAddrToUint64(false);
            RCSize = device.RCSizeToUint64(false);
            textBoxRC.Text = RCaddr.ToString("X");
            textBoxsize.Text = RCSize.ToString("X");
            textBoxRC.Enabled = false;
            textBoxsize.Enabled = false;
            */
            //获取EP端地址
            string str = diag_lib.PadBuffer(textBoxEP.Text, (DWORD)textBoxEP.Text.Length, (DWORD)2 * Bytes);
            EPaddr = Convert.ToUInt32(str.Substring(0, textBoxEP.Text.Length), 16);
            if (EPaddr < 0 || EPaddr > 0xffffffff)
                throw m_excp;

            //BAR0_REG
            Offset = 0x10;
            for (i = (int)Bytes - 1; i >= 0; i--)
                buff[i] = (byte)(RCaddr >> i * 8);
            if (!SetPCIeReg(Offset, buff))
            {
                Log.TraceLog("FT6678_PCIe_BAR0_REG(0x10) error.");
                throw m_excp;
            }
            //MEM_LIMIT_BASE_REG
            Offset = 0x20;
            UINT64 RClimit = RCaddr + RCSize - 1;
            UINT64 MEM_limit = (RClimit & 0xfff00000) | ((RCaddr & 0xfff00000) >> 16);
            for (i = (int)Bytes - 1; i >= 0; i--)
                buff[i] = (byte)(MEM_limit >> i * 8);
            if (!SetPCIeReg(Offset, buff))
            {
                //not work
                //Log.TraceLog("FT6678_PCIe_MEM_LIMIT_BASE_REG(0x20) error."); 
                //throw m_excp;
            }
            //STAT_CMD_REG
            Offset = 0x04;
            buff[0] = 0x07;
            for (i = 1; i < 4; i++)
                buff[i] = 0;
            if (!SetPCIeReg(Offset, buff))
            {
                //Log.TraceLog("FT6678_PCIe_STAT_CMD_REG(0x04) error.");
                //throw m_excp;
            }
            //IATU_VIEWPORT_REG
            Offset = 0x900;
            buff[0] = 0x03;
            buff[1] = 0x00;
            buff[2] = 0x00;
            buff[3] = 0x80;
            if (!SetPCIeReg(Offset, buff))
            {
                Log.TraceLog("FT6678_PCIe_IATU_VIEWPORT_REG(0x900) error.");
                throw m_excp;
            }
            //IATU_LWR_BASE_ADDR_REG
            Offset = 0x90c;
            for (i = (int)Bytes - 1; i >= 0; i--)
                buff[i] = (byte)(RCaddr >> i * 8);
            if (!SetPCIeReg(Offset, buff))
            {
                Log.TraceLog("FT6678_PCIe_IATU_LWR_BASE_ADDR_REG(0x90c) error.");
                throw m_excp;
            }
            //IATU_UWR_BASE_ADDR_REG
            Offset = 0x910;
            for (i = 0; i < 4; i++)
                buff[i] = 0;
            if (!SetPCIeReg(Offset, buff))
            {
                Log.TraceLog("FT6678_PCIe_IATU_UWR_BASE_ADDR_REG(0x910) error.");
                throw m_excp;
            }
            //IATU_LIMIT_ADDR_REG
            Offset = 0x914;
            for (i = (int)Bytes - 1; i >= 0; i--)
                buff[i] = (byte)(RClimit >> i * 8);
            if (!SetPCIeReg(Offset, buff))
            {
                Log.TraceLog("FT6678_PCIe_IATU_LIMIT_ADDR_REG(0x914) error.");
                throw m_excp;
            }
            //IATU_LMR_TARGET_ADDR_REG
            Offset = 0x918;
            for (i = (int)Bytes - 1; i >= 0; i--)
                buff[i] = (byte)(EPaddr >> i * 8);
            if (!SetPCIeReg(Offset, buff))
            {
                //Log.TraceLog("FT6678_PCIe_IATU_LMR_TARGET_ADDR_REG(0x918) error.");
                //throw m_excp;
            }
            //SET1_REG
            Offset = 0x904;
            for (i = 0; i < 4; i++)
                buff[i] = 0;
            if (!SetPCIeReg(Offset, buff))
            {
                Log.TraceLog("FT6678_PCIe_SET1_REG(0x904) error.");
                throw m_excp;
            }
            //SET2_REG
            Offset = 0x908;
            for (i = 0; i < 3; i++)
                buff[i] = 0;
            buff[3] = 0x80;
            if (!SetPCIeReg(Offset, buff))
            {
                Log.TraceLog("FT6678_PCIe_SET2_REG(0x908) error.");
                throw m_excp;
            }

        }

        static bool ConvertIntToByteArray(UInt32 m, ref byte[] arry)
        {
            if (arry == null) return false;
            if (arry.Length < 4) return false;
            arry[0] = (byte)(m & 0xFF);
            arry[1] = (byte)((m & 0xFF00) >> 8);
            arry[2] = (byte)((m & 0xFF0000) >> 16);
            arry[3] = (byte)((m >> 24) & 0xFF);
            return true;
        }


        //20220818 wm 设置EP端PCIe寄存器总入口
        private bool Startinit6678PCIe()
        {
            try
            {
                Init6678PCIe();
            }
            catch
            {
                MessageBox.Show(m_excp.Message, "输入数据有误",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }


        /* Open a handle to a device */
        private bool DeviceOpen(int iSelectedIndex)
        {
            DWORD dwStatus;
            FT6678_YOLO_Device device = pciDevList.Get(iSelectedIndex);

            ///* Open a handle to the device */
            //dwStatus = device.Open();
            //if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
            //{
            //    Log.ErrLog("FT6678_YOLO_diag.DeviceOpen: Failed opening a " +
            //        "handle to the device (" + device.ToString(false) + ")");
            //    return false;
            //}
            //Log.TraceLog("FT6678_YOLO_diag.DeviceOpen: " +
            //    "The device was successfully open." +
            //    "You can now activate the device through the enabled menu " +
            //    "above");
            //return true;

            if (device != null && device.Handle == IntPtr.Zero)  //bug 20220818 wm changed
            {
                /* Open a handle to the device */
                dwStatus = device.Open();
                if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
                {
                    Log.ErrLog("FT6678_YOLO_diag.DeviceOpen: Failed opening a " +
                        "handle to the device (" + device.ToString(false) + ")");
                    return false;
                }
                Log.TraceLog("FT6678_YOLO_diag.DeviceOpen: " +
                    "The device was successfully open." +
                    "You can now activate the device through the enabled menu " +
                    "above");
                return true;
            }
            return false;
        }

        /* Close handle to a FT6678_YOLO device */
        private BOOL DeviceClose(int iSelectedIndex)
        {
            FT6678_YOLO_Device device = pciDevList.Get(iSelectedIndex);
            BOOL bStatus = false;

            if (device.Handle != IntPtr.Zero && !(bStatus = device.Close()))
            {
                Log.ErrLog("FT6678_YOLO_diag.DeviceClose: Failed closing FT6678_YOLO "
                    + "device (" + device.ToString(false) + ")");
            }
            else
            {
                device.Handle = IntPtr.Zero;
            }
            return bStatus;
        }

        private void FT6678_YOLO_diag_Load(object sender, System.EventArgs e)
        {
            DWORD dwStatus = pciDevList.Init();
            if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
                goto Error;

            foreach (FT6678_YOLO_Device dev in pciDevList)
                lstBxDevices.Items.Add(dev.ToString(true));
            lstBxDevices.SelectedIndex = 0;

            return;
        Error:
            DisableMenu();
            btDevice.Enabled = false;
        }


        private void FT6678_YOLO_IntHandler(FT6678_YOLO_Device dev)
        {
            Log.TraceLog("interrupt for device {" + dev.ToString(false) +
                "} received!");

            Log.TraceLog("Interrupt Type: " +
                dev.WDC_DIAG_IntTypeDescriptionGet());

            if (dev.IsMsiInt())
                Log.TraceLog("Message Data: " + dev.GetEnableIntLastMsg());
        }



        private void FT6678_YOLO_EventHandler(ref WD_EVENT wdEvent,
            FT6678_YOLO_Device dev)
        {
            string sAction;
            switch ((WD_EVENT_ACTION)wdEvent.dwAction)
            {
                case WD_EVENT_ACTION.WD_INSERT:
                    sAction = "WD_INSERT";
                    break;
                case WD_EVENT_ACTION.WD_REMOVE:
                    sAction = "WD_REMOVE";
                    break;
                case WD_EVENT_ACTION.WD_POWER_CHANGED_D0:
                    sAction = "WD_POWER_CHANGED_D0";
                    break;
                case WD_EVENT_ACTION.WD_POWER_CHANGED_D1:
                    sAction = "WD_POWER_CHANGED_D1";
                    break;
                case WD_EVENT_ACTION.WD_POWER_CHANGED_D2:
                    sAction = "WD_POWER_CHANGED_D2";
                    break;
                case WD_EVENT_ACTION.WD_POWER_CHANGED_D3:
                    sAction = "WD_POWER_CHANGED_D3";
                    break;
                case WD_EVENT_ACTION.WD_POWER_SYSTEM_WORKING:
                    sAction = "WD_POWER_SYSTEM_WORKING";
                    break;
                case WD_EVENT_ACTION.WD_POWER_SYSTEM_SLEEPING1:
                    sAction = "WD_POWER_SYSTEM_SLEEPING1";
                    break;
                case WD_EVENT_ACTION.WD_POWER_SYSTEM_SLEEPING2:
                    sAction = "WD_POWER_SYSTEM_SLEEPING2";
                    break;
                case WD_EVENT_ACTION.WD_POWER_SYSTEM_SLEEPING3:
                    sAction = "WD_POWER_SYSTEM_SLEEPING3";
                    break;
                case WD_EVENT_ACTION.WD_POWER_SYSTEM_HIBERNATE:
                    sAction = "WD_POWER_SYSTEM_HIBERNATE";
                    break;
                case WD_EVENT_ACTION.WD_POWER_SYSTEM_SHUTDOWN:
                    sAction = "WD_POWER_SYSTEM_SHUTDOWN";
                    break;
                default:
                    sAction = wdEvent.dwAction.ToString("X");
                    break;
            }
            Log.TraceLog("Received event notification of type " + sAction +
                " on " + dev.ToString(false));
        }


        private void FT6678_YOLO_diag_Closing(object sender, System.EventArgs e)
        {
            pciDevList.Dispose();
        }

        /* list box lstBxDevices */
        private void lstBxDevices_SelectedIndexChanged(object sender,
            System.EventArgs e)
        {
            if (lstBxDevices.SelectedIndex < 0)
            {
                DisableMenu();
                btDevice.Enabled = false;
            }
            else
            {
                FT6678_YOLO_Device dev =
                    pciDevList.Get(lstBxDevices.SelectedIndex);
                UpdateMenu(lstBxDevices.SelectedIndex);
                btDevice.Enabled = true;
                if (dev.Handle == IntPtr.Zero)
                    btDevice.Text = "打开设备";
                else
                    btDevice.Text = "关闭设备";

                menuRTRegs.Visible = (dev.Regs.gFT6678_YOLO_RT_Regs.Length > 0) ?
                    true : false;
            }
        }

        private void lstBxDevices_DoubleClicked(object sender,
            System.EventArgs e)
        {
            btDevice_Click(sender, e);
        }

        /* device button */
        private void btDevice_Click(object sender, System.EventArgs e)
        {
            if (btDevice.Text == "打开设备")
            {
                bool open = DeviceOpen(lstBxDevices.SelectedIndex);
                bool pcieinit = Startinit6678PCIe();
                //20220819 wm 不得不open两次。有更好方法再改
                DeviceClose(lstBxDevices.SelectedIndex);
                open = DeviceOpen(lstBxDevices.SelectedIndex);

                if (open && pcieinit)
                {
                    btDevice.Text = "关闭设备";
                    EnableMenu();
                    Log.TraceLog("FT6678_YOLO_diag.DeviceOpen:成功打开设备\n");
                }
                else
                {
                    Log.ErrLog("FT6678_YOLO_diag.DeviceOpen:无法打开设备\n");
                    DeviceClose(lstBxDevices.SelectedIndex);
                }
                FT6678_YOLO_Device dev =
                    pciDevList.Get(lstBxDevices.SelectedIndex);
                wdc_lib_decl.WDC_DMAContigBufLock(dev.Handle, ref dev.pBuffer, 0, 0x100000, ref dev.ppDma);
                WD_DMA[] ppDma = new WD_DMA[2];
            }
            else
            {
                FT6678_YOLO_Device dev =
                    pciDevList.Get(lstBxDevices.SelectedIndex);
                DeviceClose(lstBxDevices.SelectedIndex);
                btDevice.Text = "打开设备";
                DisableMenu();
                Log.TraceLog("FT6678_YOLO_diag.DeviceOpen:已关闭设备\n");
            }
        }

        /* Menu*/
        private void UpdateMenu(int index)
        {
            FT6678_YOLO_Device dev =
                pciDevList.Get(lstBxDevices.SelectedIndex);
            if (dev.Handle == IntPtr.Zero)
                DisableMenu();
            else
                EnableMenu();
        }

        private void EnableMenu()
        {
            ToggleMenu(true);
        }

        private void DisableMenu()
        {
            ToggleMenu(false);
        }

        private void ToggleMenu(bool flag)
        {
            for (int index = 0; index < mainMenu1.MenuItems.Count; ++index)
                mainMenu1.MenuItems[index].Enabled = flag;
        }


        /* Address Space Item */
        private void menuAddrRW_Click(object sender, System.EventArgs e)   //获取到地址
        {
            FT6678_YOLO_Device dev =
                pciDevList.Get(lstBxDevices.SelectedIndex);
            string[] sBars = dev.AddrDescToString(false);
            AddrSpaceTransferForm addrSpcFrm = new
                AddrSpaceTransferForm(dev, sBars);
            addrSpcFrm.GetInput();
        }

        /* Interrupts items*/

        private void menuInterrupts_Select(object sender,
            System.EventArgs e)
        {
            if (menuInterrupts.Enabled == false)
                return;
            FT6678_YOLO_Device dev = pciDevList.Get(lstBxDevices.SelectedIndex);
            bool bIsEnb = dev.IsEnabledInt();

            menuEnableInt.Text = bIsEnb ? "Disable Interrupts" :
                "Enable Interrupts";
        }

        private void menuEnableInt_Click(object sender, System.EventArgs e)    //是否中断进程
        {
            FT6678_YOLO_Device dev = pciDevList.Get(lstBxDevices.SelectedIndex);
            if (menuEnableInt.Text == "Enable Interrupts")
            {
                DWORD dwStatus = dev.EnableInterrupts(new
                    USER_INTERRUPT_CALLBACK(FT6678_YOLO_IntHandler), dev.Handle);
                if (dwStatus == (DWORD)wdc_err.WD_STATUS_SUCCESS)
                    menuEnableInt.Text = "Disable Interrupts";
            }
            else
            {
                DWORD dwStatus = dev.DisableInterrupts();
                if (dwStatus == (DWORD)wdc_err.WD_STATUS_SUCCESS)
                    menuEnableInt.Text = "Enable Interrupts";
            }
        }



        /* Event Items*/
        private void menuEvents_Select(object sender, System.EventArgs e)
        {
            if (menuEvents.Enabled == false)
                return;
            FT6678_YOLO_Device dev = pciDevList.Get(lstBxDevices.SelectedIndex);
            menuRegisterEvent.Text = dev.IsEventRegistered() ?
                "Unregister Events" : "Register Events";
        }

        private void menuRegisterEvent_Click(object sender, System.EventArgs e)   //寄存器的事件
        {
            if (menuRegisterEvent.Text == "Register Events")
            {
                pciDevList.Get(lstBxDevices.SelectedIndex).
                    EventRegister(new USER_EVENT_CALLBACK(FT6678_YOLO_EventHandler));
                menuRegisterEvent.Text = "Unregister Events";
            }
            else
            {
                pciDevList.Get(lstBxDevices.SelectedIndex).
                    EventUnregister();
                menuRegisterEvent.Text = "Register Events";
            }
        }


        /* Configuration Space Items*/
        private void menuCfgOffset_Click(object sender, System.EventArgs e) //偏移量配置
        {
            FT6678_YOLO_Device dev =
                pciDevList.Get(lstBxDevices.SelectedIndex);
            CfgTransfersForm cfgOffsetFrom = new CfgTransfersForm(dev);
            cfgOffsetFrom.GetInput();
        }

        private void menuCfgReg_Click(object sender, System.EventArgs e)  //寄存器配置
        {
            FT6678_YOLO_Device dev =
                pciDevList.Get(lstBxDevices.SelectedIndex);
            RegistersForm regForm = new RegistersForm(dev, ACTION_TYPE.CFG);
            regForm.GetInput();
        }

        /*RunTime Registers Items*/
        private void menuRTRegsRW_Click(object sender, System.EventArgs e)
        {
            FT6678_YOLO_Device dev =
                pciDevList.Get(lstBxDevices.SelectedIndex);
            RegistersForm regForm = new RegistersForm(dev, ACTION_TYPE.RT);
            regForm.GetInput();
        }
        /*

        private void btExit_Click(object sender, System.EventArgs e)
        {
            Close();
            Dispose();
        }

        private void btLog_Click(object sender, System.EventArgs e)
        {
            txtLog.Clear();
        }

        public void LogFunc(string str)
        {
            if (txtLog != null)
                txtLog.Text += str + Environment.NewLine;
        }

        public void TraceLog(string str)
        {
            if (this.InvokeRequired)
                Invoke(new Log.TRACE_LOG(LogFunc), new object[] { str });
            else
                LogFunc(str);
        }

        public void ErrLog(string str)
        {
            if (this.InvokeRequired)
                Invoke(new Log.ERR_LOG(LogFunc), new object[] { str });
            else
                LogFunc(str);
        }
        */

        private void lblFT6678_YOLODev_Click(object sender, EventArgs e)
        {

        }
        private string pathname = string.Empty;             //定义路径名变量
        /*
         private void Btn_openimg_Click(object sender, EventArgs e)
         {
             //OpenFileDialog file = new OpenFileDialog();
             //file.InitialDirectory = ".";
             //file.Filter = "所有文件(*.*)|*.*";
             //file.ShowDialog();
             //if (file.FileName != string.Empty)
             //{
             //    try
             //    {
             //        pathname = file.FileName;   //获得文件的绝对路径
             //        FileStream fs = new FileStream(pathname, FileMode.Open, FileAccess.Read);
             //        this.pictureBox1.Image = Image.FromStream(fs);
             //        fs.Close();
             //        fs.Dispose();
             //    }
             //    catch (Exception ex)
             //    {
             //        MessageBox.Show(ex.Message);
             //    }
             //}  
         }
        */
        /*
        private void Btn_saveimg_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "所有文件(*.*)|*.*";
            save.ShowDialog();
            if (save.FileName != string.Empty)
            {
                pictureBox2.Image.Save(save.FileName);
                Log.TraceLog("FT6678_YOLO_diag:识别结果已保存\n");
            }  
        }

        */

        private void write_addrspace()
        {
            pcie_index = lstBxDevices.SelectedIndex;
            FT6678_YOLO_Device dev = pciDevList.Get(pcie_index);
            DWORD bar = 0;  //Bar0
            DWORD offset = 0x00000;   //偏移量
            DWORD total_bytes = WIDTH * HEIGHT + 1;    //传输总大小
            byte[] data = new byte[total_bytes]; ;  //传输数据
            WDC_ADDR_MODE mode = WDC_ADDR_MODE.WDC_MODE_8;  //传输字节模式
            WDC_ADDR_RW_OPTIONS options = WDC_ADDR_RW_OPTIONS.WDC_ADDR_RW_DEFAULT;
            int i, j;
            //选择图片路径
            /*  选择框
            OpenFileDialog file = new OpenFileDialog();
            FileStream fs, fs1;
            file.InitialDirectory = ".";
            file.Filter = "所有文件(*.*)|*.*";
            file.ShowDialog();
            if (file.FileName != string.Empty)
            */
            FileStream fs, fs1;
            {
                for (int p = 0; p < this.listView1.Items.Count; p++)
                {
                    if (this.listView1.Items[p].Checked)
                    {
                        pathname = "F:\\FPGA\\TOH\\data\\" + this.listView1.CheckedItems[p].SubItems[1].Text + ".bmp";

                    }
                }
                // this.ShowSuccessDialog(pathname.ToString());

                try
                {
                    //pathname = file.FileName;   //获得文件的绝对路径


                    fs = new FileStream(pathname, FileMode.Open, FileAccess.Read);
                    Bitmap img = new Bitmap(fs);
                    this.pictureBox1.Image = img;   //显示图像
                    fs.Close();
                    fs.Dispose();
                    //获取raw图像
                    for (j = 0; j < HEIGHT; j++)
                    {
                        for (i = 0; i < WIDTH; i++)
                        {
                            data[j * WIDTH + i] = img.GetPixel(i, j).B;
                        }
                    }
                    int file_len = (int)data.Length;//获取bin文件长度
                    DWORD EPaddr = 0x90000000;
                    WriteAddrdata(EPaddr, (DWORD)file_len, data);
                    string filename = "F:\\FPGA\\TOH\\data\\inputbmp.bin";
                    fs1 = new FileStream(filename, FileMode.OpenOrCreate); //初始化FileStream对象
                    BinaryWriter bw = new BinaryWriter(fs1);        //创建BinaryWriter对象
                    bw.Write(data);
                    bw.Close();
                    fs1.Close();
                    //                   data[WIDTH * HEIGHT] = 0x01;  //PCIE标志位
                    //发送数据     
                    //wdc_lib_decl.WDC_WriteAddrBlock(dev.Handle, bar, offset, total_bytes, data,mode, options);

                    //            WriteAddrdata(0x90000000, total_bytes, data);

                    //                   dma_transfer_data(0x90000000, total_bytes, RC_DMA_WRITE);     
                    /*/ 验证begin
                    offset = 0xA0000000;  //地址0xA0000000、0x90000000会蓝屏
                    wdc_lib_decl.WDC_WriteAddrBlock(dev.Handle, bar, offset, total_bytes, data, mode, options);
                    wdc_lib_decl.WDC_ReadAddrBlock(dev.Handle, bar, offset, total_bytes, data, mode, options);
                    //验证end/*/

                    Log.TraceLog("FT6678_YOLO_diag:开始识别...\n");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Log.TraceLog("FT6678_YOLO_diag:无法打开图片\n");
                }
            }
        }

        private void WriteAddrdata(DWORD EPaddr, DWORD weight_total_bytes, byte[] data)
        {
            pcie_index = lstBxDevices.SelectedIndex;
            FT6678_YOLO_Device dev = pciDevList.Get(pcie_index);
            WDC_ADDR_MODE mode = WDC_ADDR_MODE.WDC_MODE_8;  //传输字节模式
            WDC_ADDR_RW_OPTIONS options = WDC_ADDR_RW_OPTIONS.WDC_ADDR_RW_DEFAULT;
            DWORD bar = 0;                      //Bar0
            DWORD offset = 0;                  //偏移大小
            DWORD single_bytes = 0x100000;    //单次传输大小  
            IntPtr pData = Marshal.AllocHGlobal((int)single_bytes);//单次读写字节数 
            DWORD startOffset = EPaddr & 0xFFFFF;
            if (startOffset > 0)
            {
                ChangeEpAddr(EPaddr & 0xFFF00000);        //更改基地址
                single_bytes = 0x100000 - startOffset;
                if (weight_total_bytes < single_bytes)
                {
                    single_bytes = weight_total_bytes;
                }
                Marshal.Copy(data, (int)offset, pData, (int)single_bytes);  //单次写数据
                wdc_lib_decl.WDC_WriteAddrBlock(dev.Handle, bar, startOffset, single_bytes, pData, mode, options);    //offset是地址的偏移量            
                offset += single_bytes;
                weight_total_bytes -= single_bytes;
                single_bytes = 0x100000;
            }
            while (weight_total_bytes > 0)
            {
                if (weight_total_bytes < single_bytes)
                {
                    single_bytes = weight_total_bytes;
                }
                Marshal.Copy(data, (int)offset, pData, (int)single_bytes);  //单次读取数据
                ChangeEpAddr(EPaddr + offset);        //更改基地址
                wdc_lib_decl.WDC_WriteAddrBlock(dev.Handle, bar, 0, single_bytes, pData, mode, options);    //offset是地址的偏移量
                offset += single_bytes;
                weight_total_bytes -= single_bytes;
            }
            Marshal.FreeHGlobal(pData);

        }
        private void WriteAddrdata(DWORD EPaddr, DWORD weight_total_bytes, int[] data)
        {
            pcie_index = lstBxDevices.SelectedIndex;
            FT6678_YOLO_Device dev = pciDevList.Get(pcie_index);
            WDC_ADDR_MODE mode = WDC_ADDR_MODE.WDC_MODE_8;  //传输字节模式
            WDC_ADDR_RW_OPTIONS options = WDC_ADDR_RW_OPTIONS.WDC_ADDR_RW_DEFAULT;
            DWORD bar = 0;                      //Bar0
            DWORD offset = 0;                  //偏移大小
            DWORD single_bytes = 0x100000;    //单次传输大小  
            IntPtr pData = Marshal.AllocHGlobal((int)single_bytes);//单次读写字节数 
            DWORD startOffset = EPaddr & 0xFFFFF;
            if (startOffset > 0)
            {
                ChangeEpAddr(EPaddr & 0xFFF00000);        //更改基地址
                single_bytes = 0x100000 - startOffset;
                if (weight_total_bytes < single_bytes)
                {
                    single_bytes = weight_total_bytes;
                }
                Marshal.Copy(data, (int)offset / 4, pData, (int)single_bytes / 4);  //单次写数据
                wdc_lib_decl.WDC_WriteAddrBlock(dev.Handle, bar, startOffset, single_bytes, pData, mode, options);    //offset是地址的偏移量            
                offset += single_bytes;
                weight_total_bytes -= single_bytes;
                single_bytes = 0x100000;
            }
            while (weight_total_bytes > 0)
            {
                if (weight_total_bytes < single_bytes)
                {
                    single_bytes = weight_total_bytes;
                }
                Marshal.Copy(data, (int)offset / 4, pData, (int)single_bytes / 4);  //单次读取数据
                ChangeEpAddr(EPaddr + offset);        //更改基地址
                wdc_lib_decl.WDC_WriteAddrBlock(dev.Handle, bar, 0, single_bytes, pData, mode, options);    //offset是地址的偏移量
                offset += single_bytes;
                weight_total_bytes -= single_bytes;
            }
            Marshal.FreeHGlobal(pData);

        }
        private void ReadAddrdata(DWORD EPaddr, DWORD weight_total_bytes, byte[] data)
        {
            pcie_index = lstBxDevices.SelectedIndex;
            FT6678_YOLO_Device dev = pciDevList.Get(pcie_index);
            WDC_ADDR_MODE mode = WDC_ADDR_MODE.WDC_MODE_8;  //传输字节模式
            WDC_ADDR_RW_OPTIONS options = WDC_ADDR_RW_OPTIONS.WDC_ADDR_RW_DEFAULT;
            DWORD bar = 0;                      //Bar0
            DWORD offset = 0;                  //偏移大小
            DWORD single_bytes = 0x100000;    //单次传输大小  
            IntPtr pData = Marshal.AllocHGlobal((int)single_bytes);//单次读写字节数 
            DWORD startOffset = EPaddr & 0xFFFFF;
            if (startOffset > 0)
            {
                ChangeEpAddr(EPaddr & 0xFFF00000);        //更改基地址
                single_bytes = 0x100000 - startOffset;
                if (weight_total_bytes < single_bytes)
                {
                    single_bytes = weight_total_bytes;
                }
                wdc_lib_decl.WDC_ReadAddrBlock(dev.Handle, bar, startOffset, single_bytes, pData, mode, options);    //offset是地址的偏移量
                Marshal.Copy(pData, data, (int)offset, (int)single_bytes);  //单次读取数据
                offset += single_bytes;
                weight_total_bytes -= single_bytes;
                single_bytes = 0x100000;
            }
            while (weight_total_bytes > 0)
            {
                if (weight_total_bytes < single_bytes)
                {
                    single_bytes = weight_total_bytes;
                }
                ChangeEpAddr(EPaddr + offset);        //更改基地址
                wdc_lib_decl.WDC_ReadAddrBlock(dev.Handle, bar, 0, single_bytes, pData, mode, options);    //offset是地址的偏移量
                Marshal.Copy(pData, data, (int)offset, (int)single_bytes);  //单次读取数据
                offset += single_bytes;
                weight_total_bytes -= single_bytes;
            }
            Marshal.FreeHGlobal(pData);

        }
        private void ReadAddrdata(DWORD EPaddr, DWORD weight_total_bytes, int[] data)
        {
            pcie_index = lstBxDevices.SelectedIndex;
            FT6678_YOLO_Device dev = pciDevList.Get(pcie_index);
            WDC_ADDR_MODE mode = WDC_ADDR_MODE.WDC_MODE_8;  //传输字节模式
            WDC_ADDR_RW_OPTIONS options = WDC_ADDR_RW_OPTIONS.WDC_ADDR_RW_DEFAULT;
            DWORD bar = 0;                      //Bar0
            DWORD offset = 0;                  //偏移大小
            DWORD single_bytes = 0x100000;    //单次传输大小  
            IntPtr pData = Marshal.AllocHGlobal((int)single_bytes);//单次读写字节数 
            DWORD startOffset = EPaddr & 0xFFFFF;
            if (startOffset > 0)
            {
                ChangeEpAddr(EPaddr & 0xFFF00000);        //更改基地址
                single_bytes = 0x100000 - startOffset;
                if (weight_total_bytes < single_bytes)
                {
                    single_bytes = weight_total_bytes;
                }
                wdc_lib_decl.WDC_ReadAddrBlock(dev.Handle, bar, startOffset, single_bytes, pData, mode, options);    //offset是地址的偏移量
                Marshal.Copy(pData, data, (int)offset / 4, (int)single_bytes / 4);  //单次读取数据
                offset += single_bytes;
                weight_total_bytes -= single_bytes;
                single_bytes = 0x100000;
            }
            while (weight_total_bytes > 0)
            {
                if (weight_total_bytes < single_bytes)
                {
                    single_bytes = weight_total_bytes;
                }
                ChangeEpAddr(EPaddr + offset);        //更改基地址
                wdc_lib_decl.WDC_ReadAddrBlock(dev.Handle, bar, 0, single_bytes, pData, mode, options);    //offset是地址的偏移量
                Marshal.Copy(pData, data, (int)offset / 4, (int)single_bytes / 4);  //单次读取数据
                offset += single_bytes;
                weight_total_bytes -= single_bytes;
            }
            Marshal.FreeHGlobal(pData);

        }

        private void write_weight_addrspace(DWORD EPaddr)
        {
            //DWORD EPaddr = 0xa0000000;
            DWORD weight_total_bytes = (DWORD)LoadBin.length_q;    //传输总大小
            byte[] data = new byte[weight_total_bytes];
            //byte[] data1 = new byte[weight_total_bytes];  
            data = LoadBin.binchar_q;               //传输数据
            WriteAddrdata(EPaddr, weight_total_bytes, data);
            //ReadAddrdata(EPaddr, weight_total_bytes, data1);
            /*pcie_index = lstBxDevices.SelectedIndex;
            FT6678_YOLO_Device dev = pciDevList.Get(pcie_index);
            WDC_ADDR_MODE mode = WDC_ADDR_MODE.WDC_MODE_8;  //传输字节模式
            WDC_ADDR_RW_OPTIONS options = WDC_ADDR_RW_OPTIONS.WDC_ADDR_RW_DEFAULT;
            DWORD i = 0;
            DWORD bar = 0;                      //Bar0
            DWORD offset = 0;                  //偏移大小
       //   DWORD weight_total_bytes = (DWORD) LoadBin.length_q;    //传输总大小
            DWORD single_bytes = 0x100000;    //传输大小
            byte[] data = new byte[(DWORD)LoadBin.length_q]; ;  
            data = LoadBin.binchar_q;               //传输数据
            //发送数据     
            ChangeEpAddr(0xa0000000);  //更改基地址
            wdc_lib_decl.WDC_WriteAddrBlock(dev.Handle, bar, offset, single_bytes, data, mode, options);    //offset是地址的偏移量
            */
            Log.TraceLog("FT6678_YOLO_diag:加载权重参数...\n");

        }

        System.Timers.Timer timer = new System.Timers.Timer();
        private void PollingEvent(object sender, ElapsedEventArgs e)
        {
            FT6678_YOLO_Device dev = pciDevList.Get(pcie_index);
            DWORD bar = 0;  //Bar0
            DWORD offset = 0;   //偏移量
            DWORD total_bytes = WIDTH * HEIGHT;    //传输总大小
            byte[] data = new byte[total_bytes]; ;  //传输数据
            byte[,] twodim = new byte[WIDTH, HEIGHT];   //raw转bmp
            WDC_ADDR_MODE mode = WDC_ADDR_MODE.WDC_MODE_8;  //传输字节模式
            WDC_ADDR_RW_OPTIONS options = WDC_ADDR_RW_OPTIONS.WDC_ADDR_RW_DEFAULT;
            int i, j;
            byte flag_PCIE = 0; //PCIE标志位
            wdc_lib_decl.WDC_ReadAddr8(dev.Handle, bar, 0x50000, ref flag_PCIE);
            if (flag_PCIE == 0x02)
            {
                timer.Stop();
                //接收图像
                wdc_lib_decl.WDC_ReadAddrBlock(dev.Handle, bar, offset, total_bytes, data,
                                                mode, options);
                //转为二维数组
                for (j = 0; j < HEIGHT; j++)
                {
                    for (i = 0; i < WIDTH; i++)
                    {
                        twodim[i, j] = data[j * WIDTH + i];
                    }
                }
                Bitmap bitmap = new Bitmap((int)WIDTH, (int)HEIGHT);
                for (j = 0; j < HEIGHT; j++)
                {
                    for (i = 0; i < WIDTH; i++)
                    {
                        Color newcolor = Color.FromArgb(twodim[i, j], twodim[i, j], twodim[i, j]);
                        bitmap.SetPixel(i, j, newcolor);
                    }
                }
                Bitmap bitmapOld = pictureBox2.Image as Bitmap;
                pictureBox2.Image = bitmap;
                if (bitmapOld != null)
                {
                    bitmapOld.Dispose();
                }
                Log.TraceLog("FT6678_YOLO_diag:识别已完成\n");
            }
        }
        /*
                private void Btn_start_Click(object sender, EventArgs e)
                {
                    //byte[] param = { 00, 00, 00, 00 };
                    //int col, row, chn;
                    //选择图像并发送
                    write_addrspace();
                    int[] result_stauts = new int[15];
                    int featureSize = 0;
                    byte[] command = {0x02, 0x00, 0x57, 0x83};
        //            int[] featureNum = { 103 };

        //            WriteAddrdata(0xB0000030, 4, featureNum);
                    WriteAddrdata(0xB0000000, 4, command);            
                    result_stauts[0] = 1;
                    while (result_stauts[0] != 0)
                    {
                        ReadAddrdata(0xB0000000, 4, result_stauts);
                        for (int i = 0; i < 1000; i++ ) ;
                    }
                    ReadAddrdata(0xB0000000, 60, result_stauts);


                    featureSize = result_stauts[6];
                    BYTE[] result_data = new byte[featureSize];
                    ReadAddrdata(0xA0000000, (DWORD)featureSize, result_data);

                    string filename = "D:\\data\\data_221013\\featureData.bin";
                    FileStream fs;
                    fs = new FileStream(filename,FileMode.OpenOrCreate); //初始化FileStream对象
                    BinaryWriter bw = new BinaryWriter(fs);        //创建BinaryWriter对象
                    bw.Write(result_data);
                    bw.Close();
                    fs.Close();


                    //接收图像每200ms查询一次
                    timer.Start();
                    double timespend1 = (double)result_stauts[8] + (double)result_stauts[9] * 0x100000000;
                    string message1 = string.Format("网络配置时间/ns：{0}", timespend1);
                    double timespend2 = (double)result_stauts[10] + (double)result_stauts[11] * 0x100000000;
                    string message2 = string.Format("图像传输时间/ns：{0}", timespend2);
                    double timespend3 = (double)result_stauts[12] + (double)result_stauts[13] * 0x100000000;
                    string message3 = string.Format("网络计算时间/ns：{0}", timespend3);
                    string message4 = string.Format("status：{0}", result_stauts[4]);
                    string message5 = string.Format("errCode：{0}",result_stauts[5]);
                    Log.TraceLog(message1);
                    Log.TraceLog(message2);
                    Log.TraceLog(message3);
                    Log.TraceLog(message4);
                    Log.TraceLog(message5);

                    Log.TraceLog("FT6678_YOLO_diag:识别完成！\n");

                }
        */
        /*
        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            this.txtLog.SelectionStart = this.txtLog.Text.Length;
            this.txtLog.ScrollToCaret();
        }
        */
        public byte[] intToBytes(int value)
        {
            byte[] src = new byte[4];
            src[3] = (byte)((value >> 24) & 0xFF);
            src[2] = (byte)((value >> 16) & 0xFF);
            src[1] = (byte)((value >> 8) & 0xFF);
            src[0] = (byte)(value & 0xFF);
            return src;
        }
        

        //加载权重与图片并识别以及保存结果
        public void load_weight_Click(object sender, EventArgs e)
        {

            FileStream fs;
            /*
        OpenFileDialog file = new OpenFileDialog();
        FileStream fs;
        file.InitialDirectory = ".";
        file.Filter = "所有文件(*.*)|*.*";
        file.ShowDialog();

        if (file.FileName != string.Empty)
                */
            for (int p = 0; p < this.listView1.Items.Count; p++)
            {

                if (this.listView1.Items[p].Checked)
                {



                    if (this.listView1.Items[p].SubItems[3].Text == "conv")
                    {

                        for (int c = 1; c < 6; c++)   //卷积核循环
                        {
                            for (int j = 1; j < 3; j++)     //卷积步长循环
                            {

                                for (int h = 0; h < 2; h++)     //pad循环
                                {
                                    for (int pz = 0; pz < 2; pz++)     //偏置循环
                                    {


                                        string filename1 = "F:\\FPGA\\TOH\\data\\conv\\图例" + this.listView1.Items[p].SubItems[1].Text + "分辨率" + this.listView1.Items[p].SubItems[2].Text + "卷积核大小-" + c + "卷积步长-" + j + "pad-" + h + "偏置-" + pz + ".bin";
                                        FileStream fs2;
                                        fs2 = new FileStream(filename1, FileMode.OpenOrCreate);
                                        BinaryWriter bw1 = new BinaryWriter(fs2);        //创建BinaryWriter  二进制对象

                                        int num = 1;
                                        // 将 int 转换成字节数组
                                        byte[] intBuff = BitConverter.GetBytes(num);
                                        //转换后是四个字节，高位在高字节
                                        bw1.Write(intBuff);

                                        //////输入特征图通道数设定

                                        ushort testNum1 = 1;             //ushort-》byte  ,输入特征图通道数
                                        byte[] testByte1 = new byte[2];
                                        testByte1[1] = (byte)((0xff00 & testNum1) >> 8);
                                        testByte1[0] = (byte)(0xff & testNum1);
                                        bw1.Write(testByte1);


                                        /////输出特征图通道设定

                                        ushort testNum2 = 1;               //ushort-》byte  ，输出特征图通道数
                                        byte[] testByte2 = new byte[2];
                                        testByte2[1] = (byte)((0xff00 & testNum2) >> 8);
                                        testByte2[0] = (byte)(0xff & testNum2);
                                        bw1.Write(testByte2);



                                        //////卷积尺寸设定



                                        ushort testNum3 = (ushort)c;               //ushort-》byte   ，卷积行数
                                        byte[] testByte3 = new byte[2];
                                        testByte3[1] = (byte)((0xff00 & testNum3) >> 8);
                                        testByte3[0] = (byte)(0xff & testNum3);
                                        bw1.Write(testByte3);

                                        ushort testNum4 = (ushort)c;                  //ushort-》byte，  卷积列数
                                        byte[] testByte4 = new byte[2];
                                        testByte4[1] = (byte)((0xff00 & testNum4) >> 8);
                                        testByte4[0] = (byte)(0xff & testNum4);
                                        bw1.Write(testByte4);



                                        ////// inputLayer设定

                                        short number1 = -1;                    //short-》byte
                                        byte[] numberBytes1 = BitConverter.GetBytes(number1);
                                        bw1.Write(numberBytes1);



                                        /////// resLayer设定

                                        short number2 = -1;                    //short-》byte
                                        byte[] numberBytes2 = BitConverter.GetBytes(number2);
                                        bw1.Write(numberBytes2);





                                        //////computeType设定                

                                        char ss0 = (char)0;            //unchar-》byte
                                        bw1.Write(ss0);



                                        //////偏置设定

                                        char ss1 = (char)pz;
                                        bw1.Write(ss1);


                                        //////ReLu设定

                                        char ss2 = (char)0;
                                        bw1.Write(ss2);


                                        ////池化设定

                                        char ss3 = (char)0;
                                        bw1.Write(ss3);


                                        /////保留标志

                                        char ss4 = (char)0;
                                        bw1.Write(ss4);


                                        /////卷积步长

                                        char ss5 = (char)j;
                                        bw1.Write(ss5);


                                        /////池化步长

                                        char ss6 = (char)0;
                                        bw1.Write(ss6);


                                        /////池化尺寸


                                        char ss7 = (char)0;
                                        bw1.Write(ss7);



                                        /////cnnPadFlag;

                                        char ss8 = (char)h;
                                        bw1.Write(ss8);


                                        /////poolPadFlag

                                        char ss9 = (char)0;
                                        bw1.Write(ss9);


                                        /////truncBit

                                        char ss10 = (char)0;
                                        bw1.Write(ss10);


                                        /////upsampleStride

                                        char ss11 = (char)1;
                                        bw1.Write(ss11);


                                        /////outputFlag

                                        char ss12 = (char)1;
                                        bw1.Write(ss12);


                                        /////保留字

                                        char ss13 = (char)0;
                                        char ss14 = (char)0;
                                        char ss15 = (char)0;
                                        char ss16 = (char)0;
                                        char ss17 = (char)0;
                                        char ss18 = (char)0;
                                        char ss19 = (char)0;

                                        bw1.Write(ss13);
                                        bw1.Write(ss14);
                                        bw1.Write(ss15);
                                        bw1.Write(ss16);
                                        bw1.Write(ss17);
                                        bw1.Write(ss18);
                                        bw1.Write(ss19);

                                        for (int s = 0; s < (c * c); s++)
                                        {
                                            Random rd = new Random();
                                            byte[] a = new Byte[1];
                                            rd.NextBytes(a);
                                            //Console.WriteLine(a);
                                            bw1.Write(a);

                                        }

                                        Random r = new Random();
                                        int b = r.Next(-9999, 9999);
                                        byte[] bias = BitConverter.GetBytes(b);
                                        bw1.Write(bias);

                                        bw1.Close();
                                        fs2.Close();

                                    }
                                }
                            }
                        }



                    }

                    if (this.listView1.Items[p].SubItems[3].Text == "res")
                    {
                        string filename1 = "F:\\FPGA\\TOH\\data\\res\\图例" + this.listView1.Items[p].SubItems[1].Text + "分辨率" + this.listView1.Items[p].SubItems[2].Text + "res.bin";
                        FileStream fs2;
                        fs2 = new FileStream(filename1, FileMode.OpenOrCreate);
                        BinaryWriter bw1 = new BinaryWriter(fs2);        //创建BinaryWriter  二进制对象

                        int num = 3;
                        // 将 int 转换成字节数组
                        byte[] intBuff = BitConverter.GetBytes(num);
                        //转换后是四个字节，高位在高字节
                        bw1.Write(intBuff);

                        for (int c = 0; c < 2; c++)   //1*1卷积循环
                        {

                            //////输入特征图通道数设定

                            ushort testNum1 = 1;             //ushort-》byte  ,输入特征图通道数
                            byte[] testByte1 = new byte[2];
                            testByte1[1] = (byte)((0xff00 & testNum1) >> 8);
                            testByte1[0] = (byte)(0xff & testNum1);
                            bw1.Write(testByte1);


                            /////输出特征图通道设定

                            ushort testNum2 = 1;               //ushort-》byte  ，输出特征图通道数
                            byte[] testByte2 = new byte[2];
                            testByte2[1] = (byte)((0xff00 & testNum2) >> 8);
                            testByte2[0] = (byte)(0xff & testNum2);
                            bw1.Write(testByte2);


                            //////卷积尺寸设定

                            ushort testNum3 = (ushort)1;               //ushort-》byte   ，卷积行数
                            byte[] testByte3 = new byte[2];
                            testByte3[1] = (byte)((0xff00 & testNum3) >> 8);
                            testByte3[0] = (byte)(0xff & testNum3);
                            bw1.Write(testByte3);

                            ushort testNum4 = (ushort)1;                  //ushort-》byte，  卷积列数
                            byte[] testByte4 = new byte[2];
                            testByte4[1] = (byte)((0xff00 & testNum4) >> 8);
                            testByte4[0] = (byte)(0xff & testNum4);
                            bw1.Write(testByte4);


                            ////// inputLayer设定

                            short number1 = -1;                    //short-》byte
                            byte[] numberBytes1 = BitConverter.GetBytes(number1);
                            bw1.Write(numberBytes1);

                            /////// resLayer设定

                            short number2 = -1;                    //short-》byte
                            byte[] numberBytes2 = BitConverter.GetBytes(number2);
                            bw1.Write(numberBytes2);


                            //////computeType设定                

                            char ss0 = (char)0;            //unchar-》byte
                            bw1.Write(ss0);


                            //////偏置设定

                            char ss1 = (char)0;
                            bw1.Write(ss1);


                            //////ReLu设定

                            char ss2 = (char)0;
                            bw1.Write(ss2);


                            ////池化设定

                            char ss3 = (char)0;
                            bw1.Write(ss3);


                            /////保留标志

                            char ss4 = (char)0;
                            bw1.Write(ss4);


                            /////卷积步长

                            char ss5 = (char)1;
                            bw1.Write(ss5);


                            /////池化步长

                            char ss6 = (char)0;
                            bw1.Write(ss6);


                            /////池化尺寸


                            char ss7 = (char)0;
                            bw1.Write(ss7);



                            /////cnnPadFlag;

                            char ss8 = (char)0;
                            bw1.Write(ss8);


                            /////poolPadFlag

                            char ss9 = (char)0;
                            bw1.Write(ss9);


                            /////truncBit

                            char ss10 = (char)0;
                            bw1.Write(ss10);


                            /////upsampleStride

                            char ss11 = (char)1;
                            bw1.Write(ss11);


                            /////outputFlag

                            char ss12 = (char)0;
                            bw1.Write(ss12);


                            /////保留字

                            char ss13 = (char)0;
                            char ss14 = (char)0;
                            char ss15 = (char)0;
                            char ss16 = (char)0;
                            char ss17 = (char)0;
                            char ss18 = (char)0;
                            char ss19 = (char)0;

                            bw1.Write(ss13);
                            bw1.Write(ss14);
                            bw1.Write(ss15);
                            bw1.Write(ss16);
                            bw1.Write(ss17);
                            bw1.Write(ss18);
                            bw1.Write(ss19);
                        }


                        //////输入特征图通道数设定

                        ushort twoNum1 = 1;             //ushort-》byte  ,输入特征图通道数
                        byte[] twoByte1 = new byte[2];
                        twoByte1[1] = (byte)((0xff00 & twoNum1) >> 8);
                        twoByte1[0] = (byte)(0xff & twoNum1);
                        bw1.Write(twoByte1);


                        /////输出特征图通道设定

                        ushort twoNum2 = 1;               //ushort-》byte  ，输出特征图通道数
                        byte[] twoByte2 = new byte[2];
                        twoByte2[1] = (byte)((0xff00 & twoNum2) >> 8);
                        twoByte2[0] = (byte)(0xff & twoNum2);
                        bw1.Write(twoByte2);


                        //////卷积尺寸设定

                        ushort twoNum3 = (ushort)0;               //ushort-》byte   ，卷积行数
                        byte[] twoByte3 = new byte[2];
                        twoByte3[1] = (byte)((0xff00 & twoNum3) >> 8);
                        twoByte3[0] = (byte)(0xff & twoNum3);
                        bw1.Write(twoByte3);

                        ushort twoNum4 = (ushort)0;                  //ushort-》byte，  卷积列数
                        byte[] twoByte4 = new byte[2];
                        twoByte4[1] = (byte)((0xff00 & twoNum4) >> 8);
                        twoByte4[0] = (byte)(0xff & twoNum4);
                        bw1.Write(twoByte4);


                        ////// inputLayer设定

                        short two1 = 0;                    //short-》byte
                        byte[] restwo1 = BitConverter.GetBytes(two1);
                        bw1.Write(restwo1);

                        /////// resLayer设定

                        short two2 = 1;                    //short-》byte
                        byte[] restwo2 = BitConverter.GetBytes(two2);
                        bw1.Write(restwo2);


                        //////computeType设定                

                        char res0 = (char)1;            //unchar-》byte
                        bw1.Write(res0);


                        //////偏置设定

                        char res1 = (char)0;
                        bw1.Write(res1);


                        //////ReLu设定

                        char res2 = (char)0;
                        bw1.Write(res2);


                        ////池化设定

                        char res3 = (char)0;
                        bw1.Write(res3);


                        /////保留标志

                        char res4 = (char)0;
                        bw1.Write(res4);


                        /////卷积步长

                        char res5 = (char)0;
                        bw1.Write(res5);


                        /////池化步长

                        char res6 = (char)0;
                        bw1.Write(res6);


                        /////池化尺寸


                        char res7 = (char)0;
                        bw1.Write(res7);



                        /////cnnPadFlag;

                        char res8 = (char)0;
                        bw1.Write(res8);


                        /////poolPadFlag

                        char res9 = (char)0;
                        bw1.Write(res9);


                        /////truncBit

                        char res10 = (char)0;
                        bw1.Write(res10);


                        /////upsampleStride

                        char res11 = (char)1;
                        bw1.Write(res11);


                        /////outputFlag

                        char res12 = (char)1;
                        bw1.Write(res12);


                        /////保留字

                        char res13 = (char)0;
                        char res14 = (char)0;
                        char res15 = (char)0;
                        char res16 = (char)0;
                        char res17 = (char)0;
                        char res18 = (char)0;
                        char res19 = (char)0;

                        bw1.Write(res13);
                        bw1.Write(res14);
                        bw1.Write(res15);
                        bw1.Write(res16);
                        bw1.Write(res17);
                        bw1.Write(res18);
                        bw1.Write(res19);



                        for (int i = 0; i < 2; i++)
                        {
                            char a = (char)1;
                            bw1.Write(a);

                            int b = 0;
                            byte[] bias = BitConverter.GetBytes(b);
                            bw1.Write(bias);
                        }

                        bw1.Close();
                        fs2.Close();

                    }
                    if (this.listView1.Items[p].SubItems[3].Text == "池化")
                    {

                        for (int c = 2; c < 4; c++)   //池化大小循环
                        {
                            for (int j = 1; j < 3; j++)     //池化步长循环
                            {

                                for (int h = 0; h < 2; h++)     //pad循环
                                {

                                    string filename1 = "F:\\FPGA\\TOH\\data\\池化\\图例" + this.listView1.Items[p].SubItems[1].Text + "分辨率" + this.listView1.Items[p].SubItems[2].Text + "池化尺寸-" + c + " 池化步长-" + j + " pad-" + h + ".bin";
                                    FileStream fs2;
                                    fs2 = new FileStream(filename1, FileMode.OpenOrCreate);
                                    BinaryWriter bw1 = new BinaryWriter(fs2);        //创建BinaryWriter  二进制对象

                                    int num = 1;
                                    // 将 int 转换成字节数组
                                    byte[] intBuff = BitConverter.GetBytes(num);
                                    //转换后是四个字节，高位在高字节
                                    bw1.Write(intBuff);

                                    //////输入特征图通道数设定

                                    ushort testNum1 = 1;             //ushort-》byte  ,输入特征图通道数
                                    byte[] testByte1 = new byte[2];
                                    testByte1[1] = (byte)((0xff00 & testNum1) >> 8);
                                    testByte1[0] = (byte)(0xff & testNum1);
                                    bw1.Write(testByte1);


                                    /////输出特征图通道设定

                                    ushort testNum2 = 1;               //ushort-》byte  ，输出特征图通道数
                                    byte[] testByte2 = new byte[2];
                                    testByte2[1] = (byte)((0xff00 & testNum2) >> 8);
                                    testByte2[0] = (byte)(0xff & testNum2);
                                    bw1.Write(testByte2);



                                    //////卷积尺寸设定



                                    ushort testNum3 = (ushort)1;               //ushort-》byte   ，卷积行数
                                    byte[] testByte3 = new byte[2];
                                    testByte3[1] = (byte)((0xff00 & testNum3) >> 8);
                                    testByte3[0] = (byte)(0xff & testNum3);
                                    bw1.Write(testByte3);

                                    ushort testNum4 = (ushort)1;                  //ushort-》byte，  卷积列数
                                    byte[] testByte4 = new byte[2];
                                    testByte4[1] = (byte)((0xff00 & testNum4) >> 8);
                                    testByte4[0] = (byte)(0xff & testNum4);
                                    bw1.Write(testByte4);



                                    ////// inputLayer设定

                                    short number1 = -1;                    //short-》byte
                                    byte[] numberBytes1 = BitConverter.GetBytes(number1);
                                    bw1.Write(numberBytes1);



                                    /////// resLayer设定

                                    short number2 = -1;                    //short-》byte
                                    byte[] numberBytes2 = BitConverter.GetBytes(number2);
                                    bw1.Write(numberBytes2);





                                    //////computeType设定                

                                    char ss0 = (char)0;            //unchar-》byte
                                    bw1.Write(ss0);



                                    //////偏置设定

                                    char ss1 = (char)0;
                                    bw1.Write(ss1);


                                    //////ReLu设定

                                    char ss2 = (char)0;
                                    bw1.Write(ss2);


                                    ////池化设定

                                    char ss3 = (char)1;
                                    bw1.Write(ss3);


                                    /////保留标志

                                    char ss4 = (char)0;
                                    bw1.Write(ss4);


                                    /////卷积步长

                                    char ss5 = (char)1;
                                    bw1.Write(ss5);


                                    /////池化步长

                                    char ss6 = (char)j;
                                    bw1.Write(ss6);


                                    /////池化尺寸


                                    char ss7 = (char)c;
                                    bw1.Write(ss7);



                                    /////cnnPadFlag;

                                    char ss8 = (char)0;
                                    bw1.Write(ss8);


                                    /////poolPadFlag

                                    char ss9 = (char)h;
                                    bw1.Write(ss9);


                                    /////truncBit

                                    char ss10 = (char)0;
                                    bw1.Write(ss10);


                                    /////upsampleStride

                                    char ss11 = (char)1;
                                    bw1.Write(ss11);


                                    /////outputFlag

                                    char ss12 = (char)1;
                                    bw1.Write(ss12);


                                    /////保留字

                                    char ss13 = (char)0;
                                    char ss14 = (char)0;
                                    char ss15 = (char)0;
                                    char ss16 = (char)0;
                                    char ss17 = (char)0;
                                    char ss18 = (char)0;
                                    char ss19 = (char)0;

                                    bw1.Write(ss13);
                                    bw1.Write(ss14);
                                    bw1.Write(ss15);
                                    bw1.Write(ss16);
                                    bw1.Write(ss17);
                                    bw1.Write(ss18);
                                    bw1.Write(ss19);




                                    char a = (char)1;
                                    bw1.Write(a);




                                    int b = 0;
                                    byte[] bias = BitConverter.GetBytes(b);
                                    bw1.Write(bias);

                                    bw1.Close();
                                    fs2.Close();


                                }
                            }
                        }
                    }
                    if (this.listView1.Items[p].SubItems[3].Text == "激活")
                    {
                        for (int c = 0; c < 5; c++)   //激活大小循环
                        {


                            string filename1 = "F:\\FPGA\\TOH\\data\\激活\\图例" + this.listView1.Items[p].SubItems[1].Text + "分辨率" + this.listView1.Items[p].SubItems[2].Text + "激活类型" + c + ".bin";
                            FileStream fs2;
                            fs2 = new FileStream(filename1, FileMode.OpenOrCreate);
                            BinaryWriter bw1 = new BinaryWriter(fs2);        //创建BinaryWriter  二进制对象

                            int num = 1;
                            // 将 int 转换成字节数组
                            byte[] intBuff = BitConverter.GetBytes(num);
                            //转换后是四个字节，高位在高字节
                            bw1.Write(intBuff);

                            //////输入特征图通道数设定

                            ushort testNum1 = 1;             //ushort-》byte  ,输入特征图通道数
                            byte[] testByte1 = new byte[2];
                            testByte1[1] = (byte)((0xff00 & testNum1) >> 8);
                            testByte1[0] = (byte)(0xff & testNum1);
                            bw1.Write(testByte1);


                            /////输出特征图通道设定

                            ushort testNum2 = 1;               //ushort-》byte  ，输出特征图通道数
                            byte[] testByte2 = new byte[2];
                            testByte2[1] = (byte)((0xff00 & testNum2) >> 8);
                            testByte2[0] = (byte)(0xff & testNum2);
                            bw1.Write(testByte2);



                            //////卷积尺寸设定



                            ushort testNum3 = (ushort)1;               //ushort-》byte   ，卷积行数
                            byte[] testByte3 = new byte[2];
                            testByte3[1] = (byte)((0xff00 & testNum3) >> 8);
                            testByte3[0] = (byte)(0xff & testNum3);
                            bw1.Write(testByte3);

                            ushort testNum4 = (ushort)1;                  //ushort-》byte，  卷积列数
                            byte[] testByte4 = new byte[2];
                            testByte4[1] = (byte)((0xff00 & testNum4) >> 8);
                            testByte4[0] = (byte)(0xff & testNum4);
                            bw1.Write(testByte4);



                            ////// inputLayer设定

                            short number1 = -1;                    //short-》byte
                            byte[] numberBytes1 = BitConverter.GetBytes(number1);
                            bw1.Write(numberBytes1);



                            /////// resLayer设定

                            short number2 = -1;                    //short-》byte
                            byte[] numberBytes2 = BitConverter.GetBytes(number2);
                            bw1.Write(numberBytes2);





                            //////computeType设定                

                            char ss0 = (char)0;            //unchar-》byte
                            bw1.Write(ss0);



                            //////偏置设定

                            char ss1 = (char)0;
                            bw1.Write(ss1);


                            //////ReLu设定

                            char ss2 = (char)c;
                            bw1.Write(ss2);


                            ////池化设定

                            char ss3 = (char)0;
                            bw1.Write(ss3);


                            /////保留标志

                            char ss4 = (char)0;
                            bw1.Write(ss4);


                            /////卷积步长

                            char ss5 = (char)1;
                            bw1.Write(ss5);


                            /////池化步长

                            char ss6 = (char)0;
                            bw1.Write(ss6);


                            /////池化尺寸


                            char ss7 = (char)0;
                            bw1.Write(ss7);



                            /////cnnPadFlag;

                            char ss8 = (char)0;
                            bw1.Write(ss8);


                            /////poolPadFlag

                            char ss9 = (char)0;
                            bw1.Write(ss9);


                            /////truncBit

                            char ss10 = (char)0;
                            bw1.Write(ss10);


                            /////upsampleStride

                            char ss11 = (char)1;
                            bw1.Write(ss11);


                            /////outputFlag

                            char ss12 = (char)1;
                            bw1.Write(ss12);


                            /////保留字

                            char ss13 = (char)0;
                            char ss14 = (char)0;
                            char ss15 = (char)0;
                            char ss16 = (char)0;
                            char ss17 = (char)0;
                            char ss18 = (char)0;
                            char ss19 = (char)0;

                            bw1.Write(ss13);
                            bw1.Write(ss14);
                            bw1.Write(ss15);
                            bw1.Write(ss16);
                            bw1.Write(ss17);
                            bw1.Write(ss18);
                            bw1.Write(ss19);




                            char a = (char)1;
                            bw1.Write(a);




                            int b = 0;
                            byte[] bias = BitConverter.GetBytes(b);
                            bw1.Write(bias);

                            bw1.Close();
                            fs2.Close();



                        }
                    }
                    if (this.listView1.Items[p].SubItems[3].Text == "串联")
                    {
                        string filename1 = "F:\\FPGA\\TOH\\data\\串联\\图例" + this.listView1.Items[p].SubItems[1].Text + "分辨率" + this.listView1.Items[p].SubItems[2].Text + "cat.bin";
                        FileStream fs2;
                        fs2 = new FileStream(filename1, FileMode.OpenOrCreate);
                        BinaryWriter bw1 = new BinaryWriter(fs2);        //创建BinaryWriter  二进制对象

                        int num = 3;
                        // 将 int 转换成字节数组
                        byte[] intBuff = BitConverter.GetBytes(num);
                        //转换后是四个字节，高位在高字节
                        bw1.Write(intBuff);

                        for (int c = 0; c < 2; c++)   //1*1卷积循环
                        {

                            //////输入特征图通道数设定

                            ushort testNum1 = 1;             //ushort-》byte  ,输入特征图通道数
                            byte[] testByte1 = new byte[2];
                            testByte1[1] = (byte)((0xff00 & testNum1) >> 8);
                            testByte1[0] = (byte)(0xff & testNum1);
                            bw1.Write(testByte1);


                            /////输出特征图通道设定

                            ushort testNum2 = 1;               //ushort-》byte  ，输出特征图通道数
                            byte[] testByte2 = new byte[2];
                            testByte2[1] = (byte)((0xff00 & testNum2) >> 8);
                            testByte2[0] = (byte)(0xff & testNum2);
                            bw1.Write(testByte2);


                            //////卷积尺寸设定

                            ushort testNum3 = (ushort)1;               //ushort-》byte   ，卷积行数
                            byte[] testByte3 = new byte[2];
                            testByte3[1] = (byte)((0xff00 & testNum3) >> 8);
                            testByte3[0] = (byte)(0xff & testNum3);
                            bw1.Write(testByte3);

                            ushort testNum4 = (ushort)1;                  //ushort-》byte，  卷积列数
                            byte[] testByte4 = new byte[2];
                            testByte4[1] = (byte)((0xff00 & testNum4) >> 8);
                            testByte4[0] = (byte)(0xff & testNum4);
                            bw1.Write(testByte4);


                            ////// inputLayer设定

                            short number1 = -1;                    //short-》byte
                            byte[] numberBytes1 = BitConverter.GetBytes(number1);
                            bw1.Write(numberBytes1);

                            /////// resLayer设定

                            short number2 = -1;                    //short-》byte
                            byte[] numberBytes2 = BitConverter.GetBytes(number2);
                            bw1.Write(numberBytes2);


                            //////computeType设定                

                            char ss0 = (char)0;            //unchar-》byte
                            bw1.Write(ss0);


                            //////偏置设定

                            char ss1 = (char)0;
                            bw1.Write(ss1);


                            //////ReLu设定

                            char ss2 = (char)0;
                            bw1.Write(ss2);


                            ////池化设定

                            char ss3 = (char)0;
                            bw1.Write(ss3);


                            /////保留标志

                            char ss4 = (char)0;
                            bw1.Write(ss4);


                            /////卷积步长

                            char ss5 = (char)1;
                            bw1.Write(ss5);


                            /////池化步长

                            char ss6 = (char)0;
                            bw1.Write(ss6);


                            /////池化尺寸


                            char ss7 = (char)0;
                            bw1.Write(ss7);



                            /////cnnPadFlag;

                            char ss8 = (char)0;
                            bw1.Write(ss8);


                            /////poolPadFlag

                            char ss9 = (char)0;
                            bw1.Write(ss9);


                            /////truncBit

                            char ss10 = (char)0;
                            bw1.Write(ss10);


                            /////upsampleStride

                            char ss11 = (char)1;
                            bw1.Write(ss11);


                            /////outputFlag

                            char ss12 = (char)0;
                            bw1.Write(ss12);


                            /////保留字

                            char ss13 = (char)0;
                            char ss14 = (char)0;
                            char ss15 = (char)0;
                            char ss16 = (char)0;
                            char ss17 = (char)0;
                            char ss18 = (char)0;
                            char ss19 = (char)0;

                            bw1.Write(ss13);
                            bw1.Write(ss14);
                            bw1.Write(ss15);
                            bw1.Write(ss16);
                            bw1.Write(ss17);
                            bw1.Write(ss18);
                            bw1.Write(ss19);
                        }


                        //////输入特征图通道数设定

                        ushort twoNum1 = 1;             //ushort-》byte  ,输入特征图通道数
                        byte[] twoByte1 = new byte[2];
                        twoByte1[1] = (byte)((0xff00 & twoNum1) >> 8);
                        twoByte1[0] = (byte)(0xff & twoNum1);
                        bw1.Write(twoByte1);


                        /////输出特征图通道设定

                        ushort twoNum2 = 1;               //ushort-》byte  ，输出特征图通道数
                        byte[] twoByte2 = new byte[2];
                        twoByte2[1] = (byte)((0xff00 & twoNum2) >> 8);
                        twoByte2[0] = (byte)(0xff & twoNum2);
                        bw1.Write(twoByte2);


                        //////卷积尺寸设定

                        ushort twoNum3 = (ushort)0;               //ushort-》byte   ，卷积行数
                        byte[] twoByte3 = new byte[2];
                        twoByte3[1] = (byte)((0xff00 & twoNum3) >> 8);
                        twoByte3[0] = (byte)(0xff & twoNum3);
                        bw1.Write(twoByte3);

                        ushort twoNum4 = (ushort)0;                  //ushort-》byte，  卷积列数
                        byte[] twoByte4 = new byte[2];
                        twoByte4[1] = (byte)((0xff00 & twoNum4) >> 8);
                        twoByte4[0] = (byte)(0xff & twoNum4);
                        bw1.Write(twoByte4);


                        ////// inputLayer设定

                        short two1 = 0;                    //short-》byte
                        byte[] restwo1 = BitConverter.GetBytes(two1);
                        bw1.Write(restwo1);

                        /////// resLayer设定

                        short two2 = 1;                    //short-》byte
                        byte[] restwo2 = BitConverter.GetBytes(two2);
                        bw1.Write(restwo2);


                        //////computeType设定                

                        char res0 = (char)2;            //unchar-》byte
                        bw1.Write(res0);


                        //////偏置设定

                        char res1 = (char)0;
                        bw1.Write(res1);


                        //////ReLu设定

                        char res2 = (char)0;
                        bw1.Write(res2);


                        ////池化设定

                        char res3 = (char)0;
                        bw1.Write(res3);


                        /////保留标志

                        char res4 = (char)0;
                        bw1.Write(res4);


                        /////卷积步长

                        char res5 = (char)0;
                        bw1.Write(res5);


                        /////池化步长

                        char res6 = (char)0;
                        bw1.Write(res6);


                        /////池化尺寸


                        char res7 = (char)0;
                        bw1.Write(res7);



                        /////cnnPadFlag;

                        char res8 = (char)0;
                        bw1.Write(res8);


                        /////poolPadFlag

                        char res9 = (char)0;
                        bw1.Write(res9);


                        /////truncBit

                        char res10 = (char)0;
                        bw1.Write(res10);


                        /////upsampleStride

                        char res11 = (char)1;
                        bw1.Write(res11);


                        /////outputFlag

                        char res12 = (char)1;
                        bw1.Write(res12);


                        /////保留字

                        char res13 = (char)0;
                        char res14 = (char)0;
                        char res15 = (char)0;
                        char res16 = (char)0;
                        char res17 = (char)0;
                        char res18 = (char)0;
                        char res19 = (char)0;

                        bw1.Write(res13);
                        bw1.Write(res14);
                        bw1.Write(res15);
                        bw1.Write(res16);
                        bw1.Write(res17);
                        bw1.Write(res18);
                        bw1.Write(res19);



                        for (int i = 0; i < 2; i++)
                        {
                            char a = (char)1;
                            bw1.Write(a);

                            int b = 0;
                            byte[] bias = BitConverter.GetBytes(b);
                            bw1.Write(bias);
                        }

                        bw1.Close();
                        fs2.Close();
                    }
                    if (this.listView1.Items[p].SubItems[3].Text == "上采样")
                    {
                        for (int c = 1; c < 3; c++)   //上采样循环
                        {
                            string filename1 = "F:\\FPGA\\TOH\\data\\上采样\\图例" + this.listView1.Items[p].SubItems[1].Text + "分辨率" + this.listView1.Items[p].SubItems[2].Text + "上采样倍数" + c + ".bin";
                            FileStream fs2;
                            fs2 = new FileStream(filename1, FileMode.OpenOrCreate);
                            BinaryWriter bw1 = new BinaryWriter(fs2);        //创建BinaryWriter  二进制对象

                            int num = 2;
                            // 将 int 转换成字节数组
                            byte[] intBuff = BitConverter.GetBytes(num);
                            //转换后是四个字节，高位在高字节
                            bw1.Write(intBuff);
                            //////输入特征图通道数设定

                            ushort testNum1 = 1;             //ushort-》byte  ,输入特征图通道数
                            byte[] testByte1 = new byte[2];
                            testByte1[1] = (byte)((0xff00 & testNum1) >> 8);
                            testByte1[0] = (byte)(0xff & testNum1);
                            bw1.Write(testByte1);


                            /////输出特征图通道设定

                            ushort testNum2 = 1;               //ushort-》byte  ，输出特征图通道数
                            byte[] testByte2 = new byte[2];
                            testByte2[1] = (byte)((0xff00 & testNum2) >> 8);
                            testByte2[0] = (byte)(0xff & testNum2);
                            bw1.Write(testByte2);


                            //////卷积尺寸设定

                            ushort testNum3 = (ushort)1;               //ushort-》byte   ，卷积行数
                            byte[] testByte3 = new byte[2];
                            testByte3[1] = (byte)((0xff00 & testNum3) >> 8);
                            testByte3[0] = (byte)(0xff & testNum3);
                            bw1.Write(testByte3);

                            ushort testNum4 = (ushort)1;                  //ushort-》byte，  卷积列数
                            byte[] testByte4 = new byte[2];
                            testByte4[1] = (byte)((0xff00 & testNum4) >> 8);
                            testByte4[0] = (byte)(0xff & testNum4);
                            bw1.Write(testByte4);


                            ////// inputLayer设定

                            short number1 = -1;                    //short-》byte
                            byte[] numberBytes1 = BitConverter.GetBytes(number1);
                            bw1.Write(numberBytes1);

                            /////// resLayer设定

                            short number2 = -1;                    //short-》byte
                            byte[] numberBytes2 = BitConverter.GetBytes(number2);
                            bw1.Write(numberBytes2);


                            //////computeType设定                

                            char ss0 = (char)0;            //unchar-》byte
                            bw1.Write(ss0);


                            //////偏置设定

                            char ss1 = (char)0;
                            bw1.Write(ss1);


                            //////ReLu设定

                            char ss2 = (char)0;
                            bw1.Write(ss2);


                            ////池化设定

                            char ss3 = (char)0;
                            bw1.Write(ss3);


                            /////保留标志

                            char ss4 = (char)0;
                            bw1.Write(ss4);


                            /////卷积步长

                            char ss5 = (char)1;
                            bw1.Write(ss5);


                            /////池化步长

                            char ss6 = (char)0;
                            bw1.Write(ss6);


                            /////池化尺寸


                            char ss7 = (char)0;
                            bw1.Write(ss7);



                            /////cnnPadFlag;

                            char ss8 = (char)0;
                            bw1.Write(ss8);


                            /////poolPadFlag

                            char ss9 = (char)0;
                            bw1.Write(ss9);


                            /////truncBit

                            char ss10 = (char)0;
                            bw1.Write(ss10);


                            /////upsampleStride

                            char ss11 = (char)1;
                            bw1.Write(ss11);


                            /////outputFlag

                            char ss12 = (char)0;
                            bw1.Write(ss12);


                            /////保留字

                            char ss13 = (char)0;
                            char ss14 = (char)0;
                            char ss15 = (char)0;
                            char ss16 = (char)0;
                            char ss17 = (char)0;
                            char ss18 = (char)0;
                            char ss19 = (char)0;

                            bw1.Write(ss13);
                            bw1.Write(ss14);
                            bw1.Write(ss15);
                            bw1.Write(ss16);
                            bw1.Write(ss17);
                            bw1.Write(ss18);
                            bw1.Write(ss19);



                            //////输入特征图通道数设定

                            ushort twoNum1 = 1;             //ushort-》byte  ,输入特征图通道数
                            byte[] twoByte1 = new byte[2];
                            twoByte1[1] = (byte)((0xff00 & twoNum1) >> 8);
                            twoByte1[0] = (byte)(0xff & twoNum1);
                            bw1.Write(twoByte1);


                            /////输出特征图通道设定

                            ushort twoNum2 = 1;               //ushort-》byte  ，输出特征图通道数
                            byte[] twoByte2 = new byte[2];
                            twoByte2[1] = (byte)((0xff00 & twoNum2) >> 8);
                            twoByte2[0] = (byte)(0xff & twoNum2);
                            bw1.Write(twoByte2);


                            //////卷积尺寸设定

                            ushort twoNum3 = (ushort)0;               //ushort-》byte   ，卷积行数
                            byte[] twoByte3 = new byte[2];
                            twoByte3[1] = (byte)((0xff00 & twoNum3) >> 8);
                            twoByte3[0] = (byte)(0xff & twoNum3);
                            bw1.Write(twoByte3);

                            ushort twoNum4 = (ushort)0;                  //ushort-》byte，  卷积列数
                            byte[] twoByte4 = new byte[2];
                            twoByte4[1] = (byte)((0xff00 & twoNum4) >> 8);
                            twoByte4[0] = (byte)(0xff & twoNum4);
                            bw1.Write(twoByte4);


                            ////// inputLayer设定

                            short two1 = -1;                    //short-》byte
                            byte[] restwo1 = BitConverter.GetBytes(two1);
                            bw1.Write(restwo1);

                            /////// resLayer设定

                            short two2 = -1;                    //short-》byte
                            byte[] restwo2 = BitConverter.GetBytes(two2);
                            bw1.Write(restwo2);


                            //////computeType设定                

                            char res0 = (char)3;            //unchar-》byte
                            bw1.Write(res0);


                            //////偏置设定

                            char res1 = (char)0;
                            bw1.Write(res1);


                            //////ReLu设定

                            char res2 = (char)0;
                            bw1.Write(res2);


                            ////池化设定

                            char res3 = (char)0;
                            bw1.Write(res3);


                            /////保留标志

                            char res4 = (char)0;
                            bw1.Write(res4);


                            /////卷积步长

                            char res5 = (char)0;
                            bw1.Write(res5);


                            /////池化步长

                            char res6 = (char)0;
                            bw1.Write(res6);


                            /////池化尺寸


                            char res7 = (char)0;
                            bw1.Write(res7);



                            /////cnnPadFlag;

                            char res8 = (char)0;
                            bw1.Write(res8);


                            /////poolPadFlag

                            char res9 = (char)0;
                            bw1.Write(res9);


                            /////truncBit

                            char res10 = (char)0;
                            bw1.Write(res10);


                            /////upsampleStride
                            //char d = (char)(((float)c)/4 + 1);
                            char res11 = (char)c;
                            bw1.Write(res11);


                            /////outputFlag

                            char res12 = (char)1;
                            bw1.Write(res12);


                            /////保留字

                            char res13 = (char)0;
                            char res14 = (char)0;
                            char res15 = (char)0;
                            char res16 = (char)0;
                            char res17 = (char)0;
                            char res18 = (char)0;
                            char res19 = (char)0;

                            bw1.Write(res13);
                            bw1.Write(res14);
                            bw1.Write(res15);
                            bw1.Write(res16);
                            bw1.Write(res17);
                            bw1.Write(res18);
                            bw1.Write(res19);




                            char a = (char)1;
                            bw1.Write(a);

                            int b = 0;
                            byte[] bias = BitConverter.GetBytes(b);
                            bw1.Write(bias);


                            bw1.Close();
                            fs2.Close();
                        }

                    }







                    {
                        FileInfo[] files = null;

                        if (this.listView1.Items[p].SubItems[3].Text == "conv")
                        {
                            DirectoryInfo conv = new DirectoryInfo("F:\\FPGA\\TOH\\data\\conv");
                            files = conv.GetFiles("*.*");
                        }

                        if (this.listView1.Items[p].SubItems[3].Text == "池化")
                        {
                            DirectoryInfo conv = new DirectoryInfo("F:\\FPGA\\TOH\\data\\conv");
                            files = conv.GetFiles("*.*");
                        }
                        if (this.listView1.Items[p].SubItems[3].Text == "激活")
                        {
                            DirectoryInfo conv = new DirectoryInfo("F:\\FPGA\\TOH\\data\\池化");
                            files = conv.GetFiles("*.*");
                        }
                        if (this.listView1.Items[p].SubItems[3].Text == "res")
                        {
                            DirectoryInfo conv = new DirectoryInfo("F:\\FPGA\\TOH\\data\\res");
                            files = conv.GetFiles("*.*");
                        }
                        if (this.listView1.Items[p].SubItems[3].Text == "串联")
                        {
                            DirectoryInfo conv = new DirectoryInfo("F:\\FPGA\\TOH\\data\\串联");
                            files = conv.GetFiles("*.*");
                        }
                        if (this.listView1.Items[p].SubItems[3].Text == "上采样")
                        {
                            DirectoryInfo conv = new DirectoryInfo("F:\\FPGA\\TOH\\data\\上采样");
                            files = conv.GetFiles("*.*");
                        }

                        for (int f = 0; f < files.Length; f++)
                        {

                            pathname = "F:\\FPGA\\TOH\\data\\" + this.listView1.Items[p].SubItems[3].Text + "\\" + files[f];

                            // Console.WriteLine(pathname);

                            //    {
                            //        byte[] binchar = new byte[] { };
                            //        // pathname = file.FileName;   //获得文件的绝对路径


                            //        fs = new FileStream(pathname, FileMode.Open, FileAccess.Read);
                            //        BinaryReader binreader = new BinaryReader(fs);
                            //        int file_len = (int)fs.Length;//获取bin文件长度
                            //        StringBuilder str = new StringBuilder();
                            //        binchar = binreader.ReadBytes(file_len);
                            //        fs.Close();
                            //        binreader.Close();
                            //        //选择权重并发送
                            //        DWORD EPaddr = 0xC0000000;
                            //        //write_weight_addrspace(EPaddr);
                            //        WriteAddrdata(EPaddr, (DWORD)file_len, binchar);
                            //        byte[] command = { 0x01, 0x00, 0x57, 0x83 };
                            //        WriteAddrdata(0xb0000000, 4, command);
                            //        while (command[0] != 0)
                            //        {
                            //            ReadAddrdata(0xb0000000, 4, command);

                            //            int i = 0;
                            //            for (i = 0; i < 1000; i++) ;
                            //        }

                            //        //从6678DDR中回读权重数据并保存
                            //        /*   BYTE[] return_data = new byte[(DWORD)file_len];
                            //           ReadAddrdata(EPaddr, (DWORD)file_len, return_data);
                            //           FileStream fw = new FileStream("D:\\returnData.bin", FileMode.OpenOrCreate); //初始化FileStream对象
                            //           BinaryWriter bw = new BinaryWriter(fw);        //创建BinaryWriter对象
                            //           bw.Write(return_data);
                            //           bw.Close();
                            //           fw.Close();*/
                            //    }  //权重加载以及验证

                            //    //byte[] image_para1 = { 0x80, 0x02， 0x00， 0x00 };
                            //    byte[] image_para = { 0x0, 0x0, 0x0, 0x0 };
                            //    WriteAddrdata(0xb0000004, 4, image_para);
                            //    WriteAddrdata(0xb0000008, 4, image_para);
                            //    WriteAddrdata(0xb000000C, 4, image_para);

                            //    //接收权重系数每200ms查询一次
                            //    timer.Start();
                            //    int[] result_stauts = new int[15];
                            //    result_stauts[0] = 1;
                            //    while (result_stauts[0] != 0)
                            //    {
                            //        ReadAddrdata(0xB0000000, 60, result_stauts);
                            //        for (int i = 0; i < 1000; i++) ;
                            //    }
                            //    double timespend = (double)result_stauts[8] + (double)result_stauts[9] * 0x100000000;
                            //    string message1 = string.Format("status：{0}", result_stauts[4]);
                            //    string message2 = string.Format("errCode：{0}", result_stauts[5]);
                            //    string message3 = string.Format("网络初始化时间/ns：{0}", timespend);
                            //    Log.TraceLog(message1);
                            //    Log.TraceLog(message2);
                            //    Log.TraceLog(message3);

                            //    Log.TraceLog("FT6678_YOLO_diag:权重参数加载完成\n");
                            //}


                            //{
                            //    ///////////////////////////////////////////加载图像并开始识别  
                            //    //byte[] param = { 00, 00, 00, 00 };
                            //    //int col, row, chn;
                            //    //选择图像并发送
                            //    write_addrspace();
                            //    int[] result_stauts = new int[15];
                            //    int featureSize = 0;
                            //    byte[] command = { 0x02, 0x00, 0x57, 0x83 };
                            //    //            int[] featureNum = { 103 };

                            //    //            WriteAddrdata(0xB0000030, 4, featureNum);
                            //    WriteAddrdata(0xB0000000, 4, command);
                            //    result_stauts[0] = 1;
                            //    while (result_stauts[0] != 0)
                            //    {
                            //        ReadAddrdata(0xB0000000, 4, result_stauts);
                            //        for (int i = 0; i < 1000; i++) ;
                            //    }
                            //    ReadAddrdata(0xB0000000, 60, result_stauts);


                            //    featureSize = result_stauts[6];
                            //    BYTE[] result_data = new byte[featureSize];
                            //    ReadAddrdata(0xA0000000, (DWORD)featureSize, result_data);

                            //    string filename = "E:\\FPGA\\TOH\\data\\featureData.bin";
                            //    FileStream fs1;
                            //    fs1 = new FileStream(filename, FileMode.OpenOrCreate); //初始化FileStream对象
                            //    BinaryWriter bw = new BinaryWriter(fs1);        //创建BinaryWriter对象
                            //    bw.Write(result_data);
                            //    bw.Close();
                            //    fs.Close();


                            //    //接收图像每200ms查询一次
                            //    timer.Start();
                            //    double timespend1 = (double)result_stauts[8] + (double)result_stauts[9] * 0x100000000;
                            //    string message1 = string.Format("网络配置时间/ns：{0}", timespend1);
                            //    double timespend2 = (double)result_stauts[10] + (double)result_stauts[11] * 0x100000000;
                            //    string message2 = string.Format("图像传输时间/ns：{0}", timespend2);
                            //    double timespend3 = (double)result_stauts[12] + (double)result_stauts[13] * 0x100000000;
                            //    string message3 = string.Format("网络计算时间/ns：{0}", timespend3);
                            //    string message4 = string.Format("status：{0}", result_stauts[4]);
                            //    string message5 = string.Format("errCode：{0}", result_stauts[5]);
                            //    Log.TraceLog(message1);
                            //    Log.TraceLog(message2);
                            //    Log.TraceLog(message3);
                            //    Log.TraceLog(message4);
                            //    Log.TraceLog(message5);

                            //    Log.TraceLog("FT6678_YOLO_diag:识别完成！\n");

                            //}




                            //{
                            //    /////////////////////结果保存
                            //    SaveFileDialog save = new SaveFileDialog();
                            //    save.Filter = "所有文件(*.*)|*.*";
                            //    save.ShowDialog();
                            //    if (save.FileName != string.Empty)
                            //    {
                            //        pictureBox2.Image.Save(save.FileName);
                            //        Log.TraceLog("FT6678_YOLO_diag:识别结果已保存\n");
                            //    }

                            //}



                        }

                        UISymbolButton bt = new UISymbolButton();
                        this.listView1.Items[p].SubItems[4].Text = "已完成";
                        bt.Cursor = System.Windows.Forms.Cursors.Hand;
                        bt.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                        bt.Location = new System.Drawing.Point(this.listView1.Items[p].SubItems[5].Bounds.Left+40, this.listView1.Items[p].SubItems[5].Bounds.Top);
                        bt.MinimumSize = new System.Drawing.Size(1, 1);
                        bt.Name = "bt";
                        bt.Size = new System.Drawing.Size(110, 20);
                        bt.TabIndex = 8;
                        bt.Text = "查看日志";
                        bt.Style = Sunny.UI.UIStyle.Green;
                        bt.StyleCustomMode = true;
                        bt.Symbol = 61530;
                        bt.Click += new System.EventHandler(bt_Click);
                        listView1.Controls.Add(bt);
                    }
















                }

            }


        }


        private void button1_Click(object sender, EventArgs e)
        {
            Log.TraceLog("FT6678_YOLO_diag:开始获取指定层特征......\n");
            UInt32 FeatureNum = 0;
            DWORD Bytes = 4;    //单次读写字节数
            string str = diag_lib.PadBuffer(textBoxfeature.Text, (DWORD)textBoxfeature.Text.Length, (DWORD)2 * Bytes);
            FeatureNum = Convert.ToUInt32(str.Substring(0, textBoxfeature.Text.Length), 16) - 1;
            BYTE[] NumFeature = new byte[(DWORD)2 * Bytes];
            bool ok = ConvertIntToByteArray(FeatureNum, ref NumFeature);
            WriteAddrdata(0xb0000004, 8, (byte[])NumFeature);
            byte[] command = { 0x03, 0x00, 0x57, 0x83 };
            WriteAddrdata(0xb0000000, 4, command);

            int featureSize = 0;
            int[] result_stauts = new int[15];
            result_stauts[0] = 1;
            while (result_stauts[0] != 0)
            {
                ReadAddrdata(0xB0000000, 60, result_stauts);
                for (int i = 0; i < 1000; i++) ;
            }
            featureSize = result_stauts[6];
            ////


            ////
            BYTE[] result_data = new byte[featureSize];
            ReadAddrdata(0xA0000000, (DWORD)featureSize, result_data);

            string filename = "D:\\data\\data_221013\\abc.bin";
            FileStream fs;
            fs = new FileStream(filename, FileMode.OpenOrCreate); //初始化FileStream对象
            BinaryWriter bw = new BinaryWriter(fs);        //创建BinaryWriter对象
            bw.Write(result_data);
            bw.Close();
            fs.Close();

            //接收图像每200ms查询一次
            timer.Start();
            double timespend = (double)result_stauts[8] + (double)result_stauts[9] * 0x100000000;
            //  string message1 = string.Format("status：{0}", result_stauts[4]);
            //  string message2 = string.Format("errCode：{0}", result_stauts[5]);
            string message3 = string.Format("特征传输时间/ns：{0}", timespend);
            //  Log.TraceLog(message1);
            //  Log.TraceLog(message2);
            Log.TraceLog(message3);
            Log.TraceLog("FT6678_YOLO_diag:已获取指定层特征！\n");

        }

        private void button2_Click(object sender, EventArgs e)
        {

            int[] result_stauts = new int[15];
            OpenFileDialog file = new OpenFileDialog();
            FileStream fs;
            file.InitialDirectory = ".";
            file.Filter = "所有文件(*.*)|*.*";
            file.ShowDialog();
            if (file.FileName != string.Empty)
            {
                byte[] binchar = new byte[] { };
                pathname = file.FileName;   //获得文件的绝对路径
                fs = new FileStream(pathname, FileMode.Open, FileAccess.Read);
                BinaryReader binreader = new BinaryReader(fs);
                int file_len = (int)fs.Length;//获取bin文件长度
                StringBuilder str = new StringBuilder();
                binchar = binreader.ReadBytes(file_len);
                fs.Close();
                binreader.Close();
                //选择权重并发送
                DWORD EPaddr = 0xC000400;//在0xc000400地址烧写程序
                WriteAddrdata(EPaddr, (DWORD)file_len, binchar);
                result_stauts[0] = 1;
                byte[] command = { 0x04, 0x00, 0x57, 0x83 };
                WriteAddrdata(0xb0000000, 4, command);
                while (result_stauts[0] != 0)
                {
                    ReadAddrdata(0xB0000000, 60, result_stauts);
                    for (int i = 0; i < 1000; i++) ;
                }
            }
            string message1 = string.Format("status：{0}", result_stauts[4]);
            Log.TraceLog(message1);
            Log.TraceLog("FT6678_YOLO_diag:程序烧写完成！\n");
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void menuAddrSpaces_Click(object sender, EventArgs e)
        {

        }

        private void menuRTRegs_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBoxRC_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxfeature_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void tjslButton1_Click(object sender, EventArgs e)
        {

        }




        private void uiTableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void uiNavMenu1_MenuItemClick(TreeNode node, NavMenuItem item, int pageIndex)
        {

        }

        private void uiSplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }
        private void uiButton1_Click(object sender, System.EventArgs e)  //用例的全选
        {
            uiCheckBoxGroup1.SelectAll();
        }
        private void uiButton4_Click(object sender, System.EventArgs e)  //用例的全不选
        {
            uiCheckBoxGroup1.UnSelectAll();
        }
        private void uiButton5_Click(object sender, System.EventArgs e)  //卷积核的全选
        {
            uiCheckBoxGroup2.SelectAll();
        }
        private void uiButton6_Click(object sender, System.EventArgs e)  //卷积核的全不选
        {
            uiCheckBoxGroup2.UnSelectAll();
        }











        private void uiSymbolButton1_Click(object sender, EventArgs e)   //添加实例


        {
            xuanzetupian x = new xuanzetupian(this);
            x.Show();













            /*
            UIEditForm uIEditForm1 = new UIEditForm();
            uIEditForm1.Render();
            uIEditForm1.ShowDialogWithMask();


            if (uIEditForm1.IsOK)
            {
                int m;
                if (this.listView1.Items.Count == 0)
                {
                    m = 0;
                }
                else
                {
                    m = this.listView1.Items.Count;
                }
                for (int cl = 0; cl < this.uiCheckBoxGroup2.SelectedItems.Count; cl++)   //测试用例循环
                {

                    for (int jh = 0; jh < this.uiCheckBoxGroup1.SelectedItems.Count; jh++)     //卷积核循环
                    {

                        for (int jb = 0; jb < this.uiCheckBoxGroup3.SelectedItems.Count; jb++)     //卷积步长循环
                        {
                            for (int pz = 0; pz < this.uiCheckBoxGroup7.SelectedItems.Count; pz++)     //偏置循环
                            {
                                m = m + 1;
                                ListViewItem lv = new ListViewItem();
                                lv.Text = "测试" + m.ToString("D2");
                                lv.SubItems.Add(this.uiCheckBoxGroup2.SelectedItems[cl].ToString());
                                lv.SubItems.Add(this.uiCheckBoxGroup1.SelectedItems[jh].ToString());
                                lv.SubItems.Add(this.uiCheckBoxGroup3.SelectedItems[jb].ToString());
                                lv.SubItems.Add(this.uiCheckBoxGroup7.SelectedItems[pz].ToString());
                                lv.SubItems.Add("无");
                                lv.SubItems.Add("无");
                                lv.SubItems.Add("0");
                                listView1.Items.Add(lv);
                            }
                        }
                    }
                    for (int rl = 0; rl < this.uiCheckBoxGroup4.SelectedItems.Count; rl++)     //ReLu 循环
                    {
                        m = m + 1;
                        ListViewItem lv = new ListViewItem();
                        lv.Text = "测试" + m.ToString("D2");
                        lv.SubItems.Add(this.uiCheckBoxGroup2.SelectedItems[cl].ToString());
                        lv.SubItems.Add("conv1_1");
                        lv.SubItems.Add("1");
                        lv.SubItems.Add("无");
                        lv.SubItems.Add(this.uiCheckBoxGroup4.SelectedItems[rl].ToString());
                        lv.SubItems.Add("无");
                        lv.SubItems.Add("0");
                        listView1.Items.Add(lv);
                    }
                    for (int cc = 0; cc < this.uiCheckBoxGroup5.SelectedItems.Count; cc++)     //池化尺寸循环
                    {

                        for (int cb = 0; cb < this.uiCheckBoxGroup6.SelectedItems.Count; cb++)     //池化步长循环
                        {
                            m = m + 1;
                            ListViewItem lv = new ListViewItem();
                            lv.Text = "测试" + m.ToString("D2");
                            lv.SubItems.Add(this.uiCheckBoxGroup2.SelectedItems[cl].ToString());
                            lv.SubItems.Add("conv1_1");
                            lv.SubItems.Add("1");
                            lv.SubItems.Add("无");
                            lv.SubItems.Add("无");
                            lv.SubItems.Add(this.uiCheckBoxGroup5.SelectedItems[cc].ToString());
                            lv.SubItems.Add(this.uiCheckBoxGroup6.SelectedItems[cb].ToString());
                            listView1.Items.Add(lv);
                        }
                    }
                }
            }
            uIEditForm1.Dispose();
            */

        }










        public void ButtonCancelClick_Click(object sender, EventArgs e)
        {

        }











        private void uiLabel1_Click(object sender, EventArgs e)
        {

        }

        private void uiLabel2_Click(object sender, EventArgs e)
        {

        }

        private void btButton_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }



        private void qzopenButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            FileStream fs;
            file.InitialDirectory = ".";
            file.Filter = "所有文件(*.*)|*.*";
            file.ShowDialog();
            if (file.FileName != string.Empty)
            {
                uiTextBox1.Text = file.FileName;
            }
        }
        private void tpopenButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            FileStream fls;
            file.InitialDirectory = ".";
            file.Filter = "所有文件(*.*)|*.*";
            file.ShowDialog();
            if (file.FileName != string.Empty)
            {
                uiTextBox2.Text = file.FileName;
            }
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Checked)
                {

                    listView1.Items[i].Remove();


                }
            }


        }




        private void uiButton2_Click(object sender, EventArgs e)
        {
            //清除用Clear方法
            listView1.Items.Clear();
            //或者用
            //uiFlowLayoutPanel1.Panel.Controls.Clear();


        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void uIEditForm1_Load_1(object sender, EventArgs e)
        {

        }

        private void uIEditForm1_Load_2(object sender, EventArgs e)
        {

        }

        private void uiCheckBoxGroup1_ValueChanged(object sender, int index, string text, bool isChecked)
        {

        }

        private void uiCheckBoxGroup3_ValueChanged(object sender, int index, string text, bool isChecked)
        {

        }

        private void uiCheckBoxGroup4_ValueChanged(object sender, int index, string text, bool isChecked)
        {

        }

        private void uiCheckBoxGroup2_ValueChanged(object sender, int index, string text, bool isChecked)
        {

        }

        private void uiCheckBoxGroup6_ValueChanged(object sender, int index, string text, bool isChecked)
        {

        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void frame1_Load(object sender, EventArgs e)
        {

        }

        private void tabpageshuju_Click(object sender, EventArgs e)
        {

        }

        private void uiButton7_Click(object sender, EventArgs e)
        {
            log l = new log();
            l.Show();
        }
        private void bt_Click(object sender, EventArgs e)
        {
            log l = new log();
            l.Show();
        }

        private void uiButton8_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = true;  //全选
            }
        }

        private void uiButton9_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = false;  //全不选
            }
        }

        private void FT6678_YOLO_diag_Resize(object sender, EventArgs e)
        {
            float[] percent = (float[])Tag;
            int i = 2;
            foreach (Control ctrl in this.Controls)
            {
                ctrl.Left = (int)(Size.Width * percent[i++]);
                ctrl.Top = (int)(Size.Height * percent[i++]);
                ctrl.Width = (int)(Size.Width / (float)percent[0] * ((Size)ctrl.Tag).Width);
                ctrl.Height = (int)(Size.Height / (float)percent[1] * ((Size)ctrl.Tag).Height);
            }

        }

        public class diag_lib
        {
            public static string PadBuffer(string str, uint fromIndex, uint toIndex)
            {
                for (uint i = fromIndex; i < toIndex; ++i)
                    str += "0";

                return str;
            }

            public static string DisplayHexBuffer(object[] obj, DWORD dwBuffSize,
                 WDC_ADDR_MODE mode)
            {
                string display = "";

                switch (mode)
                {
                    case WDC_ADDR_MODE.WDC_MODE_8:
                        {
                            BYTE[] buff = (BYTE[])obj[0];
                            for (uint i = 0; i < dwBuffSize; ++i)
                                display = string.Concat(display,
                                    buff[i].ToString("X2"), " ");
                            break;
                        }
                    case WDC_ADDR_MODE.WDC_MODE_16:
                        {
                            WORD[] buff = (WORD[])obj[0];
                            for (int i = 0; i < dwBuffSize / 2; ++i)
                                display = string.Concat(display,
                                    buff[i].ToString("X4"), " ");
                            break;
                        }
                    case WDC_ADDR_MODE.WDC_MODE_32:
                        {
                            UINT32[] buff = (UINT32[])obj[0];
                            for (int i = 0; i < dwBuffSize / 4; ++i)
                                display = string.Concat(display,
                                    buff[i].ToString("X8"), " ");
                            break;
                        }
                    case WDC_ADDR_MODE.WDC_MODE_64:
                        {
                            UINT64[] buff = (UINT64[])obj[0];
                            for (int i = 0; i < dwBuffSize / 8; ++i)
                                display = string.Concat(display,
                                    buff[i].ToString("X16"), " ");
                            break;
                        }
                }
                return display;
            }

            public static string DisplayHexBuffer(byte[] buff, uint dwBuffSize)
            {
                return DisplayHexBuffer(new object[] { buff }, dwBuffSize,
                    WDC_ADDR_MODE.WDC_MODE_8);
            }
        }

    }
}



