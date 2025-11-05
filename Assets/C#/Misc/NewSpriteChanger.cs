using UnityEngine;

public class NewSpriteChanger : MonoBehaviour
{
    [SerializeField] Animator m_Animator;

    private void Awake()
    {
        if(m_Animator == null) m_Animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (RealityChangeManager.Instance.CuteWorld) m_Animator.SetLayerWeight(1, 1);
        else m_Animator.SetLayerWeight(1, 0);
    }
}
