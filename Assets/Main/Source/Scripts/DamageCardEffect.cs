using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCardEffect : CardEffect
{
    public int numDamage = 0;

    public override void ExecuteEffect(List<GameObject> targets)
    {
        // If we have no targets, the effect is on the player of the card
        if(targets.Count == 0)
        {
            CardGameMgr.Instance.AddDamage(numDamage);
            Debug.Log("DamageCardEffect::ExecuteEffect!");
        }
        else
        {

        }
    }
}
