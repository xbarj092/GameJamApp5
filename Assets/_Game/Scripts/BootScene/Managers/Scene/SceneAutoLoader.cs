#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
static class SceneAutoLoader
{
    static SceneAutoLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    [MenuItem("Tools/Scene Autoload/Select Master Scene...")]
    private static void SelectMasterScene()
    {
        string sceneFolderPath = Application.dataPath + "/_Game/Scenes";
        string masterScene = EditorUtility.OpenFilePanel("Select Master Scene", sceneFolderPath, "unity");
		masterScene = masterScene.Replace(Application.dataPath, "Assets");
		if (!string.IsNullOrEmpty(masterScene))
		{
			MasterScene = masterScene;
			LoadMasterOnPlay = true;
		}
    }

    [MenuItem("Tools/Scene Autoload/Load Master On Play", true)]
    private static bool ShowLoadMasterOnPlay()
    {
        return !LoadMasterOnPlay;
    }
    [MenuItem("Tools/Scene Autoload/Load Master On Play")]
    private static void EnableLoadMasterOnPlay()
    {
        LoadMasterOnPlay = true;
    }

    [MenuItem("Tools/Scene Autoload/Don't Load Master On Play", true)]
    private static bool ShowDontLoadMasterOnPlay()
    {
        return LoadMasterOnPlay;
    }
    [MenuItem("Tools/Scene Autoload/Don't Load Master On Play")]
    private static void DisableLoadMasterOnPlay()
    {
        LoadMasterOnPlay = false;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (!LoadMasterOnPlay)
        {
            return;
        }

        if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
        {
            PreviousScene = EditorSceneManager.GetActiveScene().path;
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                try
                {
                    EditorSceneManager.OpenScene(MasterScene);
                }
                catch
                {
                    Debug.LogError(string.Format("SceneAutoLoader error: master scene not found: {0}", MasterScene));
                    EditorApplication.isPlaying = false;

                }
            }
            else
            {
                EditorApplication.isPlaying = false;
            }
        }

        if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            try
            {
                EditorSceneManager.OpenScene(PreviousScene);
            }
            catch
            {
                Debug.LogError(string.Format("SceneAutoLoader error: previously opened scene not found: {0}", PreviousScene));
            }
        }
    }

    private const string cEditorPrefLoadMasterOnPlay = "SceneAutoLoader.LoadMasterOnPlay";
    private const string cEditorPrefMasterScene = "SceneAutoLoader.MasterScene";
    private const string cEditorPrefPreviousScene = "SceneAutoLoader.PreviousScene";

    private static bool LoadMasterOnPlay
    {
        get { return EditorPrefs.GetBool(cEditorPrefLoadMasterOnPlay, false); }
        set { EditorPrefs.SetBool(cEditorPrefLoadMasterOnPlay, value); }
    }

    private static string MasterScene
    {
        get { return EditorPrefs.GetString(cEditorPrefMasterScene, "Master.unity"); }
        set { EditorPrefs.SetString(cEditorPrefMasterScene, value); }
    }

    private static string PreviousScene
    {
        get { return EditorPrefs.GetString(cEditorPrefPreviousScene, EditorSceneManager.GetActiveScene().path); }
        set { EditorPrefs.SetString(cEditorPrefPreviousScene, value); }
    }
}
#endif