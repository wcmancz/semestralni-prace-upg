namespace DefaultNamespace;

public class Ship
{
    private string name;
    private double speed;
    private SpaceBalls start;
    private int capacity;
    private int transfer_speed;
    private string strategy;
    private int max_loadtime;

    public Ship(string name, double speed, SpaceBalls start, int capacity, int transferSpeed, string strategy, int maxLoadtime)
    {
        this.name = name;
        this.speed = speed;
        this.start = start;
        this.capacity = capacity;
        this.transfer_speed = transferSpeed;
        this.strategy = strategy;
        this.max_loadtime = maxLoadtime;
    }
}