using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance { get; private set; }
    public Transform QuestUIContentRectTransform;

    [Header("PlayerUI Animation Settings")]
    [SerializeField] public UISpriteAnimator PlayerUIAnimator;
    [SerializeField] string darkTransitionAnimName;
    [SerializeField] string cuteTransitionAnimName;
    [SerializeField] string darkIddleAnimName;
    [SerializeField] string cuteIddleAnimName;

    [Header("QuestUI Settings")]
    [SerializeField] GameObject tabUICanvas;
    [SerializeField] GameObject[] tabUIPanels;

    [Header("LifeBar Components")]
    [SerializeField] float showDelay;
    [SerializeField] GameObject lifeBar;
    [SerializeField] Image[] hearts;

    Coroutine delayRoutine;

    public Slider VolumeSlider;
    public event Action<float> OnVolumeChange = delegate { };
    float actualValue;

    private void Awake()
    {
        Instance = this;
        actualValue = VolumeSlider.value;

        PlayerModel.OnPlayerDie += () => {
            StopAllCoroutines();
            lifeBar.SetActive(false);
        };

        BTN_SwapTabUIPanels(0);
    }
    private void Update()
    {
        if (VolumeSlider.value != actualValue)
        {
            OnVolumeChange(VolumeSlider.value);
            actualValue = VolumeSlider.value;
        }

        RealityChangeManager.OnCuteWorldEnabled += () =>
        {
            PlayerUIAnimator.Play(cuteTransitionAnimName);
            PlayerUIAnimator.Play(cuteIddleAnimName);

            if (delayRoutine != null) {
                StopCoroutine(delayRoutine);
            }

            delayRoutine = StartCoroutine(ToggleLifebarAfterTime(showDelay, true));
        };
        RealityChangeManager.OnCuteWorldDisabled += () =>
        {
            PlayerUIAnimator.Play(darkTransitionAnimName);
            PlayerUIAnimator.Play(darkIddleAnimName);

            if (delayRoutine != null) {
                StopCoroutine(delayRoutine);
            }

            delayRoutine = StartCoroutine(ToggleLifebarAfterTime(0f, false));
        };
    }
    public void UpdateHealthBar(int currentHealth)
    {
        for(int i = 0; i < hearts.Length; i++) 
        {
            if(i < currentHealth)
                hearts[i].gameObject.SetActive(true);
            else 
                hearts[i].gameObject.SetActive(false);
        }
    }
    public void ToggleMissionPanel()
    {
        var b = tabUICanvas.activeSelf;
        tabUICanvas.SetActive(!b);
    }
    public void BTN_SwapTabUIPanels(int index) 
    {
        for(int i = 0; i < tabUIPanels.Length; i++)
        {
            if(i == index)
                tabUIPanels[i].SetActive(true);
            else
                tabUIPanels[i].SetActive(false);
        }
    }
    public IEnumerator ToggleLifebarAfterTime(float delay, bool state)
    {
        yield return new WaitForSeconds(delay);
        lifeBar.SetActive(state);
    }
}
