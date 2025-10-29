using System.Collections;
using UnityEngine;

public class BattleAnimator : MonoBehaviour
{
   // player and dealer animators
    public Animator playerAnim;
    public Animator dealerAnim;

    //Time delay before attack
    public float windup = 0.25f;

   // time of recover reaction
    public float hitReact = 0.20f;

    //Settle time between animation
    public float postBeat = 0.25f;

   //Match animation states
    public string idleStateName = "Idle";
    public string idleFallbackStateName = "Combat Idle";

    // Trigger parameter names 
    public string attackTrigger = "Attack";   // strike
    public string hurtTrigger   = "Hurt";     // getting hit (you use 'Hurt', not 'Hit')
    public string blockTrigger  = "Recover";  // we’ll use Recover as the block/immunity pose
    public string koTrigger     = "Death";    // KO
    
    //Remain Idle
    public void BothIdle()
    {
        PlayIdle(playerAnim);
        PlayIdle(dealerAnim);
    }

    // If player wins round
    public IEnumerator PlayerAttacksDealer(bool dealerBlocked, bool dealerKO)
    {
        SetTriggerSafe(playerAnim, attackTrigger);
        yield return new WaitForSeconds(windup);

        // Defender reacts
        SetTriggerSafe(dealerAnim, dealerBlocked ? blockTrigger : hurtTrigger);
        yield return new WaitForSeconds(hitReact);

        if (dealerKO) SetTriggerSafe(dealerAnim, koTrigger);
        yield return new WaitForSeconds(postBeat);
    }

    //If dealer wins round
    public IEnumerator DealerAttacksPlayer(bool playerBlocked, bool playerKO)
    {
        SetTriggerSafe(dealerAnim, attackTrigger);
        yield return new WaitForSeconds(windup);

        // Defender reacts
        SetTriggerSafe(playerAnim, playerBlocked ? blockTrigger : hurtTrigger);
        yield return new WaitForSeconds(hitReact);

        if (playerKO) SetTriggerSafe(playerAnim, koTrigger);
        yield return new WaitForSeconds(postBeat);
    }

    //Cheer state
    public void Cheer(bool playerCheer, bool dealerCheer)
    {
        if (playerCheer)
        {
            // Option A: a little flourish
            SetTriggerSafe(playerAnim, blockTrigger); // 'Recover'
            // Option B (uncomment to just settle): PlayIdle(playerAnim);
        }
        if (dealerCheer)
        {
            SetTriggerSafe(dealerAnim, blockTrigger);
            // Or: PlayIdle(dealerAnim);
        }
    }

    // player death animation
    public void KillPlayer()
    {
        SetTriggerSafe(playerAnim, koTrigger); // "Death"
    }

    //dealer death animation
    public void KillDealer()
    {
        SetTriggerSafe(dealerAnim, koTrigger); // "Death"
    }


    // helper functions

    void PlayIdle(Animator anim)
    {
        if (!anim) return;
        int layer = 0;

        // Try the primary Idle state
        if (anim.HasState(layer, Animator.StringToHash(idleStateName)))
        {
            anim.Play(idleStateName, layer, 0f);
            return;
        }
        // Fallback to "Combat Idle" if present
        if (!string.IsNullOrEmpty(idleFallbackStateName) &&
            anim.HasState(layer, Animator.StringToHash(idleFallbackStateName)))
        {
            anim.Play(idleFallbackStateName, layer, 0f);
            return;
        }
        // If neither state exists, do nothing (try to prevent erors)
    }

    void SetTriggerSafe(Animator anim, string triggerName)
    {
        if (!anim || string.IsNullOrEmpty(triggerName)) return;
        // set runtimes for triggers
        anim.ResetTrigger(triggerName); // avoid queuing duplicates across frames
        anim.SetTrigger(triggerName);
    }
}
