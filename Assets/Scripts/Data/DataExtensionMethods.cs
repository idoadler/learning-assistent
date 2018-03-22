using UnityEngine;

public static class DataExtensionMethods
{
    public static string FullPath(this string path)
    {
#if UNITY_EDITOR        
        return Application.dataPath + path;
#elif UNITY_ANDROID
        return "jar:file://" + Application.dataPath + "!/assets/" + path;
#endif
    }
}
