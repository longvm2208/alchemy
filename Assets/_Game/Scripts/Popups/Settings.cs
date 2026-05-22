using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] GameObject soundOn;
    [SerializeField] GameObject soundOff;
    [SerializeField] GameObject musicOn;
    [SerializeField] GameObject musicOff;
    [SerializeField] GameObject vibrationOn;
    [SerializeField] GameObject vibrationOff;

    private void OnEnable()
    {
        soundOn.SetActive(AudioPref.Ins.IsSoundOn);
        soundOff.SetActive(!AudioPref.Ins.IsSoundOn);
        musicOn.SetActive(AudioPref.Ins.IsMusicOn);
        musicOff.SetActive(!AudioPref.Ins.IsMusicOn);
        vibrationOn.SetActive(VibrationPref.Ins.IsEnabled);
        vibrationOff.SetActive(!VibrationPref.Ins.IsEnabled);
    }

    #region Event Listeners
    public void OnClickSound()
    {
        AudioManager.Ins.ToggleSound();
        soundOn.SetActive(AudioPref.Ins.IsSoundOn);
        soundOff.SetActive(!AudioPref.Ins.IsSoundOn);
    }

    public void OnClickMusic()
    {
        AudioManager.Ins.ToggleMusic();
        musicOn.SetActive(AudioPref.Ins.IsMusicOn);
        musicOff.SetActive(!AudioPref.Ins.IsMusicOn);
    }

    public void OnClickVibration()
    {
        VibrationManager.Ins.ToggleVibration();
        vibrationOn.SetActive(VibrationPref.Ins.IsEnabled);
        vibrationOff.SetActive(!VibrationPref.Ins.IsEnabled);
    }
    #endregion
}
