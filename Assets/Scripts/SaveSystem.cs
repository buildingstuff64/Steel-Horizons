using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using System.IO;

namespace Assets.Scripts
{
    public class SaveSystem 
    {
        private static string path = Application.persistentDataPath + "/MapSettings.json";

        private static string getPath(string filename)
        {
            return string.Format("{0}/{1}.json", Application.persistentDataPath, filename);
        }

        public static void SaveProcedural(ProceduralData data)
        {
            string json = JsonUtility.ToJson(data);
            Debug.Log(path);
            File.WriteAllText(path, json);
        }

        public static void LoadProcedural(ProceduralData data)
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                JsonUtility.FromJsonOverwrite(json, data);
            }
        }

        public static void Save(object data, string filename)
        {
            string json = JsonUtility.ToJson((object)data);
            File.WriteAllText (getPath(filename), json);
        }

        public static object Load<T>(string filename)
        {
            if (File.Exists(getPath(filename)))
            {
                string json = File.ReadAllText (getPath(filename));
                return JsonUtility.FromJson<T>(json);
            }
            return null;
        }
    }
}