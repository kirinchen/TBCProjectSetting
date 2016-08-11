using UnityEngine;
using UnityEditor;

namespace I2.Loc
{
	[CustomEditor(typeof(ResourceManager))]
	public class ResourceManagerInspector : Editor 
	{
		SerializedObject mSerializedObj;

		void OnEnable()
		{
			UpgradeManager.EnablePlugins();
			mSerializedObj = new SerializedObject( target as ResourceManager );
		}

		public override void OnInspectorGUI()
		{
			mSerializedObj.Update();

			GUILayout.Space(5);
			GUITools.DrawHeader("Assets:", true);
			GUITools.BeginContents();
				///GUILayout.Label ("Assets:");
				GUITools.DrawObjectsArray( mSerializedObj.FindProperty("Assets") );
			GUITools.EndContents();

			mSerializedObj.ApplyModifiedProperties();
		}
	}
}