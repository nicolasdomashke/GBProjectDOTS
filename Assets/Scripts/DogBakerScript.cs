using Unity.Entities;
using UnityEngine;

class DogBakerScript : MonoBehaviour
{
    public float speed;
    public float dashCD;
    public float dashDuration;
    public float dashBoost;

}

class DogBakerScriptBaker : Baker<DogBakerScript>
{
    public override void Bake(DogBakerScript authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new MovementSpeed { speed = authoring.speed });
        AddComponent(entity, new InputEnabled {});
        AddComponent(entity, new Dashable {dashCD = authoring.dashCD, dashDuration = authoring.dashDuration, dashBoost = authoring.dashBoost, dashCDCurrent = 0f, dashBoostCurrent = 0f});
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

public struct InputEnabled : IComponentData {}