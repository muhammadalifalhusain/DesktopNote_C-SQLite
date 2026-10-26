using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DesktopNote.Helpers;

public static class NoteCardHelper
{
    public const double CollapsedWidth = 44;
    public const double CollapsedHeight = 76;

    public const double ExpandedWidth = 360;
    public const double ExpandedHeight = 160;

    public const double CollapsedFontSize = 10;
    public const double ExpandedFontSize = 16;

    public const double CornerRadiusValue = 14;
    public const double CollapsedRightRadius = 0;
    public const double ExpandedRightRadius = 14;

    public const double ExpandDurationMs = 260;
    public const double CollapseDurationMs = 220;

    public const int HoveredZIndex = 100;
    public const int DefaultZIndex = 0;

    private sealed class CardAnimation
    {
        public Border Border = null!;
        public TextBlock? TypeText;
        public StackPanel? Details;

        public double FromWidth;
        public double ToWidth;
        public double FromHeight;
        public double ToHeight;
        public double FromFont;
        public double ToFont;
        public double FromRadius;
        public double ToRadius;

        public double DurationMs;
        public long StartTimestamp;
        public bool Expanding;
        public Action? Completed;
    }

    private static readonly Dictionary<Border, CardAnimation> Active = new();

    private static Border? _activeBorder;

    private static bool _rendering;

    public static void Expand(
        Border border,
        Border? previousBorder,
        Action<double>? onWidthChanged = null)
    {
        if (previousBorder is not null &&
            !ReferenceEquals(previousBorder, border))
        {
            Collapse(previousBorder, null);
        }

        _activeBorder = border;

        onWidthChanged?.Invoke(ExpandedWidth);

        SetDetailsVisibility(border, true);
        SetZIndex(border, HoveredZIndex);

        Start(
            border,
            ExpandedWidth,
            ExpandedHeight,
            ExpandedFontSize,
            ExpandedRightRadius,
            true,
            ExpandDurationMs,
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

        Start(
            border,
            CollapsedWidth,
            CollapsedHeight,
            CollapsedFontSize,
            CollapsedRightRadius,
            false,
            CollapseDurationMs,
            () =>
            {
                SetZIndex(border, DefaultZIndex);

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
        Active.Remove(border);

        StopRenderingIfIdle();

        border.Width = CollapsedWidth;
        border.Height = CollapsedHeight;

        ApplyShape(border, CollapsedRightRadius);

        if (FindTypeText(border) is TextBlock typeText)
        {
            typeText.FontSize = CollapsedFontSize;
        }

        if (FindDetails(border) is StackPanel details)
        {
            details.Opacity = 1;
        }

        SetDetailsVisibility(border, false);
        SetZIndex(border, DefaultZIndex);
    }

    private static void Start(
        Border border,
        double width,
        double height,
        double fontSize,
        double rightRadius,
        bool expanding,
        double durationMs,
        Action? completed)
    {
        var typeText = FindTypeText(border);
        var details = FindDetails(border);

        double fromWidth =
            double.IsNaN(border.Width)
                ? border.ActualWidth
                : border.Width;

        double fromHeight =
            double.IsNaN(border.Height)
                ? border.ActualHeight
                : border.Height;

        Active[border] = new CardAnimation
        {
            Border = border,
            TypeText = typeText,
            Details = details,
            FromWidth = fromWidth,
            ToWidth = width,
            FromHeight = fromHeight,
            ToHeight = height,
            FromFont = typeText?.FontSize ?? fontSize,
            ToFont = fontSize,
            FromRadius = border.CornerRadius.TopRight,
            ToRadius = rightRadius,
            DurationMs = durationMs,
            StartTimestamp = Stopwatch.GetTimestamp(),
            Expanding = expanding,
            Completed = completed
        };

        if (expanding && details is not null)
        {
            details.Opacity = 0;
        }

        if (!_rendering)
        {
            CompositionTarget.Rendering += OnRendering;
            _rendering = true;
        }
    }

    private static void OnRendering(
        object? sender,
        object e)
    {
        long now = Stopwatch.GetTimestamp();

        List<CardAnimation>? finished = null;

        foreach (var animation in Active.Values)
        {
            double elapsedMs =
                (now - animation.StartTimestamp) *
                1000.0 /
                Stopwatch.Frequency;

            double t =
                Math.Clamp(
                    elapsedMs / animation.DurationMs,
                    0,
                    1);

            double eased =
                animation.Expanding
                    ? EaseOutQuart(t)
                    : EaseInOutCubic(t);

            Apply(animation, eased, t);

            if (t >= 1)
            {
                finished ??= new List<CardAnimation>();
                finished.Add(animation);
            }
        }

        if (finished is not null)
        {
            foreach (var animation in finished)
            {
                Active.Remove(animation.Border);
            }

            foreach (var animation in finished)
            {
                animation.Completed?.Invoke();
            }
        }

        StopRenderingIfIdle();
    }

    private static void Apply(
        CardAnimation animation,
        double eased,
        double linear)
    {
        var border = animation.Border;

        border.Width =
            Lerp(animation.FromWidth, animation.ToWidth, eased);

        border.Height =
            Lerp(animation.FromHeight, animation.ToHeight, eased);

        ApplyShape(
            border,
            Lerp(animation.FromRadius, animation.ToRadius, eased));

        if (animation.TypeText is not null)
        {
            animation.TypeText.FontSize =
                Lerp(animation.FromFont, animation.ToFont, eased);
        }

        if (animation.Expanding && animation.Details is not null)
        {
            animation.Details.Opacity =
                Math.Clamp((linear - 0.35) / 0.65, 0, 1);
        }
    }

    private static void ApplyShape(
        Border border,
        double rightRadius)
    {
        double radius =
            Math.Clamp(rightRadius, 0, CornerRadiusValue);

        border.CornerRadius =
            new CornerRadius(
                CornerRadiusValue,
                radius,
                radius,
                CornerRadiusValue);

        border.BorderThickness =
            new Thickness(
                1,
                1,
                radius / CornerRadiusValue,
                1);
    }

    private static void StopRenderingIfIdle()
    {
        if (_rendering && Active.Count == 0)
        {
            CompositionTarget.Rendering -= OnRendering;
            _rendering = false;
        }
    }

    private static double Lerp(
        double from,
        double to,
        double t)
    {
        return from + (to - from) * t;
    }

    private static double EaseOutQuart(
        double t)
    {
        return 1 - Math.Pow(1 - t, 4);
    }

    private static double EaseInOutCubic(
        double t)
    {
        return t < 0.5
            ? 4 * t * t * t
            : 1 - Math.Pow(-2 * t + 2, 3) / 2;
    }

    private static void SetDetailsVisibility(
        Border border,
        bool expanded)
    {
        if (FindDetails(border) is StackPanel details)
        {
            details.Visibility =
                expanded
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
    }

    private static StackPanel? FindDetails(
        Border border)
    {
        if (border.Child is not Grid grid)
            return null;

        return grid.FindName("NoteDetails") as StackPanel;
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
}