using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] SoundType soundType;

    public void Play()
    {
        AudioManager.Ins.PlaySound(soundType);
    }
}
