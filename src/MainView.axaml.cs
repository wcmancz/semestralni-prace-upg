using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

using DefaultNamespace;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceTraffic;

public partial class MainView: UserControl
{

    private TimeSimulation? simulation;
    private DispatcherTimer? timer;
    private readonly LabelLayer labels = new();
    private readonly SpaceObjects objects;

    public MainView()
    {
        InitializeComponent();
        objects = new SpaceObjects(labels);
    }

    public void SetData(IEnumerable<SpaceBalls> bodies, IEnumerable<Ship> ships, IEnumerable<Destination> dests)
    {
        simulation = new TimeSimulation(bodies, ships, dests);
        InvalidateVisual();
    }

    public void Start()
    {
        if (simulation == null || timer != null) return;

        simulation.Start();
        timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(16) };
        timer.Tick += (_, _) =>
        {
            simulation.Tick();
            InvalidateVisual();
        }; // znovu
        timer.Start();
    }

    public void Stop() // na druhou část
    {
        if (timer == null) return;

        timer.Stop();
        timer = null;
        simulation?.Pause();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        labels.Clear();

        var rect = new Rect(Bounds.Size);
        context.FillRectangle(Brushes.Black, rect);

        if (simulation == null) return;

        var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
        double scale = Math.Min(Bounds.Width, Bounds.Height) / 2 / simulation.FurthestOrbit;
        double maxWarehouse = Math.Max(1, simulation.Bodies.Max(body => body.GetWarehouse()));

        objects.DrawBodies(context, simulation.Bodies, simulation.Day, center, scale, maxWarehouse, simulation.SizeFactor);
        objects.DrawShips(context, simulation.Ships, center, scale);
        labels.Draw(context, Bounds.Size);
        objects.DrawDate(context, simulation.CurrentDate);
    }

}