using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading;

using UnityEngine;
using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEditor.IMGUI;
// using UnityEditorInternal;


[CanEditMultipleObjects]
[CustomEditor(typeof(TerrainPrototypesGroupManager))]
public class TerrainPrototypesGroupManagerEDITOR : Editor {

	TerrainPrototypesGroupManager _TPGM;
	SerializedProperty  s_TempGroupName;
    SerializedProperty s_SelectGroup;
    SerializedProperty s_GO_Groups;
	Color c_green,c_orange,c_red, col = Color.black;
	public GUIStyle GuistyleHeader, GuistyleBoxNAME, GuistyleBoxDND;
	public bool locked,f0 = true,f1 = false, randomizer = false;
	string LogString,RandLabel = "Tree Randomzier Off",LockInspector = "Lock Inspector Off";

    void OnEnable () {
        _TPGM = (TerrainPrototypesGroupManager)target;

            s_SelectGroup = serializedObject.FindProperty("_SelectGroup");
            s_TempGroupName = serializedObject.FindProperty("_TempGroupName");
            s_GO_Groups = serializedObject.FindProperty("_GO_Groups");

            ColorUtility.TryParseHtmlString("#e65c00",out c_red);
            ColorUtility.TryParseHtmlString("#3bb300",out c_green);
            ColorUtility.TryParseHtmlString("#ffcc00",out c_orange);
            _TPGM._ter = _TPGM.GetComponent<Terrain>();
            _TPGM._ted = _TPGM._ter.terrainData;
            _TPGM._SelectAllGroups = false;

        }
    
    public void OnSceneGUI(){
        if(!_TPGM.FirstTime && randomizer){
            Randomize();
        }
    }
    public override void OnInspectorGUI(){
        
        GUIHeader();
        if(_TPGM.FirstTime && !EditorApplication.isPlaying){
            SaveOldTrees();
        }
        if(!_TPGM.FirstTime){
            if(!EditorApplication.isPlaying){
                serializedObject.Update();
                
                GroupMananger();
                DrawAllGroupList();
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.Space();

            
        }
        if(EditorApplication.isPlaying){
                OnPlayMode(c_red);
        }
    }
    
    
    void GUIHeader(){

        GuistyleHeader = new GUIStyle(GUI.skin.box);
        GUI.skin.box = GuistyleHeader;
        GuistyleHeader.fontSize = 16;
        GuistyleHeader.fontStyle = FontStyle.Bold;
        GuistyleHeader.normal.background = MakeTex( 2, 2,c_green);

        EditorGUILayout.Space();
        GUI_LineHorizontal(Color.black,1);
        Rect HeaderRect = GUILayoutUtility.GetRect(0,30,GUILayout.MinWidth(310));
        GUI.Box(HeaderRect,"Terrain Prototypes Group Manager",GuistyleHeader);
        GUI_LineHorizontal(Color.black,1);
        EditorGUILayout.Space();
        GuistyleHeader.normal.background = MakeTex( 2, 2,Color.white);
        

    }
    

    void GroupMananger(){

        f0 = EditorGUILayout.Foldout(f0,"Group Manager");
        if(f0){
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(s_TempGroupName,new GUIContent(""),false,GUILayout.MinWidth(150));

            if(GUILayout.Button("Create new Group",GUILayout.MinWidth(150))){
                if(_TPGM._TempGroupName == "")                          {_TPGM._TempGroupName = null;}
                if(_TPGM._GroupNames.Contains(_TPGM._TempGroupName))    {LogString = "Allready choose that Name!";col = c_red;}
                if(_TPGM._TempGroupName == null)                        {LogString = "String can't be null!"; col = c_red;}
                if(_TPGM._TempGroupName != null && !_TPGM._GroupNames.Contains(_TPGM._TempGroupName)){
                    _TPGM.CreateGroup(_TPGM._TempGroupName);
                    LogString = "Group Created!";col = c_green;
                }
            }

            EditorGUILayout.EndHorizontal();
            LogField("GROUPMANAGER LOG: ",LogString,col);


            //Toggle Select Groups List
            for(int i = 0; i< s_SelectGroup.arraySize;i++){
                SerializedProperty s = s_SelectGroup.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_TPGM._GroupNames[i],GUILayout.MinWidth(150));
                EditorGUILayout.PropertyField(s,new GUIContent(""),false);
                EditorGUILayout.EndHorizontal();
            }
            
            //GUI BUTTONS
            GUI_BUTTONS();
        }
        GUILayout.Space(20);
    }
    void GUI_BUTTONS(){
            //Toggle Select All Groups Button
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Select all Groups", GUILayout.MinWidth(150))){
                _TPGM._SelectAllGroups = !_TPGM._SelectAllGroups;
                _TPGM.SelectAllGroups();
            }

            //Remove Group Button
            if(GUILayout.Button("Remove Selected Groups", GUILayout.MinWidth(150))){
                _TPGM.ClearTerrain();
                _TPGM.RemoveGroups();
                if(_TPGM._SelectGroupLoaded.Any(x => x)){
                    _TPGM.SECRETLoadGroup();
                    }
                if(_TPGM._ted.treePrototypes.Length != 0){_TPGM.SECRETLoadGroup();}
                LogString = "Deleted Selected Groups!"; col = c_green;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            // Load Groups Button
            if(GUILayout.Button("Load Groups", GUILayout.MinWidth(150))){
                _TPGM.LoadGroup();
                LogString = "Groups Loaded";col = c_green;
            }

            //Clear Terrain
            if(GUILayout.Button("Clear Terrain", GUILayout.MinWidth(150))){
                
                _TPGM.ClearTerrain();
                LogString = "Terrain Cleared in " + System.Math.Round(_TPGM.TimeLog,2) + " Seconds"; col = c_green;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            //Toggle Lock Inspector Button
            if(GUILayout.Button(LockInspector,GUILayout.MinWidth(150))){
                locked = !locked;
                if(locked){     ActiveEditorTracker.sharedTracker.isLocked = true;  LockInspector = "Lock Inspector On";}
                if(!locked){    ActiveEditorTracker.sharedTracker.isLocked = false; LockInspector = "Lock Inspector Off";}
            }

            //Toggle Randomize Paint Brush Button
            if(GUILayout.Button(RandLabel,GUILayout.MinWidth(150))){
                randomizer = !randomizer;
                if(randomizer){
                    RandLabel = "Tree Randomzier On";
                }
                if(!randomizer){
                    RandLabel = "Tree Randomzier Off";
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            //TreeInstance Counter Button
            if(GUILayout.Button("Tree Instances Count",GUILayout.MinWidth(150))){            
                LogString = "TreeInstances Terrain: " + _TPGM._TreeInstances.Length;col = Color.black;
                }


            EditorGUILayout.EndHorizontal();

    }
    public void DrawAllGroupList(){
        f1 = EditorGUILayout.Foldout(f1,"Prototype Groups");
        GUI_LineHorizontal(Color.black,0.5f);

        if(f1)
        {
            for(int i = 0; i<_TPGM._GO_Groups.Length;i++){
                DrawGroupList(i);
            }
            if(_TPGM._GroupNames.Count == 0) {

                EditorGUILayout.Space();
                GUI_LineHorizontal(Color.black,1);

                EditorGUILayout.LabelField("No Group created yet!");

                EditorGUILayout.Space();
            }
        }
    }
    public void DrawGroupList(int master){

        GuistyleBoxNAME = new GUIStyle(GUI.skin.box);
        GuistyleBoxNAME.alignment = TextAnchor.MiddleLeft;
        GuistyleBoxNAME.fontStyle = FontStyle.Bold;
        GuistyleBoxNAME.fontSize = 12;
        GUI.skin.box = GuistyleBoxNAME;
        GuistyleBoxNAME.normal.background = MakeTex( 2, 2,c_orange);

        Rect GroupRect = GUILayoutUtility.GetRect(0,20,GUILayout.ExpandWidth(true));
        GUI.Box(GroupRect,"Group Nr. "+ master + " ||| " + _TPGM._GroupNames[master],GuistyleBoxNAME);

        DrawDragAndDrop(master);


        if(_TPGM._GO_Groups[master].m_GameObjects.Count != 0){

            SerializedProperty s0 = s_GO_Groups.GetArrayElementAtIndex(master).FindPropertyRelative("m_GameObjects");
            for(int i = 0; i<s0.arraySize;i++){
                SerializedProperty s1 = s0.GetArrayElementAtIndex(i);
                    if(s1.objectReferenceValue != null){
                        EditorGUILayout.PropertyField(s1,new GUIContent(_TPGM._GO_Groups[master].m_GameObjects[i].name));
                        }
                    
                    


    //Remove single GO out of the Array and Terrain
                if(s1.objectReferenceValue == null){
                    
                    if(_TPGM._SelectGroupLoaded[master]){
                    _TPGM.ClearTerrain();}
                    _TPGM.RemoveSingle(master,i);
                    s0.DeleteArrayElementAtIndex(i);
                    if(_TPGM._SelectGroupLoaded[master]){
                        _TPGM.SECRETLoadGroup();
                    }
                }
            }
        }

        if(GUILayout.Button("Clear Group Prototypes",GUILayout.MinWidth(150))){

                if(!_TPGM._SelectGroupLoaded[master]){
                        _TPGM._GO_Groups[master].m_GameObjects.Clear();
                        _TPGM._TI_Groups[master].m_TreeInstances.Clear();
                    }
                if(_TPGM._SelectGroupLoaded[master]){
                    _TPGM.ClearTerrain();}
                    _TPGM._GO_Groups[master].m_GameObjects.Clear();
                    _TPGM._TI_Groups[master].m_TreeInstances.Clear();
                    _TPGM._SelectGroupLoaded[master] = false;
                if(_TPGM._SelectGroupLoaded.Any(x => x)){
                        _TPGM.SECRETLoadGroup();
                    }
                for(int i = 0; i<_TPGM._GO_Groups[master].m_GameObjects.Count;i++){
                    s_GO_Groups.GetArrayElementAtIndex(master).FindPropertyRelative("m_GameObjects").DeleteArrayElementAtIndex(i);
                }
                
            }

        EditorGUILayout.Space();
        
    }

    void LogField(string name,string message, Color c){

        GUIStyle g = new GUIStyle();
        g.normal.textColor = c;
        g.fontStyle = FontStyle.Italic;

        GUI_LineHorizontal(Color.black,0.5f);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name,GUILayout.MinWidth(150));
        EditorGUILayout.LabelField(message,g,GUILayout.MinWidth(150));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUI_LineHorizontal(Color.black,0.5f);
    }
    void DrawDragAndDrop(int m){

        GuistyleBoxDND = new GUIStyle(GUI.skin.box);
        GuistyleBoxDND.alignment = TextAnchor.MiddleCenter;
        GuistyleBoxDND.fontStyle = FontStyle.Italic; 
        GuistyleBoxDND.fontSize = 12;
        GUI.skin.box = GuistyleBoxDND;
        GuistyleBoxDND.normal.background = MakeTex( 2, 2, Color.white);

        Event evt = Event.current;
        Rect r = new Rect();

        r = GUILayoutUtility.GetRect(0,20,GUILayout.ExpandWidth(true));
        GUI.Box(r,"Drag and Drop Prefabs to this Box!",GuistyleBoxDND);

        if (r.Contains(evt.mousePosition)){
            if (evt.type == EventType.DragUpdated){
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use ();
                }
            else if (evt.type == EventType.DragPerform){
                for(int i = 0; i<DragAndDrop.objectReferences.Length;i++){
                   if((DragAndDrop.objectReferences[i] as GameObject).GetComponentsInChildren<MeshCollider>().Any(x => !x) || !(DragAndDrop.objectReferences[i] as GameObject).GetComponent<MeshCollider>()){
                    _TPGM._GO_Groups[m].m_GameObjects.Add(DragAndDrop.objectReferences[i] as GameObject);}
                    else{
                        Debug.Log("Mesh Collider is unsupported by Terrain: "+DragAndDrop.objectReferences[i].name);
                    }
                }
                Event.current.Use ();
            }
        }
    }
    void SaveOldTrees()
        {
            
            GUIStyle g = new GUIStyle();
            g.fontStyle = FontStyle.Italic;
            g.normal.textColor = Color.black;
            EditorGUILayout.LabelField("Do you want to Save the allready Created trees in Terrain?",g);

            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Save")){
                _TPGM.FirstStart();
                _TPGM.FirstTime = false;
            }
            if(GUILayout.Button("Discard")){
                _TPGM.FirstTime = false;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        public static Texture2D MakeTex(int width, int height, Color col) {
        Color[] pix = new Color[width * height]; 
        for (int i = 0; i < pix.Length; ++i) {
            pix[i] = col; 
        }
        Texture2D result = new Texture2D(width, height); 
        result.SetPixels(pix); 
        result.Apply(); 
        return result; 
    }
    void Randomize(){
        Event evt = Event.current;
        if(evt.type == EventType.MouseDown && evt.button == 0){
            
            _TPGM.RANDOMIZER_TI = _TPGM._TreeInstances;
        }
        if(evt.type == EventType.MouseUp && evt.button == 0 || evt.type == EventType.MouseDrag && evt.button == 0 ){
            _TPGM.RandomizeTrees();
            
        }
    }


    //GUI Helpers

    void GUI_LineVertical(Color color, float i_height = 1, float i_width = 50) {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height); 
        rect.width = i_width; 
        rect.height = i_height; 
        EditorGUI.DrawRect(rect, color); 
	}
	void GUI_LineHorizontal(Color color, float i_height = 1) {
		EditorGUILayout.Space(); 
		Rect rect = EditorGUILayout.GetControlRect(false, i_height); 
		rect.height = i_height; 
		EditorGUI.DrawRect(rect, color); 
		EditorGUILayout.Space(); 
	}
    void ClearConsole () {
        var assembly = Assembly.GetAssembly(typeof(SceneView)); 
        var type = assembly.GetType("UnityEditor.LogEntries"); 
        var method = type.GetMethod("Clear"); 
        method.Invoke(new object(), null); 
    }
    void OnPlayMode(Color c){
            GUIStyle g = new GUIStyle();
            g.normal.textColor = c;
            g.alignment = TextAnchor.MiddleCenter;
            g.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField("Terrain Prototype Group Manager works only in Editor Mode",g);
            EditorGUILayout.Space();
            GUI_LineHorizontal(Color.black,0.5f);
    }


}






