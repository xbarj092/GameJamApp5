using UnityEngine;

public class SceneLoadManager : MonoSingleton<SceneLoadManager>
{
    protected override void Init()
    {
        base.Init();
        GoBootToMenu();
    }

    public void GoBootToMenu()
    {
        SceneLoader.OnSceneLoadDone += OnBootToMenuLoadDone;
        SceneLoader.LoadScene(SceneLoader.Scenes.MenuScene);
    }

    private void OnBootToMenuLoadDone(SceneLoader.Scenes scene)
    {
        PlayMenuMusic();
        Time.timeScale = 1;
        SceneLoader.OnSceneLoadDone -= OnBootToMenuLoadDone;
    }

    public void GoMenuToGame()
    {
        SceneLoader.OnSceneLoadDone += OnMenuToGameLoadDone;
        SceneLoader.LoadScene(SceneLoader.Scenes.GameScene, toUnload: SceneLoader.Scenes.MenuScene);
    }

    private void OnMenuToGameLoadDone(SceneLoader.Scenes scenes)
    {
        Time.timeScale = 1;
        PlayAmbience();
        SceneLoader.OnSceneLoadDone -= OnMenuToGameLoadDone;
    }

    public void GoGameToMenu()
    {
        SceneLoader.OnSceneLoadDone += OnGameToMenuLoadDone;
        SceneLoader.LoadScene(SceneLoader.Scenes.MenuScene, toUnload: SceneLoader.Scenes.GameScene);
    }

    private void OnGameToMenuLoadDone(SceneLoader.Scenes scenes)
    {
        Time.timeScale = 1;
        PlayMenuMusic();
        SceneLoader.OnSceneLoadDone -= OnGameToMenuLoadDone;
    }

    public void RestartGame()
    {
        SceneLoader.OnSceneLoadDone += OnRestartGameDone;
        SceneLoader.LoadScene(SceneLoader.Scenes.GameScene, toUnload: SceneLoader.Scenes.GameScene);
    }

    private void OnRestartGameDone(SceneLoader.Scenes scenes)
    {
        Time.timeScale = 1;
        PlayAmbience();
        SceneLoader.OnSceneLoadDone -= OnRestartGameDone;
    }

    public bool IsSceneLoaded(SceneLoader.Scenes sceneToCheck)
    {
        return SceneLoader.IsSceneLoaded(sceneToCheck);
    }

    private void PlayMenuMusic()
    {
        if (AudioManager.Instance.IsPlaying(SoundType.Ambience))
        {
            AudioManager.Instance.Stop(SoundType.Ambience);
        }
        
        if (!AudioManager.Instance.IsPlaying(SoundType.Menu))
        {
            AudioManager.Instance.Play(SoundType.Menu);
        }
    }

    private void PlayAmbience()
    {
        if (AudioManager.Instance.IsPlaying(SoundType.Menu))
        {
            AudioManager.Instance.Stop(SoundType.Menu);
        }

        if (!AudioManager.Instance.IsPlaying(SoundType.Ambience))
        {
            AudioManager.Instance.Play(SoundType.Ambience);
        }
    }
}
