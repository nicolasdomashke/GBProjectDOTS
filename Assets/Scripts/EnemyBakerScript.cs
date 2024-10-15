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
        AddComponent(entity, new MovementSpeed { speed = authoring.speed });
    }
}
