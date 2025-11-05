using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class UISpriteAnimator : MonoBehaviour
{
    [SerializeField] private UIAnimation[] animations;
    [SerializeField] private Image image;

    private Dictionary<string, UIAnimation> animDict;
    private Queue<UIAnimation> waitingUI = new Queue<UIAnimation>();
    [SerializeField] private UIAnimation currentAnim;

    private Coroutine playRoutine;
    private int currentFrame;

    private bool isPlaying = false;

    void Awake()
    {
        animDict = new Dictionary<string, UIAnimation>();
        foreach (var anim in animations)
        {
            if (!animDict.ContainsKey(anim.animationName))
                animDict.Add(anim.animationName, anim);
        }
    }
    private void Start()
    {
        if (animations[0] != null) Play(animations[0].animationName);
    }
    public void Play(string animationName)
    {
        if (!gameObject.activeInHierarchy) return;

        UIAnimation animation = FindAnimation(animationName);
        if (animation == null || animation.frames.Length == 0)
        {
            Debug.LogWarning($"UIAnimator: No se encontró la animación '{animationName}'");
            return;
        }

        waitingUI.Clear();

        if (isPlaying && currentAnim != null && currentAnim.hasExitTime)
        {
            waitingUI.Enqueue(animation);
            return;
        }

        if (playRoutine != null)
            StopCoroutine(playRoutine);

        currentAnim = animation;
        playRoutine = StartCoroutine(PlayAnimation());
    }
    private IEnumerator PlayAnimation()
    {
        isPlaying = true;
        currentFrame = 0;
        float frameTime = 1f / currentAnim.frameRate;

        while (true)
        {
            image.sprite = currentAnim.frames[currentFrame];
            currentFrame++;

            if (currentFrame >= currentAnim.frames.Length)
            {
                if (currentAnim.loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    if (currentAnim.hasExitTime)
                        yield return new WaitForSeconds(frameTime);

                    break;
                }
            }

            yield return new WaitForSeconds(frameTime);
        }

        isPlaying = false;

        if (waitingUI.Count > 0)
        {
            var nextAnim = waitingUI.Dequeue();
            Play(nextAnim.animationName);
        }
    }
    public void PlayAfterTime(string animationName, float delay) => StartCoroutine(DelayRoutine(animationName, delay));
    IEnumerator DelayRoutine(string animationName, float delay) 
    {
        yield return new WaitForSeconds(delay);
        Play(animationName);
    }
    private UIAnimation FindAnimation(string name) 
    {
        foreach(var anim in animations) 
        {
            if (anim != null && anim.animationName == name)
                return anim;
        }

        return null;
    }
    public void Stop()
    {
        if (playRoutine != null)
            StopCoroutine(playRoutine);

        playRoutine = null;

        isPlaying = false;
        waitingUI.Clear();
    }
    private void OnDisable()
    {
        Stop();
    }
    private void OnEnable()
    {
        if (currentAnim != null)
            Play(currentAnim.animationName);
    }
}
