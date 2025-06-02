using UnityEngine;

public class CooldownTimer
{
    private float cooldown;
    private float lastTime = -Mathf.Infinity;

    public CooldownTimer(float cooldown)
    {
        this.cooldown = cooldown;
    }

    public bool IsReady => Time.time - lastTime >= cooldown;

    public void Trigger()
    {
        lastTime = Time.time;
    }

    public void SetCooldown(float value)
    {
        cooldown = value;
    }
}
