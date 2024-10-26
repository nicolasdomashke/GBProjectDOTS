using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ConfigInjector : MonoInstaller
{
    public Settings settings;
    public override void InstallBindings()
    {
        Container.BindInstance(settings).AsSingle();
        Container.Bind<Config>().AsSingle().WithArguments(settings);
    }
}