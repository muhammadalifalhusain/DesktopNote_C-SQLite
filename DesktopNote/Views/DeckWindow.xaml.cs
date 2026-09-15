using DesktopNote.Models;
using Microsoft.UI.Xaml.Shapes;
using DesktopNote.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using Windows.Graphics;
using WinRT.Interop;

namespace DesktopNote.Views;

public sealed partial class DeckWindow : Window
{
    public MainViewModel ViewModel { get; } = new MainViewModel();

    private const double CollapsedWidth = 280;
    private const double CollapsedHeight = 42;
    private const double ExpandedWidth = 360;
    private const double ExpandedHeight = 160;
    private const double CollapsedTranslateX = 135;
    private const double ExpandedTranslateX = 0;

    private const int HoveredZIndex = 100;
    private const int DefaultZIndex = 0;

    private Border _expandedBorder;

    public DeckWindow()
    {
        InitializeComponent();

        ConfigureWindow();
        SetWindowSize();
        PositionWindow();
    }

    private void ConfigureWindow()
    {
        IntPtr hwnd = WindowNative.GetWindowHandle(this);

        var windowId =
            Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);

        var appWindow =
            Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

        appWindow.TitleBar.ExtendsContentIntoTitleBar = true;

        appWindow.TitleBar.PreferredHeightOption =
            Microsoft.UI.Windowing.TitleBarHeightOption.Collapsed;
    }

    private void SetWindowSize()
    {
        IntPtr hwnd = WindowNative.GetWindowHandle(this);

        var windowId =
            Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);

        var appWindow =
            Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

        appWindow.Resize(new SizeInt32(380, 700));
    }

    private void PositionWindow()
    {
        IntPtr hwnd = WindowNative.GetWindowHandle(this);

        var windowId =
            Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);

        var appWindow =
            Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

        var displayArea =
            Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(
                windowId,
                Microsoft.UI.Windowing.DisplayAreaFallback.Primary);

        var workArea = displayArea.WorkArea;

        int width = 380;
        int height = 700;

        int x = workArea.X + workArea.Width - width;
        int y = workArea.Y + (workArea.Height - height) / 2;

        appWindow.Move(new PointInt32(x, y));
    }

    private void NewNote_Click(
        object sender,
        RoutedEventArgs e)
    {
        var noteWindow = new NoteWindow(ViewModel);

        noteWindow.Activate();
    }

    private void Note_Tapped(
        object sender,
        TappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement element &&
            element.DataContext is Note note)
        {
            var noteWindow =
                new NoteWindow(ViewModel, note);

            noteWindow.Activate();
        }
    }

    private void NotesList_ContainerContentChanging(
        ListViewBase sender,
        ContainerContentChangingEventArgs args)
    {
        if (args.ItemContainer?.ContentTemplateRoot is not Grid grid)
            return;

        if (grid.FindName("NoteCard") is not Border border)
            return;

        ResetCardVisualState(border);

        Canvas.SetZIndex(
            args.ItemContainer,
            DefaultZIndex);
    }

    private void Note_PointerEntered(
        object sender,
        PointerRoutedEventArgs e)
    {
        if (sender is not Border border)
            return;

        if (_expandedBorder is not null &&
            !ReferenceEquals(_expandedBorder, border))
        {
            CollapseImmediately(_expandedBorder);
        }

        _expandedBorder = border;

        ToggleDetails(
            border,
            true);

        RaiseContainerZIndex(
            border,
            HoveredZIndex);

        AnimateCard(
            border,
            ExpandedTranslateX,
            ExpandedWidth,
            ExpandedHeight,
            EasingMode.EaseOut);
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

        ToggleDetails(
            border,
            false);

        RaiseContainerZIndex(
            border,
            DefaultZIndex);

        AnimateCard(
            border,
            CollapsedTranslateX,
            CollapsedWidth,
            CollapsedHeight,
            EasingMode.EaseIn);
    }

    private void CollapseImmediately(
        Border border)
    {
        RaiseContainerZIndex(
            border,
            DefaultZIndex);

        ResetCardVisualState(
            border);
    }

    private static void ResetCardVisualState(
        Border border)
    {
        border.Width =
            CollapsedWidth;

        border.Height =
            CollapsedHeight;

        if (border.RenderTransform is TranslateTransform transform)
        {
            transform.X =
                CollapsedTranslateX;
        }

        ToggleDetails(
            border,
            false);
    }

    private static void ToggleDetails(
        Border border,
        bool isExpanded)
    {
        if (border.Child is not Grid grid)
            return;

        if (grid.FindName("NoteDetails") is StackPanel details)
        {
            details.Visibility =
                isExpanded
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        if (grid.FindName("NoteHole") is Ellipse hole)
        {
            hole.Visibility =
                isExpanded
                    ? Visibility.Collapsed
                    : Visibility.Visible;
        }
    }

    private void RaiseContainerZIndex(
        Border border,
        int zIndex)
    {
        var item =
            FindListViewItem(border);

        if (item is not null)
        {
            Canvas.SetZIndex(
                item,
                zIndex);
        }
    }

    private static ListViewItem FindListViewItem(
        DependencyObject element)
    {
        var current = element;

        while (current is not null)
        {
            if (current is ListViewItem item)
                return item;

            current =
                VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    private static void AnimateCard(
        Border border,
        double targetTranslateX,
        double targetWidth,
        double targetHeight,
        EasingMode easingMode)
    {
        if (border.RenderTransform is not TranslateTransform transform)
        {
            transform = new TranslateTransform();
            border.RenderTransform = transform;
        }

        var duration =
            new Duration(
                TimeSpan.FromMilliseconds(180));

        var easing =
            new CubicEase
            {
                EasingMode = easingMode
            };

        var offsetAnimation =
            new DoubleAnimation
            {
                To = targetTranslateX,
                Duration = duration,
                EasingFunction = easing,
                EnableDependentAnimation = true
            };

        Storyboard.SetTarget(
            offsetAnimation,
            transform);

        Storyboard.SetTargetProperty(
            offsetAnimation,
            "X");

        var widthAnimation =
            new DoubleAnimation
            {
                To = targetWidth,
                Duration = duration,
                EasingFunction = easing,
                EnableDependentAnimation = true
            };

        Storyboard.SetTarget(
            widthAnimation,
            border);

        Storyboard.SetTargetProperty(
            widthAnimation,
            "Width");

        var heightAnimation =
            new DoubleAnimation
            {
                To = targetHeight,
                Duration = duration,
                EasingFunction = easing,
                EnableDependentAnimation = true
            };

        Storyboard.SetTarget(
            heightAnimation,
            border);

        Storyboard.SetTargetProperty(
            heightAnimation,
            "Height");

        var storyboard =
            new Storyboard();

        storyboard.Children.Add(
            offsetAnimation);

        storyboard.Children.Add(
            widthAnimation);

        storyboard.Children.Add(
            heightAnimation);

        storyboard.Begin();
    }
}