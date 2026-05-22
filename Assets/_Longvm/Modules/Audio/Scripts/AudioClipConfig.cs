using UnityEngine;

[CreateAssetMenu(fileName = "Audio Clip Config", menuName = "SO/Audio Clip Config")]
public class AudioClipConfig : ScriptableObject
{
    public AudioClip Clip;
    [Range(0f, 1f)]
    public float Volume = 1f;
}
