using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Settings : ScriptableObject
{
    public float playerGunCD = .5f;
    public float playerBulletSpeed = 10f;
    public bool useCloudStorage = false;
}
