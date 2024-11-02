
public static class CSV
{
    public const string Vehicle = "Vehicle";
    public const string Resource = "Resource";
}

public class Config
{
    private static Config _Instance;

    public static Config Instance
    {
        get
        {
            if(_Instance == null)
            {
                _Instance = new Config();
            }
            return _Instance;
        }
    }

    private Config()
    {
        m_Vehicle = new VehicleConfigs();
        m_Resource = new ResourceConfigs();
    }

    public VehicleConfigs m_Vehicle;
    public ResourceConfigs m_Resource;
}