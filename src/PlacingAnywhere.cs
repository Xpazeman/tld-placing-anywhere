using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace PlacingAnywhere
{
    class PlacingAnywhere
    {
        public static string modsFolder;

        public static bool isPlacing = false;
        public static Vector3 lastRotation;
        public static float mouseSensivity = 0;
        public static float rotateRate = 0.15f;
        public static float nextRotate;

        public static void OnLoad()
        {
            Debug.Log("[placing-anywhere] Version " + Assembly.GetExecutingAssembly().GetName().Version);

            modsFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static RaycastHit DoRayCast(Vector3 start, Vector3 direction, bool includeGear)
        {
            int num = (int)InvPrivMethod(GameManager.GetPlayerManagerComponent(), "GetLayerMaskForPlaceMeshRaycast", new object[0]);

            if (includeGear)
            {
                num |= 131072;
            }
            RaycastHit result;
            Physics.Raycast(start, direction, out result, float.PositiveInfinity, num);
            return result;
        }

        public static object InvPrivMethod(object inst, string name, params object[] arguments)
        {
            MethodInfo method = inst.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);

            if (!method.Equals(null))
            {
                return method.Invoke(inst, arguments);
            }

            return null;
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
