using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PerformanceUI : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    private float count;

    private IEnumerator Start()
    {
        GUI.depth = 2;
        while (true)
        {
            count = 1f / Time.unscaledDeltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        fpsText.text = "FPS: " + Mathf.Round(count);
    }

    public void CleanAndRestartECS()
    {
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
        em.CompleteAllTrackedJobs();
        foreach (var system in World.DefaultGameObjectInjectionWorld.Systems)
        {
            system.Enabled = false;
        }
        World.DefaultGameObjectInjectionWorld.Dispose();
        DefaultWorldInitialization.Initialize("Default World", false);
        if (!ScriptBehaviourUpdateOrder.IsWorldInCurrentPlayerLoop(World.DefaultGameObjectInjectionWorld))
        {
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(World.DefaultGameObjectInjectionWorld);
        }
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}