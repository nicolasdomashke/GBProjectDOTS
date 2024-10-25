using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;
using Unity.Mathematics;

class EnemyBakerScript : MonoBehaviour
{
    public float dashCD;
    public float dashDuration;
    public float dashBoost;
    public GameObject bulletPrefab;
    public float gunCD;
    public float bulletSpeed;
}

class EnemyBakerScriptBaker : Baker<EnemyBakerScript>
{
    public override void Bake(EnemyBakerScript authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new EnemyTag {});
        AddComponent(entity, new Dashable {dashCD = authoring.dashCD, dashDuration = authoring.dashDuration, dashBoost = authoring.dashBoost, dashCDCurrent = 0f, dashBoostCurrent = 0f});
        AddComponent(entity, new Shooter {bullet = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic), gunCD = authoring.gunCD, bulletSpeed = authoring.bulletSpeed});
        AddComponent(entity, new EvatuationData {distanceToClosestBullet = 0f});
        var capsuleCollider = Unity.Physics.CapsuleCollider.Create(
            new CapsuleGeometry
            {
                Radius = .5f,
                Vertex0 = new float3(0f, 1.3f, 0f),
                Vertex1 = new float3(0f, 0f, 0f),
            });
        AddComponent(entity, new PhysicsCollider {Value = capsuleCollider});
    }
}

public struct RotationSpeed : IComponentData
{
    public float speed;
}
public struct EnemyTag : IComponentData {}