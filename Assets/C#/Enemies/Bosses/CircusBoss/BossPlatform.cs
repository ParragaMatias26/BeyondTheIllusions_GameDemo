using UnityEngine;

[RequireComponent (typeof(HealthComponent))]
public class BossPlatform : MonoBehaviour
{
    private PlatformManager myManager;
    private HealthComponent healthComponent;

    [Header("Components")]
    [SerializeField] Transform bossPosition;
    [SerializeField] DamageBurst damageBurst;
    [SerializeField] Flash spriteFlash;

    [Header("Sprites")]
    [SerializeField] SpriteRenderer cwSprite;
    [SerializeField] SpriteRenderer dwSprite;
    public Vector3 BossPosition { get { return bossPosition.position; } }
    public HealthComponent PlatformHealt { get { return healthComponent; } }
    public void Initialize(PlatformManager manager)
    {
        myManager = manager;

        healthComponent = GetComponent<HealthComponent>();
        damageBurst = GetComponent<DamageBurst>();

        damageBurst.SpawnBlood = false;
        healthComponent.CanTakeDamage = false;

        if(spriteFlash != null) 
        {
            spriteFlash.AddSprites(cwSprite, dwSprite);
        }
        healthComponent.OnDamageTake += (_,_) => {
            spriteFlash.StartFlash();
            damageBurst.TriggerEffect();
        };
        healthComponent.OnDeath += (_,_) => {
            myManager.RemovePlatform(gameObject);
        };
    }
}
