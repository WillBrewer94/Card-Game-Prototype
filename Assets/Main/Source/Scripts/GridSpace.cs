using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class GridSpace : MonoBehaviour
{
    public enum GridSpaceType {
        DEFAULT = 0,
        HIGHLIGHT,
        SELECTED,
        PATH_STRAIGHT,
        PATH_TURN,
        PATH_END,
        MOVABLE,
        BLOCKED
    }

    // State Sprites
    public Sprite defaultSprite;
    public Sprite highlightSprite;
    public Sprite selectedSprite;
    public Sprite pathSprite;
    public Sprite movableSprite;
    public Sprite blockedSprite;

    // Path Sprites
    public Sprite end;
    public Sprite turn;
    public Sprite straight;

    private SpriteRenderer spriteRenderer;

    public int X { get; set; }
    public int Y { get; set; }

    public Entity ContainingEntity { get; set; }
    public bool IsBlocked { 
        get { return ContainingEntity != null; } 
    }

    public bool IsHighlighting { get; set; } = false;
    public bool IsSelected { get; set; } = false;
    public bool IsPath { get; set; } = false;
    public bool IsMovable { get; set; } = false;
    public bool IsInAttackRange { get; set; } = false;
    public GameObject TerrainTile { get; set; }
    public GameObject OccupyingEntity { get; set; }

    private Quaternion origRotation;

    // Pathfinding vars
    public int G { get; set; }
    public int H { get; set; }
    public int F { get { return G + H; } }
    public GridSpace previous { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        origRotation = spriteRenderer.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInAttackRange)
        {
            spriteRenderer.sprite = blockedSprite;
        }
        else if(IsSelected)
        {
            spriteRenderer.sprite = selectedSprite;
        }
        else if(IsPath)
        {
            // TODO: Find a better way to do this
        }
        else if(IsMovable)
        {
            spriteRenderer.sprite = movableSprite;
        }
        else if(IsHighlighting)
        {
            //Debug.Log("IsHighlighted!");
            spriteRenderer.sprite = highlightSprite;
        }
        else
        {
            spriteRenderer.sprite = null;
            // spriteRenderer.enabled = false;
        }
    }

    public void SetPathSpace(GridSpaceType gridSpaceType, float zRotation)
    {
        Vector3 rotation = spriteRenderer.gameObject.transform.rotation.eulerAngles;
        rotation.z = zRotation;
        spriteRenderer.gameObject.transform.rotation = Quaternion.Euler(rotation);

        if(gridSpaceType == GridSpaceType.PATH_STRAIGHT)
        {
            spriteRenderer.sprite = straight;
        }
        else if(gridSpaceType == GridSpaceType.PATH_TURN)
        {
            spriteRenderer.sprite = turn;
        }
        else if(gridSpaceType == GridSpaceType.PATH_END)
        {
            spriteRenderer.sprite = end;
        }

        IsPath = true;
    }

    public void ResetRotation()
    {
        spriteRenderer.gameObject.transform.rotation = origRotation;
    }
}
