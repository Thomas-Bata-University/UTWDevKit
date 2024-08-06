using UnityEngine;

namespace Script.Utils {
    public static class ProjectUtils {

        public const string JSON = ".json";
        public const string GLTF = ".gltf";

        public static Outline CreateOutline(GameObject gameObject, bool enable = false) {
            var outline = gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.enabled = enable;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 4;
            return outline;
        }

    }
} //END