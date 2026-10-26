using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinRT.Interop;

namespace DesktopNote.Helpers;

public sealed class DeckWindowSizer
{
    private readonly AppWindow _appWindow;
    private readonly IntPtr _hwnd;

    public DeckWindowSizer(Window window)
    {
        _hwnd = WindowNative.GetWindowHandle(window);

        _appWindow = AppWindow.GetFromWindowId(
            Win32Interop.GetWindowIdFromWindow(_hwnd));
    }

    public void SetSize(double widthDip, double heightDip)
    {
        var scale = GetDpiForWindow(_hwnd) / 96.0;

        var area = DisplayArea.GetFromWindowId(
            _appWindow.Id,
            DisplayAreaFallback.Primary).WorkArea;

        var width = (int)Math.Ceiling(widthDip * scale);

        var height = Math.Min(
            (int)Math.Ceiling(Math.Max(heightDip, 8) * scale),
            area.Height);

        _appWindow.MoveAndResize(
            new RectInt32(
                area.X + area.Width - width,
                area.Y + (area.Height - height) / 2,
                width,
                height));
    }

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hWnd);
}