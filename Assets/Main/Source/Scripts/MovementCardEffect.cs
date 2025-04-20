using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCardEffect : CardEffect
{
    public int numMovement = 0;

    public override void ExecuteEffect(List<GameObject> targets)
    {
        // If we have no targets, the effect is on the player of the card
        if(targets.Count == 0)
        {
            CardGameMgr.Instance.AddMovement(numMovement);
        }
        else
        {

        }
    }
}
