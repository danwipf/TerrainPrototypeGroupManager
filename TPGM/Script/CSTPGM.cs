using System;
using System.Linq; 
using System.Reflection; 
using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEditor; 


public static class CSTPGM {

    [System.Serializable]public struct list_GameObject {[SerializeField]public List<GameObject> m_GameObjects; }
	[System.Serializable]public struct list_TreeInstances {[SerializeField]public List < TreeInstance > m_TreeInstances;}
    [System.Serializable]public struct list_loaded{
    [SerializeField] public GameObject m_GO;
    [SerializeField] public List<TreeInstance> m_TI;
    }


    //GUI Helpers
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
    public static void GUI_LineVertical(Color color, float i_height = 1, float i_width = 50) {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height); 
        rect.width = i_width; 
        rect.height = i_height; 
        EditorGUI.DrawRect(rect, color); 
	}
	public static void GUI_LineHorizontal(Color color, float i_height = 1) {
		EditorGUILayout.Space(); 
		Rect rect = EditorGUILayout.GetControlRect(false, i_height); 
		rect.height = i_height; 
		EditorGUI.DrawRect(rect, color); 
		EditorGUILayout.Space(); 
	}

    // Functions
    public static void ClearConsole () {
        var assembly = Assembly.GetAssembly(typeof(SceneView)); 
        var type = assembly.GetType("UnityEditor.LogEntries"); 
        var method = type.GetMethod("Clear"); 
        method.Invoke(new object(), null); 
    }
    public static void OnPlayMode(Color c){
            GUIStyle g = new GUIStyle();
            g.normal.textColor = c;
            g.alignment = TextAnchor.MiddleCenter;
            g.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField("Terrain Prototype Group Manager works only in Editor Mode",g);
            EditorGUILayout.Space();
            CSTPGM.GUI_LineHorizontal(Color.black,0.5f);
    }



    //Array Helpers
    public static IEnumerable < T > Add < T > (this IEnumerable < T > sequence, T item) {
        return (sequence??Enumerable.Empty < T > ()).Concat(new[] {item }); 
    }

    public static T[] AddRangeToArray < T > (this T[] sequence, T[] items) {
        return (sequence??Enumerable.Empty < T > ()).Concat(items).ToArray(); 
    }

    public static T[] AddToArray < T > (this T[] sequence, T item) {
        return Add(sequence, item).ToArray(); 
    }
    
    public static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                arr[a] = arr[a + 1];
            }
            Array.Resize(ref arr, arr.Length - 1);
    }
   


    



}
