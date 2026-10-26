using DesktopNote.Helpers;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinRT.Interop;
using WinUIEx;

namespace DesktopNote.Services;

public static class WindowService
{
    private const int GWL_EXSTYLE = -20;

    private const long WS_EX_TOOLWINDOW = 0x00000080L;
    private const long WS_EX_APPWINDOW = 0x00040000L;

    private const int DeckWidth = (int)NoteCardHelper.CollapsedWidth;
    private const int DeckHeight = (int)NoteCardHelper.CollapsedHeight;

    private const int DWMWA_BORDER_COLOR = 34;
    private const int DWMWA_COLOR_NONE = unchecked((int)0xFFFFFFFE);

    private const int DWMWA_NCRENDERING_POLICY = 2;
    private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    private const int DWMNCRP_DISABLED = 2;
    private const int DWMWCP_DONOTROUND = 1;

    private const int GWL_STYLE = -16;

    private const long WS_CAPTION = 0x00C00000L;
    private const long WS_THICKFRAME = 0x00040000L;
    private const long WS_BORDER = 0x00800000L;
    private const long WS_DLGFRAME = 0x00400000L;

    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_FRAMECHANGED = 0x0020;

    private const int GCL_STYLE = -26;

    private const long CS_DROPSHADOW = 0x00020000L;

    [DllImport(
        "user32.dll",
        SetLastError = true,
        EntryPoint = "GetClassLongPtrW")]
    private static extern IntPtr GetClassLongPtr(
        IntPtr hWnd,
        int nIndex);

    [DllImport(
        "user32.dll",
        SetLastError = true,
        EntryPoint = "SetClassLongPtrW")]
    private static extern IntPtr SetClassLongPtr(
        IntPtr hWnd,
        int nIndex,
        IntPtr dwNewLong);

    [DllImport(
        "user32.dll",
        SetLastError = true)]
    private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags);

    [DllImport(
        "user32.dll",
        SetLastError = true,
        EntryPoint = "GetWindowLongPtrW")]
    private static extern IntPtr GetWindowLongPtr(
        IntPtr hWnd,
        int nIndex);

    [DllImport(
        "user32.dll",
        SetLastError = true,
        EntryPoint = "SetWindowLongPtrW")]
    private static extern IntPtr SetWindowLongPtr(
        IntPtr hWnd,
        int nIndex,
        IntPtr dwNewLong);

    [DllImport(
        "user32.dll",
        SetLastError = true)]
    private static extern int GetDpiForWindow(
        IntPtr hWnd);

    [DllImport(
        "dwmapi.dll",
        PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(
        IntPtr hwnd,
        int dwAttribute,
        ref int pvAttribute,
        int cbAttribute);

    public static void Configure(Window window)
    {
        IntPtr hwnd =
            WindowNative.GetWindowHandle(window);

        var windowId =
            Microsoft.UI.Win32Interop
                .GetWindowIdFromWindow(hwnd);

        var appWindow =
            AppWindow.GetFromWindowId(windowId);

        appWindow.TitleBar.ExtendsContentIntoTitleBar =
            true;

        appWindow.TitleBar.PreferredHeightOption =
            TitleBarHeightOption.Collapsed;
    }

    public static void ConfigureDeck(Window window)
    {
        IntPtr hwnd =
            WindowNative.GetWindowHandle(window);

        var windowId =
            Microsoft.UI.Win32Interop
                .GetWindowIdFromWindow(hwnd);

        var appWindow =
            AppWindow.GetFromWindowId(windowId);

        appWindow.TitleBar.ExtendsContentIntoTitleBar =
            true;

        appWindow.TitleBar.PreferredHeightOption =
            TitleBarHeightOption.Collapsed;

        ApplyDeckWindowStyle(hwnd);

        window.SystemBackdrop =
            new TransparentTintBackdrop();

        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;

            presenter.SetBorderAndTitleBar(
                false,
                false);
        }

        RemoveWindowFrame(hwnd);

        RemoveWindowShadow(hwnd);

        RemoveBorderAccentColor(hwnd);

        RemoveDwmShadow(hwnd);

        double scale =
            GetDpiForWindow(hwnd) / 96.0;

        int physicalWidth =
            (int)(DeckWidth * scale);

        int physicalHeight =
            (int)(DeckHeight * scale);

        appWindow.Resize(
            new SizeInt32(
                physicalWidth,
                physicalHeight));

        MoveToRightCenter(
            window,
            physicalWidth,
            physicalHeight);
    }

    public static void ReapplyBackdrop(Window window)
    {
        IntPtr hwnd =
            WindowNative.GetWindowHandle(window);

        window.SystemBackdrop =
            new TransparentTintBackdrop();

        var windowId =
            Microsoft.UI.Win32Interop
                .GetWindowIdFromWindow(hwnd);

        var appWindow =
            AppWindow.GetFromWindowId(windowId);

        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.SetBorderAndTitleBar(
                false,
                false);
        }

        RemoveWindowFrame(hwnd);

        RemoveWindowShadow(hwnd);

        RemoveBorderAccentColor(hwnd);

        RemoveDwmShadow(hwnd);
    }

    private static void ApplyDeckWindowStyle(
        IntPtr hwnd)
    {
        long style =
            GetWindowLongPtr(
                hwnd,
                GWL_EXSTYLE).ToInt64();

        style |= WS_EX_TOOLWINDOW;

        style &= ~WS_EX_APPWINDOW;

        SetWindowLongPtr(
            hwnd,
            GWL_EXSTYLE,
            new IntPtr(style));
    }

    private static void RemoveWindowFrame(
        IntPtr hwnd)
    {
        long style =
            GetWindowLongPtr(
                hwnd,
                GWL_STYLE).ToInt64();

        style &= ~WS_CAPTION;
        style &= ~WS_THICKFRAME;
        style &= ~WS_BORDER;
        style &= ~WS_DLGFRAME;

        SetWindowLongPtr(
            hwnd,
            GWL_STYLE,
            new IntPtr(style));

        SetWindowPos(
            hwnd,
            IntPtr.Zero,
            0,
            0,
            0,
            0,
            SWP_NOMOVE |
            SWP_NOSIZE |
            SWP_NOZORDER |
            SWP_FRAMECHANGED);
    }

    private static void RemoveWindowShadow(
        IntPtr hwnd)
    {
        long classStyle =
            GetClassLongPtr(
                hwnd,
                GCL_STYLE).ToInt64();

        classStyle &= ~CS_DROPSHADOW;

        SetClassLongPtr(
            hwnd,
            GCL_STYLE,
            new IntPtr(classStyle));
    }

    private static void RemoveBorderAccentColor(
        IntPtr hwnd)
    {
        int borderColorNone =
            DWMWA_COLOR_NONE;

        DwmSetWindowAttribute(
            hwnd,
            DWMWA_BORDER_COLOR,
            ref borderColorNone,
            sizeof(int));
    }

    private static void RemoveDwmShadow(
        IntPtr hwnd)
    {
        int policy =
            DWMNCRP_DISABLED;

        DwmSetWindowAttribute(
            hwnd,
            DWMWA_NCRENDERING_POLICY,
            ref policy,
            sizeof(int));

        int corner =
            DWMWCP_DONOTROUND;

        DwmSetWindowAttribute(
            hwnd,
            DWMWA_WINDOW_CORNER_PREFERENCE,
            ref corner,
            sizeof(int));
    }

    public static void SetSize(
        Window window,
        int width,
        int height)
    {
        IntPtr hwnd =
            WindowNative.GetWindowHandle(window);

        var windowId =
            Microsoft.UI.Win32Interop
                .GetWindowIdFromWindow(hwnd);

        var appWindow =
            AppWindow.GetFromWindowId(windowId);

        double scale =
            GetDpiForWindow(hwnd) / 96.0;

        appWindow.Resize(
            new SizeInt32(
                (int)(width * scale),
                (int)(height * scale)));
    }

    public static void MoveToRightCenter(
        Window window,
        int physicalWidth,
        int physicalHeight)
    {
        IntPtr hwnd =
            WindowNative.GetWindowHandle(window);

        var windowId =
            Microsoft.UI.Win32Interop
                .GetWindowIdFromWindow(hwnd);

        var appWindow =
            AppWindow.GetFromWindowId(windowId);

        var displayArea =
            DisplayArea.GetFromWindowId(
                windowId,
                DisplayAreaFallback.Primary);

        var workArea =
            displayArea.WorkArea;

        int x =
            workArea.X +
            workArea.Width -
            physicalWidth;

        int y =
            workArea.Y +
            (workArea.Height - physicalHeight) / 2;

        appWindow.Move(
            new PointInt32(
                x,
                y));
    }
}