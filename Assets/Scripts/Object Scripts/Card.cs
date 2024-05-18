using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    // Component and Script References
    private Animator selectionStateAnimatorComp;
    private SpriteRenderer spriteRendererComp;
    private MoveToPoint moveMeComp;
    private CardGameMgr cardGameMgr;

    // Card Canvas References
    public GameObject canvasObj;
    public GameObject manaCostObj;
    public GameObject nameObj;
    public GameObject descObj;

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

    private Constants.SelectionState _currentState = 0;
    public Constants.SelectionState CurrentState
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

    private Constants.Zone _currentZone;
    public Constants.Zone CurrentZone
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

            if (_currentZone == Constants.Zone.Deck)
            {
                canvasObj.SetActive(false);
            }
            else
            {
                canvasObj.SetActive(true);
            }

            switch (_currentZone)
            {
                case Constants.Zone.Deck:
                    SortingLayer = "Deck";
                    return;
                case Constants.Zone.Hand:
                    SortingLayer = "Hand";
                    return;
                case Constants.Zone.Discard:
                    SortingLayer = "Discard";
                    return;
                default:
                    SortingLayer = "Default";
                    return;
            }
        }
    }

    public int CurrentCardManaCost { get; set; }

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

        moveMeComp = this.GetComponent<MoveToPoint>();
        if (moveMeComp == null)
        {
            Debug.LogError("MoveToPoint component not found!");
        }

        if (manaCostObj == null || nameObj == null || descObj == null)
        {
            Debug.LogError("manaCostObj/nameObj/descObj not found!");
        }

        // WBREWER TESTING: Set up initial mana costs for testing
        CurrentCardManaCost = Random.Range(1, 4);
        TextMeshProUGUI manaTextMesh = manaCostObj.GetComponent<TextMeshProUGUI>();
        if (manaTextMesh)
        {
            manaTextMesh.SetText("{0}", CurrentCardManaCost);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // wbrewer TODO: Add mana cost checking here
        float moveDelta = Mathf.Abs(moveMeComp.TargetPosition.y - this.transform.position.y);
        if (CurrentState == Constants.SelectionState.Selected && moveDelta >= kCardPlayThreshhold
            && CurrentCardManaCost <= CardGameMgr.Instance.CurrentMana)
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
