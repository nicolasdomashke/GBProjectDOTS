using Unity.Entities;
using UnityEngine;

class EnemyBakerScript : MonoBehaviour
{
    public float speed;
}

class EnemyBakerScriptBaker : Baker<EnemyBakerScript>
{
    public override void Bake(EnemyBakerScript authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new RotationSpeed { speed = authoring.speed });
    }
}

public struct RotationSpeed : IComponentData
{
    public float speed;
}