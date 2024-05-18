using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
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
}
