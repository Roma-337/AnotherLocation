using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnotherLocation.SceneModifyingTags
{
    internal class SceneModifierTag : SceneModifierBase
    {
        public string SceneName = "";

        public List<ColliderBox> HazardBoxes = new();

        // Single hazard respawn box.
        // Replace with a List if multiple hazard respawns are needed.
        public Vector2 HazardRespawnTriggerPos;
        public Vector2 HazardRespawnTriggerSize;
        public Vector2 HazardRespawnMarkerPos;
        public bool RespawnRight;

        protected override string GetSceneName() => SceneName;

        protected override void ModifyScene(Scene scene)
        {
            foreach (var box in HazardBoxes)
            {
                box.MakeGameObject();
            }

            AddHazardRespawnTrigger();
        }

        private void AddHazardRespawnTrigger()
        {
            GameObject markerObj = new("HazardRespawnMarker");
            markerObj.transform.position = HazardRespawnMarkerPos;
            var marker = markerObj.AddComponent<HazardRespawnMarker>();
            marker.respawnFacingRight = RespawnRight;

            GameObject triggerObj = new("Trigger")
            {
                layer = (int)PhysLayers.HERO_DETECTOR
            };
            triggerObj.transform.position = HazardRespawnTriggerPos;

            var box = triggerObj.AddComponent<BoxCollider2D>();
            box.size = HazardRespawnTriggerSize;
            box.isTrigger = true;

            var trigger = triggerObj.AddComponent<HazardRespawnTrigger>();
            trigger.respawnMarker = marker;
        }
    }
}