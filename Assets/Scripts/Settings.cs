using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Settings : ScriptableObject
{
    public float playerSpeed = 4f;
    public float playerDashCD = 3f;
    public float playerDashDuration = .07f;
    public float playerDashBoost = 30f;
    public float playerGunCD = .5f;
    public float playerBulletSpeed = 10f;
    public float enemyDashCD = 10f;
    public float enemyDashDuration = .07f;
    public float enemyDashBoost = 30f;
    public float enemyGunCD = 3f;
    public float enemyBulletSpeed = 3f;
    public bool useCloudStorage = false;
}
public class Config
{
    public Settings settings;
    public Config(Settings settings)
    {
        this.settings = settings;
    }
}