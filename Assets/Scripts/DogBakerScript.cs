using Unity.Entities;
using UnityEngine;

class DogBakerScript : MonoBehaviour
{
    public float speed;
}

class DogBakerScriptBaker : Baker<DogBakerScript>
{
    public override void Bake(DogBakerScript authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new MovementSpeed { speed = authoring.speed });
    }
}

public struct MovementSpeed : IComponentData
{
    public float speed;
}