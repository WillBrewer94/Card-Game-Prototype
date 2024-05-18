using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPoint : MonoBehaviour
{
    public float kSpeed = 5;

    public Vector2 TargetPosition { get; set; }

    private Card cardComp;

    // Start is called before the first frame update
    void Start()
    {
        TargetPosition = this.transform.position;

        Card comp = this.GetComponent<Card>();
        if(comp == null)
        {
            Debug.LogError("MoveToPoint card component not found!");
        }

        cardComp = comp;
    }

    // Update is called once per frame
    void Update()
    {
        if (cardComp.CurrentState == Constants.SelectionState.Selected)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, TargetPosition, kSpeed * Time.deltaTime);
        }
    }
}
