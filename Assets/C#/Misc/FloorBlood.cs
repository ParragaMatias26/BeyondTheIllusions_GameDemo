using System.Collections;
using UnityEngine;

public class FloorBlood : MonoBehaviour
{
    [SerializeField] GameObject bloodPrefab => VFXManager.Instance.floorBlood;
    [SerializeField] Vector2 bloodOffset;

    public void CreateBlood()
    {
        Vector3 pos = new Vector3(transform.position.x + bloodOffset.x, transform.position.y + bloodOffset.y, 0f);
        var obj = Instantiate(bloodPrefab, pos, Quaternion.identity);
    }
}
