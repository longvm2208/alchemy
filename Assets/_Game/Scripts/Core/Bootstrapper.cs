using System.Collections;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    IEnumerator Start()
    {
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
