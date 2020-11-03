using Harmony;
using UnityEngine;
using System;

namespace PlacingAnywhere
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

            //X Rotation - G/H
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.H))
            {
                if (InputManager.GetSprintDown(InputManager.m_CurrentContext))
                {
                    gameObject.transform.Rotate(Vector3.right, PlacingAnywhere.rotateAngle / 4f, Space.Self);
                }
                else
                {
                    gameObject.transform.Rotate(Vector3.right, PlacingAnywhere.rotateAngle, Space.Self);
                }
                
            }
            else if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.G))
            {
                if (InputManager.GetSprintDown(InputManager.m_CurrentContext))
                {
                    gameObject.transform.Rotate(Vector3.right, -(PlacingAnywhere.rotateAngle / 4f), Space.Self);
                }
                else
                {
                    gameObject.transform.Rotate(Vector3.right, -PlacingAnywhere.rotateAngle, Space.Self);
                }
                
            }

            //Y Rotation - Q/E - Handled by vanilla 

            //Z Rotation - T/Y
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Y))
            {
                if (InputManager.GetSprintDown(InputManager.m_CurrentContext))
                {
                    gameObject.transform.Rotate(Vector3.forward, PlacingAnywhere.rotateAngle / 4f, Space.Self);
                }
                else
                {
                    gameObject.transform.Rotate(Vector3.forward, PlacingAnywhere.rotateAngle, Space.Self);
                }
                
            }
            else if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.T))
            {
                if (InputManager.GetSprintDown(InputManager.m_CurrentContext))
                {
                    gameObject.transform.Rotate(Vector3.forward, -(PlacingAnywhere.rotateAngle / 4f), Space.Self);
                }
                else
                {
                    gameObject.transform.Rotate(Vector3.forward, -PlacingAnywhere.rotateAngle, Space.Self);
                }
            }

            //Y Position - B/N
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.N))
            {
                if (InputManager.GetSprintDown(InputManager.m_CurrentContext))
                {
                    PlacingAnywhere.positionYOffset += (PlacingAnywhere.positionOffset / 4f);
                }
                else
                {
                    PlacingAnywhere.positionYOffset += PlacingAnywhere.positionOffset;
                }
                
            }
            else if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.B))
            {
                if (InputManager.GetSprintDown(InputManager.m_CurrentContext))
                {
                    PlacingAnywhere.positionYOffset -= (PlacingAnywhere.positionOffset / 4f);
                }
                else
                {
                    PlacingAnywhere.positionYOffset -= PlacingAnywhere.positionOffset;
                }
            }

            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + PlacingAnywhere.positionYOffset, gameObject.transform.position.z);

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
            PlacingAnywhere.positionYOffset = 0;
        }
    }

    [HarmonyPatch(typeof(PlayerManager), "CancelPlaceMesh")]
    internal class PlayerManager_CancelPlaceMesh
    {
        private static void Postfix(PlayerManager __instance)
        {
            PlacingAnywhere.isPlacing = false;
            PlacingAnywhere.positionYOffset = 0;
        }
    }
}
