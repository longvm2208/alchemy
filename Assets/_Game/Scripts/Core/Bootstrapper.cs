using System.Collections;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    IEnumerator Start()
    {
        DatabaseManager.Ins.Init(GameManager.Ins.ItemDatabaseConfig);

        SceneController.Ins.ActiveAll();

        yield return null;

        if (GamePref.Ins.LevelIndex == 0)
        {
            SceneController.Ins.Game();
        }
        else
        {
            SceneController.Ins.Home();
        }
    }
}
