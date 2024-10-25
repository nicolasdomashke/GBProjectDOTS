using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;

class BulletBakerScript : MonoBehaviour
{
    
}

class BulletBakerScriptBaker : Baker<BulletBakerScript>
{
    public override void Bake(BulletBakerScript authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        var sphereCollider = Unity.Physics.SphereCollider.Create(
            new SphereGeometry
            {
                Radius = .25f,
            },
            new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0,
            },
            new Unity.Physics.Material
            {
                CollisionResponse = CollisionResponsePolicy.CollideRaiseCollisionEvents
            });
        AddComponent(entity, new PhysicsCollider {Value = sphereCollider});
        AddComponent(entity, new Prefab {});
        AddComponent(entity, new BulletTag {});
    }
}

public struct BulletTag : IComponentData {}