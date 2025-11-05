using UnityEngine;

public class BlockInteractKey : MonoBehaviour
{
    [SerializeField] Sprite cuteEnabledSprite;
    [SerializeField] Sprite darkBlockedSprite;

    [SerializeField] SpriteRenderer mySprite;

    HallucinationManager hallucinationManager;
    private void Start()
    {
        hallucinationManager = GameManager.Instance.Player._hallucinationManager;
    }
    private void Update()
    {
        if(hallucinationManager.HallucinationAmmount > 10f)
            mySprite.sprite = cuteEnabledSprite;
        else 
            mySprite.sprite = darkBlockedSprite;
    }
}
