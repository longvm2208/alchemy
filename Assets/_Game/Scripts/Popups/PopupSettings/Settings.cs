using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] Toggle soundToggle;
    [SerializeField] Toggle musicToggle;
    [SerializeField] Toggle vibrationToggle;

    private void OnEnable()
    {
        soundToggle.Switch(AudioPref.Ins.IsSoundOn, false);
        musicToggle.Switch(AudioPref.Ins.IsMusicOn, false);
        vibrationToggle.Switch(VibrationPref.Ins.IsEnabled, false);
    }

    #region Event Listeners
    public void OnClickSound()
    {
        AudioManager.Ins.ToggleSound();
        soundToggle.Switch(AudioPref.Ins.IsSoundOn, true);
    }

    public void OnClickMusic()
    {
        AudioManager.Ins.ToggleMusic();
        musicToggle.Switch(AudioPref.Ins.IsMusicOn, true);
    }

    public void OnClickVibration()
    {
        VibrationManager.Ins.ToggleVibration();
        vibrationToggle.Switch(VibrationPref.Ins.IsEnabled, true);
    }
    #endregion
}
