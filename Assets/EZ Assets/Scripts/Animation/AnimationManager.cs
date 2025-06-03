
// AnimationManager.cs - Manages all player animations
using UnityEngine;

public class AnimationManager : MonoBehaviour, IAnimationManager
{
    private Animator animator;
    private bool isKnockedOut = false; // Trạng thái để ngăn animation khi bị hạ gục
    private bool isVictorious = false; // Trạng thái khi chiến thắng

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
        if (isKnockedOut || isVictorious) return HitType.HeadPunch; // hoặc giá trị mặc định
        int randomPunch = Random.Range(0, 3);
        if (randomPunch == 0)
        {
            animator.SetTrigger("HeadPunch");
            return HitType.HeadPunch;
        }
        else if (randomPunch == 1)
        {
            animator.SetTrigger("KidneyPunchLeft");
            return HitType.KidneyPunchLeft;
        }
        else
        {
            animator.SetTrigger("StomachPunch");
            return HitType.StomachPunch;
        }
    }

    public HitType PlayHoldPunch()
    {
        if (isKnockedOut || isVictorious) return HitType.HeadPunch;
        animator.SetTrigger("KidneyPunchRight");
        return HitType.KidneyPunchRight; // Trả về KidneyPunchLeft như một ví dụ, có thể thay đổi tùy ý
    }





    public void PlayBlock(bool isBlocking)
    {
        if (isKnockedOut || isVictorious) return;
        animator.SetBool("BlockIdle", isBlocking);
    }

    public void PlayHit(HitType hitType)
    {
        if (isKnockedOut || isVictorious) return;
        switch (hitType)
        {
            case HitType.HeadPunch:
            case HitType.KidneyPunchRight:
                animator.SetTrigger("HeadHit");
                
                break;
            case HitType.KidneyPunchLeft:
             // Fixed the syntax issue 
            case HitType.Kick:
                animator.SetTrigger("KidneyHit");
                break;
            case HitType.StomachPunch:
                animator.SetTrigger("StomachHit");
                break;
            case HitType.Jumping:
            case HitType.Jumping1:
            case HitType.Jumping2:
                animator.SetTrigger("HoldKickHit");
                break;
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
    public HitType PlayKick()
    {
        if (isKnockedOut || isVictorious) return HitType.Kick; // Hoặc giá trị mặc định
        animator.SetTrigger("Kick");
        return HitType.Kick; // Trả về KidneyPunchLeft như một ví dụ, có thể thay đổi tùy ý

    }

    public HitType PlayHoldKick()
    {
        if (isKnockedOut || isVictorious) return HitType.Jumping;
        // Chọn ngẫu nhiên giữa các animation nhảy tấn công
        int randomJumpAttack = Random.Range(0, 3);
        switch (randomJumpAttack)
        {
            case 0:
                animator.SetTrigger("Jumping");
                return HitType.Jumping; // Trả về JumpingKick như một ví dụ, có thể thay đổi tùy ý
            case 1:
                animator.SetTrigger("Jumping1");
                return HitType.Jumping1; // Trả về Jumping1 như một ví dụ, có thể thay đổi tùy ý
            case 2:
                animator.SetTrigger("Jumping2");
                return HitType.Jumping2; // Trả về Jumping2 như một ví dụ, có thể thay đổi tùy ý
        }
        return HitType.Jumping; // Trả về HeadPunch như một giá trị mặc định nếu không có trường hợp nào khớp
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