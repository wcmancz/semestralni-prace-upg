using System;

namespace DefaultNamespace;

public class SpaceBodies
{
    private string Name { get; }
    private string Type { get; }
    private double Size { get; }
    private SpaceBodies? Parent_ball { get; set; }
    private double Orbit_radius { get; }
    private double Initial_angle { get; }
    private double Angular_speed { get; }
    private int Warehouse { get; set; }
    private int Prod_min { get; }
    private int Prod_max { get; }

    public SpaceBodies(string name, string type, double size, double orbit_radius, double initial_angle, double angular_speed, int warehouse, int prod_min, int prod_max)
    {
        Name = name;
        Type = type;
        Size = size;
        Orbit_radius = orbit_radius;
        Initial_angle = initial_angle;
        Angular_speed = angular_speed;
        Warehouse = warehouse;
        Prod_min = prod_min;
        Prod_max = prod_max;
    }
    // gettery setter
    public string GetName() => Name;
    public double GetSize() => Size;
    public string GetBallType() => Type;
    public double GetOrbitRadius() => Orbit_radius;
    public SpaceBodies? GetParent() => Parent_ball;
    public int GetWarehouse() => Warehouse;
    public void SetParent(SpaceBodies parent)
    {
        Parent_ball = parent;
    }
    // získání max. vzdálenosti
    public double GetMaxDistance()
    {
        return Orbit_radius + (Parent_ball?.GetMaxDistance() ?? 0);
    }
    //získání pozice x, y v daném čase a pracuje s pozicí rodiče pokud ho má
    public (double X, double Y) GetPositionAt(double time)
    {
        double angle = (Initial_angle + Angular_speed * time) * Math.PI / 180.0;
        double x = Orbit_radius * Math.Cos(angle);
        double y = Orbit_radius * Math.Sin(angle);

        if (Parent_ball != null)
        {
            var (px, py) = Parent_ball.GetPositionAt(time);
            x += px;
            y += py;
        }
        return (x, y);
    }
    // random jednotky
    public void Produce(Random rng)
    {
        Warehouse += rng.Next(Prod_min, Prod_max + 1);
    }

    public void AddToWarehouse(int amount)
    {
        Warehouse += amount;
    }

    public int TakeFromWarehouse(int wanted)
    {
        int taken = Math.Min(wanted, Warehouse);
        Warehouse -= taken;
        return taken;
    }
}