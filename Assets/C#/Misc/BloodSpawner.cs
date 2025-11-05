using System.Collections;
using UnityEngine;

public class BloodSpawner : MonoBehaviour
{
    public ParticleSystem bloodParticles;
    public GameObject[] bloodStainPrefabs;
    public Vector2 localScaleParameter;

    private ParticleSystem.Particle[] particles;

    private void Start()
    {
        StartCoroutine(DestroyAferTime());
    }
    void LateUpdate()
    {
        if (particles == null || particles.Length < bloodParticles.main.maxParticles)
            particles = new ParticleSystem.Particle[bloodParticles.main.maxParticles];

        int count = bloodParticles.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            if (particles[i].remainingLifetime < 0.1f)
            {
                GameObject p = Instantiate(bloodStainPrefabs[Random.Range(0, bloodStainPrefabs.Length)], particles[i].position, Quaternion.identity);
                
                float randomScale = Random.Range(localScaleParameter.x, localScaleParameter.y);
                p.transform.localScale = new Vector3(randomScale, randomScale, 1f);

                particles[i].remainingLifetime = 0;
            }
        }

        bloodParticles.SetParticles(particles, count);
    }
    IEnumerator DestroyAferTime() 
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
