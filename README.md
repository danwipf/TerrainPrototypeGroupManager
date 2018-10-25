# Terrain Prototype Group Manager <i>for Unity3d 2018.1 </i>

_______________________________

<b>What is TPGM?</b>

TPGM is designed to help managing the Trees for Terrain. You can create Groups and load by drag and drop, 
valid Treeprefabs into the Groups, which you've created.

It holds aswell a tool which allows you to Randomize Trees with other Loaded Trees as you're painting. 
<i>Does kind of the same like the function "Mass Place Trees" in Terrain, but just along when you paint 
Trees on to the Terrain.</i>

_______________________________

<b>How do i use TPGM?</b>

First, add TPGM to your GameObject , which holds the Terrain Component.
Now if you have any Trees already on Terrain TPGM asks you if you want to save them into a new Group.
Do as you like, but remeber once you've clicked Discard the Trees won't be Saved to TPGM.<i>(Readd TPGM for saving the Trees)</i>

Now you're good to go to Create a Group. Type a Name into the Empty Field and hit "Create new Group".
Open the foldout Prototype Groups. You See now one or two Groups, depends what you've selected earlier.

You can now drag and drop TreePrefabs on to the List. What valid prefabs are is explained later. 
For multiple prefabs you want to drag and drop, hit the Button "Lock Inspector Off". <i>(Locks the Focus on the Terrain GameObject)</i>

If you're done, choose in the List below the "GROUPMANAGER LOG" which Group/Groups you want to Load and hit Load Groups.
Now you can paint the Trees as you know it from Terrain. The Trees'll be automaticly saved if you press "Load Groups" / "Unload Terrain".

_______________________________

<b>What are valid Prefabs for TPGM</b>

- Speedtree
- TreeCreator
- LOD Prefabs
- Custom Trees with Ambient Occlusion Shader (bend factor will be added in Future release)
- Vegetation
- Stones (not recomended)
- Buildings (not recomended)
_______________________________

<b>Button Explenation from TPGM</b>

"Create new Group"
  - Creates new Group inside TPGM, with Name which user can give.
  
"Select all Groups"
  - Toggles all Groups inside TPGM to Selected. <i>(for quick loading or deleting)</i>
  
"Remove Selected Groups"
  - Removes all Groups which are Selected in the list (Toggle Ticked to true).
  
"Load Groups"
  - Loads all Groups which are Selected in the list.
  
"Unload Terrain"
  - Unloads all Treeprototypes from Terrain. <i>("Terrain Tree Part is Empty")</i>
  
"Lock Inspector Off/On"
  - Locks the Inspector to the Terrain GameObject. Usefull if you Drag and Drop multiple Prefabs to the List.
  
"Tree Randomizer Off/On"
  - Randomizes the Trees in the Terrain, when you're painting.<i>(Like Mass Place Trees)</i>
  
"Tree Instance Count"
  - Debug function, which counts all the Trees on the Terrain.
  
 "Clear Group Prototypes"
 - Clears out all Prefabs from a Group.
 
_______________________________

Thank you and have fun

if any Questions mail me at wipf.daniel(at)gmx.ch
_______________________________

25.October 2018 / dan
