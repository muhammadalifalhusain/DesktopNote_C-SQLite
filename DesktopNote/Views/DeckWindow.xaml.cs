
using DesktopNote.Helpers;
using DesktopNote.Models;
using DesktopNote.Services;
using DesktopNote.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Runtime.InteropServices;

namespace DesktopNote.Views;

public sealed partial class DeckWindow : Window
{
    public MainViewModel ViewModel { get; } =
        new MainViewModel();

    private Border? _expandedBorder;

    private ManagementWindow? _managementWindow;

    private const int HotKeyIdNewNote = 1001;
    private const int HotKeyIdManagement = 1002;

    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;

    private const uint VK_N = 0x4E;
    private const uint VK_A = 0x41;

    private const uint WM_HOTKEY = 0x0312;

    private const int GWLP_WNDPROC = -4;

    private delegate IntPtr WndProcDelegate(
        IntPtr hWnd,
        uint msg,
        IntPtr wParam,
        IntPtr lParam);

    private WndProcDelegate? _wndProcDelegate;

    private IntPtr _oldWndProc;

    public DeckWindow()
    {
        InitializeComponent();

        WindowService.Configure(this);

        WindowService.SetSize(
            this,
            380,
            700);

        WindowService.MoveToRightCenter(
            this,
            380,
            700);

        RegisterGlobalHotKeys();

        Closed += DeckWindow_Closed;
    }

    private void RegisterGlobalHotKeys()
    {
        IntPtr hwnd =
            WinRT.Interop.WindowNative
                .GetWindowHandle(this);

        _wndProcDelegate =
            WindowProc;

        _oldWndProc =
            SetWindowLongPtr(
                hwnd,
                GWLP_WNDPROC,
                Marshal.GetFunctionPointerForDelegate(
                    _wndProcDelegate));

        bool newNoteRegistered =
            RegisterHotKey(
                hwnd,
                HotKeyIdNewNote,
                MOD_CONTROL | MOD_ALT,
                VK_N);

        bool managementRegistered =
            RegisterHotKey(
                hwnd,
                HotKeyIdManagement,
                MOD_CONTROL | MOD_ALT,
                VK_A);

        if (!newNoteRegistered ||
            !managementRegistered)
        {
            if (newNoteRegistered)
            {
                UnregisterHotKey(
                    hwnd,
                    HotKeyIdNewNote);
            }

            if (managementRegistered)
            {
                UnregisterHotKey(
                    hwnd,
                    HotKeyIdManagement);
            }

            if (_oldWndProc != IntPtr.Zero)
            {
                SetWindowLongPtr(
                    hwnd,
                    GWLP_WNDPROC,
                    _oldWndProc);

                _oldWndProc =
                    IntPtr.Zero;
            }

            _wndProcDelegate = null;
        }
    }

    private IntPtr WindowProc(
        IntPtr hWnd,
        uint msg,
        IntPtr wParam,
        IntPtr lParam)
    {
        if (msg == WM_HOTKEY)
        {
            int hotKeyId =
                wParam.ToInt32();

            if (hotKeyId == HotKeyIdNewNote)
            {
                DispatcherQueue.TryEnqueue(
                    OpenNewNote);

                return IntPtr.Zero;
            }

            if (hotKeyId == HotKeyIdManagement)
            {
                DispatcherQueue.TryEnqueue(
                    OpenManagementWindow);

                return IntPtr.Zero;
            }
        }

        if (_oldWndProc != IntPtr.Zero)
        {
            return CallWindowProc(
                _oldWndProc,
                hWnd,
                msg,
                wParam,
                lParam);
        }

        return IntPtr.Zero;
    }

    private void OpenNewNote()
    {
        var window =
            new NoteWindow(ViewModel);

        window.Activate();
    }

    private void OpenManagementWindow()
    {
        if (_managementWindow is not null)
        {
            _managementWindow.Activate();
            return;
        }

        _managementWindow =
            new ManagementWindow(ViewModel);

        _managementWindow.Closed +=
            ManagementWindow_Closed;

        _managementWindow.Activate();
    }

    private void ManagementWindow_Closed(
        object sender,
        WindowEventArgs args)
    {
        _managementWindow = null;
    }

    private void DeckWindow_Closed(
        object sender,
        WindowEventArgs args)
    {
        IntPtr hwnd =
            WinRT.Interop.WindowNative
                .GetWindowHandle(this);

        UnregisterHotKey(
            hwnd,
            HotKeyIdNewNote);

        UnregisterHotKey(
            hwnd,
            HotKeyIdManagement);

        if (_oldWndProc != IntPtr.Zero)
        {
            SetWindowLongPtr(
                hwnd,
                GWLP_WNDPROC,
                _oldWndProc);

            _oldWndProc =
                IntPtr.Zero;
        }

        _wndProcDelegate = null;
    }

    private void NewNote_Click(
        object sender,
        RoutedEventArgs e)
    {
        OpenNewNote();
    }

    private void OpenManagement_Click(
        object sender,
        RoutedEventArgs e)
    {
        OpenManagementWindow();
    }

    private void Note_Tapped(
        object sender,
        TappedRoutedEventArgs e)
    {
        if (e.Handled)
            return;

        if (sender is not FrameworkElement element)
            return;

        if (element.DataContext is not Note note)
            return;

        var window =
            new NoteWindow(
                ViewModel,
                note);

        window.Activate();

        e.Handled = true;
    }

    private void NotesList_ContainerContentChanging(
        ListViewBase sender,
        ContainerContentChangingEventArgs args)
    {
        if (args.ItemContainer?
            .ContentTemplateRoot is not Grid grid)
            return;

        if (grid.FindName("NoteCard")
            is not Border border)
            return;

        NoteCardHelper.Reset(border);
    }

    private void Note_PointerEntered(
        object sender,
        PointerRoutedEventArgs e)
    {
        if (sender is not Border border)
            return;

        NoteCardHelper.Expand(
            border,
            _expandedBorder);

        _expandedBorder =
            border;
    }

    private void Note_PointerExited(
        object sender,
        PointerRoutedEventArgs e)
    {
        if (sender is not Border border)
            return;

        if (ReferenceEquals(
            _expandedBorder,
            border))
        {
            _expandedBorder = null;
        }

        NoteCardHelper.Collapse(border);
    }

    private void CompleteNote_Tapped(
        object sender,
        TappedRoutedEventArgs e)
    {
        e.Handled = true;
    }

    private void CompleteNote_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not Note note)
            return;

        ViewModel.ToggleCompleted(note);
    }

    [DllImport(
        "user32.dll",
        SetLastError = true)]
    private static extern bool RegisterHotKey(
        IntPtr hWnd,
        int id,
        uint fsModifiers,
        uint vk);

    [DllImport(
        "user32.dll",
        SetLastError = true)]
    private static extern bool UnregisterHotKey(
        IntPtr hWnd,
        int id);

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
        SetLastError = true,
        EntryPoint = "CallWindowProcW")]
    private static extern IntPtr CallWindowProc(
        IntPtr lpPrevWndFunc,
        IntPtr hWnd,
        uint Msg,
        IntPtr wParam,
        IntPtr lParam);
}