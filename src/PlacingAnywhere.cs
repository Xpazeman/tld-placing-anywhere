using System.Reflection;
using UnityEngine;
using MelonLoader;

namespace PlacingAnywhere
{
    class PlacingAnywhere : MelonMod
    {
        public static bool isPlacing = false;
        public static Vector3 lastRotation;
        public static float positionYOffset = 0f;
        public static float mouseSensivity = 0;
        public static float rotateAngle = 15f;
        public static float positionOffset = 0.02f;
        public static float nextRotate;

        public override void OnApplicationStart()
        {
            Debug.Log("[placing-anywhere] Version " + Assembly.GetExecutingAssembly().GetName().Version);
        }

        public static RaycastHit DoRayCast(Vector3 start, Vector3 direction, bool includeGear)
        {
            int num = PlayerManager.GetLayerMaskForPlaceMeshRaycast();

            if (includeGear)
            {
                num |= 131072;
            }
            RaycastHit result;
            Physics.Raycast(start, direction, out result, float.PositiveInfinity, num);
            return result;
        }

        public static Mesh GetMesh(GameObject go)
        {
            MeshFilter componentInChildren = go.GetComponentInChildren<MeshFilter>();
            if (componentInChildren != null)
            {
                if (componentInChildren.mesh != null)
                {
                    return componentInChildren.mesh;
                }
                if (componentInChildren.sharedMesh != null)
                {
                    return componentInChildren.sharedMesh;
                }
            }
            SkinnedMeshRenderer componentInChildren2 = go.GetComponentInChildren<SkinnedMeshRenderer>();
            if (!(componentInChildren2 != null))
            {
                return null;
            }
            return componentInChildren2.sharedMesh;
        }

        public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {
            return Vector3.Dot(planeNormal, point - planePoint);
        }
    }
}
