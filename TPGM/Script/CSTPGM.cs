using System;
using System.Linq; 
using System.Reflection; 
using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEditor; 


public static class CSTPGM {

    [System.Serializable]public struct list_GameObject {[SerializeField]public List<GameObject> m_GameObjects; }
	[System.Serializable]public struct list_TreeInstances {[SerializeField]public List < FIX_TreeInstance > m_TreeInstances;}
    [System.Serializable] public struct FIX_TreeInstance
    {
        public Vector3 position;
        public float widthScale;
        public float heightScale;
        public float rotation;
        public Color32 color;
        public Color32 lightmapColor;
        public int prototypeIndex;
        public FIX_TreeInstance(TreeInstance aTree)
        {
            position = aTree.position;
            widthScale = aTree.widthScale;
            heightScale = aTree.heightScale;
            rotation = aTree.rotation;
            color = aTree.color;
            lightmapColor = aTree.lightmapColor;
            prototypeIndex = aTree.prototypeIndex;
        }
        public static implicit operator TreeInstance (FIX_TreeInstance aTree)
        {
            TreeInstance inst = new TreeInstance();
            inst.position = aTree.position;
            inst.widthScale = aTree.widthScale;
            inst.heightScale = aTree.heightScale;
            inst.rotation = aTree.rotation;
            inst.color = aTree.color;
            inst.lightmapColor = aTree.lightmapColor;
            inst.prototypeIndex = aTree.prototypeIndex;
            return inst;
        }
    }

    


    //GUI Helpers



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
