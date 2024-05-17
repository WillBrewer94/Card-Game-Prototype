using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    // Component and Script References
    private Animator selectionStateAnimatorComp;
    private SpriteRenderer spriteRendererComp;
    private MoveToPoint moveMeComp;
    private CardGameMgr cardGameMgr;
    public GameObject canvasObj;

    // Editor Constants
    public float kCardPlayThreshhold;

    // Public
    private bool _isPlayable;
    public bool IsPlayable
    {
        get
        {
            return _isPlayable;
        }
        private set
        {
            _isPlayable = value;

            if (selectionStateAnimatorComp)
            {
                selectionStateAnimatorComp.SetBool("IsPlayable", _isPlayable);
            }
        }
    }
    
    private string _sortingLayer;
    public string SortingLayer
    {
        get
        {
            return _sortingLayer;
        }
        set
        {
            _sortingLayer = value;

            if (spriteRendererComp != null)
            {
                spriteRendererComp.sortingLayerName = value;
            }
        }
    }

    private int _orderInLayer;
    public int OrderInLayer
    {
        get
        {
            return _orderInLayer;
        }
        set
        {
            _orderInLayer = value;

            if (spriteRendererComp != null)
            {
                spriteRendererComp.sortingOrder = _orderInLayer;
            }
        }
    }

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

            if (selectionStateAnimatorComp)
            {
                selectionStateAnimatorComp.SetInteger("SelectionState", (int)_currentState);
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

            if (selectionStateAnimatorComp)
            {
                selectionStateAnimatorComp.SetInteger("CardZone", (int)_currentZone);
            }

            if (_currentZone == Zone.Deck)
            {
                canvasObj.SetActive(false);
            }
            else
            {
                canvasObj.SetActive(true);
            }

            switch (_currentZone)
            {
                case Zone.Deck:
                    SortingLayer = "Deck";
                    return;
                case Zone.Hand:
                    SortingLayer = "Hand";
                    return;
                case Zone.Discard:
                    SortingLayer = "Discard";
                    return;
                default:
                    SortingLayer = "Default";
                    return;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Grab GameMgr
        cardGameMgr = CardGameMgr.Instance;

        // Grab anim component
        selectionStateAnimatorComp = this.GetComponent<Animator>();
        if(selectionStateAnimatorComp == null)
        {
            Debug.LogError("Animator component not found!");
        }

        // Grab sorting layer component
        spriteRendererComp = this.GetComponent<SpriteRenderer>();
        if (spriteRendererComp == null)
        {
            Debug.LogError("Renderer component not found!");
        }

        moveMeComp = this.GetComponent<MoveToPoint>();
        if (moveMeComp == null)
        {
           Debug.LogError("MoveToPoint component not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // wbrewer TODO: Add mana cost checking here
        float moveDelta = Mathf.Abs(moveMeComp.TargetPosition.y - this.transform.position.y);
        if (CurrentState == SelectionState.Selected && moveDelta >= kCardPlayThreshhold)
        {
            IsPlayable = true;
        }
        else
        {
            IsPlayable = false;
        }
    }

    public void Play()
    {
        Debug.Log("PLAYING CARD!");
    }

    //void OnMouseEnter()
    //{
    //    Debug.Log("ON MOUSE ENTER");

    //    if (CurrentState == SelectionState.Selected
    //        || CurrentZone != Zone.Hand)
    //    {
    //        return;
    //    }

    //    Debug.Log("SET HIGHLIGHT");
    //    // WBREWER TODO: Is there a better way to get this property to the animator?
    //    CurrentState = SelectionState.Highlighted;
    //}

    //private void OnMouseExit()
    //{
    //    Debug.Log("ON MOUSE EXIT");

    //    if (CurrentState == SelectionState.Selected
    //        || CurrentZone != Zone.Hand)
    //    {
    //        return;
    //    }

    //    // WBREWER TODO: Is there a better way to get this property to the animator?
    //    CurrentState = SelectionState.Idle;
    //}
}
