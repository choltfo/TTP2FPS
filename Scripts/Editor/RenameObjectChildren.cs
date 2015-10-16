using UnityEngine;
using UnityEditor;
using System.Collections;

public class RenameObjectChildren : EditorWindow {
	// Recursively searches through all of a GameObject's children and performs a prefix replacement.
	
	// Opens window
	[MenuItem("Choltfo/Edit/Rename Children")]
	public static void ShowWindow() {
		EditorWindow.GetWindow<RenameObjectChildren>();
	}
	
	string prefix = "replace this";
	string newPfx = "with this";
	GameObject target = null;
	
	void OnGUI() {
		prefix = GUILayout.TextField(prefix);
		newPfx = GUILayout.TextField(newPfx);
		target = (GameObject)EditorGUILayout.ObjectField(target,typeof(GameObject),true);
		if (GUILayout.Button("Run!")) FindAndReplace(target);
	}
	
	void FindAndReplace(GameObject current) {
		Debug.Log("Editing " + current.name);
		if (current.name.StartsWith(prefix)) {
			current.name = newPfx + current.name.Substring(prefix.Length);
		}
		foreach (Transform r in current.transform) {
			FindAndReplace(r.gameObject);
		}
		EditorUtility.SetDirty(current);
	}
	
}
