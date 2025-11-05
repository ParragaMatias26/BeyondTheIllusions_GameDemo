using UnityEngine;

[SelectionBase]
public class ColliderFixObject : MonoBehaviour, IColliderFix
{
    [Header("Colliders")]
    [SerializeField] private GameObject topCollider;
    [SerializeField] private GameObject bottomCollider;
    [SerializeField] private SpriteRenderer[] sprites;
    public void FixCollider(Vector2 playerPos)
    {
        bool playerIsAbove = playerPos.y >= transform.position.y;

        if (playerIsAbove) 
        {
            foreach(var sprite in sprites) 
            {
                if(sprite == null) 
                {
                    continue;
                }

                sprite.sortingOrder = 1;
            }
               
            if(topCollider != null) topCollider.SetActive(true);
            if(bottomCollider != null) bottomCollider.SetActive(false);
        }
        else 
        {
            foreach (var sprite in sprites) 
            {
                if (sprite == null)
                {
                    continue;
                }

                sprite.sortingOrder = -1;
            }

            if (topCollider != null) bottomCollider.SetActive(true);
            if (bottomCollider != null) topCollider.SetActive(false);
        }
    }
}
