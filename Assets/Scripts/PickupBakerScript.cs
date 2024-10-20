using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;

class PickupBakerScript : MonoBehaviour
{
    public float speed;
}

class PickupBakerScriptBaker : Baker<PickupBakerScript>
{
    public override void Bake(PickupBakerScript authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new RotationSpeed { speed = authoring.speed });
        var boxCollider = Unity.Physics.BoxCollider.Create(
            new BoxGeometry
            {
                Center = float3.zero,
                Size = new float3(.5f, .5f, .5f),
                Orientation = Quaternion.identity
            },
            new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0,
            },
            new Unity.Physics.Material
            {
                CollisionResponse = CollisionResponsePolicy.RaiseTriggerEvents
            });

        AddComponent(entity, new PhysicsCollider {Value = boxCollider});
        AddComponent(entity, new CollectableTag {});
    }
}
public struct CollectableTag : IComponentData {}