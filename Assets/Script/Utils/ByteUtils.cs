using UnityEngine;

namespace Script.Utils {
    public class ByteUtils : MonoBehaviour {

        public static string ToKB(long bytes, string decimals = "F2") {
            return (bytes / 1024.0).ToString(decimals);
        }

        public static string ToMB(long bytes, string decimals = "F2") {
            return (bytes / 1048576.0).ToString(decimals);
        }

        public static string ToGB(long bytes, string decimals = "F2") {
            return (bytes / 1073741824.0).ToString(decimals);
        }

    }
}//END