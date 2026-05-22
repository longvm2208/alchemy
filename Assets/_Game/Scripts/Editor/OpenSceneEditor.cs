using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

public class OpenSceneEditor
{
    const string PathToScenesFolder = "Assets/_Game/Scenes/";

    [MenuItem("Open Scene/Load #1", priority = 1)]
    public static void OpenLoad()
    {
        OpenScene("Load");
    }

    [MenuItem("Open Scene/Load #1", true, priority = 1)]
    public static bool OpenLoadValidate()
    {
        return OpenSceneValidate("Load");
    }

    [MenuItem("Open Scene/Game #2", priority = 2)]
    public static void OpenGame()
    {
        OpenScene("Game");
    }

    [MenuItem("Open Scene/Game #2", true, priority = 2)]
    public static bool OpenGameValidate()
    {
        return OpenSceneValidate("Game");
    }

    [MenuItem("Open Scene/Test #3", priority = 101)]
    public static void OpenTest()
    {
        OpenScene("Test");
    }

    [MenuItem("Open Scene/Test #3", true, priority = 101)]
    public static bool OpenTestValidate()
    {
        return OpenSceneValidate("Test");
    }

    static void OpenScene(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(PathToScenesFolder + sceneName + ".unity");
        }
    }

    static bool OpenSceneValidate(string sceneName)
    {
        return File.Exists(PathToScenesFolder + sceneName + ".unity");
    }
}