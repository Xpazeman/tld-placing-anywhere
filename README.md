# Placing Anywhere

Mod for The Long Dark that removes restrictions when placing items, and adds controls to rotate and position the object.

## Changelog
### v2.2
* Added placing HUD
* Added conform to surface functionality (ON by default)
* Added snapping to object below functionality (OFF by default)
* Keys will change color in the HUD to reflect current active placement modes
* Added mouse rotation (press LeftAlt)
* Added button to reset object rotation to be falt on current surface
* Added multiple new settings to the mod
* Added an option to fix some object colliders (with simple shapes) so they stack properly without gaps
* Added options for default placement modes (conform & stacking) and if they reset after placing or keep current setting
* Added option to remove green object tint when placing
* Added option to reset object rotation on pickup
* Added option to rebind the different keys
* Switched input to KeyboardUtilities
* Fixed bug with nearby broken down objects making stacked objects fall to the ground
* Fixed issue with some objects behaving weirdly with conform to surface on due to them being made of multiple sub objects

## Installing the mod
* If you haven't done so already, install MelonLoader by downloading and running [MelonLoader.Installer.exe](https://github.com/HerpDerpinstine/MelonLoader/releases/latest/download/MelonLoader.Installer.exe).
* You'll need to download and install [ModSettings](https://github.com/zeobviouslyfakeacc/ModSettings/releases/latest/download/ModSettings.dll) by Zeo and [KeyboardUtilities](https://github.com/ds5678/KeyboardUtilities/releases/latest/download/KeyboardUtilities.dll) by ds5678. **MOD WON'T WORK WITHOUT THEM**
* Download the latest version of PlacingAnywhere.dll from the [releases page](https://github.com/Xpazeman/tld-placing-anywhere/releases/latest and place in your Mods folder.

## Default Controls
* The game by default rotates objects on the Y axis with the mouse wheel or with Q/E.
* With T/Y you can rotate on X axis, and with G/H on the Z axis.
* B/N offset the object up and down.
* Pressing Shift makes the increments smaller (25%)
* Rotation is applied using the object as refrence, so what direction "forward" is will change if you rotate the object on a different axis.
* Pressing LeftAlt enables user to rotate object with the mouse
* K resets rotation of the object to make it flat on the surface under the cursor
* Z toggles conform to surface mode
* X toggles snap to object below mode

## Mod Settings
* **ShowHUD**: Hides/shows the placing HUD
* **Simple HUD**: If enabled, rotation and offset keys will be hidden in the HUD
* **Conform to surface default**: If enabled, conform to surface will be enabled by default
* **Snap to objects default**: If enabled, snapping to objects will be enabled by default
* **Reset modes after placement**: If enabled, state of snap and conform will be reset to settings default after object has been placed, if disabled, they will be kept as in las used
* **Reset rotation to surface on pickup**: If enabled, object rotation will be reset to the surface when you start placing it
* **Disable Mesh coloring**: If enabled, green/red tint won't be applied to meshes when moving
* **Try to fix colliders (Requires scene reload)**: If enabled, some colliders (for objects with simple shapes) will be adjusted to actual object so they can be placed better, disable to keep vanilla colliders. Disable if you would like to use vanilla colliders.
