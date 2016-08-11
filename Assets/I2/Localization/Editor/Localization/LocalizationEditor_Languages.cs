using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace I2.Loc
{
	public partial class LocalizationEditor
	{
		#region Variables

		#endregion

		void OnGUI_Languages()
		{
			//GUILayout.Space(5);

			OnGUI_ShowMsg();

			OnGUI_LanguageList();
		}

		#region GUI Languages
		
		void OnGUI_LanguageList()
		{
			SerializedProperty Prop_Languages = mSerializedObj_Source.FindProperty("mLanguages");

			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			GUILayout.FlexibleSpace();
			GUILayout.Label ("Languages:", EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
			GUILayout.FlexibleSpace();
			GUILayout.Label ("Code:", EditorStyles.miniLabel, GUILayout.Width(76));
			GUILayout.EndHorizontal();
			
			//--[ Language List ]--------------------------

			int IndexLanguageToDelete = -1;
			int LanguageToMoveUp = -1;
			int LanguageToMoveDown = -1;
			mScrollPos_Languages = GUILayout.BeginScrollView( mScrollPos_Languages, "AS TextArea", GUILayout.MinHeight (100));
			
			for (int i=0, imax=Prop_Languages.arraySize; i<imax; ++i)
			{
				GUILayout.BeginHorizontal();

				SerializedProperty Prop_Lang = Prop_Languages.GetArrayElementAtIndex(i);
				SerializedProperty Prop_LangName = Prop_Lang.FindPropertyRelative("Name");
				SerializedProperty Prop_LangCode = Prop_Lang.FindPropertyRelative("Code");

				if (GUILayout.Button ("X", "toolbarbutton", GUILayout.ExpandWidth(false)))
				{
					IndexLanguageToDelete = i;
				}
				
				GUILayout.BeginHorizontal(EditorStyles.toolbar);

				GUI.changed = false;
				string LanName = EditorGUILayout.TextField(Prop_LangName.stringValue, GUILayout.ExpandWidth(true));
				if (GUI.changed && !string.IsNullOrEmpty(LanName))
				{
					Prop_LangName.stringValue = LanName;
					GUI.changed = false;
				}

				List<string> codes = new List<string>(GoogleTranslation.mLanguageDef.Values);
				codes.Sort();
				codes.Insert(0, string.Empty);
				int Index = Mathf.Max(0, codes.IndexOf (Prop_LangCode.stringValue));
				GUI.changed = false;
				Index = EditorGUILayout.Popup(Index, codes.ToArray(), EditorStyles.toolbarPopup, GUILayout.Width(60));
				if (GUI.changed && Index>=0)
				{
					Prop_LangCode.stringValue = codes[Index];
				}

				GUILayout.EndHorizontal();

				GUI.enabled = (i<imax-1);
				if (GUILayout.Button( "\u25BC", EditorStyles.toolbarButton, GUILayout.Width(18))) LanguageToMoveDown = i;
				GUI.enabled = i>0;
				if (GUILayout.Button( "\u25B2", EditorStyles.toolbarButton, GUILayout.Width(18))) LanguageToMoveUp = i;
				GUI.enabled = true;
				
				GUILayout.EndHorizontal();
			}
			
			GUILayout.EndScrollView();
			
			OnGUI_AddLanguage( Prop_Languages );
			
			if (IndexLanguageToDelete>=0)
			{
				LocalizationEditorDB.mLanguageSource.RemoveLanguage( LocalizationEditorDB.mLanguageSource.mLanguages[IndexLanguageToDelete].Name );
				ParseTerms(true, false);
			}

			if (LanguageToMoveUp>=0)   SwapLanguages( LanguageToMoveUp, LanguageToMoveUp-1 );
			if (LanguageToMoveDown>=0) SwapLanguages( LanguageToMoveDown, LanguageToMoveDown+1 );
		}

		void SwapLanguages( int iFirst, int iSecond )
		{
			mSerializedObj_Source.ApplyModifiedProperties();
			LanguageSource Source = LocalizationEditorDB.mLanguageSource;

			SwapValues( Source.mLanguages, iFirst, iSecond );
			foreach (TermData termData in Source.mTerms)
				SwapValues ( termData.Languages, iFirst, iSecond );
		}

		void SwapValues( List<LanguageData> mList, int Index1, int Index2 )
		{
			LanguageData temp = mList[Index1];
			mList[Index1] = mList[Index2];
			mList[Index1] = temp;
		}
		void SwapValues( string[] mList, int Index1, int Index2 )
		{
			string temp = mList[Index1];
			mList[Index1] = mList[Index2];
			mList[Index1] = temp;
		}

		
		void OnGUI_AddLanguage( SerializedProperty Prop_Languages)
		{
			LanguageSource mLanguageSource = LocalizationEditorDB.mLanguageSource;
			//--[ Add Language Upper Toolbar ]-----------------
			
			if (DragAndDrop.visualMode == DragAndDropVisualMode.Link)
				GUI.color = Color.Lerp(Color.blue, Color.white, 0.8f);
			
			
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			mLanguages_NewLanguage = EditorGUILayout.TextField("", mLanguages_NewLanguage, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();

			if (GUILayout.Button("Add", EditorStyles.toolbarButton, GUILayout.Width(50)))
			{
				Prop_Languages.serializedObject.ApplyModifiedProperties();
				mLanguageSource.AddLanguage( mLanguages_NewLanguage, GoogleTranslation.GetLanguageCode(mLanguages_NewLanguage) );
				mLanguages_NewLanguage = "";
			}
			
			GUILayout.EndHorizontal();
			
			
			//--[ Add Language Bottom Toolbar ]-----------------
			
			GUILayout.BeginHorizontal();
			
			//-- Language Dropdown -----------------
			List<string> Languages = GoogleTranslation.GetSupportedLanguages(mLanguages_NewLanguage);

			for (int i=0, imax=mLanguageSource.mLanguages.Count; i<imax; ++i)
				Languages.Remove( GoogleTranslation.GetClosestLanguage(mLanguageSource.mLanguages[i].Name) );

			GUI.changed = false;
			int index = EditorGUILayout.Popup(0, Languages.ToArray(), EditorStyles.toolbarDropDown);

			if (GUI.changed && index>=0)
				mLanguages_NewLanguage = GoogleTranslation.GetFormatedLanguageName( Languages[index] );
			
			
			if (GUILayout.Button("Add", EditorStyles.toolbarButton, GUILayout.Width(50)) && index>=0)
			{
				Prop_Languages.serializedObject.ApplyModifiedProperties();
				mLanguages_NewLanguage = GoogleTranslation.GetFormatedLanguageName( Languages[index] );
				if (!string.IsNullOrEmpty(mLanguages_NewLanguage)) 
					mLanguageSource.AddLanguage( mLanguages_NewLanguage, GoogleTranslation.GetLanguageCode(mLanguages_NewLanguage) );
				mLanguages_NewLanguage = "";
			}
			
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUI.color = Color.white;
			
			if (GUILayoutUtility.GetLastRect().Contains (Event.current.mousePosition) && IsValidDraggedLanguage())
			{
				if (Event.current.type == EventType.DragUpdated)
					DragAndDrop.visualMode = DragAndDropVisualMode.Link;
				
				if (Event.current.type == EventType.DragPerform)
				{
					Prop_Languages.serializedObject.ApplyModifiedProperties();

					for (int i=0, imax=DragAndDrop.objectReferences.Length; i<imax; ++i)
						LocalizationEditorDB.Import_TextAsset( DragAndDrop.objectReferences[i] as TextAsset );

					ParseTerms(true, false);
					mSelectedCategories = LocalizationEditorDB.GetCategories();
					DragAndDrop.AcceptDrag();
					Event.current.Use();
				}
			}
		}

		
		static bool IsValidDraggedLanguage()
		{
			if (DragAndDrop.objectReferences==null || DragAndDrop.objectReferences.Length<=0)
				return false;
			
			for (int i=0, imax=DragAndDrop.objectReferences.Length; i<imax; ++i)
				if ((DragAndDrop.objectReferences[i] as TextAsset) == null || LocalizationEditorDB.mLanguageSource.GetLanguageIndex(DragAndDrop.objectReferences[i].name)>=0)
					return false;
			
			return true;
		}

		#endregion
	}
}