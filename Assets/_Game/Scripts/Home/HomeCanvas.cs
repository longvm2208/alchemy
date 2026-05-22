using TMPro;
using UnityEngine;

public class HomeCanvas : SingletonCanvas<HomeCanvas>
{
	[SerializeField] TMP_Text levelText;

	public void Init()
	{
        levelText.text = $"Level {GamePref.Ins.LevelIndex + 1}";
    }

	#region Event Listeners
	public void OnClickPlay()
	{
		SceneController.Ins.ToGame();
	}
	#endregion
}
