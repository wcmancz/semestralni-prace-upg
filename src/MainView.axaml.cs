using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;

using DefaultNamespace;

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Diagnostics;

namespace SpaceTraffic;

public partial class MainView: UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var rect = new Rect(Bounds.Size);
        context.FillRectangle(Brushes.Black, rect);

	//TODO: vykreslen� vesmirnych teles a transportnich lodi v aktualnim case
        SpaceObjects(context, bodies, day);
        DrawTime(context);
    }

    private List<SpaceBalls> bodies = new();
    private List<Ship> ships = new();

    private readonly Stopwatch clock = new();
    private DispatcherTimer? timer;
    private double day;
    private double dayOffset;

    private double furthest_orbit = 0;

    private static readonly DateTime StartDate = new DateTime(2026, 9, 21);

    public void SetData(IEnumerable<SpaceBalls> bodies, IEnumerable<Ship> ships)
    {
        this.bodies = bodies.ToList();
        this.ships = ships.ToList();
        InvalidateVisual();
    }

    // vykrslování

    private void SpaceObjects(DrawingContext context, IEnumerable<SpaceBalls> objs, double day)
    {
        foreach (var obj in objs)
        {
            if (obj.GetOrbitRadius() > furthest_orbit)
            {
                furthest_orbit = obj.GetOrbitRadius() + 2;
            }
        }

        var pen_orbit = new Pen(Colors.White.ToUInt32(), 1);
        var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
        double scale = Math.Min(Bounds.Width, Bounds.Height) / 2 / furthest_orbit; // zatím pevně daná vzálenost

        foreach (var obj in objs)
        {
            var (x, y) = obj.GetPositionAt(day);
            var position = new Point(center.X + x * scale, center.Y + y * scale);

            double planet_radius = obj.GetSize() * scale * 0.2;
            double planet_orbit_radius = obj.GetOrbitRadius() * scale;

            var parent = obj.GetParent();
            if (parent != null && obj.GetBallType() != "asteroid")
            {
                var (px, py) = parent.GetPositionAt(day);
                var orbit_position = new Point(center.X + px * scale, center.Y + py * scale);

                context.DrawEllipse(null, pen_orbit, orbit_position, planet_orbit_radius, planet_orbit_radius);
            }

            IBrush planet_brush = obj.GetBallType() switch
            {
                "planet" => obj.GetName() switch
                {
                    "Earth" => Brushes.Green,
                    "Mars" => Brushes.Red,
                    "Jupiter" or "Saturn" => Brushes.Orange,
                    _ => Brushes.Black
                },
                "moon" => Brushes.Gray,
                "star" => Brushes.Gold,
                "asteroid" => Brushes.Brown,
                _ => Brushes.Black
            };

            context.DrawEllipse(planet_brush, null, position, planet_radius, planet_radius);


        }
    }

    // časy a hraní s časem

    public void Start()
    {
        if (timer != null) return;

        clock.Restart();
        timer = new DispatcherTimer() {  Interval = TimeSpan.FromMilliseconds(1) };
        timer.Tick += (_, _) =>
        {
            day = dayOffset + clock.Elapsed.TotalSeconds;
            InvalidateVisual();
        };
        timer.Start();
    }

    public void Stop()
    {
        if (timer == null) return;

        timer.Stop();
        timer = null;
        dayOffset = day;
    }

    private void DrawTime(DrawingContext context)
    {
        DateTime current = StartDate.AddDays(day);
        string text = $"{current:d. M. yyyy}";

        var formatted = new FormattedText(
            text,
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface("Inter"),
            16,
            Brushes.White);

        context.DrawText(formatted, new Point(10, 10));
    }
}