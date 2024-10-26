using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public int collectablesFound;
    public PlayerStats()
    {
        collectablesFound = 0;
    }
}