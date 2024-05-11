using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int Num { get; private set; } = 0;

    //public GameEvent onGrabEvent;

    private Animator selectionStateAnimator;

    private CardGameMgr mgr;

    // WBREWER TODO: Move this to a constants file on the gamemgr singleton?
    public enum SelectionState
    {
        Idle = 0,
        Highlighted = 1,
        Selected = 2
    }

    public SelectionState State { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        // Create and store a random number at creation
        Num = Random.Range(0, 10);

        // Grab GameMgr
        mgr = CardGameMgr.Instance;

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
        if(State == SelectionState.Selected)
        {
            this.transform.position = Input.mousePosition;
        }
    }

    void OnMouseEnter()
    {
        Debug.Log("ON MOUSE ENTER");

        // WBREWER TODO: Is there a better way to get this property to the animator?
        State = SelectionState.Highlighted;
        mgr.SetHighlightedCard(gameObject);
        selectionStateAnimator.SetInteger("SelectionState", (int)SelectionState.Highlighted);
    }

    private void OnMouseExit()
    {
        Debug.Log("ON MOUSE EXIT");

        // WBREWER TODO: Is there a better way to get this property to the animator?
        State = SelectionState.Idle;
        mgr.SetHighlightedCard(null);
        selectionStateAnimator.SetInteger("SelectionState", (int)SelectionState.Idle);
    }

    public void OnGrabEvent()
    {
        Debug.Log("Card::OnGrabEvent()!");
    }
}
