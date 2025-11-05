using UnityEngine;

[SelectionBase]
public class MemoryObject : InteractableProp
{
    [Header("Components")]
    [SerializeField] Dialogue memoryDialogue;

    [Header("Photo Values")]
    [SerializeField] int m_index;
    [SerializeField] public Sprite m_ImageSprite;
    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue_Memory(memoryDialogue, this);
        MemoriesManager.Instance.ShowFragment(m_index);
    }
}
