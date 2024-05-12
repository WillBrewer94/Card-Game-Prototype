using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameObject selectedCard;

    Ray mouseRay;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //// Check collision here
        //Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //if (Physics2D.Raycast(mousePos, Vector2.zero))
        //{
        //    Debug.Log("Card collision!");
        //}
    }

    public void OnGrabEvent()
    {
        // Check collision here
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit)
        {
            Debug.Log("Card collision!");
            selectedCard = hit.transform.gameObject;

            Card hitCard = selectedCard.GetComponent<Card>();

            if (hitCard)
            {
                hitCard.CurrentState = Card.SelectionState.Selected;
            }
        }

        //Card card = highlightedCard.GetComponent<Card>();
        //if (card != null)
        //{
        //    if (card.State != Card.SelectionState.Highlighted)
        //    {
        //        return;
        //    }

        //    Debug.Log("Card::OnGrabEvent()!");
        //    card.State = Card.SelectionState.Selected;
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
        //    if (card.State != Card.SelectionState.Selected)
        //    {
        //        return;
        //    }

        //    Debug.Log("Card::OnReleaseEvent()!");
        //    card.State = Card.SelectionState.Idle;
        //}

        Card card = selectedCard.GetComponent<Card>();
        if (card)
        {
            card.CurrentState = Card.SelectionState.Idle;
        }

        selectedCard = null;
    }
}
