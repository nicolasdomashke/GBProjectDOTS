using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;

class WallBakerScript : MonoBehaviour
{
    public float speed;
}

class WallBakerScriptBaker : Baker<WallBakerScript>
{
    public override void Bake(WallBakerScript authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new RotationSpeed { speed = authoring.speed });
        var boxCollider = Unity.Physics.BoxCollider.Create(
            new BoxGeometry
            {
                Center = float3.zero,
                Size = new float3(4f, 6f, 4f),
                Orientation = Quaternion.identity
            },
            new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0,
            });
        AddComponent(entity, new PhysicsCollider {Value = boxCollider});
    }
}
