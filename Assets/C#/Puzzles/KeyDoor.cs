using UnityEngine;

public class KeyDoor : InteractableProp
{
    [Header("KeyDoorValues")]
    [SerializeField] GameObject myKey;
    [SerializeField] BoxCollider2D myCollider;
    
    [Header("View Values")]
    [SerializeField] Animator cwAnimator;
    [SerializeField] Animator dwAnimator;

    GameObject keyReference;
    public override void Interact()
    {
        var h = GameManager.Instance.Player._hallucinationManager.HallucinationAmmount;

        if (keyReference != null && keyReference == myKey && h > 15f)
        {
            Debug.Log("Abriendo puerta");
            myCollider.enabled = false;

            cwAnimator.SetBool("isOpen", true);
            dwAnimator.SetBool("isOpen", true);
            Destroy(keyReference);

            canShowMark = false;
        }
    }
    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject == myKey)
            keyReference = other.gameObject;
    }
}
