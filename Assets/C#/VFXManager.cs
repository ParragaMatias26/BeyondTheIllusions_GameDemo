using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("VFX Prefabs")]
    public GameObject HitVFXPrefab;
    public GameObject bloodParticlesPrefab;
    public GameObject floorBlood;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else Destroy(gameObject);
    }
}
