using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController[] _posibleControllers;
    [SerializeField] Sprite[] _posibleCuteSprites;
    [SerializeField] Sprite[] _posibleDarkSprites;
    [SerializeField] SpriteRenderer mCW_SpriteRenderer;
    [SerializeField] SpriteRenderer mDW_SpriteRenderer;
    [SerializeField] Animator m_animator;

    private void Start() 
    {
        m_animator = GetComponentInChildren<Animator>();

        Sprite cuteSprite = null;
        Sprite darkSprite = null;

        if (_posibleControllers.Length > 0 && m_animator != null) m_animator.runtimeAnimatorController = _posibleControllers[Random.Range(0, _posibleControllers.Length)];

        if(_posibleCuteSprites.Length > 0) cuteSprite = _posibleCuteSprites[Random.Range(0, _posibleControllers.Length)];
        if(_posibleDarkSprites.Length > 0) darkSprite = _posibleDarkSprites[Random.Range(0, _posibleDarkSprites.Length)];

        mCW_SpriteRenderer.sprite = cuteSprite;
        mDW_SpriteRenderer.sprite = darkSprite;
    }
}
