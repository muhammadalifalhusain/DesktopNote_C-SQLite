
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;

namespace DesktopNote.Helpers;

public static class NoteCardHelper
{
    public const double CollapsedWidth = 179;
    public const double CollapsedHeight = 76;

    public const double ExpandedWidth = 360;
    public const double ExpandedHeight = 160;

    public const double CollapsedTranslateX = 135;
    public const double ExpandedTranslateX = 0;

    public const int HoveredZIndex = 100;
    public const int DefaultZIndex = 0;

    public static void Expand(
        Border border,
        Border? previousBorder)
    {
        if (previousBorder is not null &&
            !ReferenceEquals(previousBorder, border))
        {
            CollapseImmediately(previousBorder);
        }

        SetDetailsVisibility(
            border,
            true);

        SetZIndex(
            border,
            HoveredZIndex);

        Animate(
            border,
            ExpandedTranslateX,
            ExpandedWidth,
            ExpandedHeight,
            EasingMode.EaseOut);
    }

    public static void Collapse(
        Border border)
    {
        SetDetailsVisibility(
            border,
            false);

        SetZIndex(
            border,
            DefaultZIndex);

        Animate(
            border,
            CollapsedTranslateX,
            CollapsedWidth,
            CollapsedHeight,
            EasingMode.EaseIn);
    }

    public static void CollapseImmediately(
        Border border)
    {
        SetZIndex(
            border,
            DefaultZIndex);

        Reset(border);
    }

    public static void Reset(
        Border border)
    {
        border.Width =
            CollapsedWidth;

        border.Height =
            CollapsedHeight;

        if (border.RenderTransform
            is TranslateTransform transform)
        {
            transform.X =
                CollapsedTranslateX;
        }

        SetDetailsVisibility(
            border,
            false);

        SetZIndex(
            border,
            DefaultZIndex);
    }

    private static void SetDetailsVisibility(
        Border border,
        bool expanded)
    {
        if (border.Child is not Grid grid)
            return;

        if (grid.FindName("NoteDetails")
            is StackPanel details)
        {
            details.Visibility =
                expanded
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
    }

    private static void SetZIndex(
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

    private static ListViewItem? FindListViewItem(
        DependencyObject element)
    {
        DependencyObject? current =
            element;

        while (current is not null)
        {
            if (current is ListViewItem item)
                return item;

            current =
                VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    private static void Animate(
        Border border,
        double translateX,
        double width,
        double height,
        EasingMode easingMode)
    {
        if (border.RenderTransform
            is not TranslateTransform transform)
        {
            transform =
                new TranslateTransform();

            border.RenderTransform =
                transform;
        }

        var duration =
            new Duration(
                TimeSpan.FromMilliseconds(180));

        var easing =
            new CubicEase
            {
                EasingMode = easingMode
            };

        var storyboard =
            new Storyboard();

        var positionAnimation =
            new DoubleAnimation
            {
                To = translateX,
                Duration = duration,
                EasingFunction = easing,
                EnableDependentAnimation = true
            };

        Storyboard.SetTarget(
            positionAnimation,
            transform);

        Storyboard.SetTargetProperty(
            positionAnimation,
            "X");

        var widthAnimation =
            new DoubleAnimation
            {
                To = width,
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
                To = height,
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

        storyboard.Children.Add(
            positionAnimation);

        storyboard.Children.Add(
            widthAnimation);

        storyboard.Children.Add(
            heightAnimation);

        storyboard.Begin();
    }
}