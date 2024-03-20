using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SettingsInitializer : MonoBehaviour
{
    public GameObject performanceUI;
    public FlyCamera flyCamera;
    public TMP_InputField numRobots;
    public TMP_InputField numVictims;
    public TMP_InputField minDistanceBetweenRobotAndVictim;
    public TMP_InputField distanceThreshold;
    public TMP_InputField simulationArea_x;
    public TMP_InputField simulationArea_y;

    public void StartSimulation()
    {
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery q = em.CreateEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[] { ComponentType.ReadWrite<Settings>() }
        });
        if (q.TryGetSingleton<Settings>(out var newSettings))
        {
            Entity settingsEntity = q.GetSingletonEntity();

            newSettings.numRobots = int.Parse(numRobots.text);
            newSettings.numVictims = int.Parse(numVictims.text);
            newSettings.minDistanceBetweenRobotAndVictim = float.Parse(minDistanceBetweenRobotAndVictim.text);
            newSettings.distanceThreshold = float.Parse(distanceThreshold.text);

            newSettings.simulationArea = new float2(
                float.Parse(simulationArea_x.text),
                float.Parse(simulationArea_y.text)
                );

            newSettings.simulationStarted = true;

            em.SetComponentData(settingsEntity, newSettings);

            flyCamera.enabled = true;
            performanceUI.SetActive(true);

            Destroy(gameObject);
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}