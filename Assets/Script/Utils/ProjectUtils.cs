using UnityEngine;

namespace Script.Utils {
    public static class ProjectUtils {

        public const string JSON = ".json";
        public const string GLTF = ".gltf";

        public static Outline CreateOutline(GameObject gameObject) {
            var outline = gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.enabled = false;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 4;
            return outline;
        }

    }
}//END