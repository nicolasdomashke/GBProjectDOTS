using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;
using System.Threading.Tasks;
using Zenject;

class DogBakerScript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject shootEffect;
    public GameObject deathEffect;
    public GameObject animPrefab;
}

class DogBakerScriptBaker : Baker<DogBakerScript>
{
    //public Config projConfig;

    public override void Bake(DogBakerScript authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        ConfigLoad(entity, authoring);
        //AddComponent(entity, new MovementSpeed { speed = projConfig.settings.playerSpeed });
        AddComponent(entity, new InputEnabled {});
        AddComponent(entity, new PlayerTag {deathEffect = GetEntity(authoring.deathEffect, TransformUsageFlags.Dynamic)});
        AddComponent(entity, new CollectableInfo {collectableCounter = 0 });
        //AddComponent(entity, new Dashable {dashCD = projConfig.settings.playerDashCD, dashDuration = projConfig.settings.playerDashDuration, dashBoost = projConfig.settings.playerDashBoost, dashCDCurrent = 0f, dashBoostCurrent = 0f});
        var animObject = Object.Instantiate(authoring.animPrefab);
        AddComponentObject(entity, new PlayerAnim {animObject = animObject, anim = animObject.GetComponent<Animator>()});
        var capsuleCollider = Unity.Physics.CapsuleCollider.Create(
            new CapsuleGeometry
            {
                Radius = .3f,
                Vertex0 = new float3(0f, 1.3f, 0f),
                Vertex1 = new float3(0f, 0f, 0f),
            });
        AddComponent(entity, new PhysicsCollider {Value = capsuleCollider});
        //AddComponent(entity, new Shooter {bullet = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic), shootEffect = GetEntity(authoring.shootEffect, TransformUsageFlags.Dynamic), gunCD = projConfig.settings.playerGunCD, bulletSpeed = projConfig.settings.playerBulletSpeed});
    }

    public async void ConfigLoad(Entity e, DogBakerScript authoring)
    {
        await ProjectContextLoad(e, authoring);
    }

    async Task ProjectContextLoad(Entity entity, DogBakerScript authoring)
    {
        Config projConfig = ProjectContext.Instance.Container.Resolve<Config>();
        AddComponent(entity, new MovementSpeed { speed = projConfig.settings.playerSpeed });
        AddComponent(entity, new Dashable {dashCD = projConfig.settings.playerDashCD, dashDuration = projConfig.settings.playerDashDuration, dashBoost = projConfig.settings.playerDashBoost, dashCDCurrent = 0f, dashBoostCurrent = 0f});
        AddComponent(entity, new Shooter {bullet = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic), shootEffect = GetEntity(authoring.shootEffect, TransformUsageFlags.Dynamic), gunCD = projConfig.settings.playerGunCD, bulletSpeed = projConfig.settings.playerBulletSpeed});
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
public struct Shooter : IComponentData
{
    public Entity bullet;
    public Entity shootEffect;
    public float gunCD;
    public float bulletSpeed;
    [HideInInspector] public float gunCDCurrent;
}

public struct CollectableInfo : IComponentData 
{
    public int collectableCounter;
}

public struct InputEnabled : IComponentData {}
public struct PlayerTag : IComponentData 
{
    public Entity deathEffect;
}

public class PlayerAnim : IComponentData
{
    public GameObject animObject;
    public Animator anim;
}