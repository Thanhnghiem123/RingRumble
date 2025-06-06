
// AnimationManager.cs - Manages all player animations
using UnityEngine;
public class AnimationManager : MonoBehaviour, IAnimationManager
{
    private Animator animator;
    private bool isKnockedOut = false; // Trạng thái để ngăn animation khi bị hạ gục
    private bool isVictorious = false; // Trạng thái khi chiến thắng

    // Them 4 bien quan ly thoi gian animation
    [SerializeField]
    private float normalHitTime = 0.5f; // Thời gian cho các đòn đánh bình thường
    [SerializeField]
    private float specialHitTime = 3.5f; // Thời gian cho đòn đánh punch



    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    public void PlayIdle()
    {
        if (isKnockedOut || isVictorious) return;
        animator.SetTrigger("Idle");
    }

    public void PlayRun(bool isRunning)
    {
        if (isKnockedOut || isVictorious) return;
        animator.SetBool("Run", isRunning);
    }

    public HitType PlayHeadPunch()
    {
        if (isKnockedOut || isVictorious) return HitType.HeadPunch;
        int randomPunch = Random.Range(0, 3);
        if (randomPunch == 0)
        {
            animator.SetTrigger("HeadPunch");
            // punch
            string[] punchClips = { "punch", "punch2", "punch3" };
            string randomClip = punchClips[Random.Range(0, punchClips.Length)];
            AudioManager.Instance.PlaySFX(randomClip);
            return HitType.HeadPunch;
        }
        else if (randomPunch == 1)
        {
            animator.SetTrigger("KidneyPunchLeft");
            string[] punchClips = { "punch", "punch2", "punch3" };
            string randomClip = punchClips[Random.Range(0, punchClips.Length)];
            AudioManager.Instance.PlaySFX(randomClip);
            return HitType.KidneyPunchLeft;
        }
        else
        {
            animator.SetTrigger("StomachPunch");
            string[] punchClips = { "punch", "punch2", "punch3" };
            string randomClip = punchClips[Random.Range(0, punchClips.Length)];
            AudioManager.Instance.PlaySFX(randomClip);
            return HitType.StomachPunch;
        }
    }

    public HitType PlayHoldPunch()
    {
        if (isKnockedOut || isVictorious) return HitType.HeadPunch;
        animator.SetTrigger("KidneyPunchRight");
        // punch mạnh
        AudioManager.Instance.PlaySFX("punch3");
        return HitType.KidneyPunchRight;
    }

    public HitType PlayKick()
    {
        if (isKnockedOut || isVictorious) return HitType.Kick;
        animator.SetTrigger("Kick");
        // kick
        string[] kickClips = { "kick", "kick2" };
        string randomClip = kickClips[Random.Range(0, kickClips.Length)];
        AudioManager.Instance.PlaySFX(randomClip);
        return HitType.Kick;
    }

    public HitType PlayHoldKick()
    {
        if (isKnockedOut || isVictorious) return HitType.Jumping;
        int randomJumpAttack = Random.Range(0, 3);
        switch (randomJumpAttack)
        {
            case 0:
                animator.SetTrigger("Jumping");
                AudioManager.Instance.PlaySFX("holdKick");
                return HitType.Jumping;
            case 1:
                animator.SetTrigger("Jumping1");
                AudioManager.Instance.PlaySFX("holdKick");
                return HitType.Jumping1;
            case 2:
                animator.SetTrigger("Jumping2");
                AudioManager.Instance.PlaySFX("holdKick");
                return HitType.Jumping2;
        }
        return HitType.Jumping;
    }



    public void PlayBlock(bool isBlocking)
    {
        if (isKnockedOut || isVictorious) return;
        animator.SetBool("BlockIdle", isBlocking);
    }

    public float PlayHit(HitType hitType)
    {
        if (isKnockedOut || isVictorious) return 0;
        switch (hitType)
        {
            case HitType.HeadPunch:
            case HitType.KidneyPunchRight:
                animator.SetTrigger("HeadHit");
                return normalHitTime;
            case HitType.KidneyPunchLeft:
            case HitType.Kick:
                animator.SetTrigger("KidneyHit");
                return normalHitTime;
                break;
            case HitType.StomachPunch:
                animator.SetTrigger("StomachHit");
                return normalHitTime;
                break;
            case HitType.Jumping:
            case HitType.Jumping1:
            case HitType.Jumping2:
                animator.SetTrigger("HoldKickHit");
                Debug.Log("PlayHit: HoldKickHit animation triggered" + specialHitTime);
                return specialHitTime;
                break;
            default:
                return 0;
        }
    }


    public void PlayJump()
    {
        if (isKnockedOut || isVictorious) return;
        // Chọn ngẫu nhiên giữa các animation nhảy
        int randomJump = Random.Range(0, 2);
        switch (randomJump)
        {
            case 0:
                animator.SetTrigger("BigJump");
                break;
            case 1:
                animator.SetTrigger("Jump2");
                break;

        }
    }

    public void PlayJumpOverIntro()
    {
        if (isKnockedOut || isVictorious) return;
        animator.SetTrigger("JumpingOverIntro");
    }
    

    public HitType PlayKnockedOut()
    {
        if (isKnockedOut || isVictorious) return HitType.KnockedOut;
        animator.SetTrigger("KnockedOut");
        isKnockedOut = true;
        return HitType.KnockedOut; // Trả về KnockedOut như một giá trị mặc định
    }

    public void PlayVictory()
    {
        if (isKnockedOut || isVictorious) return;
        animator.SetTrigger("Victory");
        isVictorious = true;
    }

    public void PlayDefeat()
    {
        //if (isKnockedOut) return;
        animator.SetTrigger("Death");
        //isKnockedOut = true; // Đặt trạng thái là bị hạ gục
    }



}