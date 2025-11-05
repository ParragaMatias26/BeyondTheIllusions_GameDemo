using System.Collections.Generic;
using UnityEngine;

public class CustomNode : MonoBehaviour
{
    public List<CustomNode> neighbors;
    public float Cost => 1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (neighbors != null)
        {
            foreach (var v in neighbors)
            {
                Gizmos.DrawLine(transform.position, v.transform.position);
            }
        }
    }
}
