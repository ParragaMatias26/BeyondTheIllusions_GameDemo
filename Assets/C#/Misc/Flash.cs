using System.Collections;
using UnityEngine;
public class Flash : MonoBehaviour
{
    [SerializeField] private Material _whiteFlashMat;
    [SerializeField] private float restoreDefaultMatTime = .2f;

    private SpriteRenderer cwSprite;
    private SpriteRenderer dwSprite;
    private Material cwDefaultMat;
    private Material dwDefaultMat;
    public void AddSprites(SpriteRenderer _cwSprite, SpriteRenderer _dwSprite)
    {
        cwSprite = _cwSprite;
        dwSprite = _dwSprite;

        if(cwSprite != null) cwDefaultMat = cwSprite.material;
        if(dwSprite != null) dwDefaultMat = dwSprite.material;
    }
    public void StartFlash()
    {
        if(cwSprite != null) StartCoroutine(FlashRoutine(cwSprite, cwDefaultMat));
        if(dwSprite != null) StartCoroutine(FlashRoutine(dwSprite, dwDefaultMat));
    }
    IEnumerator FlashRoutine(SpriteRenderer sprite, Material defaultMat)
    {
        sprite.material = _whiteFlashMat;
        yield return new WaitForSeconds(restoreDefaultMatTime);
        sprite.material = defaultMat;
    }
}
