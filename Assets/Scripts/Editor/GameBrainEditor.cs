using UnityEngine;
using UnityEditor;
using System.IO;

public class GameBrainEditor : EditorWindow
{
    public BrainData brainData;

    private string brainDataProjectFilePath = "/StreamingAssets/brain.json";

    [MenuItem("Window/Game Brain Editor")]
    static void Init()
    {
        GetWindow(typeof(GameBrainEditor)).Show();
    }

    Vector2 scrollPos;
    void OnGUI()
    {
        scrollPos =
            EditorGUILayout.BeginScrollView(scrollPos);

        if (brainData != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("brainData");
            EditorGUILayout.PropertyField(serializedProperty, true);

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Save data"))
            {
                SaveBrainData();
            }
        }

        if (GUILayout.Button("Load data"))
        {
            LoadBrainData();
        }

        EditorGUILayout.EndScrollView();
    }

    private void LoadBrainData()
    {
        string filePath = brainDataProjectFilePath.FullPath();

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            brainData = JsonUtility.FromJson<BrainData>(dataAsJson);
            brainData.PrepareForDisplay();
        }
        else
        {
            brainData = new BrainData();
        }
    }

    private void SaveBrainData()
    {
//        brainData.PrepareForSave();
        string dataAsJson = JsonUtility.ToJson(brainData);

        string filePath = brainDataProjectFilePath.FullPath();
        File.WriteAllText(filePath, dataAsJson);

    }
}