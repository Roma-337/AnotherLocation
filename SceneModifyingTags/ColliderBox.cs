using GlobalEnums;
using Modding.Converters;
using Newtonsoft.Json;
using SFCore.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnotherLocation.SceneModifyingTags
{
    internal record ColliderBox
    {
        [JsonConverter(typeof(Vector2Converter))] public Vector2 p1;
        [JsonConverter(typeof(Vector2Converter))] public Vector2 p2;
        public bool Spikes;
        public bool Terrain;

        internal GameObject MakeGameObject()
        {
            GameObject obj = new("Hazard");
            obj.transform.position = (p1 + p2) / 2;

            var box = obj.AddComponent<BoxCollider2D>();
            box.size = new(Mathf.Abs(p2.x - p1.x), Mathf.Abs(p2.y - p1.y));

            if (Terrain) obj.layer = (int)PhysLayers.TERRAIN;
            else
            {
                box.isTrigger = true;
                obj.layer = (int)PhysLayers.ENEMIES;
                var damage = obj.AddComponent<DamageHero>();
                damage.damageDealt = 1;
                damage.hazardType = 2;

                if (!Spikes) obj.AddComponent<NonBouncer>();
                else
                {
                    var tink = obj.AddComponent<TinkEffect>();
                    //tink.blockEffect = BugPrincePreloader.Instance.Goam!.GetComponent<TinkEffect>().blockEffect;
                    //requires more preloading and isn't needed yet(code stolen from https://github.com/dplochcoder/HollowKnight.BugPrince)
                    tink.SetAttr("boxCollider", box);
                    tink.useNailPosition = true;
                }
            }

            return obj;
        }
    }
}