using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace I2.Loc
{
	public partial class LocalizationEditor
	{
		#region Variables
		#endregion

		#region GUI

		void OnGUI_ScenesList( bool SmallSize = false )
		{
			OnGUI_ScenesList_TitleBar();			

			mScrollPos_BuildScenes = GUILayout.BeginScrollView( mScrollPos_BuildScenes, "AS TextArea", GUILayout.Height ( SmallSize ? 100 : 200));
			
			bool bShowCurrentScene = true;
			for (int i=0, imax=EditorBuildSettings.scenes.Length; i<imax; ++i)
			{
				GUILayout.BeginHorizontal();
				
				OnGUI_SelectableToogleListItem( EditorBuildSettings.scenes[i].path, ref mSelectedScenes, "OL Toggle" );
				
				bool bSelected = mSelectedScenes.Contains(EditorBuildSettings.scenes[i].path);
				GUI.color = (bSelected ? Color.white : Color.Lerp(Color.gray, Color.white, 0.5f));
				if (GUILayout.Button (EditorBuildSettings.scenes[i].path, "Label"))
				{
					if (mSelectedScenes.Contains(EditorBuildSettings.scenes[i].path))
						mSelectedScenes.Remove(EditorBuildSettings.scenes[i].path);
					else
						mSelectedScenes.Add(EditorBuildSettings.scenes[i].path);
				}
				GUI.color = Color.white;
				
				if (EditorBuildSettings.scenes[i].path == EditorApplication.currentScene)
					bShowCurrentScene = false;
				
				GUILayout.EndHorizontal();
			}
			
			if (bShowCurrentScene) 
			{
				GUILayout.BeginHorizontal();
				OnGUI_SelectableToogleListItem( EditorApplication.currentScene, ref mSelectedScenes, "OL Toggle" );
				
				bool bSelected = mSelectedScenes.Contains(EditorApplication.currentScene);
				GUI.color = (bSelected ? Color.white : Color.Lerp(Color.gray, Color.white, 0.5f));
				
				if (GUILayout.Button (EditorApplication.currentScene, "Label"))
				{
					if (mSelectedScenes.Contains(EditorApplication.currentScene))
						mSelectedScenes.Remove(EditorApplication.currentScene);
					else
						mSelectedScenes.Add(EditorApplication.currentScene);
				}
				GUI.color = Color.white;
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}

		void OnGUI_ScenesList_TitleBar()
		{
			GUILayout.BeginHorizontal();
				GUILayout.Label("Scenes to Parse:", "toolbarbutton");
				if (GUILayout.Button("All", "toolbarbutton", GUILayout.ExpandWidth(false)))  
				{ 
					mSelectedScenes.Clear(); 
					for (int i=0, imax=EditorBuildSettings.scenes.Length; i<imax; ++i)
						mSelectedScenes.Add (EditorBuildSettings.scenes[i].path);
					if (!mSelectedScenes.Contains(EditorApplication.currentScene))
						mSelectedScenes.Add (EditorApplication.currentScene);
				}
				if (GUILayout.Button("None", "toolbarbutton", GUILayout.ExpandWidth(false))) { mSelectedScenes.Clear(); }
				if (GUILayout.Button("Used", "toolbarbutton", GUILayout.ExpandWidth(false)))  
				{ 
					mSelectedScenes.Clear(); 
					for (int i=0, imax=EditorBuildSettings.scenes.Length; i<imax; ++i)
						if (EditorBuildSettings.scenes[i].enabled)
							mSelectedScenes.Add (EditorBuildSettings.scenes[i].path);
				}
				if (GUILayout.Button("Current", "toolbarbutton", GUILayout.ExpandWidth(false)))  
				{ 
					mSelectedScenes.Clear(); 
					mSelectedScenes.Add (EditorApplication.currentScene);
				}
			GUILayout.EndHorizontal();
		}
		
		void SelectUsedScenes()
		{
			mSelectedScenes.Clear();
			for (int i=0, imax=EditorBuildSettings.scenes.Length; i<imax; ++i)
				if (EditorBuildSettings.scenes[i].enabled)
					mSelectedScenes.Add( EditorBuildSettings.scenes[i].path );
		}
		
		#endregion
	
		#region Iterate thru the Scenes

		delegate void Delegate0();

		void ExecuteActionOnSelectedScenes( Delegate0 Action )
		{
			string InitialScene = EditorApplication.currentScene;
			
			if (mSelectedScenes.Count<=0)
				mSelectedScenes.Add (InitialScene);
			
			bool HasSaved = false;
			
			foreach (string ScenePath in mSelectedScenes)
			{
				if (ScenePath != EditorApplication.currentScene)
				{
					if (!HasSaved)	// Saving the initial scene to avoid loosing changes
					{
						EditorApplication.SaveScene();
						HasSaved = true;
					}
					EditorApplication.OpenScene( ScenePath );
				}

				Action();
			}
			
			if (InitialScene != EditorApplication.currentScene)
				EditorApplication.OpenScene( InitialScene );
			
			if (LocalizationEditorDB.mLanguageSource)
				Selection.activeObject = LocalizationEditorDB.mLanguageSource.gameObject;
		}
		#endregion
	}
}