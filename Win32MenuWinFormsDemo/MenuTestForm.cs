
using System.Diagnostics;
using Win32Menu;


namespace Win32MenuWinFormsDemo;

public partial class MenuTestForm : Form
{
    private NativeMenu? menu;
    public MenuTestForm()
    {
        InitializeComponent();
        
        //file.AppendMenu(exit);
        //menu.AppendMenu(file,true);
    }
    

    protected override void WndProc(ref Message m)
    {
        if (m.Msg is (int)WndProcMsgType.WmCommand or (int)WndProcMsgType.WmSystemCommand)
        {
            if (menu is not null)
            {
                if (menu.ProcessWndProcParams(m.WParam))
                {
                    Debug.WriteLine("Menu clicked");
                }
            }
            
        }
            
        base.WndProc(ref m);
    }

    private void button1_Click(object sender, EventArgs e)
    {
        if (menu is null) return;
        if (checkBox1.Checked)
        {
            if (!menu.IsSystemMenu)
                menu.SetupForSystemMenu(Handle);
        }
        else
        {
            menu.SetMenu(Handle);
        }
    }

    private void button2_Click(object sender, EventArgs e)
    {
        //NativeMenu.SetNullMenu(Handle);
        if (menu is null) return;
        if (checkBox1.Checked)
        {
            if (menu.IsSystemMenu)
                menu?.SetupForSystemMenu(Handle, true);
        }
        else
        {
            NativeMenu.SetNullMenu(Handle);
        }
        
        
    }

    private void MenuTestForm_Load(object sender, EventArgs e)
    {
        menu = new NativeMenu()
        {
            Text = "菜单",
            Uid =888,
        };

        var file = new NativeMenu
        {
            Uid = 1,
            Text = "文件"
        };
        
        var disable = new NativeMenu
        {
            Uid = 3,
            Text = "禁用",
            Status = MenuStatus.Disabled
        };
        var check = new NativeMenu
        {
            Uid = 4,
            Text = "勾选",
            Checked = true
        };
        check.Click += (NativeMenu nativeMenu, ref bool modify) =>
        {
            nativeMenu.Checked = !nativeMenu.Checked;
            modify = true;
        };
        var radio = new NativeMenu()
        {
            Uid = 5,
            Text = "单选项",
        };
        var radio1 = new NativeMenu()
        {
            Uid = 7,
            Text = "单选项1",
            IsRadioCheck = true,
        };
        var radio2 = new NativeMenu()
        {
            Uid = 8,
            Text = "单选项2",
            IsRadioCheck = true,
        };
        var radio3 = new NativeMenu()
        {
            Uid = 9,
            Text = "单选项3",
            IsRadioCheck = true,
            Checked = true
        };
        radio1.Click += RadioOnClick;
        radio2.Click += RadioOnClick;
        radio3.Click += RadioOnClick;

        void RadioOnClick(NativeMenu nativeMenu, ref bool needModify)
        {
            nativeMenu.Checked = true;
            needModify = true;
        }

        radio.AppendMenu(radio1);
        radio.AppendMenu(radio2);
        radio.AppendMenu(radio3);
        var sep = new NativeMenu
        {
            Uid = 6,
            Text = "-",
            Type = MenuType.Separator
        };
        var exit = new NativeMenu
        {
            Uid = 2,
            Text = "退出",
        };
        file.AppendMenu(disable);
        file.AppendMenu(check);
        file.AppendMenu(radio,true);
        file.AppendMenu(sep);
        file.AppendMenu(exit);
        menu.AppendMenu(file,true);
        var xMenu = new NativeMenu()
        {
            Uid = 10,
            Text = "x",
            RightJustify = true
        };
        menu.AppendMenu(xMenu);

        exit.Click += (NativeMenu _, ref bool _) =>
        {
            Close();
        };
        xMenu.Click += (NativeMenu _, ref bool _) =>
        {
            MessageBox.Show("点击了X");
        };
    }
}