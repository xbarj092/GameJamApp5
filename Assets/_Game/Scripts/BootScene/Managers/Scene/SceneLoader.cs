using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scenes
    {
        BootScene,
        MenuScene,
        GameScene,
        AttackScene
    }

    private static readonly float MAX_LOAD_PROGRESS = 1f;
    private static Coroutine _loadingCoroutine = null;
    private static AsyncOperation _asyncOp;
    private static Scenes _sceneToLoad;
    private static Scenes _sceneToUnload;
    private static bool _activateAfterLoad;

    private static DummyMonoBehaviour _loaderMonoBehaviour;
    private static DummyMonoBehaviour LoaderMonoBehaviour
    {
        get
        {
            if (_loaderMonoBehaviour == null)
            {
                GameObject loaderGameObject = new GameObject("SceneLoader Game Object");
                _loaderMonoBehaviour = loaderGameObject.AddComponent<DummyMonoBehaviour>();
            }

            return _loaderMonoBehaviour;
        }
    }

    public static event Action<float> OnSceneLoadProgress;
    public static event Action<Scenes> OnSceneLoadDone;
    public static event Action<Scenes> OnSceneUnloadDone;
    public static event Action<Scenes> OnSceneMadeActive;

    public static void LoadScene(Scenes scene, bool additive = true, bool setActive = true, Scenes? toUnload = null, Action onSuccess = null)
    {
        if (toUnload != null)
        {
            UnloadScene((Scenes)toUnload);
        }

        if (_loadingCoroutine != null)
        {
            Debug.LogError("[Scene loader] - Cannot load scene " + scene + ". Scene loading already in progress, multiple scene loading is not currently supported");
            return;
        }

        _sceneToLoad = scene;
        _activateAfterLoad = setActive;
        _loadingCoroutine = LoaderMonoBehaviour.StartCoroutine(LoadSceneCR(_sceneToLoad, additive ? LoadSceneMode.Additive : LoadSceneMode.Single, onSuccess));
    }

    public static void SetSceneAsActive(Scenes scene)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));

        OnSceneMadeActive?.Invoke(scene);
    }

    public static Scenes GetActiveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        Enum.TryParse(activeScene.name, out Scenes currScene);

        return currScene;
    }

    public static bool IsSceneLoaded(Scenes scene)
    {
        int countLoaded = SceneManager.sceneCount;
        for (int i = 0; i < countLoaded; i++)
        {
            Scene currScene = SceneManager.GetSceneAt(i);
            Enum.TryParse(currScene.name, out Scenes sceneToCompare);
            if (scene == sceneToCompare)
            {
                return true;
            }
        }

        return false;
    }

    public static void UnloadScene(Scenes scene)
    {
        _sceneToUnload = scene;
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(scene.ToString());
        if (unloadOp != null)
        {
            unloadOp.completed += SceneUnloadDone;
        }
    }

    private static void SceneUnloadDone(AsyncOperation unloadOp)
    {
        unloadOp.completed -= SceneUnloadDone;

        LoaderMonoBehaviour.StartCoroutine(DelayedUnloadDoneInvoke());
    }

    private static IEnumerator LoadSceneCR(Scenes scene, LoadSceneMode sceneLoadMode, Action onSuccess)
    {
        _asyncOp = SceneManager.LoadSceneAsync(scene.ToString(), sceneLoadMode);
        _asyncOp.completed += SceneLoadingDone;

        while (_asyncOp.progress < MAX_LOAD_PROGRESS)
        {
            OnSceneLoadProgress?.Invoke(_asyncOp.progress);
            yield return null;
        }

        OnSceneLoadProgress?.Invoke(MAX_LOAD_PROGRESS);
        _loadingCoroutine = null;
        onSuccess?.Invoke();
    }

    private static void SceneLoadingDone(AsyncOperation asyncOp)
    {
        _asyncOp.completed -= SceneLoadingDone;
        if (_activateAfterLoad)
        {
            SetSceneAsActive(_sceneToLoad);
        }

        LoaderMonoBehaviour.StartCoroutine(DelayedLoadDoneInvoke());
    }

    private static IEnumerator DelayedLoadDoneInvoke()
    {
        yield return null;
        OnSceneLoadDone?.Invoke(_sceneToLoad);
    }

    private static IEnumerator DelayedUnloadDoneInvoke()
    {
        yield return null;
        OnSceneUnloadDone?.Invoke(_sceneToUnload);
    }
}
