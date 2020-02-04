// using UnityEditor;

// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// // Copy meshes from children into the parent's Mesh.
// // CombineInstance stores the list of meshes.  These are combined
// // and assigned to the attached Mesh.

// [RequireComponent(typeof(MeshFilter))]
// [RequireComponent(typeof(MeshRenderer))]
// public class MergeMesh : MonoBehaviour
// {
//     void Start()
//     {
//         Debug.Log("mezrging mesh");
//         MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
//         CombineInstance[] combine = new CombineInstance[meshFilters.Length];

//         int i = 0;
//         while (i < meshFilters.Length)
//         {
//             combine[i].mesh = meshFilters[i].sharedMesh;
//             combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
//             meshFilters[i].gameObject.SetActive(false);

//             i++;
//             Debug.Log("mezrging mesh " + i);

//         }
//         Mesh finalMesh = new Mesh();
//         finalMesh.CombineMeshes(combine);
//         transform.GetComponent<MeshFilter>().mesh = meshFilters[0].sharedMesh;
//             // combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
//         transform.gameObject.SetActive(true);
//         // SaveMesh(transform.GetComponent<MeshFilter>().mesh, "generated", false, false);

//         // string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", "generated", "asset");
// 		// if (string.IsNullOrEmpty(path)) return;

//         // AssetDatabase.CreateAsset( transform.GetComponent<MeshFilter>().sharedMesh, FileUtil.GetProjectRelativePath(path));
//         // AssetDatabase.SaveAssets();
//     }

//     [MenuItem("CONTEXT/MeshFilter/Save Mesh...")]
// 	public static void SaveMeshInPlace (MenuCommand menuCommand) {
// 		MeshFilter mf = menuCommand.context as MeshFilter;
// 		Mesh m = mf.sharedMesh;
// 		SaveMesh(m, m.name, false, true);
// 	}

// 	[MenuItem("CONTEXT/MeshFilter/Save Mesh As New Instance...")]
// 	public static void SaveMeshNewInstanceItem (MenuCommand menuCommand) {
// 		MeshFilter mf = menuCommand.context as MeshFilter;
// 		Mesh m = mf.sharedMesh;
// 		SaveMesh(m, m.name, true, true);
// 	}

//     public static void SaveMesh (Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh) {
// 		string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
// 		if (string.IsNullOrEmpty(path)) return;
        
// 		path = FileUtil.GetProjectRelativePath(path);

// 		Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;
		
// 		if (optimizeMesh)
// 		     MeshUtility.Optimize(meshToSave);
        
// 		AssetDatabase.CreateAsset(meshToSave, path);
// 		AssetDatabase.SaveAssets();
// 	}
// }