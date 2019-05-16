using Harmony;
using UnityEngine;

namespace PlacingAnywhere
{
    class Patches
    {
        [HarmonyPatch(typeof(PlayerManager), "DoPositionCheck")]
        internal class PlayerManager_DoPositionCheck
        {
            private static void Postfix(PlayerManager __instance, ref MeshLocationCategory __result)
            {
                GameObject gameObject = __instance.GetObjectToPlace();

                if (!PlacingAnywhere.isPlacing)
                {
                    PlacingAnywhere.isPlacing = true;
                    PlacingAnywhere.lastRotation = gameObject.transform.eulerAngles;
                }

                gameObject.transform.eulerAngles = PlacingAnywhere.lastRotation;

                if (Input.mouseScrollDelta.y != 0)
                {
                    float angleUp = Input.mouseScrollDelta.y;
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        angleUp *= 5;
                    }

                    gameObject.transform.Rotate(Vector3.up, angleUp, Space.World);
                }

                /*if (Input.GetKey(KeyCode.N) && Time.time > PlacingAnywhere.nextRotate)
                {
                    PlacingAnywhere.nextRotate = Time.time + PlacingAnywhere.rotateRate;

                    System.Random r = new System.Random();
                    float angleUp = r.Next(0, 360);

                    gameObject.transform.Rotate(Vector3.up, angleUp, Space.World);
                }*/

                if (Input.GetKey(KeyCode.Period) && Time.time > PlacingAnywhere.nextRotate)
                {
                    PlacingAnywhere.nextRotate = Time.time + PlacingAnywhere.rotateRate;
                    gameObject.transform.Rotate(Vector3.up, 45f, Space.Self);
                }

                if (Input.GetKey(KeyCode.Comma) && Time.time > PlacingAnywhere.nextRotate)
                {
                    PlacingAnywhere.nextRotate = Time.time + PlacingAnywhere.rotateRate;
                    gameObject.transform.Rotate(Vector3.right, 45f, Space.Self);
                }

                //Conform to surface
                if (Input.GetKey(KeyCode.Z))
                {
                    vp_FPSCamera cam = GameManager.GetVpFPSPlayer().FPSCamera;

                    RaycastHit raycastHit = PlacingAnywhere.DoRayCast(cam.transform.position, cam.transform.forward, true);

                    gameObject.transform.position = raycastHit.point;

                    Mesh mesh = PlacingAnywhere.GetMesh(gameObject);
                    if (mesh == null)
                    {
                        Debug.LogError("Failed to get mesh for " + gameObject.name);
                        return;
                    }

                    Vector3[] vertices = new Vector3[0];

                    if (mesh.isReadable)
                    {
                        vertices = mesh.vertices;
                    }                    
                    
                    float closestPoint = float.PositiveInfinity;

                    foreach (Vector3 position in vertices)
                    {
                        Vector3 point = gameObject.transform.TransformPoint(position);
                        float dist = PlacingAnywhere.SignedDistancePlanePoint(raycastHit.normal, raycastHit.point, point);
                        if (dist < closestPoint)
                        {
                            closestPoint = dist;
                        }
                    }


                    //Only apply if point found
                    if (!float.IsPositiveInfinity(closestPoint))
                    {
                        gameObject.transform.Translate(raycastHit.normal * -closestPoint, Space.World);
                    }
                }

                Vector2 cameraMovementMouse = InputManager.GetCameraMovementMouse(__instance);
                Transform transform = GameManager.GetMainCamera().transform;

                if (Input.GetKey(KeyCode.LeftAlt))
                {
                    //Stop camera movement
                    GameManager.GetVpFPSPlayer().FPSCamera.MouseSensitivity = 0f;

                    float angleForward = cameraMovementMouse.x * 1f;
                    float angleRight = cameraMovementMouse.y * 1f;

                    gameObject.transform.Rotate(transform.forward, -angleForward, Space.World);
                    gameObject.transform.Rotate(transform.right, angleRight, Space.World);

                    //PlacingAnywhere.lastRotation = gameObject.transform.eulerAngles;
                }
                else
                {
                    //Restore camera movement
                    if (PlacingAnywhere.mouseSensivity > 0)
                        GameManager.GetVpFPSPlayer().FPSCamera.MouseSensitivity = PlacingAnywhere.mouseSensivity;
                }

                PlacingAnywhere.lastRotation = gameObject.transform.eulerAngles;

                __result = MeshLocationCategory.Valid;
            }
        }

        [HarmonyPatch(typeof(PlayerManager), "PlaceMeshInWorld")]
        internal class PlayerManager_PlaceMeshInWorld
        {
            private static void Postfix(PlayerManager __instance)
            {
                PlacingAnywhere.isPlacing = false;
                if (PlacingAnywhere.mouseSensivity > 0)
                    GameManager.GetVpFPSPlayer().FPSCamera.MouseSensitivity = PlacingAnywhere.mouseSensivity;
            }
        }

        [HarmonyPatch(typeof(PlayerManager), "CancelPlaceMesh")]
        internal class PlayerManager_CancelPlaceMesh
        {
            private static void Postfix(PlayerManager __instance)
            {
                PlacingAnywhere.isPlacing = false;
                if (PlacingAnywhere.mouseSensivity > 0)
                    GameManager.GetVpFPSPlayer().FPSCamera.MouseSensitivity = PlacingAnywhere.mouseSensivity;
            }
        }

        [HarmonyPatch(typeof(PlayerManager), "StartPlaceMesh")]
        internal class PlayerManager_StartPlaceMesh
        {
            private static bool Prefix(PlayerManager __instance, GameObject objectToPlace, ref bool __result)
            {
                PlacingAnywhere.mouseSensivity = GameManager.GetVpFPSPlayer().FPSCamera.MouseSensitivity;

                return true;
            }
        }

        // = GameManager.GetVpFPSPlayer().FPSCamera.MouseSensitivity
    }
}
