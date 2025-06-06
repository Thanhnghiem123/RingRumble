using System.Collections.Generic;
using UnityEngine;

public class KnockbackConfig : MonoBehaviour
{
    [Tooltip("Khoảng cách bị đẩy lùi khi bị đánh bằng đòn punch")]
    public float punchKnockback = 0.5f;
    [Tooltip("Khoảng cách bị đẩy lùi khi bị đánh bằng đòn kick")]
    public float kickKnockback = 0.5f;
    [Tooltip("Khoảng cách bị đẩy lùi khi bị đánh bằng đòn hold punch")]
    public float holdPunchKnockback = 1f;
    [Tooltip("Khoảng cách bị đẩy lùi khi bị đánh bằng đòn hold kick")]
    public float holdKickKnockback = 3f;

    public KnockbackConfig(float punchKnockback, float kickKnockback, float holdPunchKnockback, float holdKickKnockback)
    {
        this.punchKnockback = punchKnockback;
        this.kickKnockback = kickKnockback;
        this.holdPunchKnockback = holdPunchKnockback;
        this.holdKickKnockback = holdKickKnockback;
    }

    /// <summary>
    /// Lấy lực đẩy lùi dựa trên loại đòn đánh.
    /// </summary>
    /// <param name="hitType"></param>
    /// <returns></returns>
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