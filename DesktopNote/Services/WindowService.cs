using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;
using WinRT.Interop;

namespace DesktopNote.Services;

public static class WindowService
{
    public static void Configure(Window window)
    {
        IntPtr hwnd =
            WindowNative.GetWindowHandle(window);

        var windowId =
            Microsoft.UI.Win32Interop
                .GetWindowIdFromWindow(hwnd);

        var appWindow =
            AppWindow.GetFromWindowId(windowId);

        appWindow.TitleBar
            .ExtendsContentIntoTitleBar = true;

        appWindow.TitleBar
            .PreferredHeightOption =
                TitleBarHeightOption.Collapsed;
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

        appWindow.Resize(
            new SizeInt32(
                width,
                height));
    }

    public static void MoveToRightCenter(
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

        var displayArea =
            DisplayArea.GetFromWindowId(
                windowId,
                DisplayAreaFallback.Primary);

        var workArea =
            displayArea.WorkArea;

        int x =
            workArea.X +
            workArea.Width -
            width;

        int y =
            workArea.Y +
            (workArea.Height - height) / 2;

        appWindow.Move(
            new PointInt32(
                x,
                y));
    }
}