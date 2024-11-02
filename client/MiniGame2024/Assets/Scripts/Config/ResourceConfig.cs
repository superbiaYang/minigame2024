using System.Collections.Generic;
using UnityEngine;
public class ResourceConfig
{
    public int m_Type;
    public int m_DestoryEnergy;
    public int m_Coin;
    public int m_Mass;
}
public class ResourceConfigs
{
    private Dictionary<int, ResourceConfig> m_Configs;
    public ResourceConfigs()
    {
        m_Configs = new Dictionary<int, ResourceConfig>();
        Dictionary<string,Dictionary<string,string>> data = CsvManager.Instance.GetCsvData(CSV.Resource).GetAllData();
        foreach(KeyValuePair<string, Dictionary<string,string>> kvp in data)
        {
            var config = new ResourceConfig();
            config.m_Type = int.Parse(kvp.Value["Type"]);
            config.m_DestoryEnergy = int.Parse(kvp.Value["DestoryEnergy"]);
            config.m_Coin = int.Parse(kvp.Value["Coin"]);
            config.m_Mass = int.Parse(kvp.Value["Mass"]);
            m_Configs.Add(config.m_Type, config);
        }
    }

    public ResourceConfig GetConfig(int type)
    {
        if (!m_Configs.ContainsKey(type))
        {
            Debug.LogError($"Invalid type {type}");
            return null;
        }
        return m_Configs[type];
    }
}