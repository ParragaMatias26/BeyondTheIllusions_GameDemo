using UnityEngine;
public class DamageBurst : MonoBehaviour
{
    [Header("VFX Settings")]
    [SerializeField] private bool spawnBlood = true;
    public bool SpawnBlood {  get { return spawnBlood; } set {  spawnBlood = value; } }

    [Header("VFX Prefabs")]
    [SerializeField] GameObject damageVFXPrefab;
    [SerializeField] GameObject bloodParticles;
    [SerializeField] Vector2 spawnOffset = new Vector2(0f, 1f);
    private void Start()
    {
        if (damageVFXPrefab == null)
            damageVFXPrefab = VFXManager.Instance.HitVFXPrefab;
        if (bloodParticles == null)
            bloodParticles = VFXManager.Instance.bloodParticlesPrefab;
    }
    public void TriggerEffect()
    {
        if(spawnBlood)
            Instantiate(bloodParticles, transform.position, Quaternion.identity);

        var VFX = Instantiate(damageVFXPrefab, (Vector2)transform.position + spawnOffset, Quaternion.identity);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + spawnOffset, .3f);
    }
}
