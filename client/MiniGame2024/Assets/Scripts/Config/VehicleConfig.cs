using System.Collections.Generic;
using UnityEngine;
public class VehicleConfig
{
    public int m_Type;
    public string m_Name;
    public int m_Level;
    public int m_Hp;
    public int m_Mass;
    public int m_Acceleration;
    public int m_MaxSpeed;
    public float m_DmgReduction;
    public float m_Drag;
    public int m_TurnSpeed;
    public int m_AutoRecover;
}
public class VehicleConfigs
{
    private Dictionary<int, Dictionary<int, VehicleConfig>> m_Configs;
    public VehicleConfigs()
    {
        m_Configs = new Dictionary<int, Dictionary<int, VehicleConfig>>();
        Dictionary<string,Dictionary<string,string>> data = CsvManager.Instance.GetCsvData(CSV.Vehicle).GetAllData();
        foreach(KeyValuePair<string, Dictionary<string,string>> kvp in data)
        {
            var config = new VehicleConfig();
            config.m_Type = int.Parse(kvp.Value["Type"]);
            config.m_Name = kvp.Value["Name"];
            config.m_Level = int.Parse(kvp.Value["Level"]);
            config.m_Hp = int.Parse(kvp.Value["Hp"]);
            config.m_Mass = int.Parse(kvp.Value["Mass"]);
            config.m_Acceleration = int.Parse(kvp.Value["Acceleration"]);
            config.m_MaxSpeed = int.Parse(kvp.Value["MaxSpeed"]);
            config.m_DmgReduction = float.Parse(kvp.Value["DmgReduction"]);
            config.m_Drag = float.Parse(kvp.Value["Drag"]);
            config.m_TurnSpeed = int.Parse(kvp.Value["TurnSpeed"]);
            config.m_AutoRecover = int.Parse(kvp.Value["AutoRecover"]);
            
            if (!m_Configs.ContainsKey(config.m_Type))
            {
                m_Configs.Add(config.m_Type, new Dictionary<int, VehicleConfig>());
            }
            m_Configs[config.m_Type].Add(config.m_Level, config);
        }
    }

    public VehicleConfig GetConfig(int type, int level)
    {
        if (!m_Configs.ContainsKey(type))
        {
            Debug.LogError($"Invalid type {type}");
            return null;
        }
        var typeConfig = m_Configs[type];
        if (!typeConfig.ContainsKey(level))
        {
            Debug.LogError($"Invalid level {level}");
            return null;
        }
        return typeConfig[level];
    }

    public int GetNextLevelRequiredMass(int type, int curLevel)
    {
        
        if (!m_Configs.ContainsKey(type))
        {
            Debug.LogError($"Invalid type {type}");
            return int.MaxValue;
        }
        var typeConfig = m_Configs[type];
        if (!typeConfig.ContainsKey(curLevel+1))
        {
            return int.MaxValue;
        }
        return typeConfig[curLevel+1].m_Mass;
    }
}