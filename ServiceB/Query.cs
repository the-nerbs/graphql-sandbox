namespace ServiceB;

public record ComponentInfo(string Name, string Version);

public record BravoData(double x, double y, int id);

public class Query
{
    public IEnumerable<BravoData> GetData()
    {
        return new[]
        {
            new BravoData( 0,  0, 0),
            new BravoData(-1, -1, 1),
            new BravoData( 1,  1, 2),
        };
    }

    public ComponentInfo GetComponentInfo() {
        return new ComponentInfo("Service Bravo", "2.3.4.5");
    }
}