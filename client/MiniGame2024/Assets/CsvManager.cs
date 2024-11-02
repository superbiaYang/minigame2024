using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVData
{
    private Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

    public void LoadData(string csvText)
    {
        StringReader reader = new StringReader(csvText);
        string[] headers = reader.ReadLine().Split(',');
        string line;

        while ((line = reader.ReadLine()) != null)
        {
            string[] fields = line.Split(',');
            Dictionary<string, string> entry = new Dictionary<string, string>();

            for (int i = 0; i < headers.Length; i++)
            {
                entry[headers[i]] = fields[i];
            }

            data[fields[0]] = entry;
        }
    }

    public Dictionary<string, Dictionary<string, string>> GetAllData()
    {
        return data;
    }

    public Dictionary<string, string> GetIdData(string id)
    {
        return data[id];
    }

    public string GetData(string id, string field)
    {
        return data[id][field];
    }
}

public class CsvManager
{

    private Dictionary<string, CSVData> csvDatas = new Dictionary<string, CSVData>();

    private static CsvManager _Instance;

    public static CsvManager Instance
    {
        get
        {
            if(_Instance == null)
            {
                _Instance = new CsvManager();
            }
            return _Instance;
        }
    }

    public void LoadCSV(string filename)
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Configs/"+filename);
        CSVData csvData = new CSVData();
        csvData.LoadData(csvFile.text);
        csvDatas[filename] = csvData;
    }

    public CSVData GetCsvData(string filename)
    {
        if (!csvDatas.ContainsKey(filename))
        {
            LoadCSV(filename);
        }
        return  csvDatas[filename];
    }

    public string GetData(string filename, string id, string field)
    {
        if (!csvDatas.ContainsKey(filename))
        {
            LoadCSV(filename);
        }
        return csvDatas[filename].GetData(id, field);
    }
}
