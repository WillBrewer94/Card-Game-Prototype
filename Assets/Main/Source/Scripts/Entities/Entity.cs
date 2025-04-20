using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int MoveRange { get; set; } = 3;
    public int JumpHeight { get; set; } = 1;
    public int Health { get; set; } = 10;
    public int Mana { get; set; } = 10;
    public int AttackRange { get; set; } = 2;
    public Sprite Portrait { get; set; }
    public List<GridSpace> currentPath;
    [SerializeField]
    private float interpSpeed = 10.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (currentPath.Count != 0)
        {
            Vector3 currentPos = transform.position;
            Vector3 nextPos = currentPath.First().transform.position;
            if(Vector3.Distance(currentPos, nextPos) > 0.01)
            {
                // Vector3.SmoothDamp(currentPos, nextPos, ref interpVelocity, interpSpeed * Time.deltaTime);
                transform.position = Vector3.MoveTowards(currentPos, nextPos, interpSpeed * Time.deltaTime);
            }
            else
            {
                if(currentPath.Count == 1)
                {
                    currentPath.First().ContainingEntity = this;
                }
                currentPath.Remove(currentPath.First());
            }
        }
    }

    public void SetPath(List<GridSpace> path) 
    {
        currentPath = path;

        // TODO: Not sure if this class should have authority on setting containing entities
        path.First().ContainingEntity = null;
    }
}
