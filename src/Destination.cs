namespace DefaultNamespace;

public class Destination
{
    private SpaceBalls source;
    private SpaceBalls destination;
    private int probability;
    
    public Destination(SpaceBalls sourcePlanet, SpaceBalls destPlanet, int prob)
    {
        this.source = sourcePlanet;
        this.destination = destPlanet;
        this.probability = prob;
    }
}