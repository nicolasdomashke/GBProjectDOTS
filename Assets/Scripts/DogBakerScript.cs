using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;

class DogBakerScript : MonoBehaviour
{
    public float speed;
    public float dashCD;
    public float dashDuration;
    public float dashBoost;
    public GameObject bulletPrefab;
    public Settings settings;
}

class DogBakerScriptBaker : Baker<DogBakerScript>
{
    public override void Bake(DogBakerScript authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new MovementSpeed { speed = authoring.speed });
        AddComponent(entity, new InputEnabled {});
        AddComponent(entity, new PlayerTag {});
        AddComponent(entity, new CollectableInfo {collectableCounter = 0 });
        AddComponent(entity, new Dashable {dashCD = authoring.dashCD, dashDuration = authoring.dashDuration, dashBoost = authoring.dashBoost, dashCDCurrent = 0f, dashBoostCurrent = 0f});
        var capsuleCollider = Unity.Physics.CapsuleCollider.Create(
            new CapsuleGeometry
            {
                Radius = .3f,
                Vertex0 = new float3(0f, 1.3f, 0f),
                Vertex1 = new float3(0f, 0f, 0f),
            });
        AddComponent(entity, new PhysicsCollider {Value = capsuleCollider});
        AddComponent(entity, new Shooter {bullet = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic), gunCD = authoring.settings.playerGunCD, bulletSpeed = authoring.settings.playerBulletSpeed});
    }
}

public struct MovementSpeed : IComponentData
{
    public float speed;
}

public struct Dashable : IComponentData
{
    public float dashCD;
    public float dashDuration;
    public float dashBoost;

    [HideInInspector] public float dashCDCurrent;
    [HideInInspector] public float dashBoostCurrent;
}
public struct Shooter : IComponentData
{
    public Entity bullet;
    public float gunCD;
    public float bulletSpeed;
    [HideInInspector] public float gunCDCurrent;
}

public struct CollectableInfo : IComponentData 
{
    public int collectableCounter;
}

public struct InputEnabled : IComponentData {}
public struct PlayerTag : IComponentData {}