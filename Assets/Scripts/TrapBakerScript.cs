using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;

class TrapBakerScript : MonoBehaviour
{
    public GameObject deathEffect;
    public float speed;
}

class TrapBakerScriptBaker : Baker<TrapBakerScript>
{
    public override void Bake(TrapBakerScript authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new RotationSpeed { speed = authoring.speed });
        AddComponent(entity, new TrapTag {isTriggered = false, isFinished = false, deathEffect = GetEntity(authoring.deathEffect, TransformUsageFlags.Dynamic)});
    }
}
public struct TrapTag : IComponentData {
    public bool isTriggered;
    public bool isFinished;
    public Entity deathEffect;
}