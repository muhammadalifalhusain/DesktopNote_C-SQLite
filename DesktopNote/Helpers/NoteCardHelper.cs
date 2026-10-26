using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Runtime.CompilerServices;

namespace DesktopNote.Helpers;

public static class NoteCardHelper
{
    public const double CollapsedWidth = 44;
    public const double CollapsedHeight = 76;

    public const double ExpandedWidth = 360;
    public const double ExpandedHeight = 160;

    public const double CollapsedFontSize = 10;
    public const double ExpandedFontSize = 16;

    public const int HoveredZIndex = 100;
    public const int DefaultZIndex = 0;

    private static readonly ConditionalWeakTable<Border, Storyboard> Storyboards = new();

    private static Border? _activeBorder;

    public static void Expand(
        Border border,
        Border? previousBorder,
        Action<double>? onWidthChanged = null)
    {
        if (previousBorder is not null &&
            !ReferenceEquals(previousBorder, border))
        {
            CollapseImmediately(previousBorder);
        }

        _activeBorder = border;

        onWidthChanged?.Invoke(ExpandedWidth);

        SetDetailsVisibility(border, true);
        SetZIndex(border, HoveredZIndex);

        Animate(
            border,
            ExpandedWidth,
            ExpandedHeight,
            ExpandedFontSize,
            EasingMode.EaseOut,
            null);
    }

    public static void Collapse(
        Border border,
        Action<double>? onWidthChanged = null)
    {
        if (ReferenceEquals(_activeBorder, border))
        {
            _activeBorder = null;
        }

        SetDetailsVisibility(border, false);
        SetZIndex(border, DefaultZIndex);

        Animate(
            border,
            CollapsedWidth,
            CollapsedHeight,
            CollapsedFontSize,
            EasingMode.EaseIn,
            () =>
            {
                if (_activeBorder is null)
                {
                    onWidthChanged?.Invoke(CollapsedWidth);
                }
            });
    }

    public static void CollapseImmediately(
        Border border)
    {
        if (ReferenceEquals(_activeBorder, border))
        {
            _activeBorder = null;
        }

        Reset(border);
    }

    public static void Release(
        Action<double>? onWidthChanged = null)
    {
        _activeBorder = null;

        onWidthChanged?.Invoke(CollapsedWidth);
    }

    public static void Reset(
        Border border)
    {
        if (Storyboards.TryGetValue(border, out var running))
        {
            running.Stop();
            Storyboards.Remove(border);
        }

        border.Width = CollapsedWidth;
        border.Height = CollapsedHeight;

        if (FindTypeText(border) is TextBlock typeText)
        {
            typeText.FontSize = CollapsedFontSize;
        }

        SetDetailsVisibility(border, false);
        SetZIndex(border, DefaultZIndex);
    }

    private static void SetDetailsVisibility(
        Border border,
        bool expanded)
    {
        if (border.Child is not Grid grid)
            return;

        if (grid.FindName("NoteDetails") is StackPanel details)
        {
            details.Visibility =
                expanded
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
    }

    private static TextBlock? FindTypeText(
        Border border)
    {
        if (border.Child is not Grid grid)
            return null;

        return grid.FindName("NoteTypeText") as TextBlock;
    }

    private static void SetZIndex(
        Border border,
        int zIndex)
    {
        var item = FindListViewItem(border);

        if (item is not null)
        {
            Canvas.SetZIndex(item, zIndex);
        }
    }

    private static ListViewItem? FindListViewItem(
        DependencyObject element)
    {
        DependencyObject? current = element;

        while (current is not null)
        {
            if (current is ListViewItem item)
                return item;

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    private static DoubleAnimation CreateAnimation(
        DependencyObject target,
        string property,
        double to,
        Duration duration,
        EasingFunctionBase easing)
    {
        var animation = new DoubleAnimation
        {
            To = to,
            Duration = duration,
            EasingFunction = easing,
            EnableDependentAnimation = true
        };

        Storyboard.SetTarget(animation, target);
        Storyboard.SetTargetProperty(animation, property);

        return animation;
    }

    private static void Animate(
        Border border,
        double width,
        double height,
        double fontSize,
        EasingMode easingMode,
        Action? onCompleted)
    {
        var duration = new Duration(TimeSpan.FromMilliseconds(180));

        var easing = new CubicEase
        {
            EasingMode = easingMode
        };

        var storyboard = new Storyboard();

        storyboard.Children.Add(
            CreateAnimation(border, "Width", width, duration, easing));

        storyboard.Children.Add(
            CreateAnimation(border, "Height", height, duration, easing));

        var typeText = FindTypeText(border);

        if (typeText is not null)
        {
            storyboard.Children.Add(
                CreateAnimation(typeText, "FontSize", fontSize, duration, easing));
        }

        Storyboards.Remove(border);
        Storyboards.Add(border, storyboard);

        storyboard.Completed += (_, _) =>
        {
            if (!Storyboards.TryGetValue(border, out var current) ||
                !ReferenceEquals(current, storyboard))
            {
                return;
            }

            storyboard.Stop();
            Storyboards.Remove(border);

            border.Width = width;
            border.Height = height;

            if (typeText is not null)
            {
                typeText.FontSize = fontSize;
            }

            onCompleted?.Invoke();
        };

        storyboard.Begin();
    }
}