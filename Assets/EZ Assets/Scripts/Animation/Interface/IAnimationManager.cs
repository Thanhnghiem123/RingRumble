// IAnimationManager.cs - Interface for animation management
public interface IAnimationManager
{
    void PlayIdle();
    void PlayRun(bool isRunning);
    HitType PlayHeadPunch();
    HitType PlayHoldPunch();
    HitType PlayKick();
    HitType PlayHoldKick();
    void PlayBlock(bool isBlocking);
    void PlayHit(HitType hitType);
    void PlayJump();
    void PlayJumpOverIntro();
    HitType PlayKnockedOut();
    void PlayVictory();
}