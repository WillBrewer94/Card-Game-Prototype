using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputMgr : MonoBehaviour
{

    //public GameEvent grabEvent;

    //#region Singleton
    //private static InputMgr _instance;

    //public static InputMgr Instance
    //{
    //    get
    //    {
    //        if (_instance is null)
    //        {
    //            Debug.LogError("InputMgr is NULL");
    //        }

    //        return _instance;
    //    }
    //}

    //private void Awake()
    //{
    //    if (_instance == null)
    //    {
    //        _instance = this;
    //    }
    //    else if (Instance != this)
    //    {
    //        Destroy(this);
    //    }
    //}
    //#endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public static event Action OnGrabEvent;

    public GameEvent grabEvent;
    public GameEvent releaseEvent;
    public void GrabRelease(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("Grab Input Pressed!");
            grabEvent.TriggerEvent();
        }
        else if(context.canceled)
        {
            Debug.Log("Grab Input Released!");
            releaseEvent.TriggerEvent();
        }
    }

}
