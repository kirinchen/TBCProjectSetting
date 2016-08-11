//#define NGUI
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace I2.Loc
{
	public partial class LocalizationEditor
	{
		#region Variables
		
		#endregion
		
		#region GUI Find NoLocalized Terms
		
		void OnGUI_Tools_NoLocalized()
		{
			//OnGUI_ScenesList();
			
			GUILayout.Space (5);
			
			GUI.backgroundColor = Color.Lerp (Color.gray, Color.white, 0.2f);
			GUILayout.BeginVertical("AS TextArea", GUILayout.Height(1));
			GUI.backgroundColor = Color.white;
			
			EditorGUILayout.HelpBox("This selects all labels in the current scene that don't have a Localized component", UnityEditor.MessageType.Info);

			GUILayout.Space (5);
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Select No Localized Labels"))
				EditorApplication.update += SelectNoLocalizedLabels;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.EndVertical();
		}
		
		#endregion
		
		
		#region Find No Localized

		void SelectNoLocalizedLabels()
		{
			EditorApplication.update -= SelectNoLocalizedLabels;

#if NGUI
			UILabel[] Labels = (UILabel[])Resources.FindObjectsOfTypeAll(typeof(UILabel));
			
			if (Labels==null)
				return;
			
			List<GameObject> Objs = new List<GameObject>();
			
			for (int i=0, imax=Labels.Length; i<imax; ++i)
			{
				UILabel label = Labels[i];
				if (label==null || label.gameObject==null || !GUITools.ObjectExistInScene(label.gameObject))
					continue;
				
				if (Labels[i].GetComponent<UILocalize>()==null)
					Objs.Add( Labels[i].gameObject );
			}
			
			if (Objs.Count>0)
				Selection.objects = Objs.ToArray();
			else
				ShowWarning("All labels in this scene have a Localize component assigned");
#endif
		}

		#endregion
	}
}
