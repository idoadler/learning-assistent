using UnityEditor;

[InitializeOnLoad]
public class PreloadSigningAlias
{
    static PreloadSigningAlias()
    {
        PlayerSettings.Android.keystorePass = "mindcet";
        PlayerSettings.Android.keyaliasName = "todobot";
        PlayerSettings.Android.keyaliasPass = "mindcet";
    }
}