using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shuffle()
    {
        CardGameMgr.Instance.Shuffle();
    }

    public void Draw()
    {
        CardGameMgr.Instance.Draw();
    }
}
