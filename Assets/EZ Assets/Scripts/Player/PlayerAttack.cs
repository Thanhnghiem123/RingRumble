using UnityEngine;

public class PlayerAttack : MonoBehaviour, IPlayerAttack
{
    [SerializeField]
    private bool isBlocking = false;

   

    

    void Start()
    {
        }

    public void Punch()
    {
       Debug.Log("Punch executed");
     
    }

    public void HoldPunch()
    {
       
    }

    public void Kick()
    {
        
    }

    public void HoldKick()
    {
       
    }

    public void Jump()
    {
       
    }

    public void JumpOverIntro()
    {
        
    }

    public bool Block()
    {
        isBlocking = true;
        return isBlocking;
    }

    public bool UnBlock()
    {
        return isBlocking = false;
    }


    public bool IsBlocking() => isBlocking;
}