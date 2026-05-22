using Lofelt.NiceVibrations;

public class VibrationManager : SingletonMonoBehaviour<VibrationManager>
{
    public void ToggleVibration()
    {
        VibrationPref.Ins.IsEnabled = !VibrationPref.Ins.IsEnabled;
    }

    public void Vibrate(HapticPatterns.PresetType type = HapticPatterns.PresetType.MediumImpact)
    {
        if (VibrationPref.Ins.IsEnabled)
        {
            HapticPatterns.PlayPreset(type);
        }
    }
}
