--------------------
     FairyGUI
Copyright © 2014-2024 FairyGUI.com
Version 5.1.0
support@fairygui.com
--------------------
FairyGUI is a flexible UI framework for Unity, working with the professional FREE Game UI Editor: FairyGUI Editor. 
Download the editor from here: https://www.fairygui.com/

--------------------
Get Started
--------------------
Run demos in Assets/FairyGUI/Examples/Scenes.
The UI project is in Examples-UIProject.zip, unzip it anywhere. Use FairyGUI Editor to open it.

Using FairyGUI in Unity:
* Place a UIPanel in scene by using editor menu GameObject/FairyGUI/UIPanel.
* Or using UIPackage.CreateObject to create UI component dynamically, and then use GRoot.inst.AddChild to show it on screen.

-----------------
 Version History
-----------------
5.1.0
1. Support TextMeshPro fallback font and multi-atlas.
2. Add Stage.mouseWheelScale.
3. Fix IME input issue on new input system.

5.0.0
1. Add support for unity input system. (use macro FAIRYGUI_INPUT_SYSTEM)
2. Support async loading of Spine.
3. Add GLoader.useResize.
4. Fix WebGL multi touch issue & IME text input issue. (use macro FAIRYGUI_WEBGL_TEXT_INPUT)
5. Fix some issues of displaying TextMeshPro text.
6. Fixed incorrect position of input text cursor.
7. Upgrade example project to Unity 2022.

4.3.0
1. Support truncating text with ellipsis.
2. Add UIConfig.defaultScrollSnappingThreshold/defaultScrollPagingThreshold.
3. Add support for changing skeleton animation in controller and transition.
4. Increase compatibility of package path detection.
5. Provide a better way for cloning spine materials.
6. Ensure GList.defaultItem to be in normalized format for performance issue.
7. Fixed a shape hit test bug.
8. Fixed a scrolling bug if custom input is enabled.

4.2.0
1. Improve GoWrapper clipping.
2. Fix a bitmap font bug.
3. Add enter/exit sound for GComponent.
4. Add click sound for GCombobox/GLabel/GProgressbar.
5. Fix emoji align issue.
6. Adapt to new TextMeshPro version.
7. Fix a text shrinking behavior bug.

4.1.0
1. Add Stage.GetTouchTarget.
2. Add CustomEase.
3. Add Atlas reference manage mechanism.
4. Fixed: the line settings of polygon is missing.
5. Fixed: nested transitions may not be played.
6. Fixed: wrong parameter for loading Spine/Dragonbones by bundle.
7. Fixed: exceptions when a UIPanel in prefab is being destroyed in editor, since 2018.3.
8. Upgrade example project to Unity 2018.