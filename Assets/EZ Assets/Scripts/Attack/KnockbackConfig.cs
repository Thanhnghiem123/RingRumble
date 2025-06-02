using System.Collections.Generic;
using UnityEngine;

public class KnockbackConfig : MonoBehaviour
{

    public float punchKnockback = 0.5f;
    public float kickKnockback = 0.5f;
    public float holdPunchKnockback = 1f;
    public float holdKickKnockback = 3f;

    public KnockbackConfig(float punchKnockback, float kickKnockback, float holdPunchKnockback, float holdKickKnockback)
    {
        this.punchKnockback = punchKnockback;
        this.kickKnockback = kickKnockback;
        this.holdPunchKnockback = holdPunchKnockback;
        this.holdKickKnockback = holdKickKnockback;
    }

    public float GetKnockbackForce(HitType hitType)
    {
        switch (hitType)
        {
            case HitType.HeadPunch:
            case HitType.KidneyPunchLeft:
            case HitType.StomachPunch:
                return punchKnockback;

            case HitType.Kick:
                return kickKnockback;

            case HitType.KidneyPunchRight:
                return holdPunchKnockback;

            case HitType.Jumping:
            case HitType.Jumping1:
            case HitType.Jumping2:
                return holdKickKnockback;

            default:
                return 0.5f;

        }
    }
}