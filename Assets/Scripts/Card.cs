using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private Animator selectionStateAnimator;

    private CardGameMgr cardGameMgr;

    // WBREWER TODO: Move this to a constants file on the gamemgr singleton?
    public enum SelectionState
    {
        Idle = 0,
        Highlighted = 1,
        Selected = 2
    }

    public enum Zone
    {
        Deck = 0,
        Hand = 1,
        Discard = 2
    }

    private SelectionState _currentState = 0;
    public SelectionState CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            _currentState = value;

            if (selectionStateAnimator)
            {
                selectionStateAnimator.SetInteger("SelectionState", (int)_currentState);
            }
        }
    }

    private Zone _currentZone;
    public Zone CurrentZone
    {
        get
        {
            return _currentZone;
        }
        set
        {
            _currentZone = value;

            if (selectionStateAnimator)
            {
                selectionStateAnimator.SetInteger("CardZone", (int)_currentZone);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Grab GameMgr
        cardGameMgr = CardGameMgr.Instance;

        // Grab anim component
        selectionStateAnimator = this.GetComponent<Animator>();
        if(selectionStateAnimator == null)
        {
            Debug.LogError("Animator not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseEnter()
    {
        Debug.Log("ON MOUSE ENTER");

        if (CurrentState == SelectionState.Selected
            || CurrentZone != Zone.Hand)
        {
            return;
        }

        // WBREWER TODO: Is there a better way to get this property to the animator?
        CurrentState = SelectionState.Highlighted;
    }

    private void OnMouseExit()
    {
        Debug.Log("ON MOUSE EXIT");

        if (CurrentState == SelectionState.Selected
            || CurrentZone != Zone.Hand)
        {
            return;
        }

        // WBREWER TODO: Is there a better way to get this property to the animator?
        CurrentState = SelectionState.Idle;
    }
}
