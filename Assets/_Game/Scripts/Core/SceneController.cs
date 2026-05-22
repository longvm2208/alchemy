using UnityEngine;

public enum Scene
{
    Home,
    Game,
}

public class SceneController : SingletonMonoBehaviour<SceneController>
{
    [SerializeField] GameObject homeScene;
    [SerializeField] GameObject gameScene;
    
    Scene currentScene;
    public Scene CurrentScene => currentScene;

    public void ActiveAll()
    {
        homeScene.SetActive(true);
        gameScene.SetActive(true);
    }

    public void Home(bool transition = true)
    {
        gameScene.SetActive(false);
        homeScene.SetActive(true);
        currentScene = Scene.Home;
        AudioManager.Ins.PlayMusic(MusicType.Home);

        HomeCanvas.Ins.Init();

        if (transition)
        {
            UIManager.Ins.TransitionAnimation.Open();
        }

        //AdsManager.Ins.HideBannerBg();
        //AdsManager.Ins.HideBanner();
    }

    public void Game(bool transition = true)
    {
        homeScene.SetActive(false);
        gameScene.SetActive(true);
        currentScene = Scene.Game;
        AudioManager.Ins.PlayMusic(MusicType.Game);

        LevelManager.Ins.LoadLevel();

        //AdsManager.Ins.ShowBannerBg();

        if (transition)
        {
            UIManager.Ins.TransitionAnimation.Open(() =>
            {
                //AdsManager.Ins.ShowBanner();
            });
        }
        else
        {
            //AdsManager.Ins.ShowBanner();
        }
    }

    public void ToHome()
    {
        UIManager.Ins.TransitionAnimation.Close(() =>
        {
            UIManager.Ins.CloseAll();
            Home();
        });
    }

    public void ToGame()
    {
        UIManager.Ins.TransitionAnimation.Close(() =>
        {
            UIManager.Ins.CloseAll();
            Game();
        });
    }
}
