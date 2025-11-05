using UnityEngine;
using UnityEngine.UI;

public class UISpriteChanger : MonoBehaviour
{
    [SerializeField] Image m_Image;
    [SerializeField] Sprite cuteSprite;
    [SerializeField] Sprite darkSprite;

    private void Update()
    {
        var reality = RealityChangeManager.Instance.CuteWorld;
        if(reality) m_Image.sprite = cuteSprite;
        else m_Image.sprite = darkSprite;
    }
}
