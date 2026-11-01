using DefaultNamespace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SpaceTraffic;

public class TimeSimulation
{
    public List<SpaceBodies> Bodies { get; }
    public List<Ship> Ships { get; }
    public List<Destination> Destinations { get; }

    public double Day { get; private set; }
    public double FurthestOrbit { get; }
    public double SizeFactor { get; }

    public static readonly DateTime StartDate = new(2026, 9, 21);
    public DateTime CurrentDate => StartDate.AddDays(Day);

    private readonly Stopwatch clock = new();
    private readonly Random rng = new();
    private double dayOffset;
    private double lastDay;
    private int lastDayInt;
    
    public TimeSimulation(IEnumerable<SpaceBodies> bodies, IEnumerable<Ship> ships, IEnumerable<Destination> dests)
    {
        Bodies = bodies.ToList();
        Ships = ships.ToList();
        Destinations = dests.ToList();
        FurthestOrbit = Bodies.Max(b => b.GetMaxDistance()) + 0.5;
        SizeFactor = Bodies
            .Where(body => body.GetParent() != null && body.GetOrbitRadius() > 0)
            .Min(body => body.GetOrbitRadius() / (body.GetParent()!.GetSize() + body.GetSize())) * 0.7;
    }

    public void Start() => clock.Restart();

    public void Pause()
    {
        dayOffset = Day;
        clock.Reset();
    }

    public void Tick()
    {
        if (!clock.IsRunning) return;

        Day = dayOffset + clock.Elapsed.TotalSeconds;

        int today = (int)Day;
        while (lastDayInt < today)
        {
            lastDayInt++;
            foreach (var body in  Bodies) {
                body.Produce(rng);
            }
            foreach (var ship in Ships)
            {
                ship.DailyStep(Destinations, rng, lastDayInt);
            }
        }
        double dt = Math.Max(0, Day - lastDay);
        lastDay = Day;
        foreach (var ship in Ships)
        {
            ship.Update(Day, dt);
        }
    }
}