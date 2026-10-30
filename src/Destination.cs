namespace DefaultNamespace;

public class Destination
{
    private SpaceBalls Source;
    private SpaceBalls Dest;
    private int Probability;
    
    public Destination(SpaceBalls sourcePlanet, SpaceBalls destPlanet, int prob)
    {
        Source = sourcePlanet;
        Dest = destPlanet;
        Probability = prob;
    }

    public SpaceBalls GetSource() => Source;
    public SpaceBalls GetDest() => Dest;
    public int GetProbability() => Probability;
}