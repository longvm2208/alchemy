using UnityEngine;

public class MergeImpact : PooledMonoBehaviour<MergeImpact>
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] float duration;

    public void Play()
    {
        ps.Play();

        this.Invoke(duration, Recycle);
    }

    public void Destroy()
    {
        Recycle();
    }
}
