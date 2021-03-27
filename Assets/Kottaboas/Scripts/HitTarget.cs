using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//What happens when you land a shot on the kottboas stand targets or inside the cups
public class HitTarget : MonoBehaviour
{
    public GameManager gm;

    /// <summary>
    /// Find if the collision is with the targets do something
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Target")) 
        {
            gm.addScore();
            gm.scored = true;
            gm.ContinueRound = true;
            gm.isHit = true;
        }
        else 
        {
            gm.ContinueRound = true;
        }
    }
}
