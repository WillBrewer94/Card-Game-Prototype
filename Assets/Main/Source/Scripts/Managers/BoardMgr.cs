using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardMgr : MonoBehaviour
{
    // Public
    public static BoardMgr Instance;
    public int m_gridSizeX = 5;
    public int m_gridSizeY = 5;
    public int moveRange = 3;

    // Prefabs
    public GameObject terrainPrefab;
    public GameObject terrainParent;
    public GameObject overlayPrefab;
    public GameObject overlayParent;
    public GameObject allyUnitPrefab;
    public GameObject allyUnitParent;

    // Component References
    public Camera gameCamera;
    public GameObject cursor;

    // Private
    private PlayerInputs playerInputs;

    private Dictionary<Vector3Int, GameObject> m_terrainGrid;
    private Dictionary<Vector3Int, GridSpace> m_overlayGrid;
    public GridSpace CurrentHighlight { get; set; }
    public GridSpace CurrentSelected { get; private set; }
    private PathFinder pathFinder;
    private List<GridSpace> currentPath;
    private List<GridSpace> currentMovable;
    private List<GridSpace> currentAttackable;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        playerInputs = new PlayerInputs();
        playerInputs.Player.Select.performed += OnSelect;
        playerInputs.Player.Cancel.performed += OnCancel;
        playerInputs.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_terrainGrid = new Dictionary<Vector3Int, GameObject>();
        m_overlayGrid = new Dictionary<Vector3Int, GridSpace>();

        pathFinder = new PathFinder();
        
        // Calculate spacing for prefabs
        Vector3 gridSpaceSize = terrainPrefab.GetComponent<BoxCollider>().size;

        Vector3 currentSpace = new Vector3(0.0f, 0.0f, 0.0f);

        // TODO: Ideal method: Each piece of terrain can be flagged as TopLevel or not
        // If flagged with TopLevel, create a grid overlay space above it, with rotation set to normal at center point
        for(int x = 0; x < m_gridSizeX; ++x)
        {
            for(int y = 0; y < m_gridSizeY; ++y)
            {
                GameObject newTerrain = Instantiate(terrainPrefab, currentSpace, Quaternion.identity, terrainParent.transform);
                newTerrain.name = String.Format("TerrainBlock_{0}_{1}", x, y);
                m_terrainGrid.Add(new Vector3Int(x, y, 0), newTerrain);

                Vector3 currentOverlaySpacePos = new Vector3(currentSpace.x, currentSpace.y + 0.02f, currentSpace.z);
                GameObject newOverlayTile = Instantiate(overlayPrefab, currentOverlaySpacePos, Quaternion.LookRotation(Vector3.up), overlayParent.transform);
                
                newOverlayTile.name = String.Format("Overlay_{0}_{1}", x, y);
                GridSpace newGridSpace = newOverlayTile.GetComponent<GridSpace>();
                newGridSpace.X = x;
                newGridSpace.Y = y;

                // TODO: This is bad, redo how to get component
                m_overlayGrid.Add(new Vector3Int(x, y, 0), newOverlayTile.GetComponent<GridSpace>());

                currentSpace.z += gridSpaceSize.z;
            }

            currentSpace.z = 0.0f;
            currentSpace.x += gridSpaceSize.x;
        }


        GridSpace space = m_overlayGrid[new Vector3Int(0, 0, 0)];
        if(space)
        {
            GameObject newAllyEntity = Instantiate(allyUnitPrefab, space.transform.position, Quaternion.LookRotation(Vector3.forward), allyUnitParent.transform);
            space.ContainingEntity = newAllyEntity.GetComponent<AllyUnit>(); 
        }
    }

    void OnDisable()
    {
        playerInputs.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // // GameObject hit = MouseCast();
        // GameObject hit = CursorCast();
        // if(hit && hit.GetComponent<GridSpace>())
        // {
        //     GridSpace hitSpace = hit.GetComponent<GridSpace>();
        //     hitSpace.IsHighlighting = true;

        //     // Move to New Highlight
        //     if(currentHighlight && hit != currentHighlight)
        //     {
        //         currentHighlight.GetComponent<GridSpace>().IsHighlighting = false;
        //     }

        //     currentHighlight = hit;
        // } 
        // else if(currentHighlight && currentHighlight.GetComponent<GridSpace>())
        // {
        //     // Reset highlight
        //     currentHighlight.GetComponent<GridSpace>().IsHighlighting = false;
        //     currentHighlight = null;
        // }
    }

    private GameObject MouseCast()
    {
        Ray ray = new Ray();
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = 5.0f;

        ray.origin = gameCamera.ScreenToWorldPoint(mousePos);
        ray.direction = gameCamera.transform.forward;

        Ray newRay = gameCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit castResult;
        if(Physics.Raycast(newRay, out castResult, 100.0f))
        {
            return castResult.transform.gameObject;
        }

        return null;
    }

    private GameObject CursorCast()
    {
        Ray ray = new Ray();
        ray.origin = cursor.transform.position;
        ray.direction = Vector3.down;

        RaycastHit castResult;
        if(Physics.Raycast(ray, out castResult, 100.0f))
        {
            return castResult.transform.gameObject;
        }

        return null;
    }

    void OnSelect(InputAction.CallbackContext context)
    {
        if(CurrentSelected && CurrentSelected.ContainingEntity)
        {
            CurrentSelected.ContainingEntity.SetPath(currentPath);
            CancelAll();
        }
        else if (!CurrentSelected && CurrentHighlight.ContainingEntity)
        {
            CurrentHighlight.IsSelected = true;
            CurrentSelected = CurrentHighlight;
            SetMoveRange(pathFinder.GetPossibleSpaces(CurrentSelected, CurrentHighlight.ContainingEntity.MoveRange, m_overlayGrid));
            SetAttackRange(pathFinder.GetPossibleSpaces(CurrentSelected, 
                                CurrentHighlight.ContainingEntity.MoveRange + CurrentHighlight.ContainingEntity.AttackRange, 
                                m_overlayGrid));
        }
    }

    void OnCancel(InputAction.CallbackContext context)
    {
        CancelAll();
    }

    private void CancelAll()
    {
        if(CurrentHighlight && CurrentSelected)
        {
            CurrentSelected.IsSelected = false;
            CurrentSelected = null;
            SetCurrentPath(null);
            SetMoveRange(null);
            SetAttackRange(null);
        }
    }

    public void SetHighlight(GridSpace highlight)
    {
        // If we have a selected, draw the path if the highlight does NOT equal current highlight
        if(highlight != CurrentHighlight)
        {
            if(!highlight)
            {
                CurrentHighlight.IsHighlighting = false;
                CurrentHighlight = null;
            }
            else
            {
                if(CurrentHighlight)
                {
                    CurrentHighlight.IsHighlighting = false;
                }

                CurrentHighlight = highlight;
                CurrentHighlight.IsHighlighting = true;
                Debug.Log(string.Format("SetHighlight: ({0}, {1})", CurrentHighlight.X, CurrentHighlight.Y));
            }

            if (CurrentSelected)
            {
                SetCurrentPath(pathFinder.GetPath(CurrentSelected, CurrentHighlight, moveRange, m_overlayGrid));
            }

            // Generate path to new highlight
        }

        // If we don't have a selected, set new highlight and remove old (if we had one)
    }

    void SetCurrentPath(List<GridSpace> path)
    {
        // We havea current path, reset its state and rotation
        if(currentPath != null && currentPath.Any())
        {
            foreach(var pathSpace in currentPath)
            {
                pathSpace.IsPath = false;
                pathSpace.ResetRotation();
            }
        }

        // We have a new path. Iterate through it and set path sprites
        if(path != null && path.Any())
        {
            // Path is only starting space, no need to draw anything
            if(path.Count == 1)
            {
                return;
            }

            for(int i = 1; i < path.Count; ++i)
            {
                GridSpace currentSpace = path[i];
                Vector2 currentSpacePos = new Vector2(currentSpace.X, currentSpace.Y);

                GridSpace prevSpace = path[i - 1];
                Vector2 prevSpacePos = new Vector2(path[i-1].X, path[i-1].Y);

                // Handle end node
                if(i == path.Count - 1)
                {
                    Vector2 dir = currentSpacePos - prevSpacePos;
                    dir.Normalize();
                    float theta = Mathf.Atan2(dir.y * (-1), dir.x) * Mathf.Rad2Deg;

                    currentSpace.SetPathSpace(GridSpace.GridSpaceType.PATH_END, theta);
                }
                else
                {
                    GridSpace nextSpace = path[i + 1];
                    Vector2 nextSpacePos = new Vector2(nextSpace.X, nextSpace.Y);
                    // Handle straight lines
                    if(currentSpace.X == nextSpace.X && currentSpace.X == prevSpace.X 
                        || currentSpace.Y == nextSpace.Y && currentSpace.Y == prevSpace.Y)
                    {
                        // Multiply y comp to move to right-hand coordinates
                        Vector2 dir = nextSpacePos - currentSpacePos;
                        dir.Normalize();
                        float theta = Mathf.Atan2(dir.y * (-1), dir.x) * Mathf.Rad2Deg;
                
                        currentSpace.SetPathSpace(GridSpace.GridSpaceType.PATH_STRAIGHT, theta);
                    }
                    // Handle turns
                    else 
                    {
                        // Turn
                        Vector2 dir = nextSpacePos - currentSpacePos;
                        dir.Normalize();
                        float theta = Mathf.Atan2(dir.y * (-1), dir.x) * Mathf.Rad2Deg;

                        // Turn
                        Vector2 dir2 = currentSpacePos - prevSpacePos;
                        dir2.Normalize();

                        // if((dir2.y > 0 || dir2.x > 0) && dir.x > 0 && dir.y > 0)
                        // {
                        //     theta += 90.0f * Mathf.Sign(theta);
                        // }
                        // else if((dir2.y > 0 || dir2.x > 0) && dir.x < 0 && dir.y < 0)
                        // {
                        //     theta += 90.0f * Mathf.Sign(theta);
                        // }

                        if(Mathf.Sign(dir.x + dir.y) > 0 && Mathf.Sign(dir2.x + dir2.y) > 0 || 
                            Mathf.Sign(dir.x + dir.y) < 0 && Mathf.Sign(dir2.x + dir2.y) < 0)
                        {
                            theta += 90.0f;
                        }

                        // if(Mathf.Sign(dir.x) == Mathf.Sign(dir2.x) && Mathf.Sign(dir.y) == Mathf.Sign(dir2.y))
                        // {
                        //     theta += 90.0f;
                        // }

                        // else if(dir2.y < 0 || dir2.x < 0)
                        // {
                        //     theta += 180 * Mathf.Sign(theta);
                        // }
                        // else if(dir2.y < 0 || dir2.x < 0)
                        // {
                        //     theta += 180.0f;
                        // }

                        currentSpace.SetPathSpace(GridSpace.GridSpaceType.PATH_TURN, theta);
                    }
                }
            }
        }

        currentPath = path;
    }

    void SetMoveRange(List<GridSpace> possibleSpaces)
    {
        if(currentMovable != null && currentMovable.Any())
        {
            foreach(var movSpace in currentMovable)
            {
                movSpace.IsMovable = false;
            }
        }

        if(possibleSpaces != null && possibleSpaces.Any())
        {
            foreach(var movSpace in possibleSpaces)
            {
                movSpace.IsMovable = true;
            }
        }

        currentMovable = possibleSpaces;
    }

    void SetAttackRange(List<GridSpace> attackRange)
    {
        if(currentAttackable != null && currentAttackable.Any())
        {
            foreach(var attackSpace in currentAttackable)
            {
                attackSpace.IsInAttackRange = false;
            }
        }

        if(attackRange != null && attackRange.Any())
        {
            foreach(var attackSpace in attackRange)
            {
                if(!currentMovable.Contains(attackSpace))
                {
                    attackSpace.IsInAttackRange = true;
                }
            }
        }

        currentAttackable = attackRange;
    }
}
