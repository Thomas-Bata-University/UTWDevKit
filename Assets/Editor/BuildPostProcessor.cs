using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor {
    public class BuildPostProcessor : IPostprocessBuildWithReport {

        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report) {
            try {
                string sourcePath = Path.Combine(Application.dataPath, "StreamingAssets");
                string targetPath = Path.Combine(report.summary.outputPath, "StreamingAssets");

                if (Directory.Exists(sourcePath)) {
                    foreach (string file in Directory.GetFiles(sourcePath)) {
                        string fileName = Path.GetFileName(file);
                        string destFile = Path.Combine(targetPath, fileName);
                        File.Copy(file, destFile, true);
                    }
                }
            }
            catch (Exception e) {
            }
        }

    }
} //END