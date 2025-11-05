using UnityEngine;

public class RealitySpriteChanger : MonoBehaviour
{
    [SerializeField] bool haveAnimation = false;

    [Header("Values")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    
    [Header("Sprites")]
    [SerializeField] Sprite cuteWorldSprite;
    [SerializeField] Sprite darkWorldSprite;

    [Header("AnimatorControllers")]
    [SerializeField] RuntimeAnimatorController cuteWorldAnimator;
    [SerializeField] RuntimeAnimatorController darkWorldAnimator;

    void Update()
    {
        ChangeSpriteReality();
    }

    public void ChangeSpriteReality()
    {
        if (haveAnimation)
        {
            if (RealityChangeManager.Instance.CuteWorld) animator.runtimeAnimatorController = cuteWorldAnimator;
            else animator.runtimeAnimatorController = darkWorldAnimator;
        }

        else
        {
            if (RealityChangeManager.Instance.CuteWorld) spriteRenderer.sprite = cuteWorldSprite;
            else spriteRenderer.sprite = darkWorldSprite;
        }
    }
}
