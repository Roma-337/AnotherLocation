using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using AnotherLocation.ConditionChecks;

namespace AnotherLocation.LocationSpecifics
{
    internal static class CitySpikeRoofFix
    {
        private const string SceneName = "Ruins1_25";
        private const string TargetName = "Shiny Item-East_Sanctum_Spike_Roof";

        public static void Hook()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != SceneName)
                return;

            if (!BugPrinceCheck.ShouldEnableBugPrinceAdvancedLocation())
                return;

            if (GameManager.instance != null)
                GameManager.instance.StartCoroutine(ApplyAfterLoad());
        }

        private static IEnumerator ApplyAfterLoad()
        {
            for (int i = 0; i < 30; i++)
            {
                var go = GameObject.Find(TargetName);
                if (go != null)
                {
                    ForceForeground(go);
                    yield break;
                }

                yield return null;
            }
        }
        //render the shiny in front of the foreground spikes
        private static void ForceForeground(GameObject go)
        {
            foreach (var sr in go.GetComponentsInChildren<SpriteRenderer>(true))
            {
                sr.sortingLayerName = "Immediate FG";
                sr.sortingOrder = 5000;
            }
        }
    }
}