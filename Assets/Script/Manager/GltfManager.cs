using System;
using System.IO;
using System.Threading.Tasks;
using GLTFast;
using Script.Component.Parts;
using UnityEngine;
using Logger = Script.Log.Logger;

namespace Script.Manager {
    public class GltfManager : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public static GltfManager Instance;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }

        public async Task<bool> OnMeshLoad(string path, Transform objectInstance) {
            if (String.IsNullOrEmpty(path)) {
                return false;
            }

            if (!File.Exists(path)) {
                Logger.Instance.LogErrorMessage($"Mesh cannot be loaded. File does not exist {path}");
                return false;
            }

            var gltf = new GltfImport();
            var settings = new ImportSettings {
                GenerateMipMaps = true,
                AnisotropicFilterLevel = 3,
                NodeNameMethod = NameImportMethod.OriginalUnique
            };

            try {
                var success = await gltf.Load(path, settings);

                if (success) {
                    var instantiator = new GameObjectInstantiator(gltf, objectInstance);
                    return await gltf.InstantiateMainSceneAsync(instantiator);
                }

                throw new Exception($"An error occured during importing GLTF file: {path}");
            }
            catch (Exception e) {
                Logger.Instance.LogErrorMessage(e.Message);
            }

            return false;
        }

        public async Task<GraphicComponent.Data> ImportMesh(string path, Transform objectInstance) {
            if (!File.Exists(path)) {
                Logger.Instance.LogErrorMessage($"Mesh cannot be loaded. File does not exist {path}");
                return null;
            }

            var gltf = new GltfImport();
            var settings = new ImportSettings {
                GenerateMipMaps = true,
                AnisotropicFilterLevel = 3,
                NodeNameMethod = NameImportMethod.OriginalUnique
            };

            try {
                var success = await gltf.Load(path, settings);

                if (success) {
                    var instantiator = new GameObjectInstantiator(gltf, objectInstance);
                    return new GraphicComponent.Data(gltf, objectInstance, instantiator, Path.GetFileName(path), path);
                }

                throw new Exception($"An error occured during importing GLTF file: {path}");
            }
            catch (Exception e) {
                Logger.Instance.LogErrorMessage(e.Message);
                return null;
            }
        }

    }
} //END