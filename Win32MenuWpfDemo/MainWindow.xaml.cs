using System.Windows;
using System.Windows.Interop;
using Win32Menu;

namespace Win32MenuWpfDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            menu =new NativeMenu()
            {
                Text = "菜单",
                Uid =888,
            };
            var file = new NativeMenu
            {
                Uid = 1,
                Text = "文件"
            };
            var exit = new NativeMenu
            {
                Uid = 2,
                Text = "退出",
                Checked = true,
                IsRadioCheck = true
            };
            var xMenu = new NativeMenu()
            {
                Uid = 3,
                Text = "x",
                RightJustify = true
            };
            file.AppendMenu(exit);
            menu.AppendMenu(file,true);
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

        private readonly NativeMenu menu;

        private nint WndProc(nint hWnd, int msg, nint wParam, nint lParam, ref bool handled)
        {
            if (msg is (int)WndProcMsgType.WmCommand or (int)WndProcMsgType.WmSystemCommand)
            {
                menu.ProcessWndProcParams(wParam);
            }

            return 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (PresentationSource.FromVisual(this) is not HwndSource hs) throw new NullReferenceException(nameof(hs));

            if (CheckBox1.IsChecked is true)
            {
                if (!menu.IsSystemMenu)
                    menu.SetupForSystemMenu(hs.Handle);
            }
            else
            {
                menu.SetMenu(hs.Handle);
            }
            
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (PresentationSource.FromVisual(this) is not HwndSource hs) throw new NullReferenceException(nameof(hs));
            if (CheckBox1.IsChecked is true)
            {
                if (menu.IsSystemMenu)
                    menu.SetupForSystemMenu(hs.Handle,true);
            }
            else
            {
                NativeMenu.SetNullMenu(hs.Handle);
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hs = PresentationSource.FromVisual(this) as HwndSource;
            if (hs is null) throw new NullReferenceException(nameof(hs));
            hs.AddHook(WndProc);
        }
    }
}