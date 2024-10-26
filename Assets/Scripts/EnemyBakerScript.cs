using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;
using Unity.Mathematics;
using Zenject;

class EnemyBakerScript : MonoBehaviour
{
    public GameObject bulletPrefab;
}

class EnemyBakerScriptBaker : Baker<EnemyBakerScript>
{
    public override void Bake(EnemyBakerScript authoring)
    {
        Config projConfig = ProjectContext.Instance.Container.Resolve<Config>();

        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new EnemyTag {});
        AddComponent(entity, new Dashable {dashCD = projConfig.settings.enemyDashCD, dashDuration = projConfig.settings.enemyDashDuration, dashBoost = projConfig.settings.enemyDashBoost, dashCDCurrent = 0f, dashBoostCurrent = 0f});
        AddComponent(entity, new Shooter {bullet = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic), gunCD = projConfig.settings.enemyGunCD, bulletSpeed = projConfig.settings.enemyBulletSpeed});
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