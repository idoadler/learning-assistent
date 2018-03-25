// Clear all the editor prefs keys.
//
// Warning: this will also remove editor preferences as the opened projects, etc.

using UnityEngine;
using UnityEditor;

public class DeletePlayerPrefs : ScriptableObject
{
    [MenuItem("Window/Clear all Editor Preferences")]
    static void deletePrefs()
    {
        if (EditorUtility.DisplayDialog("Delete all player preferences.",
                "Are you sure you want to delete all the player preferences? " +
                "This action cannot be undone.", "Yes", "No"))
        {
            Debug.Log("prefs deleted");
            PlayerPrefs.DeleteAll();
        }
    }
}