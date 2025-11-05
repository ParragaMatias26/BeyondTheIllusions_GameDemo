using System.Collections;
using UnityEngine;

public class DMGEffect : MonoBehaviour
{
    [Header("Visual Components")]
    [SerializeField] GameObject impactPrefab;

    [Header("Spawn Offset")]
    [SerializeField] Vector3 offset;

    public void TriggerDamageEffect(Vector3 instigatorPos)
    {
        var impact = Instantiate(impactPrefab, transform.position + offset, Quaternion.identity, this.transform);
        var dir = instigatorPos - transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        impact.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + offset, .1f);
    }
}
