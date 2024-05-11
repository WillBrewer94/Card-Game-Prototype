using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class CardGameMgr : MonoBehaviour
{
    #region Singleton
    private static CardGameMgr _instance;

    public static CardGameMgr Instance
    {
        get
        {
            if (_instance is null)
            {
                Debug.LogError("CardGameMgr is NULL");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else if(Instance != this)
        {
            Destroy(this);
        }
    }
    #endregion

    // Public
    public GameObject cardPrefab;

    [SerializeField]
    private int deckSize;

    [SerializeField]
    private int handSize;

    public List<GameObject> handAnchorPoints;
    public GameObject       deckAnchorPoint;

    // Private Members
    private List<GameObject> deck;
    private List<GameObject> hand;

    private int handAnchorIdx = 0;

    private GameObject highlightedCard;
    private GameObject selectedCard;

    // Start is called before the first frame update
    void Start()
    {
        deck = new List<GameObject>(deckSize);
        hand = new List<GameObject>(handSize);

        if (cardPrefab == null)
        {
            Debug.LogError("Card prefab is NULL!");
            return;
        }

        if(deckAnchorPoint == null)
        {
            Debug.LogError("deckAnchorPoint prefab is NULL!");
            return;
        }

        // Initialize a deck of card gameobjects
        for (int i = 0; i < deckSize; ++i)
        {
            GameObject gameObject = Instantiate(cardPrefab);

            gameObject.transform.position = Camera.main.ScreenToWorldPoint(deckAnchorPoint.transform.position);

            deck.Add(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shuffle()
    {
        Debug.Log("Shuffling!");

        if (deck.Count == 0)
        {
            return;
        }

        // Fischer-Yates Shuffle
        int currIdx = deck.Count;
        while(currIdx > 1)
        {
            currIdx--;

            int randIdx = Random.Range(0, currIdx);

            GameObject value = deck[randIdx];
            deck[randIdx] = deck[currIdx];
            deck[currIdx] = value;
        }
    }

    public void Draw()
    {
        if(hand.Count >= handSize)
        {
            Debug.Log("Hit hand size limit!");
            return;
        }

        if (deck.Count == 0)
        {
            Debug.Log("Deck Empty!");
            return;
        }

        GameObject drawnCard = deck[deck.Count - 1];
        hand.Add(drawnCard);
        deck.RemoveAt(deck.Count - 1);


        // Find number of segments based on hand size
        int segmentCount = hand.Count + 1;

        Vector2 leftPos = Camera.main.ScreenToWorldPoint(handAnchorPoints[0].transform.position);
        Vector2 rightPos = Camera.main.ScreenToWorldPoint(handAnchorPoints[handAnchorPoints.Count-1].transform.position);
        float dist = Vector2.Distance(leftPos, rightPos);
        float segmentLen = dist / segmentCount;

        // Find half-way point for distance between two anchors
        for (int i = 0; i < hand.Count; ++i)
        {
            MoveToPoint moveMeComponent = hand[i].GetComponent<MoveToPoint>();
            if(moveMeComponent)
            {
                Vector2 targetPos = leftPos;
                targetPos.x += segmentLen * (i + 1);
                moveMeComponent.TargetPosition = targetPos;
            }
        }

        // Move drawn card to anchor point on screen
        //MoveToPoint moveMe = drawnCard.GetComponent<MoveToPoint>();
        //if(moveMe != null && handAnchorPoints.Count > 0)
        //{
        //    Vector2 worldSpacePoint = Camera.main.ScreenToWorldPoint(handAnchorPoints[handAnchorIdx].transform.position);
        //    MoveToPoint moveMeComponent = drawnCard.GetComponent<MoveToPoint>();

        //    if(moveMeComponent != null)
        //    {
        //        moveMeComponent.TargetPosition = worldSpacePoint;
        //    }
        //}

        handAnchorIdx++;
    }

    //public void Grab(InputAction.CallbackContext context)
    //{
    //    if(context.performed)
    //    {
    //        Debug.Log("CLICK!");

    //        if (highlightedCard != null)
    //        {
    //            Debug.Log("Grabbing highlighted card!");
    //            Card card = highlightedCard.GetComponent<Card>();
    //            if(card != null)
    //            {
    //                selectedCard = highlightedCard;
    //                card.State = Card.SelectionState.Selected;
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("CardGameMgr::Grab() highlighted card is null!");
    //        }
    //    }
    //    else if(context.canceled)
    //    {
    //        Debug.Log("RELEASE!");

    //        if(highlightedCard != null)
    //        {
    //            Debug.Log("Releasing highlighted card!");
    //            Card card = highlightedCard.GetComponent<Card>();
    //            if (card != null)
    //            {
    //                card.State = Card.SelectionState.Idle;
    //            }
    //        }

    //        selectedCard = null;
    //    }
    //}

    public void SetHighlightedCard(GameObject card)
    {
        if(card != null)
        {
            Debug.Log("Setting highlighted card");
        }
        else
        {
            Debug.Log("Setting highlighted card to null");
        }

        highlightedCard = card;
    }
}