using System.Reflection;
using System.Collections.Generic;
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
        public static float rotateRate = 0.10f;
        public static float nextRotate;
        public static List<Vector3> currentVertices = new List<Vector3>();

        private const float COLLIDER_OFFSET = 0.001f;
        private const float CONTACT_DISTANCE = 0.01f;
        private const float RAYCAST_DISTANCE = 0.1f;

        public static bool conformToggle = true;
        public static bool snapToggle = false;

        public static GameObject paHUD = null;
        public static ButtonPrompt conformButton = null;
        public static ButtonPrompt snapButton = null;

        public static bool isSticking = false;

        public static Dictionary<string, string> buttonConfig = new Dictionary<string, string>();
        public static Dictionary<string, string> buttonLabels = new Dictionary<string, string>();
        public static List<ButtonPrompt> buttonList;

        public static bool GameManagerIsAwake = false;

        public static List<string> fixableObjects = new List<string> { "ammobox", "beefjerky", "book", "bottleantibiotics", "bottlepainkillers", "coffeetin", "condensedmilk", "emergencystim", "energybar", "granolabar", "greenteapackage", "hardcase", "ketchupchips", "mre", "peanutbutter", "sewingkit", "soda", "spraypaintcan", "stumpremover", "water1000", "water500", "waterpurificationtablets" };

        public override void OnApplicationStart()
        {
            Debug.Log("[placing-anywhere] Version " + Assembly.GetExecutingAssembly().GetName().Version);

            Settings.OnLoad();

            conformToggle = Settings.options.conformDefault;
            snapToggle = Settings.options.snapDefault;

            buttonLabels.Add("RXL", "ROTATE X -");
            buttonLabels.Add("RXR", "ROTATE X +");
            buttonLabels.Add("RZL", "ROTATE Z -");
            buttonLabels.Add("RZR", "ROTATE Z +");
            buttonLabels.Add("MD", "MOVE DOWN");
            buttonLabels.Add("MU", "MOVE UP");
            buttonLabels.Add("CONFORM", "CONFORM TO SURFACE");
            buttonLabels.Add("SNAP", "SNAP TO OBJECT");
            buttonLabels.Add("FINE", "FINE ADJUST");
            buttonLabels.Add("FREE", "MOUSE ROTATION");
            buttonLabels.Add("RESET", "RESET ROTATION");

            UpdateButtonConfig();

            buttonList = new List<ButtonPrompt>();
        }

        public override void OnGUI()
        {
            base.OnGUI();

            UpdateHUDState();
        }

        public static void UpdateButtonConfig()
        {
            buttonConfig.Clear();

            if (!Settings.options.simpleHUD)
            {
                buttonConfig.Add("RXL", Settings.options.rotateXLeft.ToString());
                buttonConfig.Add("RXR", Settings.options.rotateXRight.ToString());
                buttonConfig.Add("RZL", Settings.options.rotateZLeft.ToString());
                buttonConfig.Add("RZR", Settings.options.rotateZRight.ToString());
                buttonConfig.Add("MD", Settings.options.moveDown.ToString());
                buttonConfig.Add("MU", Settings.options.moveUp.ToString());
            }
            
            buttonConfig.Add("CONFORM", Settings.options.conformToggleKey.ToString());
            buttonConfig.Add("SNAP", Settings.options.snapKey.ToString());
            buttonConfig.Add("FINE", "SHIFT");
            buttonConfig.Add("FREE", "LEFT ALT");
            buttonConfig.Add("RESET", Settings.options.resetRotation.ToString());
        }

        public static void CreatePlacingHUD()
        {
            if (paHUD != null)
            {
                return;
            }

            paHUD = new GameObject();

            EquipItemPopup placingHUD = InterfaceManager.m_Panel_HUD.m_EquipItemPopup;
            
            GameObject bpButton = placingHUD.m_ButtonPromptRight.gameObject;

            float baseX = Screen.width / 4f;
            float baseY = 160f;

            AddHUDButtons(bpButton, baseX, baseY);
        }

        public static void AddHUDButtons(GameObject buttonBP, float baseX, float baseY)
        {
            int index = 0;

            foreach (KeyValuePair<string, string> bData in buttonConfig)
            {
                GameObject buttonGO = NGUITools.AddChild(paHUD, buttonBP);

                buttonGO.name = "PA_Button_"+bData.Key;
                ButtonPrompt button = buttonGO.transform.GetComponent<ButtonPrompt>();
                string label = buttonLabels[bData.Key];

                button.UpdatePromptLabel(label);
                button.m_KeyboardButtonLabel.text = bData.Value;

                if (bData.Key == "CONFORM")
                {
                    conformButton = button;
                }
                else if (bData.Key == "SNAP")
                {
                    snapButton = button;
                }


                if (bData.Value == "SHIFT")
                {
                    button.m_KeyboardButtonSprite.transform.localScale = new Vector3(2.6f, 1f, 1f);
                }

                if (bData.Value == "LEFT ALT")
                {
                    button.m_KeyboardButtonSprite.transform.localScale = new Vector3(3.0f, 1f, 1f);
                }

                float posX = baseX + ((index % 2) * 110f);
                float posY = baseY - (Mathf.Floor(index / 2) * 80);
                buttonGO.transform.localPosition = new Vector3(posX, posY, 0);

                buttonList.Add(button);

                index++;
            }
        }

        public static void UpdateHUDButtons()
        {
            
            UpdateButtonConfig();

            Object.Destroy(paHUD);
            paHUD = null;
        }

        public static void UpdateHUDState()
        {
            if (conformButton != null)
            {
                if (conformToggle == true)
                {
                    conformButton.m_KeyboardButtonLabel.color = new Color32(255, 140, 0, 255);
                    conformButton.m_KeyboardButtonSprite.color = new Color32(255, 140, 0, 255);
                }
                else
                {
                    conformButton.m_KeyboardButtonLabel.color = Color.white;
                    conformButton.m_KeyboardButtonSprite.color = Color.white;

                }
            }

            if (snapButton != null)
            {
                if (snapToggle == true)
                {
                    snapButton.m_KeyboardButtonLabel.color = new Color32(255, 140, 0, 255);
                    snapButton.m_KeyboardButtonSprite.color = new Color32(255, 140, 0, 255);
                }
                else
                {
                    snapButton.m_KeyboardButtonLabel.color = Color.white;
                    snapButton.m_KeyboardButtonSprite.color = Color.white;

                }
            }
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

        public static List<Mesh> GetAllMeshes(GameObject go)
        {
            List<Mesh> meshes = new List<Mesh>();

            MeshFilter[] meshComponents = go.GetComponentsInChildren<MeshFilter>();

            foreach (MeshFilter meshInChildren in meshComponents)
            {
                if (meshInChildren != null)
                {
                    if (meshInChildren.mesh != null)
                    {
                        meshes.Add(meshInChildren.mesh);
                    }
                    if (meshInChildren.sharedMesh != null)
                    {

                        meshes.Add(meshInChildren.sharedMesh);
                    }
                }
            }
            SkinnedMeshRenderer[] skinnedMeshComponents = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer componentInChildren2 in skinnedMeshComponents)
            {
                if (componentInChildren2 != null)
                {
                    meshes.Add(componentInChildren2.sharedMesh);
                }
            }

            return meshes;
        }

        public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {
            return Vector3.Dot(planeNormal, point - planePoint);
        }

        internal static void SnapToPositionBelow(GameObject gameObject)
        {
            GameObject gearItemBelow = GetGearItemBelow(gameObject, RAYCAST_DISTANCE);
            if (gearItemBelow != null)
            {
                Vector3 relativePosition = gameObject.transform.position - gearItemBelow.transform.position;
                Vector3 projectedRelativePosition = Vector3.Project(relativePosition, gearItemBelow.transform.up);

                gameObject.transform.position = gearItemBelow.transform.position + projectedRelativePosition;
            }
        }

        internal static void SnapToRotationBelow(GameObject gameObject)
        {
            GameObject gearItemBelow = GetGearItemBelow(gameObject, RAYCAST_DISTANCE);
            if (gearItemBelow != null)
            {
                SetRotation(Quaternion.Inverse(GameManager.GetMainCamera().transform.rotation) * gearItemBelow.transform.rotation);
            }
        }

        private static GameObject GetGearItemBelow(GameObject gameObject, float maxDistance)
        {
            RaycastHit[] hits = Physics.RaycastAll(gameObject.transform.position + gameObject.transform.up * CONTACT_DISTANCE, -gameObject.transform.up, maxDistance, 1 << vp_Layer.Gear);
            foreach (RaycastHit eachHit in hits)
            {
                if (eachHit.transform != gameObject.transform)
                {
                    return eachHit.collider.gameObject;
                }
            }

            return null;
        }

        private static void SetRotation(Quaternion rotation)
        {
            GameManager.GetPlayerManagerComponent().m_RotationInCameraSpace = rotation;
        }

        internal static void FixBoxCollider(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            Renderer renderer = Utils.GetLargestBoundsRenderer(gameObject);
            if (renderer == null)
            {
                return;
            }

            BoxCollider[] boxColliders = gameObject.GetComponentsInChildren<BoxCollider>();
            foreach (BoxCollider eachBoxCollider in boxColliders)
            {
                if (eachBoxCollider.isTrigger)
                {
                    Object.Destroy(eachBoxCollider);
                }
            }

            BoxCollider boxCollider = gameObject.GetComponentInChildren<BoxCollider>();
            if (boxCollider == null)
            {
                Object.Destroy(gameObject.GetComponent<MeshCollider>());

                boxCollider = gameObject.gameObject.AddComponent<BoxCollider>();
                boxCollider.size = renderer.bounds.extents * 2;
            }

            float meshHeight = -1;

            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter eachMeshFilter in meshFilters)
            {
                if (eachMeshFilter.transform.parent && "OpenedMesh" == eachMeshFilter.transform.parent.name)
                {
                    continue;
                }

                GameObject transformObject = new GameObject();
                transformObject.transform.localRotation = eachMeshFilter.transform.localRotation;
                transformObject.transform.localScale = eachMeshFilter.transform.localScale;
                meshHeight = Mathf.Max(meshHeight, Mathf.Abs(transformObject.transform.TransformVector(eachMeshFilter.mesh.bounds.size).y));
            }

            if (meshHeight <= 0)
            {
                return;
            }

            boxCollider.center = new Vector3(boxCollider.center.x, meshHeight / 2f + COLLIDER_OFFSET, boxCollider.center.z);
            boxCollider.size = new Vector3(boxCollider.size.x, meshHeight - COLLIDER_OFFSET, boxCollider.size.z);
        }

        internal static void RemovePickupHelper(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            Transform pickupHelper = gameObject.transform.Find("PickupHelper");
            if (pickupHelper == null)
            {
                return;
            }

            pickupHelper.gameObject.SetActive(false);
        }

        internal static void AddItemsToPhysicalCollisionMask()
        {
            Utils.m_PhysicalCollisionLayerMask |= 1 << vp_Layer.Gear;
        }

        internal static void RemoveItemsFromPhysicalCollisionMask()
        {
            Utils.m_PhysicalCollisionLayerMask &= ~(1 << vp_Layer.Gear);
        }
    }
}
