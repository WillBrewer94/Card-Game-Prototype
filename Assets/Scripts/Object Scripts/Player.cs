using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Private
    private string prevCardSortingLayer = "Default";

    private Card lastHitCard;
    private Card selectedCard;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedCard == null)
        {
            // Check collision here
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit)
            {
                Debug.Log("Card collision!");
                GameObject hitObj = hit.transform.gameObject;
                if (hitObj)
                {
                    Card hitCard = hitObj.GetComponent<Card>();
                    if (hitCard && hitCard.CurrentState == Constants.SelectionState.Idle)
                    {
                        if (lastHitCard)
                        {
                            lastHitCard.CurrentState = Constants.SelectionState.Idle;
                            lastHitCard = null;
                        }

                        lastHitCard = hitCard;
                        hitCard.CurrentState = Constants.SelectionState.Highlighted;
                    }
                }
            }
            else if (lastHitCard)
            {
                lastHitCard.CurrentState = Constants.SelectionState.Idle;
                lastHitCard = null;
            }
        }
    }

    public void OnGrabEvent()
    {
        // Check collision here
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit)
        {
            Debug.Log("Card collision!");
            GameObject hitObj = hit.transform.gameObject;
            if (hitObj)
            {
                Card hitCard = hitObj.GetComponent<Card>();
                if (hitCard && hitCard.CurrentZone == Constants.Zone.Hand)
                {
                    selectedCard = hitCard;
                    hitCard.CurrentState = Constants.SelectionState.Selected;
                    prevCardSortingLayer = hitCard.SortingLayer;

                    // wbrewer TODO: I don't like this being a raw string, make this an enum somewhere or something
                    hitCard.SortingLayer = "Focus";
                }
            }
        }

        //Card card = highlightedCard.GetComponent<Card>();
        //if (card != null)
        //{
        //    if (card.State != Constants.SelectionState.Highlighted)
        //    {
        //        return;
        //    }

        //    Debug.Log("Card::OnGrabEvent()!");
        //    card.State = Constants.SelectionState.Selected;
        //}
    }

    public void OnReleaseEvent()
    {
        if (selectedCard == null)
        {
            return;
        }

        //Card card = selectedCard.GetComponent<Card>();
        //if (card != null)
        //{
        //    if (card.State != Constants.SelectionState.Selected)
        //    {
        //        return;
        //    }

        //    Debug.Log("Card::OnReleaseEvent()!");
        //    card.State = Constants.SelectionState.Idle;
        //}

        if (selectedCard.IsPlayable)
        {
            selectedCard.Play();
            CardGameMgr.Instance.AddCardToDisc(selectedCard.gameObject);
            CardGameMgr.Instance.CurrentMana -= selectedCard.CurrentCardManaCost;
        }
 
        selectedCard.CurrentState = Constants.SelectionState.Idle;
        selectedCard.SortingLayer = prevCardSortingLayer;

        selectedCard = null;
    }
}
