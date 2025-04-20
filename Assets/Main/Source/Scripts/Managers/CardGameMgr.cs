using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;


public class CardGameMgr : MonoBehaviour
{
    // Singleton Registration
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
        if (_instance == null)
        {
            _instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }
    #endregion

    // Public
    public GameObject cardPrefab;

    [SerializeField]
    private int kDeckSize;

    [SerializeField]
    private int kHandSize;

    [SerializeField]
    private int kCardsDrawnPerTurn;

    [SerializeField]
    private float kCardDrawWaitTime = 0.5f;

    [SerializeField]
    private int kStartingMana;

    [SerializeField]
    private int kStartingManaMax;

    [SerializeField]
    private int kManaIncrementPerTurn;

    public GameObject currentManaText;
    public GameObject currentDamageText;
    public GameObject currentMovementText;

    public GameObject handAnchorLeft;
    public GameObject handAnchorRight;
    public GameObject deckAnchorPoint;
    public GameObject discAnchorPoint;

    // Private Members
    private List<GameObject> deck;
    private List<GameObject> hand;
    private List<GameObject> disc;

    private TextMeshProUGUI manaTextMesh;
    private TextMeshProUGUI damageTextMesh;
    private TextMeshProUGUI movementTextMesh;

    private int _currentMana;
    public int CurrentMana
    {
        get
        {
            return _currentMana;
        }
        set
        {
            _currentMana = value;

            manaTextMesh = currentManaText.GetComponent<TextMeshProUGUI>();
            if (manaTextMesh)
            {
                manaTextMesh.SetText("{0}/{1}", _currentMana, CurrentManaMax);
            }
        }
    }

    private int _currentManaMax;
    public int CurrentManaMax
    {
        get
        {
            return _currentManaMax;
        }
        set
        {
            _currentManaMax = value;

            manaTextMesh = currentManaText.GetComponent<TextMeshProUGUI>();
            if (manaTextMesh)
            {
                manaTextMesh.SetText("{0}/{1}", CurrentMana, _currentManaMax);
            }
        }
    }

    private int _currentDamage;
    public int CurrentDamage
    {
        get
        {
            return _currentDamage;
        }
        set
        {
            _currentDamage = value;

            damageTextMesh = currentDamageText.GetComponent<TextMeshProUGUI>();
            if (damageTextMesh)
            {
                damageTextMesh.SetText("Damage: {0}", _currentDamage);
            }
        }
    }

    private int _currentMovement;
    public int CurrentMovement
    {
        get
        {
            return _currentMovement;
        }
        set
        {
            _currentMovement = value;

            movementTextMesh = currentMovementText.GetComponent<TextMeshProUGUI>();
            if (movementTextMesh)
            {
                movementTextMesh.SetText("Movement:{0}", _currentMovement);
            }
        }
    }

    private int handAnchorIdx = 0;

    private GameObject highlightedCard;
    private GameObject selectedCard;

    // Start is called before the first frame update
    void Start()
    {
        deck = new List<GameObject>(kDeckSize);
        hand = new List<GameObject>(kHandSize);
        disc = new List<GameObject>(kDeckSize);

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

        if (discAnchorPoint == null)
        {
            Debug.LogError("deckAnchorPoint prefab is NULL!");
            return;
        }

        if (currentManaText == null)
        {
            Debug.LogError("currentManaText prefab is NULL!");
            return;
        }

        // Initialize a deck of card gameobjects
        for (int i = 0; i < kDeckSize; ++i)
        {
            // Instantiate new card
            GameObject newCard = Instantiate(cardPrefab);
            deck.Add(newCard);

            // Set created card zone to deck
            Card cardComp = newCard.GetComponent<Card>();
            if (cardComp != null)
            {
                cardComp.CurrentZone = Constants.Zone.Deck;
            }

            // Set position to deck anchor point
            newCard.transform.position = deckAnchorPoint.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shuffle()
    {
        Debug.Log("Shuffling!");

        foreach(GameObject obj in disc)
        {
            deck.Add(obj);
            Card card = obj.GetComponent<Card>();
            if (card)
            {
                card.CurrentZone = Constants.Zone.Deck;
            }

            MoveToPoint moveMeComp = obj.GetComponent<MoveToPoint>();
            moveMeComp.TargetPosition = deckAnchorPoint.transform.position;
        }
        disc.Clear();

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

    public void Draw(int numDraw=1)
    {
        if(hand.Count >= kHandSize)
        {
            Debug.Log("Hit hand size limit!");
            return;
        }

        if (deck.Count == 0)
        {
            Debug.Log("Deck Empty!");
            return;
        }

        for(int i = 0; i < numDraw; ++i)
        {
            // Draw card from deck and add to hand
            GameObject drawnCard = deck[deck.Count - 1];
            hand.Add(drawnCard);
            deck.RemoveAt(deck.Count - 1);

            // Set cards zone
            Card card = drawnCard.GetComponent<Card>();
            if (card != null)
            {
                card.CurrentZone = Constants.Zone.Hand;
                card.OrderInLayer = kHandSize - hand.Count;
            }

            RecalculateHandSpacing();
        }
    }

    public void AddDamage(int numDamage=1)
    {
        CurrentDamage = CurrentDamage + numDamage;
    }

    public void AddMovement(int numMovement=1)
    {
        CurrentMovement = CurrentMovement + numMovement;
    }

    public void StartGame()
    {
        // Set initial mana
        manaTextMesh = currentManaText.GetComponent<TextMeshProUGUI>();
        if (manaTextMesh == null)
        {
            Debug.LogError("currentManaText object missing TextMeshProUGUI component!");
            return;
        }
        else
        {
            CurrentMana = kStartingMana;
            CurrentManaMax = kStartingManaMax;
            manaTextMesh.SetText("{0}/{1}", CurrentMana, CurrentManaMax);
        }

        // Draw starting hand coroutine up to hand size
        StartCoroutine(DrawCards(kHandSize));
    }

    public void EndTurn()
    {
        CurrentManaMax += kManaIncrementPerTurn;
        CurrentMana = CurrentManaMax;
        StartCoroutine(DrawCards(kCardsDrawnPerTurn));
    }

    public void AddCardToDisc(GameObject cardToDisc)
    {
        // Find card in hand
        foreach(GameObject card in hand)
        {
            if (card == cardToDisc)
            {
                hand.Remove(cardToDisc);
                disc.Add(cardToDisc);

                MoveToPoint moveMeComp = cardToDisc.GetComponent<MoveToPoint>();
                if (moveMeComp)
                {
                    moveMeComp.TargetPosition = discAnchorPoint.transform.position;
                }

                break;
            }
        }

        RecalculateHandSpacing();
    }

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

    private void RecalculateHandSpacing()
    {
        // Find out what the card's position should be 
        // Find number of segments based on hand size
        int segmentCount = hand.Count + 1;

        Vector2 leftPos = handAnchorLeft.transform.position;
        Vector2 rightPos = handAnchorRight.transform.position;
        float dist = Vector2.Distance(leftPos, rightPos);
        float segmentLen = dist / segmentCount;

        // Find half-way point for distance between two anchors
        for (int i = 0; i < hand.Count; ++i)
        {
            MoveToPoint moveMeComponent = hand[i].GetComponent<MoveToPoint>();
            if (moveMeComponent)
            {
                Vector2 targetPos = leftPos;
                targetPos.x += segmentLen * (i + 1);
                moveMeComponent.TargetPosition = targetPos;
            }
        }

        handAnchorIdx++;
    }

    private IEnumerator DrawCards(int numCards)
    {
        for(int cardsDrawn = 0; cardsDrawn < numCards; cardsDrawn += 1)
        {
            Draw();
            yield return new WaitForSeconds(kCardDrawWaitTime);
        }
    }
}
