﻿using MelonLoader;
using PoY_HoldScaler;
using UnityEngine;

[assembly: MelonInfo(typeof(Mod), "Hold Scaler", PluginInfo.PLUGIN_VERSION, "Kalico")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

namespace PoY_HoldScaler
{
    public class Mod : MelonMod
    {
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            ProcessAllClimbableMeshes();
        }

        private void ProcessAllClimbableMeshes()
        {
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
            int processedCount = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.CompareTag("Climbable"))
                {
                    ProcessMeshFromCollider(obj);
                    ScaleObject(obj, 2.0f);
                    processedCount++;
                }
            }

            MelonLogger.Msg($"Processed {processedCount} 'Climbable' objects.");
        }

        private void ProcessMeshFromCollider(GameObject obj)
        {
            MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();

            if (meshCollider == null || meshCollider.sharedMesh == null)
            {
                MelonLogger.Warning($"No valid MeshCollider found on '{obj.name}'");
                return;
            }

            if (meshFilter == null)
            {
                MelonLogger.Warning($"No MeshFilter found on '{obj.name}'");
                return;
            }

            meshFilter.sharedMesh = meshCollider.sharedMesh;
            MelonLogger.Msg($"Mesh '{meshCollider.sharedMesh.name}' on '{obj.name}' has been duplicated and assigned to the MeshFilter.");
        }

        private void ScaleObject(GameObject obj, float scaleFactor)
        {
            obj.transform.localScale *= scaleFactor;
            MelonLogger.Msg($"Scaled '{obj.name}' to {obj.transform.localScale}");
        }
    }
}