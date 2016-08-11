using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace I2.Loc
{
	/*public partial class LocalizationEditor
	{
		#region Variables

		List<LanguageSource> mSources = new List<LanguageSource>();
		string[] mSourcesName;
		List<LanguageSource> mSelectedSources = new List<LanguageSource>();
		LanguageSource mSelectedSource;
		
		#endregion
		
		#region Sources

		void UpdateSourcesList()
		{
			mSources.Clear();
			
			//mSources.Add (LocalizationManager.Resource);
			
			LanguageSource[] aSources = Resources.FindObjectsOfTypeAll<LanguageSource>();
			for (int i=0, imax=aSources.Length; i<imax; ++i)
				mSources.Add (aSources[i]);
			
			mSourcesName = new string[ mSources.Count ];
			for (int i=0, imax=mSources.Count; i<imax; ++i)
				mSourcesName[i] = mSources[i].GetSourceName();
			
			// Remove Sources that no longer exist
			for (int i=mSelectedSources.Count-1; i>=0; --i)
				if (!mSources.Contains(mSelectedSources[i]))
					mSelectedSources.RemoveAt(i);
			
			// If no one selected, then select all sources (this will happen at startup)
			if (mSelectedSources.Count==0)
				mSelectedSources.AddRange( mSources );
		}

		void UpdateSourcesListAndSelect()
		{
			EditorApplication.update -= UpdateSourcesListAndSelect;

			UpdateSourcesList();

			// Selected sources
			List<GameObject> objs = new List<GameObject>();
			for (int i=0, imax=mSelectedSources.Count; i<imax; ++i)
			{
				MonoBehaviour pComp = (mSelectedSources[i] as MonoBehaviour);
				if (pComp && pComp.gameObject)
					objs.Add (pComp.gameObject);
			}
			Selection.objects = objs.ToArray();
		}
		
		void SelectFirstSource()
		{
			UpdateSourcesList();
			if (mSources.Contains(mSelectedSource))
				return;

			if (mSelectedSources.Count>0)
				SelectSource (mSelectedSources[0]);
			else
				if (mSources.Count>0)
					SelectSource (mSources[0]);
		}
		
		void SelectSource( LanguageSource Source )
		{
			mSelectedSource = Source;
		}

		#endregion

		#region GUI
		
		void OnGUI_SourceMaskField()
		{
			//--[ Compress Mask ]-------------------
			int Mask = 0;
			for (int i=0, imax=mSources.Count; i<imax; ++i)
				if (mSelectedSources.Contains(mSources[i]))
					Mask |= (1<<i);

			//--[ GUI ]-----------------------------
			GUI.changed = false;
			GUILayout.BeginHorizontal();
				Mask = EditorGUILayout.MaskField(Mask, mSourcesName, EditorStyles.toolbarDropDown);

				if (GUILayout.Button("@", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
					EditorApplication.update += UpdateSourcesListAndSelect;
			GUILayout.EndHorizontal();

			//--[ Decompress Mask ]-------------------
			if (GUI.changed)
			{
				GUI.changed = false;
				if (Mask==0) Mask = 1;
				mSelectedSources.Clear();
				for (int i=0, imax=mSources.Count; i<imax; ++i)
					if ( (Mask & (1<<i)) > 0 )
						mSelectedSources.Add (mSources[i]);
			}
		}

		void OnGUI_SourceDropDown()
		{
			if (!mSources.Contains(mSelectedSource))
				SelectFirstSource();

			int Index = mSources.IndexOf(mSelectedSource);

			GUI.changed = false;
			GUILayout.BeginHorizontal();
				GUILayout.Label ("Source:", GUILayout.ExpandWidth(false));
				Index = EditorGUILayout.Popup( Index, mSourcesName, EditorStyles.toolbarDropDown);
						
				if (GUILayout.Button("@", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
					EditorApplication.update += SelectFirstSource;
			GUILayout.EndHorizontal();

			if (GUI.changed)
			{
				GUI.changed = true;
				SelectSource( Index>=0 ? mSources[Index] : null );
			}
		}

		
		#endregion
	}*/
}