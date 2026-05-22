using UnityEngine;

public class SimulateParticleSystemOnEnable : MonoBehaviour
{
    [SerializeField] float simulateTime;
    [SerializeField] ParticleSystem ps;

    private void OnEnable()
    {
        if (simulateTime > 0.001f)
        {
            ps.Simulate(simulateTime, true, true);
        }

        ps.Play();
    }
}
