using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimHandler : MonoBehaviour
{

    public Animator anim;

    public void ChangeSpeed(float Speed)
    {
        if (!anim) return;

        anim.SetFloat("Input Z", Speed);
    }

    #region States
    
    public virtual void OnStartMove()
    {
        if (!anim) return;

        anim.SetBool("Moving", true);
    }

    public virtual void OnStopMove()
    {
        if (!anim) return;

        anim.SetBool("Moving", false);
    }

    public virtual void OnDeath()
    {
        if (!anim) return;

        OnStopMove();
        anim.SetTrigger("DeathTrigger");
    }

    #endregion
}
