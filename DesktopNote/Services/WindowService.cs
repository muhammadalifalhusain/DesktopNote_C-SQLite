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

    private const int DeckWidth = 400;
    private const int DeckHeight = 700;

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
        }

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