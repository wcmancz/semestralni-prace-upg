namespace DefaultNamespace;

public class Destination
{
    private SpaceBodies Source;
    private SpaceBodies Dest;
    private int Probability;
    
    public Destination(SpaceBodies sourcePlanet, SpaceBodies destPlanet, int prob)
    {
        Source = sourcePlanet;
        Dest = destPlanet;
        Probability = prob;
    }

    public SpaceBodies GetSource() => Source;
    public SpaceBodies GetDest() => Dest;
    public int GetProbability() => Probability;
}