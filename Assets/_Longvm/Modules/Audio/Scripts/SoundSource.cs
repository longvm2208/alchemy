using UnityEngine;

public class SoundSource : PooledMonoBehaviour<SoundSource>
{
    [SerializeField] AudioSource source;

    public void PlayOneShot(AudioClipConfig clipConfig)
    {
        source.volume = clipConfig.Volume;
        source.PlayOneShot(clipConfig.Clip);
        this.Invoke(clipConfig.Clip.length, Destroy);
    }

    public void Stop()
    {
        source.Stop();
    }

    public void Destroy()
    {
        Recycle();
    }
}
