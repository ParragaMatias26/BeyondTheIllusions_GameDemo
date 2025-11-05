using System.Collections;
using UnityEngine;

public class OneAnimationPlayer : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] bool loop = true;
    [SerializeField] bool playOnAwake = true;
    [SerializeField][Range(0f, .5f)] float frameRate = .2f;

    [Header("Animation References")]
    [SerializeField] SpriteRenderer m_SpriteRenderer;
    [SerializeField] Sprite[] myAnimFrames;

    int currentFrame = 0;
    private void Awake()
    {
        if (m_SpriteRenderer == null)
            m_SpriteRenderer = GetComponent<SpriteRenderer>();

        if (m_SpriteRenderer != null && playOnAwake) StartCoroutine(PlayAnim_Execute());
    }
    IEnumerator PlayAnim_Execute()
    {
        while (true)
        {
            currentFrame++;
            if (currentFrame == myAnimFrames.Length)
            {
                if (loop)
                    currentFrame = 0;

                else yield break;
            }

            m_SpriteRenderer.sprite = myAnimFrames[currentFrame];
            yield return new WaitForSeconds(frameRate);
        }
    }
}
