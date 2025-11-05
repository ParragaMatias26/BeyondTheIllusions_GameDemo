using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderSystem : MonoBehaviour
{
    [SerializeField] private float detectRadius = 2f;
    private List<IColliderFix> detectedObjects = new List<IColliderFix>();

    public void DetectObjects(Vector2 playerDir) 
    {
        detectedObjects.Clear();
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius);

        foreach(var hit in hits) 
        {
            IColliderFix colliderFix = hit.GetComponent<IColliderFix>();
            if(colliderFix != null && !detectedObjects.Contains(colliderFix))
            {
                detectedObjects.Add(colliderFix);
            }
        }
    }
    public void UpdateColliders()
    {
        foreach(var colliderFix in detectedObjects) 
        {
            colliderFix.FixCollider(transform.position);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
