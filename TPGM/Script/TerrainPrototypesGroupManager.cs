using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Linq;

// [ExecuteInEditMode]
public class TerrainPrototypesGroupManager : MonoBehaviour {

	public Terrain _ter;
	public TerrainData _ted;
	public string _TempGroupName = "";
	public List<string> _GroupNames = new List<string>();
	public bool _SelectAllGroups = new bool(),FirstTime;
	public List<bool> _SelectGroup = new List<bool>(),_SelectGroupLoaded = new List<bool>();
	public CSTPGM.list_GameObject[] _GO_Groups = new CSTPGM.list_GameObject[0];
	public CSTPGM.list_TreeInstances[] _TI_Groups = new CSTPGM.list_TreeInstances[0];
	public List<CSTPGM.FIX_TreeInstance> _TI_ToLoad = new List<CSTPGM.FIX_TreeInstance>();
	public List<TreePrototype> _TP_ToLoad = new List<TreePrototype>();
	
	public CSTPGM.FIX_TreeInstance[] _TreeInstances = new CSTPGM.FIX_TreeInstance[0];
	List<int> GroupSave_LoadedList = new List<int>();


	//Threading
	public Thread SavingTPGM;
	CSTPGM.FIX_TreeInstance[] THREAD_TI = new CSTPGM.FIX_TreeInstance[0];
	public float TimeLog;

	// Painting Randomizer
	public CSTPGM.FIX_TreeInstance[] RANDOMIZER_TI = new CSTPGM.FIX_TreeInstance[0];

	void Reset(){
		_ter = GetComponent<Terrain>();
		_ted = _ter.terrainData;
		if(Terrain.activeTerrain.terrainData.treePrototypes.Length != 0){
        	FirstTime = true;}
		else{
			FirstTime = false;}
    }
	void Start(){
		_ter = GetComponent<Terrain>();
		_ted = _ter.terrainData;
	}
	public void GetTI()
    {
        _TreeInstances = new CSTPGM.FIX_TreeInstance[_ted.treeInstances.Length];
        for(int i = 0; i < _ted.treeInstances.Length; i++)
        {
            _TreeInstances[i] = new CSTPGM.FIX_TreeInstance(_ted.treeInstances[i]);
        }
    }
	public void SetTI()
    {
        TreeInstance[] trees = new TreeInstance[_TreeInstances.Length];
        for(int i = 0; i < _TreeInstances.Length; i++)
        {
            // implicit conversion
            trees[i] = _TreeInstances[i];
        }
        _ted.treeInstances = trees;
		_ted.RefreshPrototypes();
		_ter.Flush();
    }
	public void RandomizeTrees(){
		
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition );
		RaycastHit hit;
		if( Physics.Raycast( ray, out hit ) )
		{
			GetTI();
			if(RANDOMIZER_TI.Length != _TreeInstances.Length){
				for(int i = RANDOMIZER_TI.Length; i<_TreeInstances.Length; i++){
					_TreeInstances[i].prototypeIndex = UnityEngine.Random.Range(0,_ted.treePrototypes.Length);
				}
			}
			SetTI();
			
		}
		
		
	}
	public void CreateGroup(string Name){
        if(!_GroupNames.Contains(Name)){
            _GroupNames.Add(Name);
            bool b = false;
            _SelectGroup.Add(b);
			_SelectGroupLoaded.Add(b);
			_GO_Groups = _GO_Groups.AddToArray(new CSTPGM.list_GameObject());
			_TI_Groups = _TI_Groups.AddToArray(new CSTPGM.list_TreeInstances());
			_GO_Groups[_GO_Groups.Length-1].m_GameObjects = new List<GameObject>();
			_TI_Groups[_TI_Groups.Length-1].m_TreeInstances = new List<CSTPGM.FIX_TreeInstance>();
        }
    }
	
	public void RemoveGroups(){
		for(int i = _SelectGroup.Count-1; i>=0;i--){
			if(_SelectGroup[i]){
				CSTPGM.RemoveAt(ref _GO_Groups,i);
				CSTPGM.RemoveAt(ref _TI_Groups,i);
				_GroupNames.RemoveAt(i);
				_SelectGroupLoaded.RemoveAt(i);
				_SelectGroup.RemoveAt(i);
			}
		}
	}
	public void RemoveSingle(int master, int GOind){
		List<CSTPGM.FIX_TreeInstance> TIm = _TI_Groups[master].m_TreeInstances;
		for(int i = TIm.Count -1;i >= 0; i--){
			if(TIm[i].prototypeIndex == GOind){
				TIm.RemoveAt(i);
			}
			
		}
		for(int i = 0; i< TIm.Count; i++){
			if(TIm[i].prototypeIndex > GOind){
				CSTPGM.FIX_TreeInstance t = TIm[i];
				t.prototypeIndex -= 1;
				TIm[i] = t;
			}
		}
		_GO_Groups[master].m_GameObjects.RemoveAt(GOind);
	}
    public void SelectAllGroups(){
        
		for(int i = 0; i<_SelectGroup.Count; i++){
			if(_SelectAllGroups){
				_SelectGroup[i] = true;
			}
			if(!_SelectAllGroups){
				_SelectGroup[i] = false;
			}
        }
    }
	public void FirstStart(){
		
		CreateGroup("Terrain Start Up Trees");
		for(int i = 0; i<_GroupNames.Count;i++){
			if(_GroupNames[i] == "Terrain Start Up Trees"){
				for(int ii = 0; ii<_ted.treePrototypes.Length;ii++){
					_GO_Groups[i].m_GameObjects.Add(_ted.treePrototypes[ii].prefab);
				}
				GetTI();
				_TreeInstances = _TreeInstances.OrderBy(x => x.prototypeIndex).ToArray();
				_TI_Groups[i].m_TreeInstances.AddRange(_TreeInstances);
				_SelectGroupLoaded[i] = true;
			}
		}
		

	}
	public void LoadGroup(){
		
        ClearTerrain();
		
		_TP_ToLoad.Clear();
		_TI_ToLoad.Clear();
		_SelectGroupLoaded.Clear();
		_SelectGroupLoaded.AddRange(_SelectGroup);

		for(int i = 0; i<_GO_Groups.Length;i++){
            if(_SelectGroup[i]){
				
                foreach(GameObject obj in _GO_Groups[i].m_GameObjects){
					if(!obj.GetComponent<MeshCollider>() || obj.GetComponentsInChildren<MeshCollider>().Any(x=>!x)){
						TreePrototype TP = new TreePrototype();
						TP.prefab = obj;
						_TP_ToLoad.Add(TP);
					} else {Debug.Log("Mesh Collider is unsupported by Terrain: "+obj.name);}

                }
			}
        }
        int c = 0;
		for(int i = 0; i<_TI_Groups.Length;i++){
			
            if(_SelectGroup[i]){
                List<CSTPGM.FIX_TreeInstance> t = new List<CSTPGM.FIX_TreeInstance>();
                t.AddRange(_TI_Groups[i].m_TreeInstances);
                LoadGroup_INSTANCES(t,c);
                c += _GO_Groups[i].m_GameObjects.Count;
            }
		}
		GetTI();
		_ted.treePrototypes = _TP_ToLoad.ToArray();
		_TreeInstances = _TI_ToLoad.ToArray();
		SetTI();
	
    }
	
    public void SECRETLoadGroup(){
		
		
        ClearTerrain();
		
		_TP_ToLoad.Clear();
		_TI_ToLoad.Clear();

		for(int i = 0; i<_GO_Groups.Length;i++){
            if(_SelectGroupLoaded[i]){
                foreach(GameObject obj in _GO_Groups[i].m_GameObjects){
                    TreePrototype TP = new TreePrototype();
                    TP.prefab = obj;
                    _TP_ToLoad.Add(TP);
                }
			}
        }
        int c = 0;
		for(int i = 0; i<_TI_Groups.Length;i++){	
            if(_SelectGroupLoaded[i]){
                List<CSTPGM.FIX_TreeInstance> t = new List<CSTPGM.FIX_TreeInstance>();
                t.AddRange(_TI_Groups[i].m_TreeInstances);
                LoadGroup_INSTANCES(t,c);
                c += _GO_Groups[i].m_GameObjects.Count;
            }
		}
		GetTI();
		_ted.treePrototypes = _TP_ToLoad.ToArray();
		_TreeInstances = _TI_ToLoad.ToArray();
		SetTI();
		
		
    }


    void LoadGroup_INSTANCES(List<CSTPGM.FIX_TreeInstance> t,int c){
		for(int i = 0; i<t.Count;i++){
			CSTPGM.FIX_TreeInstance TI = t[i];
			TI.prototypeIndex += c;
			_TI_ToLoad.Add(TI);
			
		}
	}
    
    public void SaveGroup(){
		
		TimeLog = Time.realtimeSinceStartup;				//Timecounter between Initialize and Loaded

		GroupSave_LoadedList.Clear();
		for(int i = 0;i<_SelectGroupLoaded.Count;i++){
			if(_SelectGroupLoaded[i]){
				_TI_Groups[i].m_TreeInstances.Clear();
			}
		}
				//Check which Groups are Loaded and add the int to the List
		GetTI();
		for(int i = 0; i < _SelectGroupLoaded.Count; i++)
		{
			if(_SelectGroupLoaded[i])
			{
				GroupSave_LoadedList.Add(i);
				_TI_Groups[ i ].m_TreeInstances.Capacity = _TreeInstances.Length;
			}
		}
		
		_TreeInstances = _TreeInstances.OrderBy(i => i.prototypeIndex).ToArray();
		THREAD_TI = _TreeInstances;
		SavingTPGM = new Thread(SaveGroupMain_OPTI);
		SavingTPGM.Start();
		TimeLog = Time.realtimeSinceStartup -TimeLog;
		SetTI();
    }

	void SaveGroupMain_OPTI()
		
            {
			int TreePrototype_IndexCount = 0; 					//Counter for all TreePrototypes in Terrain
			int GroupSave_LoadedList_Counter = 0;				//Counter for LoadedGroups in Terrain
			int CurrGroupSave_PrefabIndex = 0;					//Counter for the current Group which is saving.
			int PrefabCountInt = 0;								//Additive counter of the Prefab Group length
			int GSLLi = 0; // < 1 >
			int TIPi = 0;  // < 2 >
	
	
			//define start size of the first valid Group to Save
			GSLLi = GroupSave_LoadedList[GroupSave_LoadedList_Counter]; // < 1 >

			
			PrefabCountInt = _GO_Groups[ GSLLi ].m_GameObjects.Count; // < 1 >
      
              //Check which Groups are Loaded and add the int to the List
              for(int i = 0; i < _SelectGroupLoaded.Count; i++)
              {
                  if(_SelectGroupLoaded[i])
                  {
                      GroupSave_LoadedList.Add(i);
                  }
              }
      
              //define start size of the first valid Group to Save
 
              GSLLi = GroupSave_LoadedList[GroupSave_LoadedList_Counter]; // < 1 >
              PrefabCountInt = _GO_Groups[ GSLLi ].m_GameObjects.Count; // < 1 >
      
              // for loop for each TreeInstances in the Terrain
              for(int i = 0; i<THREAD_TI.Length;i++)
              {
                  //check if prototype index matches first TreePrototype if not +1 to the TreePrototype_IndexCount & +1 to the Current Group TreePrototype Count
 
                  TIPi = THREAD_TI[i].prototypeIndex; // < 2 >
 
                  if( TIPi != TreePrototype_IndexCount) // < 2 >
                  {
                      TreePrototype_IndexCount++;
                      CurrGroupSave_PrefabIndex++;
                  }
 
                  //check if the actual TreePrototype_IndexCount matches the lenght of the first Group. if yes jump to the second Group in GroupSave_LoadedList // Resets the counter of the current Group
                  if(TreePrototype_IndexCount == PrefabCountInt)
                  {
                      CurrGroupSave_PrefabIndex = 0;
                      GroupSave_LoadedList_Counter++;
 
                      GSLLi = GroupSave_LoadedList[GroupSave_LoadedList_Counter]; // < 1 > 
         
                      PrefabCountInt += _GO_Groups[ GSLLi ].m_GameObjects.Count; // < 1 >
                  }
 
                  //check if the prototypeIndex is valid to the current TreePrototype_IndexCount if yes, save it to the given Group and change the prototypeIndex to the matching Prefab.
                  if( TIPi == TreePrototype_IndexCount ) // < 2 >
 
                  {
                      CSTPGM.FIX_TreeInstance TI = THREAD_TI[i];
                      TI.prototypeIndex = CurrGroupSave_PrefabIndex;
                      _TI_Groups[ GSLLi ].m_TreeInstances.Add(TI); // < 1 > < 3 >
                  }
              }
          }
	public void ClearTerrain(){

		if(_ted.treePrototypes.Length != 0 && _GO_Groups.Length != 0){
        SaveGroup();
		SavingTPGM = null;
        }

		GetTI();
        _TreeInstances = new CSTPGM.FIX_TreeInstance[0];
		SetTI();
        _ted.treePrototypes = new TreePrototype[0];
		
   }

}





