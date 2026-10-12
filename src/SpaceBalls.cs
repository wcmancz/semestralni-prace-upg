namespace DefaultNamespace;

public class SpaceBalls
{
    private string name;
    private string type;
    private double size;
    private SpaceBalls parent_ball;
    private double orbit_radius;
    private double initial_angle;
    private double angular_speed;
    private int warehouse;
    private int prod_min;
    private int prod_max;

    public SpaceBalls(string name, string type, double size, SpaceBalls parent_ball, double orbit_radius, double initial_angle, double angular_speed, int warehouse, int prod_min, int prod_max)
    {
        this.name = name;
        this.type = type;
        this.size = size;
        this.parent_ball = parent_ball;
        this.orbit_radius = orbit_radius;
        this.initial_angle = initial_angle;
        this.angular_speed = angular_speed;
        this.warehouse = warehouse;
        this.prod_min = prod_min;
        this.prod_max = prod_max;
    }
    
}