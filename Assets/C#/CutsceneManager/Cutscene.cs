using System;
using System.Collections;
using UnityEngine;

public abstract class Cutscene : MonoBehaviour
{
    public event Action OnCutsceneEnd = delegate { };
    public abstract IEnumerator Play(PlayerModel player, Camera mainCamera);
    public virtual void EndCutscene() => OnCutsceneEnd?.Invoke();
}
