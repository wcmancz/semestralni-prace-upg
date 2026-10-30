using Avalonia;
using Avalonia.Media;
using System.Collections.Generic;
using System.Globalization;
using System;

namespace SpaceTraffic;

public class LabelLayer
{
    private class Label
    {
        public Point Anchor;
        public Rect Box;
        public FormattedText Formatted = null!;
        public double? Ratio;
        public IBrush? BarBrush;
    }

    private readonly List<Label> labels = new();
    private static readonly Typeface Face = new("Inter");
    private static readonly IBrush Background = new SolidColorBrush(Color.FromArgb(200, 30, 30, 30));
    private static readonly Pen Border = new(Brushes.Gray, 1);

    private const double BarHeight = 6;
    private const double BarGap = 4;

    public void Clear() => labels.Clear();

    public void Add(string text, Point anchor, double? ratio = null, IBrush? barBrush = null)
    {
        var formatted = new FormattedText(text, CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight, Face, 11, Brushes.White);

        double extra = ratio.HasValue ? BarHeight + BarGap : 0;

        var origin = new Point(anchor.X + 10, anchor.Y - formatted.Height - extra - 10);
        var box = new Rect(origin.X - 4, origin.Y - 2, formatted.Width + 8, formatted.Height + extra + 4);

        labels.Add(new Label 
        { 
            Anchor = anchor, 
            Box = box, 
            Formatted = formatted,
            Ratio = ratio,
            BarBrush = barBrush,    
        });
    }

    // rozmístí a vykreslí
    public void Draw(DrawingContext context, Size bounds)
    {
        Layout(bounds);

        foreach (var label in labels)
        {
            context.DrawRectangle(Background, Border, label.Box, 4, 4);
            context.DrawLine(Border, label.Anchor, new Point(label.Box.Left, label.Box.Bottom));
            context.DrawText(label.Formatted, new Point(label.Box.X + 4, label.Box.Y + 2));
            if (label.Ratio.HasValue)
            {
                double ratio = Math.Clamp(label.Ratio.Value, 0, 1);
                double barWidth = label.Box.Width - 8;
                var frame = new Rect(label.Box.X + 4, label.Box.Bottom - BarHeight - 3, barWidth, BarHeight);
                var fill = new Rect(frame.X, frame.Y, barWidth * ratio, BarHeight);

                context.DrawRectangle(label.BarBrush ?? Brushes.LimeGreen, null, fill);
                context.DrawRectangle(null, Border, frame);
            }
        }
    }

    private void Layout(Size bounds)
    {
        var placed = new List<Rect>();

        foreach (var label in labels)
        {
            var box = Clamp(label.Box, bounds);

            bool moved = true;
            int guard = 0;

            while (moved && guard++ < 50)
            {
                moved = false;
                foreach (var other in placed)
                {
                    if (box.Intersects(other))
                    {
                        box = Clamp(box.WithY(other.Bottom + 2), bounds);
                        moved = true;
                    }
                }
            }

            label.Box = box;
            placed.Add(box);
        }
    }

    private static Rect Clamp(Rect box, Size bounds)
    {
        double x = Math.Max(0, Math.Min(box.X, bounds.Width - box.Width));
        double y = Math.Max(0, Math.Min(box.Y, bounds.Height - box.Height));
        return new Rect(x, y, box.Width, box.Height);
    }
}