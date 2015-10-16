using UnityEngine;
using UnityEditor;
using System.Collections;

// For Future Reference

/*[CustomEditor(typeof(CoverSystem))]
public class CoverEditor : Editor {
	void OnInspectorGUI() {
		
		CoverSystem sys = (CoverSystem)target;
		
		if (GUILayout.Button("Update Nodes")) sys.updateAllNodes();
		GUILayout.Label("");
		
		for (int i = 0; i < sys.nodes.Length; i++) {
			CoverNode node = sys.nodes[i];
			EditorGUI.indentLevel = 0;
			GUILayout.Label("Node "+i);
			EditorGUI.indentLevel = 1;
			node.position = EditorGUILayout.Vector3Field("Position:", node.position);
			for (int o = 0; o < node.endpointIndex.Length; o++) {
				
			}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add")) {
				
			}
			if (GUILayout.Button("Subtract")) {
				
			}
			GUILayout.EndHorizontal();
		}
		
		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}
}
*/