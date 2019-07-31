using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;                                                        // The System.IO namespace contains functions related to loading and saving files

[System.Serializable]
struct JsonList {
    public Gene[] geneList;
}
public class DataManager : MonoBehaviour 
{
    private static string gameDataFileName = "genes.json";
    [SerializeField]

    void Start() {
        DontDestroyOnLoad(gameObject);
    }
    public static void SaveGeneData(Gene[] population) {

        JsonList geneJson = new JsonList();
        geneJson.geneList = population;
        string dataAsJson = JsonUtility.ToJson(geneJson);

        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);
        File.WriteAllText (filePath, dataAsJson);

    }
    public static Gene[] LoadGeneData() {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);

        if(File.Exists(filePath)) {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath); 
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            JsonList loadedData = JsonUtility.FromJson<JsonList>(dataAsJson);
            return loadedData.geneList;
        }
        else {
            Debug.LogError("Cannot load game data!");
            return null;
        }
    }
}