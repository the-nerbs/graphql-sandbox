namespace ServiceA;

public record ComponentInfo(string Name, string Version);

public record AlphaData(double x, double y, string name);

public class Query
{
    public IEnumerable<AlphaData> GetData()
    {
        return new[]
        {
            new AlphaData(0, 0, "origin"),
            new AlphaData(-1, -1, "alpha-1"),
            new AlphaData(1, 1, "alpha+1"),
        };
    }

    public ComponentInfo GetComponentInfo() {
        return new ComponentInfo("Service Alpha", "1.2.3.4");
    }
}