using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

public static class SaveLoader
{
    private const string fileName = "data.save";
    
    public static async Task<bool> Save(GameStateData data)
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            Stream stream = File.Open(path, FileMode.OpenOrCreate);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
            stream.Close();
        
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    } 
    
    public async static Task<GameStateData> Load()
    {
        GameStateData data = new GameStateData();
        string path = Path.Combine(Application.persistentDataPath, fileName);
        //Debug.Log(path);

        if (File.Exists(path))
        {
            Stream stream = File.Open(path, FileMode.OpenOrCreate);
            BinaryFormatter formatter = new BinaryFormatter();

            data = (GameStateData)formatter.Deserialize(stream);
        }

        return data;
    }

    public static async Task Clear()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}