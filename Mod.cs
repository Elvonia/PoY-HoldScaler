using MelonLoader;
using PoY_HoldScaler;
using UnityEngine;

[assembly: MelonInfo(typeof(Mod), "Hold Scaler", PluginInfo.PLUGIN_VERSION, "Kalico")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

namespace PoY_HoldScaler
{
    public class Mod : MelonMod
    {
        private float scaleFactor = 0.25f;
        private string[] holdTypes = new string[]
        {
            "Climbable",
            "ClimbableMicroHold",
            "ClimbableRigidbody",
            "Crack",
            "ClimbablePitch",
            "Ice",
            "PinchHold",
            "Volume"
        };

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
                for (int i = 0; i < holdTypes.Length; i++)
                {
                    if (holdTypes[i].Equals(obj.tag))
                    {
                        if (ProcessMeshFromCollider(obj))
                        {
                            ScaleObject(obj, scaleFactor);
                        }
                        if (ProcessSphereFromCollider(obj))
                        {
                            ScaleObject(obj, scaleFactor);
                        }
                        if (ProcessBoxFromCollider(obj))
                        {
                            ScaleObject(obj, scaleFactor);
                        }

                        processedCount++;
                    }
                }
            }

            MelonLogger.Msg($"Processed {processedCount} 'Climbable' objects.");
        }

        private bool ProcessMeshFromCollider(GameObject obj)
        {
            MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();

            if (meshCollider == null || meshCollider.sharedMesh == null)
            {
                meshCollider = obj.GetComponentInChildren<MeshCollider>();
                if (meshCollider == null) // Child Ice
                    return false;
            }

            if (meshFilter == null)
            {
                meshFilter = obj.GetComponentInChildren<MeshFilter>();
                if (meshFilter == null) // Child Ice
                    return false;
            }

            meshFilter.sharedMesh = meshCollider.sharedMesh;
            MelonLogger.Msg($"Mesh '{meshCollider.sharedMesh.name}' on '{obj.name}' assigned.");
            return true;
        }

        private bool ProcessSphereFromCollider(GameObject obj)
        {
            SphereCollider sphereCollider = obj.GetComponent<SphereCollider>();

            if (sphereCollider == null)
            {
                return false;
            }

            GameObject spherePrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();

            if (meshFilter == null)
            {
                return false;
            }

            float worldRadius = sphereCollider.radius * Mathf.Max(
                obj.transform.localScale.x, 
                obj.transform.localScale.y, 
                obj.transform.localScale.z
                );
            float scaleFactor = worldRadius / 0.5f;

            obj.transform.localScale = Vector3.one;
            sphereCollider.radius = spherePrimitive.GetComponent<SphereCollider>().radius;
            sphereCollider.center = spherePrimitive.GetComponent<SphereCollider>().center;
            meshFilter.sharedMesh = spherePrimitive.GetComponent<MeshFilter>().sharedMesh;
            obj.transform.localScale *= scaleFactor;

            GameObject.DestroyImmediate(spherePrimitive);

            return true;
        }

        private bool ProcessBoxFromCollider(GameObject obj)
        {
            BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                return false;
            }

            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                return false;
            }
            GameObject cubePrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Vector3 scaleFactor = Vector3.Scale(boxCollider.size, obj.transform.localScale);          
            
            obj.transform.localScale = Vector3.one;
            boxCollider.size = cubePrimitive.GetComponent<BoxCollider>().size;
            boxCollider.center = cubePrimitive.GetComponent<BoxCollider>().center;
            meshFilter.sharedMesh = cubePrimitive.GetComponent<MeshFilter>().sharedMesh;
            obj.transform.localScale = scaleFactor;

            GameObject.DestroyImmediate(cubePrimitive);

            return true;
        }

        private void ScaleObject(GameObject obj, float scaleFactor)
        {
            obj.transform.localScale *= scaleFactor;
            MelonLogger.Msg($"Scaled: '{obj.name}' to {obj.transform.localScale}");
        }
    }
}