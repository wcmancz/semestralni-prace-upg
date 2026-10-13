namespace DefaultNamespace;

public class Ship
{
    private string Name { get; }
    private double Speed { get; }
    private SpaceBalls Start { get; }
    private int Capacity { get; }
    private int Transfer_speed { get; }
    private string Strategy { get; }
    private int Max_loadtime { get; }

    public Ship(string name, double speed, SpaceBalls start, int capacity, int transferSpeed, string strategy, int maxLoadtime)
    {
        Name = name;
        Speed = speed;
        Start = start;
        Capacity = capacity;
        Transfer_speed = transferSpeed;
        Strategy = strategy;
        Max_loadtime = maxLoadtime;
    }
}