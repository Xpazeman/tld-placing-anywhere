using HarmonyLib;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PlacingAnywhere
{
    [HarmonyPatch(typeof(PlayerManager), "DoPositionCheck")]
    internal class PlayerManager_DoPositionCheck
    {
        private static void Postfix(PlayerManager __instance, ref MeshLocationCategory __result)
        {
            if (__instance == null)
            {
                return;
            }

            GameObject gameObject = __instance.GetObjectToPlace();

            if (!PlacingAnywhere.isPlacing)
            {
                PlacingAnywhere.isPlacing = true;
                PlacingAnywhere.lastRotation = gameObject.transform.eulerAngles;
            }

            if (gameObject == null)
            {
                return;
            }

            //vp_FPSCamera cam = GameManager.GetVpFPSPlayer().FPSCamera;
            vp_FPSCamera cam = GameManager.GetVpFPSCamera();
            RaycastHit raycastHit = PlacingAnywhere.DoRayCast(cam.transform.position, cam.transform.forward, true);

            GearPlacePoint gearPlacePoint = null;

            if (raycastHit.collider != null)
            {
                gearPlacePoint = __instance.GetGearPlacePoint(raycastHit.collider.gameObject, raycastHit.point);
            }

            if (gearPlacePoint == null)
            {
                //gameObject.transform.eulerAngles = PlacingAnywhere.lastRotation;
                gameObject.transform.eulerAngles = new Vector3(PlacingAnywhere.lastRotation.x, gameObject.transform.eulerAngles.y, PlacingAnywhere.lastRotation.z);
            }

            //Z Rotation - G/H
            if (KeyboardUtilities.InputManager.GetKey(Settings.options.rotateZRight) && Time.time > PlacingAnywhere.nextRotate)
            {
                PlacingAnywhere.nextRotate = Time.time + PlacingAnywhere.rotateRate;

                if (KeyboardUtilities.InputManager.GetKey(KeyCode.LeftShift))
                {
                    gameObject.transform.Rotate(Vector3.right, PlacingAnywhere.rotateAngle / 4f, Space.Self);
                }
                else
                {
                    gameObject.transform.Rotate(Vector3.right, PlacingAnywhere.rotateAngle, Space.Self);
                }

            }
            else if (KeyboardUtilities.InputManager.GetKey(Settings.options.rotateZLeft) && Time.time > PlacingAnywhere.nextRotate)
            {
                PlacingAnywhere.nextRotate = Time.time + PlacingAnywhere.rotateRate;

                if (KeyboardUtilities.InputManager.GetKey(KeyCode.LeftShift))
                {
                    gameObject.transform.Rotate(Vector3.right, -(PlacingAnywhere.rotateAngle / 4f), Space.Self);
                }
                else
                {
                    gameObject.transform.Rotate(Vector3.right, -PlacingAnywhere.rotateAngle, Space.Self);
                }

            }

            //Y Rotation - Q/E - Handled by vanilla 

            //X Rotation - T/Y
            if (KeyboardUtilities.InputManager.GetKey(Settings.options.rotateXRight) && Time.time > PlacingAnywhere.nextRotate)
            {
                PlacingAnywhere.nextRotate = Time.time + PlacingAnywhere.rotateRate;

                if (KeyboardUtilities.InputManager.GetKey(KeyCode.LeftShift))
                {
                    gameObject.transform.Rotate(Vector3.forward, PlacingAnywhere.rotateAngle / 4f, Space.Self);
                }
                else
                {
                    gameObject.transform.Rotate(Vector3.forward, PlacingAnywhere.rotateAngle, Space.Self);
                }

            }
            else if (KeyboardUtilities.InputManager.GetKey(Settings.options.rotateXLeft) && Time.time > PlacingAnywhere.nextRotate)
            {
                PlacingAnywhere.nextRotate = Time.time + PlacingAnywhere.rotateRate;

                if (KeyboardUtilities.InputManager.GetKey(KeyCode.LeftShift))
                {
                    gameObject.transform.Rotate(Vector3.forward, -(PlacingAnywhere.rotateAngle / 4f), Space.Self);
                }
                else
                {
                    gameObject.transform.Rotate(Vector3.forward, -PlacingAnywhere.rotateAngle, Space.Self);
                }
            }

            //Y Position - B/N
            if (KeyboardUtilities.InputManager.GetKey(Settings.options.moveUp) && Time.time > PlacingAnywhere.nextRotate)
            {
                PlacingAnywhere.nextRotate = Time.time + PlacingAnywhere.rotateRate;

                if (KeyboardUtilities.InputManager.GetKey(KeyCode.LeftShift))
                {
                    PlacingAnywhere.positionYOffset += (PlacingAnywhere.positionOffset / 4f);
                }
                else
                {
                    PlacingAnywhere.positionYOffset += PlacingAnywhere.positionOffset;
                }

            }
            else if (KeyboardUtilities.InputManager.GetKey(Settings.options.moveDown) && Time.time > PlacingAnywhere.nextRotate)
            {
                PlacingAnywhere.nextRotate = Time.time + PlacingAnywhere.rotateRate;

                if (KeyboardUtilities.InputManager.GetKey(KeyCode.LeftShift))
                {
                    PlacingAnywhere.positionYOffset -= (PlacingAnywhere.positionOffset / 4f);
                }
                else
                {

                    PlacingAnywhere.positionYOffset -= PlacingAnywhere.positionOffset;
                }
            }

            if (KeyboardUtilities.InputManager.GetKeyDown(Settings.options.resetRotation))
            {
                float diff = Vector3.Dot(Vector3.up, raycastHit.normal);
                if (diff > 0.05f)
                {
                    Quaternion lhs = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
                    gameObject.transform.rotation = Quaternion.Euler(0f, gameObject.transform.rotation.eulerAngles.y, 0f);
                    gameObject.transform.rotation = lhs * gameObject.transform.rotation;
                    gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x, gameObject.transform.rotation.eulerAngles.y, gameObject.transform.rotation.eulerAngles.z);
                }
            }

            //Conform to surface
            if (KeyboardUtilities.InputManager.GetKeyDown(Settings.options.conformToggleKey))
            {
                if (PlacingAnywhere.conformToggle)
                {
                    PlacingAnywhere.conformToggle = false;
                    HUDMessage.AddMessage("Conform to surface: off", false, false);
                    PlacingAnywhere.RemoveItemsFromPhysicalCollisionMask();
                }
                else
                {
                    PlacingAnywhere.conformToggle = true;
                    PlacingAnywhere.positionYOffset = 0;
                    HUDMessage.AddMessage("Conform to surface: on", false, false);
                    PlacingAnywhere.AddItemsToPhysicalCollisionMask();
                    
                }
            }

            //Snap to object below
            if (KeyboardUtilities.InputManager.GetKeyDown(Settings.options.snapKey))
            {
                if (PlacingAnywhere.snapToggle)
                {
                    PlacingAnywhere.snapToggle = false;
                    HUDMessage.AddMessage("Snap to objects: off", false, false);
                }
                else
                {
                    PlacingAnywhere.snapToggle = true;
                    HUDMessage.AddMessage("Snap to objects: on", false, false);

                }
            }

            Vector2 cameraMovementMouse = InputManager.GetCameraMovementMouse(__instance);
            Transform transform = GameManager.GetMainCamera().transform;

            if (KeyboardUtilities.InputManager.GetKey(Settings.options.mouseRotationKey))
            {
                //Stop camera movement
                if (cam.MouseSensitivity > 0)
                    PlacingAnywhere.mouseSensivity = cam.MouseSensitivity;

                cam.MouseSensitivity = 0f;

                float angleForward = cameraMovementMouse.x * 1f;
                float angleRight = cameraMovementMouse.y * 1f;

                if (KeyboardUtilities.InputManager.GetKey(KeyCode.LeftShift))
                {
                    angleForward *= 0.25f;
                    angleRight *= 0.25f;
                }


                if (Mathf.Abs(angleForward) > Mathf.Abs(angleRight))
                {
                    gameObject.transform.Rotate(Vector3.forward, -angleForward, Space.World);
                }
                else
                {
                    gameObject.transform.Rotate(Vector3.right, -angleRight, Space.World);
                }
            }
            else
            {
                //Restore camera movement
                if (PlacingAnywhere.mouseSensivity > 0)
                    cam.MouseSensitivity = PlacingAnywhere.mouseSensivity;
            }

            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + PlacingAnywhere.positionYOffset, gameObject.transform.position.z);

            if (PlacingAnywhere.snapToggle && gearPlacePoint == null)
            {
                if (raycastHit.collider != null)
                {
                    GameObject goHit = raycastHit.collider.gameObject;

                    if (goHit.GetComponent<GearItem>() != null)
                    {
                        gameObject.transform.rotation = goHit.transform.rotation;
                        gameObject.transform.position = new Vector3(goHit.transform.position.x, gameObject.transform.position.y, goHit.transform.position.z);

                    }
                }
            }

            if (PlacingAnywhere.snapToggle && gearPlacePoint == null)
            {
                PlacingAnywhere.SnapToPositionBelow(gameObject);
                PlacingAnywhere.SnapToRotationBelow(gameObject);
            }

            PlacingAnywhere.lastRotation = gameObject.transform.eulerAngles;

            __result = MeshLocationCategory.Valid;
        }
    }

    [HarmonyPatch(typeof(PlayerManager), "StartPlaceMesh", new Type[] { typeof(GameObject), typeof(float), typeof(PlaceMeshFlags) })]
    internal class PlayerManager_StartPlaceMesh
    {
        private static void Prefix(PlayerManager __instance, GameObject objectToPlace)
        {
            PlacingAnywhere.mouseSensivity = GameManager.GetVpFPSPlayer().FPSCamera.MouseSensitivity;

            if (!Settings.options.autoResetRotation)
            {
                PlacingAnywhere.isPlacing = true;
                PlacingAnywhere.lastRotation = objectToPlace.transform.eulerAngles;
            }

            PlacingAnywhere.currentVertices.Clear();

            if (PlacingAnywhere.conformToggle)
            {
                PlacingAnywhere.AddItemsToPhysicalCollisionMask();
            }

            if (__instance.m_ItemInHands != null && __instance.m_ItemInHands.gameObject == objectToPlace)
            {
                PlacingAnywhere.RemoveItemsFromPhysicalCollisionMask();
            }

            List<Mesh> meshes = PlacingAnywhere.GetAllMeshes(objectToPlace);

            foreach (Mesh mesh in meshes)
            {
                if (mesh == null)
                {
                    continue;
                }

                if (mesh.isReadable)
                {
                    foreach (Vector3 v in mesh.vertices)
                    {
                        PlacingAnywhere.currentVertices.Add(v);
                    }
                }
            }
        }

        private static void Postfix(PlayerManager __instance)
        {
            PlacingAnywhere.CreatePlacingHUD();

            if (PlacingAnywhere.paHUD != null && Settings.options.showHUD)
            {
                if (!Utils.IsGamepadActive())
                {
                    PlacingAnywhere.paHUD.SetActive(true);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerManager), "ExitMeshPlacement")]
    internal class PlayerManager_PlaceMeshInWorld
    {
        private static void Postfix(PlayerManager __instance)
        {
            PlacingAnywhere.isPlacing = false;
            PlacingAnywhere.positionYOffset = 0;

            PlacingAnywhere.currentVertices.Clear();

            if (PlacingAnywhere.mouseSensivity > 0)
                GameManager.GetVpFPSPlayer().FPSCamera.MouseSensitivity = PlacingAnywhere.mouseSensivity;

            PlacingAnywhere.RemoveItemsFromPhysicalCollisionMask();

            if (Settings.options.resetModes)
            {
                PlacingAnywhere.conformToggle = Settings.options.conformDefault;
                PlacingAnywhere.snapToggle = Settings.options.snapDefault;
            }

            if (PlacingAnywhere.paHUD != null)
            {
                PlacingAnywhere.paHUD.SetActive(false);
            }
        }
    }

    /*[HarmonyPatch(typeof(PlayerManager), "CancelPlaceMesh")]
    internal class PlayerManager_CancelPlaceMesh
    {
        private static void Postfix(PlayerManager __instance)
        {
            PlacingAnywhere.isPlacing = false;
            PlacingAnywhere.positionYOffset = 0;

            PlacingAnywhere.currentVertices.Clear();

            if (PlacingAnywhere.mouseSensivity > 0)
                GameManager.GetVpFPSPlayer().FPSCamera.MouseSensitivity = PlacingAnywhere.mouseSensivity;

            PlacingAnywhere.RemoveItemsFromPhysicalCollisionMask();

            if (Settings.options.resetModes)
            {
                PlacingAnywhere.conformToggle = Settings.options.conformDefault;
                PlacingAnywhere.snapToggle = Settings.options.snapDefault;
            }

            if (PlacingAnywhere.paHUD != null)
            {
                PlacingAnywhere.paHUD.SetActive(false);
            }
        }
    }*/

    [HarmonyPatch(typeof(PlayerManager), "TintObject", new Type[] { typeof(GameObject), typeof(MeshLocationCategory) })]
    internal class PlayerManager_TintObject
    {
        private static bool Prefix(PlayerManager __instance)
        {
            if (Settings.options.disableColor)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    //Fix for ModComponent Compatibility
    [HarmonyPatch(typeof(GameManager), "Awake")]
    internal class GameManager_Awake
    {
        public static void Postfix(GameManager __instance)
        {
            PlacingAnywhere.GameManagerIsAwake = true;
        }
    }

    [HarmonyPatch(typeof(GearManager), "Add")]
    internal class GearManager_Add
    {
        public static void Prefix(GearItem gi)
        {
            if (!PlacingAnywhere.GameManagerIsAwake)
            {
                return;
            }

            if (gi == null || gi.gameObject == null)
            {
                return;
            }

            if (Settings.options.fixColliders && PlacingAnywhere.fixableObjects.Any(s => gi.gameObject.name.ToLower().Contains(s)))
            {
                PlacingAnywhere.FixBoxCollider(gi.gameObject);
                PlacingAnywhere.RemovePickupHelper(gi.gameObject);
            }
        }
    }

    [HarmonyPatch(typeof(BreakDown), "StickSurfaceObjectsToGround")]
    internal class BreakDown_StickSurfaceObjectsToGround
    {
        public static void Prefix()
        {
            PlacingAnywhere.AddItemsToPhysicalCollisionMask();
        }

        public static void Postfix()
        {
            PlacingAnywhere.RemoveItemsFromPhysicalCollisionMask();
        }
    }
}
