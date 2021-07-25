using ModSettings;
using UnityEngine;
using System.Reflection;

namespace PlacingAnywhere
{
    class PlacingAnywhereSettings : JsonModSettings
    {
        [Section("General Options")]

        [Name("Conform to surface default")]
        [Description("If enabled, conform to surface will be enabled by default.")]
        public bool conformDefault = true;

        [Name("Snap to objects default")]
        [Description("If enabled, snapping to objects will be enabled by default.")]
        public bool snapDefault = false;

        [Name("Reset modes after placement")]
        [Description("If enabled, state of snap and conform will be reset after object has been placed.")]
        public bool resetModes = false;

        [Name("Reset rotation to surface on pickup")]
        [Description("If enabled, object rotation will be reset to be flat on the current surface when you start placing it.")]
        public bool autoResetRotation = false;

        [Name("Disable Mesh coloring")]
        [Description("If enabled, green/red tint won't be applied to meshes when moving.")]
        public bool disableColor = false;

        [Name("Try to fix colliders (Requires scene reload)")]
        [Description("If enabled, some colliders will be adjusted to actual object so they can be placed better, disable to keep vanilla colliders.")]
        public bool fixColliders = true;

        [Section("HUD Options")]

        [Name("Show HUD")]
        [Description("If disabled, mod's placing HUD will be hidden.")]
        public bool showHUD = true;

        [Name("Simple HUD")]
        [Description("If enabled, rotation and offset keys will be hidden.")]
        public bool simpleHUD = false;

        [Name("HUD offset X")]
        [Description("Allows user to offset HUD horizontally from original position")]
        [Slider(-2000, 2000)] // -5, -4, ..., 4, 5
        public int hudOffsetX = 0;

        [Name("HUD offset Y")]
        [Description("Allows user to offset HUD vertically from original position")]
        [Slider(-600, 600)] // -5, -4, ..., 4, 5
        public int hudOffsetY = 0;

        [Section("Key Bindings")]

        [Name("Rotate X -")]
        [Description("Decrease X rotation.")]
        public KeyCode rotateXLeft = KeyCode.T;

        [Name("Rotate X +")]
        [Description("Increase X rotation.")]
        public KeyCode rotateXRight = KeyCode.Y;

        [Name("Rotate Z -")]
        [Description("Decrease Z rotation.")]
        public KeyCode rotateZLeft = KeyCode.G;

        [Name("Rotate Z +")]
        [Description("Increase Z rotation.")]
        public KeyCode rotateZRight = KeyCode.H;

        [Name("Move Object Down")]
        [Description("Decrease object's Y position.")]
        public KeyCode moveDown = KeyCode.B;

        [Name("Move Object Up")]
        [Description("Increase object's Y position.")]
        public KeyCode moveUp = KeyCode.N;

        [Name("Reset Rotation")]
        [Description("Reset object's rotation to be flat on the surface below the cursor.")]
        public KeyCode resetRotation = KeyCode.K;

        [Name("Snap to object below")]
        [Description("Matches position and rotation of object under cursor")]
        public KeyCode snapKey = KeyCode.X;

        [Name("Conform Toggle")]
        [Description("Toggle if object uses default positioning or if it conforms to the surface under the cursor.")]
        public KeyCode conformToggleKey = KeyCode.Z;

        [Name("Mouse Rotation")]
        [Description("Allows rotating the object with the mouse instead of the keys.")]
        public KeyCode mouseRotationKey = KeyCode.LeftAlt;

        protected override void OnConfirm()
        {
            base.OnConfirm();
            PlacingAnywhere.snapToggle = snapDefault;
            PlacingAnywhere.conformToggle = conformDefault;
            PlacingAnywhere.UpdateHUDButtons();
        }
    }

    internal static class Settings
    {
        public static PlacingAnywhereSettings options;

        public static void OnLoad()
        {
            options = new PlacingAnywhereSettings();
            options.AddToModSettings("Placing Anywhere Settings");
        }
    }
}
