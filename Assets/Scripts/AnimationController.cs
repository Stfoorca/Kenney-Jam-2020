using System.Collections;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public AnimationData startingAnimation;
    public bool DespawnOnAnimationEnd = false;

    [Tooltip("Leave empty if it's on the same object.")]
    public SpriteRenderer targetSpriteRenderer;

    private AnimationData currentAnimation;
    private AnimationData previousAnimation;
    private AnimationData queuedAnimation;

    private float playbackSpeed;
    private int currentFrame;

    private bool paused = false;

    private void Awake()
    {
        if (targetSpriteRenderer == null) targetSpriteRenderer = GetComponent<SpriteRenderer>();
        if (targetSpriteRenderer == null) Debug.LogError("Missing Sprite Renderer reference.");
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        StopAllCoroutines();
        if (startingAnimation != null)
        {
            PlayAnimationData(startingAnimation);
        }
        ForceRestartAnimation();
    }

    public void Pause()
    {
        StopAllCoroutines();
        paused = true;
    }

    public void PlaySingleFrame(AnimationData animation, int frameIndex)
    {
        if (animation != null)
        {
            previousAnimation = currentAnimation;

            currentAnimation = animation;
            if (frameIndex < animation.sprites.Length)
            {
                targetSpriteRenderer.sprite = animation.sprites[frameIndex];
            }
            Pause();
        }
    }

    public void QueueUpAnimatonData(AnimationData animation)
    {
        queuedAnimation = animation;
    }

    public void ForceRestartAnimation()
    {
        if(currentAnimation!=null)
        {
            PlayAnimationWrapper(currentAnimation);
        }
    }

    //start from frame doesn't work
    public void PlayAnimationData(AnimationData animation, bool ignorePriority = false, int startFromFrame = 0)
    {
        //Debug.Log(animation.name);
        if (animation != null && (animation != currentAnimation || paused))
        {
            if (currentAnimation == null)
            {
                PlayAnimationWrapper(animation, startFromFrame);
            }
            else
            {
                if (animation.priority >= currentAnimation.priority || ignorePriority)
                {
                    queuedAnimation = null;
                    PlayAnimationWrapper(animation, startFromFrame);
                }
                else
                {
                    QueueUpAnimatonData(animation);
                }
            }
        }
    }

    private void PlayAnimationWrapper(AnimationData animation, int startFrameOverride = 0)
    {
        paused = false;
        previousAnimation = currentAnimation;
        currentAnimation = animation;
        playbackSpeed = animation.defaultSpeed;
        StopAllCoroutines();

        StartCoroutine(AnimateSpritesCoroutine(animation.sprites, animation.defaultSpeed,
            animation.playBackwards, (startFrameOverride != 0) ? startFrameOverride : animation.startingFrame, animation.loop));
    }

    private IEnumerator AnimateSpritesCoroutine(Sprite[] animationSprites, float framesPerSecond,
        bool playBackwards = false, int startingFrame = 0, bool loop = true)
    {
        Sprite[] sprites = animationSprites;

        int frame = (startingFrame < sprites.Length) ? startingFrame : 0;

        currentFrame = 0;
        for (int i = 0; i < sprites.Length; i++)
        {
            currentFrame = i;

            targetSpriteRenderer.sprite = sprites[frame];

            frame += (playBackwards ? -1 : 1);

            if (frame == sprites.Length && !playBackwards) frame -= sprites.Length;
            else if (frame - 1 < 0 && playBackwards) frame += sprites.Length;

            yield return new WaitForSeconds(1 / framesPerSecond);
        }

        yield return null;

        AnimationEnd();

        if (loop)
        {
            StartCoroutine(AnimateSpritesCoroutine(animationSprites, framesPerSecond, playBackwards, startingFrame, loop));
            //Debug.Log($"Animation looped on {gameObject.name}");
        } 
        else
        {
            if (queuedAnimation != null)
            {
                currentAnimation = null;
                PlayAnimationData(queuedAnimation);
                queuedAnimation = null;
            }
        }
    }

    private void AnimationEnd()
    {
        if (DespawnOnAnimationEnd)
        {
            ResetAll();
            Destroy(gameObject);
            //ObjectPooling.Despawn(gameObject);
        }
    }

    private void ResetAll()
    {
        StopAllCoroutines();
        currentFrame = 0;
        currentAnimation = null;
        queuedAnimation = null;
        previousAnimation = null;
    }

    public int GetCurrentFrame()
    {
        return currentFrame;
    }

    public int GetCurrentAnimationLength()
    {
        return currentAnimation.sprites.Length;
    }

    public bool IsLastFrame()
    {
        return (GetCurrentFrame() == GetCurrentAnimationLength() - 1);
    }

    public AnimationData GetCurrentAnimation()
    {
        return currentAnimation;
    }

    public AnimationData GetPreviousAnimation()
    {
        return previousAnimation;
    }
}