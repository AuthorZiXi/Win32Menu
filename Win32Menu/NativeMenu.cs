using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Win32Menu;
/// <summary>
/// Win32原生菜单高度封装实现
/// </summary>
public class NativeMenu : IDisposable
{
    /// <summary>
    /// 创建原生菜单
    /// </summary>
    /// <exception cref="Win32Exception">创建失败</exception>
    public NativeMenu()
    {
        hMenu = PInvoke.CreateMenu();
        if (hMenu == IntPtr.Zero)
            throw new Win32Exception(Marshal.GetLastWin32Error());
        destroyMenuSafeHandle = new DestroyMenuSafeHandle(hMenu, ownsHandle: true);
        MenuHandle = destroyMenuSafeHandle.DangerousGetHandle();
    }
    /// <summary>
    /// 菜单的点击事件
    /// </summary>
    /// <remarks>子菜单注册之前请注意，Uid必须为非零不重复数，否则会被其他菜单项拦截</remarks>
    public event MenuItemEventHandler? Click;
    /// <summary>
    /// 可传递的菜单信息处理
    /// </summary>
    /// <remarks>
    /// <para>该函数会将消息传递到其子菜单，因此只需要调用主菜单的此方法即可</para>
    /// <para>子菜单注册之前请注意，Uid必须为非零不重复数，否则会被其他菜单项拦截</para>
    /// </remarks>
    /// <example>
    /// <para>你需要在窗口中Hook或重写修改WndProc在其中判断是否符合以下条件(普通菜单):</para>
    /// <code language="csharp">m.Msg == (int)WndProcMsgType.WmCommand</code>
    /// <para>然后这么调用：</para>
    /// <code>menu.ProcessWndProcParams(m.WParam)</code>
    /// </example>
    /// <param name="wParam">这是窗口消息的Unicode参数</param>
    /// <returns>是否被处理</returns>
    public bool ProcessWndProcParams(nint wParam)
    {
        if (wParam == 0) return false;
        if (parentMenu == null || wParam != Uid) //加个parentMenu的判断条件是因为窗口主菜单会作为菜单栏，是不能使用Modify的。 
            return entrustedMenus is not null && entrustedMenus.Any(menu => menu.ProcessWndProcParams(wParam));
        var needModify = false;
        Click?.Invoke(this,ref needModify);
        if (needModify)
            Modify(); // 有的开发者可能懒得调用状态更新，这里我们直接自己调。 
        return true;

    }
    /// <summary>
    /// 菜单句柄
    /// </summary>
    /// <remarks>请不要在外部使用其他方法更改菜单，可能会引起异常</remarks>
    public readonly IntPtr MenuHandle;
    // 菜单在Win32中的数据
    private readonly HMENU hMenu;
    // 菜单的安全句柄
    private readonly DestroyMenuSafeHandle destroyMenuSafeHandle;
    // 父菜单
    private NativeMenu? parentMenu;
    // 当前菜单在父菜单的位置
    private uint posOfParentMenu;
    // 声明的菜单标识符
    private nuint newItemId;
    /// <summary>
    /// 菜单Id，需要手动指定
    /// </summary>
    /// <remarks>子菜单注册之前请注意，Uid必须为非零不重复数，否则会被其他菜单项拦截</remarks>
    public uint Uid { get; set; }
    /// <summary>
    /// 菜单文本
    /// </summary>
    public string Text { get; set; } = string.Empty;
    /// <summary>
    /// 菜单状态
    /// </summary>
    public MenuStatus Status { get; set; }
    /// <summary>
    /// 菜单类型
    /// </summary>
    public MenuType Type { get; set; }
    /// <summary>
    /// 向右对齐（需要是窗口的主菜单）
    /// </summary>
    public bool RightJustify { get; set; }
    /// <summary>
    /// 是否选中
    /// </summary>
    public bool Checked { get; set; }
    /// <summary>
    /// 单选按钮样式
    /// </summary>
    public bool IsRadioCheck { get; set; }
    
    /// <summary>
    /// 进行菜单修改（需要在修改菜单数据后手动调用）
    /// </summary>
    /// <exception cref="Exception">只有存在父菜单的子菜单可以修改</exception>
    /// <exception cref="ArgumentOutOfRangeException">枚举数据存在错误</exception>
    /// <exception cref="Win32Exception">底层win32报错，可能是外部修改引起的</exception>
    public void Modify()
    {
        // 检查父菜单是否为空
        if (parentMenu is null) throw new Exception("ParentMenu is null");
        // 设置菜单项标志
        var flags = MENU_ITEM_FLAGS.MF_BYPOSITION;
        // 如果右对齐，则添加右对齐标志
        if (RightJustify) flags |= MENU_ITEM_FLAGS.MF_RIGHTJUSTIFY;
        // 根据菜单状态设置菜单项标志
        flags |= Status switch
        {
            MenuStatus.Disabled => MENU_ITEM_FLAGS.MF_GRAYED,
            MenuStatus.Enabled => MENU_ITEM_FLAGS.MF_ENABLED,
            _ => throw new ArgumentOutOfRangeException()
        };

        // 根据菜单项是否选中设置菜单项标志
        flags |= Checked switch
        {
            true => MENU_ITEM_FLAGS.MF_CHECKED,
            false => MENU_ITEM_FLAGS.MF_UNCHECKED
        };

        // 根据菜单项类型设置菜单项标志
        flags |= Type switch
        {
            MenuType.String => MENU_ITEM_FLAGS.MF_STRING,
            MenuType.Separator => MENU_ITEM_FLAGS.MF_SEPARATOR,
            MenuType.MenuBarBreak => MENU_ITEM_FLAGS.MF_MENUBARBREAK,
            MenuType.MenuBreak => MENU_ITEM_FLAGS.MF_MENUBREAK,
            _ => throw new ArgumentOutOfRangeException()
        };

        // 调用PInvoke.ModifyMenu方法修改菜单项
        var success = PInvoke.ModifyMenu(parentMenu.destroyMenuSafeHandle, posOfParentMenu, flags, newItemId, Text);
        // 如果修改失败，则抛出Win32Exception异常
        if (!success) throw new Win32Exception(Marshal.GetLastWin32Error());

        // 如果菜单项不是单选按钮，则直接返回
        if (!IsRadioCheck || !Checked) return;
        // 调用PInvoke.CheckMenuRadioItem方法设置单选按钮
        PInvoke.CheckMenuRadioItem(parentMenu.destroyMenuSafeHandle, 0u, (uint)parentMenu.entrustedMenus!.Count, posOfParentMenu,(uint)MENU_ITEM_FLAGS.MF_BYPOSITION);
        
    }
    
    /// <summary>
    /// 设置为主菜单
    /// </summary>
    /// <param name="hWnd">窗口句柄</param>
    /// <returns>是否成功</returns>
    public bool SetMenu(IntPtr hWnd)
    {
        return PInvoke.SetMenu((HWND)hWnd, hMenu);
    }
    /// <summary>
    /// 清空窗口主菜单
    /// </summary>
    /// <param name="hWnd">窗口句柄</param>
    /// <returns>是否成功</returns>
    public static bool SetNullMenu(IntPtr hWnd)
    {
        return PInvoke.SetMenu(new HWND(hWnd),new DestroyMenuSafeHandle(IntPtr.Zero));
    }
    /// <summary>
    /// 绘制菜单栏
    /// </summary>
    /// <param name="hWnd">窗口句柄</param>
    /// <returns>是否成功</returns>
    public static bool DrawMenuBar(IntPtr hWnd)
    {
        return PInvoke.DrawMenuBar(new HWND(hWnd));
    }
    // 受托管的菜单 ： 注意，千万不要在外部操纵，会导致找不到或找错菜单，最坏是抛异常。
    private List<NativeMenu>? entrustedMenus;
    /// <summary>
    /// 添加菜单
    /// </summary>
    /// <param name="menu">菜单对象</param>
    /// <param name="isPopup">是否作为弹出菜单（相当于是否有子项）</param>
    /// <exception cref="Win32Exception">操作失败</exception>
    public void AppendMenu(NativeMenu menu,bool isPopup=false)
    {
        entrustedMenus ??= [];
        
        var flag = isPopup ? MENU_ITEM_FLAGS.MF_POPUP : MENU_ITEM_FLAGS.MF_APPEND;
        var newItem = isPopup ? (nuint)menu.MenuHandle : menu.Uid;
        menu.newItemId = newItem;
        menu.parentMenu = this;
        menu.posOfParentMenu = (uint)entrustedMenus.Count;
        var success = PInvoke.AppendMenu(destroyMenuSafeHandle, flag, newItem, menu.Text);
        if (!success) throw new Win32Exception(Marshal.GetLastWin32Error());
        
        entrustedMenus.Add(menu);
        menu.Modify();

    }
    /// <summary>
    /// 移除菜单
    /// </summary>
    /// <param name="menu">菜单</param>
    /// <exception cref="Win32Exception">操作出错</exception>
    /// <exception cref="NullReferenceException">托管子菜单的数据为Null或父菜单不存在</exception>
    public void RemoveMenu(NativeMenu menu)
    {
        if (parentMenu is null) throw new NullReferenceException("parentMenu is null");
        var success = PInvoke.RemoveMenu(parentMenu.destroyMenuSafeHandle, menu.posOfParentMenu, MENU_ITEM_FLAGS.MF_REMOVE);
        if (!success) throw new Win32Exception(Marshal.GetLastWin32Error());
        if (entrustedMenus is null) throw new NullReferenceException("entrustedMenus is null");
        entrustedMenus.Remove(menu);
        for (var index = 0; index < entrustedMenus.Count; index++)
        {
            var menus = entrustedMenus[index];
            menus.posOfParentMenu = (uint)index;
        }
    }
    
    // 获取的系统菜单
    private DestroyMenuSafeHandle? systemMenu=null;

    /// <summary>
    /// 设置为系统菜单
    /// </summary>
    /// <remarks>
    /// <para>注意：需要没有父菜单，必须有Text设置</para>
    /// <para>请不要是系统菜单时，切换为窗口主菜单。</para>
    /// <para>无论如何，具有同一个菜单的只能是同一个窗口。</para>
    /// <para>请不要混用，因为会引发一些难以预料的问题。</para>
    /// <para>而且菜单不是一个可以共用的数据结构/记录。</para>
    /// </remarks>
    /// <example>
    /// 如果需要捕获这类系统菜单的事件，请在调用<see cref="ProcessWndProcParams"/>的WndProc内补充以下条件：
    /// <code>(int)WndProcMsgType.WmSystemCommand</code>
    /// 注意：部分系统命令的Id可能会与您菜单的Uid冲突（如窗口上的关闭，最小化等），请自行处理。
    /// </example>
    /// <param name="hWnd">窗口句柄</param>
    /// <param name="isRemove">是否是删除操作</param>
    /// <exception cref="InvalidOperationException">无效操作</exception>
    public void SetupForSystemMenu(IntPtr hWnd, bool isRemove = false)
    {
        // 仅可以用在不存在父菜单的菜单中
        if (parentMenu is not null) throw new InvalidOperationException("It can only be used in menus where there is no parent menu");
        
        var sysMenu = PInvoke.GetSystemMenu(new HWND(hWnd), false);
        var h = new DestroyMenuSafeHandle(sysMenu, ownsHandle: true);
        if (!isRemove)
        {
            if (systemMenu is not null) throw new InvalidOperationException("It can only be used once");
            var success = PInvoke.AppendMenu(h, MENU_ITEM_FLAGS.MF_POPUP, (uint)MenuHandle, Text);
            if (!success) throw new Win32Exception(Marshal.GetLastWin32Error());
            systemMenu = h;
        }
        else
        {
            if (systemMenu is null) throw new InvalidOperationException("He is not the system menu yet");
            var success = PInvoke.RemoveMenu(h, (uint)MenuHandle, MENU_ITEM_FLAGS.MF_BYCOMMAND);
            if (!success) throw new Win32Exception(Marshal.GetLastWin32Error());
            systemMenu = null;
        }
    }
    /// <summary>
    /// 是否已经挂载到系统菜单
    /// </summary>
    public bool IsSystemMenu => systemMenu is not null;
    /// <summary>
    /// 销毁菜单
    /// </summary>
    public void Dispose()
    {
        PInvoke.DestroyMenu(hMenu);
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// 窗口信息类型
/// </summary>
public enum WndProcMsgType
{
    /// <summary>
    /// 菜单命令(通常不需要使用这个)
    /// </summary>
    WmMenuCommand = 0x0126,
    /// <summary>
    /// 菜单命令(如果挂载到窗口上请这么使用)
    /// </summary>
    WmCommand = 0x0111,
    /// <summary>
    /// 菜单命令(挂载到系统菜单上请使用这个)
    /// </summary>
    WmSystemCommand = 0x0112,
}
/// <summary>
/// 菜单状态
/// </summary>
public enum MenuStatus
{
    /// <summary>
    /// 启用
    /// </summary>
    Enabled,
    /// <summary>
    /// 禁用
    /// </summary>
    Disabled
}
/// <summary>
/// 菜单类型
/// </summary>
public enum MenuType
{
    /// <summary>
    /// 字符串
    /// </summary>
    String,
    //RadioCheck,这个已变成一个bool属性了
    /// <summary>
    /// 分隔符
    /// </summary>
    Separator,
    /// <summary>
    /// 新行中或弹出菜单的新列中放置菜单项(一般用不到)
    /// </summary>
    MenuBarBreak,
    /// <summary>
    /// 新行中或弹出菜单的新列中放置菜单项(一般用不到)
    /// </summary>
    MenuBreak
}
/// <summary>
/// 菜单项
/// </summary>
/// <param name="menu">触发事件的菜单项</param>
/// <param name="needModify">是否需要修改菜单（是则由菜单主动调用状态同步，也就是更新菜单）</param>
public delegate void MenuItemEventHandler(NativeMenu menu,ref bool needModify);
