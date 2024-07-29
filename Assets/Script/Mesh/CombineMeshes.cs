using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.Mesh {
    public class CombineMeshes : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public void Merge() {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

            List<MeshFilter> childMeshFilters =
                new List<MeshFilter>(meshFilters).Where(mf => mf.gameObject != gameObject).ToList();
            List<MeshRenderer> childMeshRenderers =
                new List<MeshRenderer>(meshRenderers).Where(mr => mr.gameObject != gameObject).ToList();

            List<Material> materials = new List<Material>();
            foreach (MeshRenderer renderer in childMeshRenderers) {
                materials.AddRange(renderer.sharedMaterials);
            }

            materials = materials.Distinct().ToList();

            List<CombineInstance>[] combineInstances = new List<CombineInstance>[materials.Count];
            for (int i = 0; i < materials.Count; i++) {
                combineInstances[i] = new List<CombineInstance>();
            }

            for (int i = 0; i < childMeshFilters.Count; i++) {
                MeshRenderer renderer = childMeshFilters[i].GetComponent<MeshRenderer>();
                for (int j = 0; j < renderer.sharedMaterials.Length; j++) {
                    int materialIndex = materials.IndexOf(renderer.sharedMaterials[j]);
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = childMeshFilters[i].sharedMesh;
                    ci.subMeshIndex = j;
                    ci.transform = childMeshFilters[i].transform.localToWorldMatrix;
                    combineInstances[materialIndex].Add(ci);
                }

                childMeshFilters[i].gameObject.SetActive(false);
            }

            MeshFilter parentMeshFilter = GetComponent<MeshFilter>();
            UnityEngine.Mesh combinedMesh = new UnityEngine.Mesh();

            combinedMesh.subMeshCount = materials.Count;
            CombineInstance[] finalCombine = new CombineInstance[materials.Count];
            for (int i = 0; i < materials.Count; i++) {
                UnityEngine.Mesh tempMesh = new UnityEngine.Mesh();
                tempMesh.CombineMeshes(combineInstances[i].ToArray(), true);
                finalCombine[i].mesh = tempMesh;
                finalCombine[i].transform = Matrix4x4.identity;
            }

            combinedMesh.CombineMeshes(finalCombine, false);
            parentMeshFilter.mesh = combinedMesh;

            MeshRenderer parentMeshRenderer = GetComponent<MeshRenderer>();
            parentMeshRenderer.materials = materials.ToArray();

            MeshCollider parentMeshCollider = GetComponent<MeshCollider>();
            if (parentMeshCollider == null) {
                parentMeshCollider = gameObject.AddComponent<MeshCollider>();
            }

            parentMeshCollider.sharedMesh = combinedMesh;

            gameObject.SetActive(true);
        }

    }
} //END