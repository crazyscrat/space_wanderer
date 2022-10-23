using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

public static class SaveLoader
{
    private const string fileName = "data.save";
    
    public static bool Save(UserData data)
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            Stream stream = File.Open(path, FileMode.OpenOrCreate);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
            stream.Close();
        
            Debug.Log("Save data");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    } 
    
    public static UserData Load()
    {
        UserData data = new UserData();
        string path = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log(path);

        if (File.Exists(path))
        {
            Stream stream = File.Open(path, FileMode.OpenOrCreate);
            BinaryFormatter formatter = new BinaryFormatter();

            data = (UserData)formatter.Deserialize(stream);
        }

        return data;
    }

    public static bool Clear()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);

            return true;
        }
        return false;
    }
}