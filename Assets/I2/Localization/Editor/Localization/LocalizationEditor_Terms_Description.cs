//#define UGUI
//#define NGUI
//#define DFGUI

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace I2.Loc
{
	public partial class LocalizationEditor
	{
		#region Variables	
		static internal bool mKeysDesc_AllowEdit = false;
		#endregion
		
		#region Key Description
		
		void OnGUI_KeyList_ShowKeyDetails()
		{
			GUI.backgroundColor = Color.Lerp(Color.blue, Color.white, 0.9f);
			GUILayout.BeginVertical("AS TextArea", GUILayout.Height(1));
			OnGUI_Keys_Languages(mKeyToExplore, null);
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Delete"))
				EditorApplication.update += DeleteCurrentKey;
			
			if (GUILayout.Button("Merge"))
			{
				mCurrentViewMode = eViewMode.Tools;
				mCurrentToolsMode = eToolsMode.Merge;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUI.backgroundColor = Color.white;
		}

		void DeleteTerm( string Term )
		{
			LocalizationEditorDB.mLanguageSource.RemoveTerm (Term);
			mParsedTerms.Remove (Term);
			mSelectedKeys.Remove(Term);
			if (Term==mKeyToExplore)
				mKeyToExplore = string.Empty;
		}
		
		void DeleteCurrentKey()
		{
			EditorApplication.update -= DeleteCurrentKey;
			DeleteTerm (mKeyToExplore);
			mKeyToExplore = "";
		}

		TermData AddTerm( string Term, bool AutoSelect = true )
		{
			TermData data = LocalizationEditorDB.mLanguageSource.AddTerm(Term, eTermType.Text);
			if (!mParsedTerms.ContainsKey(Term))
				mParsedTerms[Term]=0;

			if (AutoSelect)
			{
				if (!mSelectedKeys.Contains(Term))
					mSelectedKeys.Add(Term);

				string sCategory = LocalizationEditorDB.GetCategoryFromFullTerm(Term);
				if (!mSelectedCategories.Contains(sCategory))
					mSelectedCategories.Add(sCategory);
			}
			return data;
		}
		
		// this method shows the key description and the localization to each language
		public static void OnGUI_Keys_Languages( string Key, Localize localizeCmp )
		{
			if (Key==null)
				Key = string.Empty;

			TermData termdata = null;

			if (string.IsNullOrEmpty(Key))
			{
				EditorGUILayout.HelpBox( "Select a Term to Localize", UnityEditor.MessageType.Info );
				return;
			}
			else
			{
				termdata = LocalizationEditorDB.mLanguageSource.GetTermData(Key);
				if (termdata==null)
				{
					EditorGUILayout.HelpBox( string.Format("Key '{0}' is not Localized or it is in a different Language Source", Key), UnityEditor.MessageType.Error );
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Add Term to Source"))
					{
						LocalizationEditorDB.mLanguageSource.AddTerm(Key, eTermType.Text);
						if (!mParsedTerms.ContainsKey(Key))
							mParsedTerms[Key]=0;
					}
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					
					return;
				}
			}

			//--[ Type ]----------------------------------
			if (localizeCmp==null)
			{
				GUILayout.BeginHorizontal();
					GUILayout.Label ("Type:", GUILayout.ExpandWidth(false));
				eTermType NewType = (eTermType)EditorGUILayout.EnumPopup(termdata.TermType, GUILayout.ExpandWidth(true));
				if (termdata.TermType != NewType)
					termdata.TermType = NewType;
				GUILayout.EndHorizontal();
			}


			//--[ Description ]---------------------------

			mKeysDesc_AllowEdit = GUILayout.Toggle(mKeysDesc_AllowEdit, "Description", EditorStyles.foldout, GUILayout.ExpandWidth(true));

			if (mKeysDesc_AllowEdit)
			{
				string NewDesc = EditorGUILayout.TextArea( termdata.Description );
				if (NewDesc != termdata.Description)
				{
					termdata.Description = NewDesc;
					EditorUtility.SetDirty(LocalizationEditorDB.mLanguageSource);
				}
			}
			else
				EditorGUILayout.HelpBox( string.IsNullOrEmpty(termdata.Description) ? "No description" : termdata.Description, UnityEditor.MessageType.Info );

			//--[ Languages ]---------------------------
			GUILayout.BeginVertical("AS TextArea", GUILayout.Height (1));
			
			for (int i=0; i<LocalizationEditorDB.mLanguageSource.mLanguages.Count; ++ i)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label (LocalizationEditorDB.mLanguageSource.mLanguages[i].Name, GUILayout.Width(100));

				string Translation = termdata.Languages[i] ?? string.Empty;

				if (termdata.TermType == eTermType.Text)
				{
					GUI.changed = false;
					string CtrName = "TranslatedText"+i;
					GUI.SetNextControlName(CtrName);
					Translation = EditorGUILayout.TextArea(Translation, GUILayout.Width(Screen.width-230));
					if (GUI.changed)
					{
						termdata.Languages[i] = Translation;
						EditorUtility.SetDirty(LocalizationEditorDB.mLanguageSource);
					}

					if (localizeCmp!=null &&
					    (GUI.changed/* || (Event.current != null && GUI.GetNameOfFocusedControl()==CtrName && EditorGUI.co)*/))
					{
						string PreviousLanguage = LocalizationManager.CurrentLanguage;
						LocalizationManager.PreviewLanguage(LocalizationEditorDB.mLanguageSource.mLanguages[i].Name);
						localizeCmp.OnLocalize();
						LocalizationManager.PreviewLanguage(PreviousLanguage);
					}
					
					if (GUILayout.Button("Translate", GUILayout.Width(80)))
						Translate( Key, ref termdata, ref termdata.Languages[i], LocalizationEditorDB.mLanguageSource.mLanguages[i].Code );
				}
				else
				{

					Object Obj = null;
					if (localizeCmp==null)
						Obj = ResourceManager.pInstance.LoadFromResources(Translation);
					else
						Obj = localizeCmp.FindTranslatedObject(Translation);

					if (Obj==null && LocalizationEditorDB.mLanguageSource!=null)
						Obj = LocalizationEditorDB.mLanguageSource.FindAsset(Translation);

					System.Type ObjType = typeof(Object);
					switch (termdata.TermType)
					{
						case eTermType.Font			: ObjType = typeof(Font); break;
						case eTermType.Texture		: ObjType = typeof(Texture); break;
						case eTermType.AudioClip	: ObjType = typeof(AudioClip); break;
						case eTermType.GameObject	: ObjType = typeof(GameObject); break;
#if UGUI
						case eTermType.Sprite		: ObjType = typeof(Sprite); break;
#endif
#if NGUI
						case eTermType.UIAtlas		: ObjType = typeof(UIAtlas); break;
						case eTermType.UIFont		: ObjType = typeof(UIFont); break;
#endif
#if DFGUI
						case eTermType.dfFont		: ObjType = typeof(dfFont); break;
						case eTermType.dfAtlas		: ObjType = typeof(dfAtlas); break;
#endif

#if TK2D
						case eTermType.TK2dFont			: ObjType = typeof(tk2dFont); break;
						case eTermType.TK2dCollection	: ObjType = typeof(tk2dSpriteCollection); break;
#endif

#if TextMeshPro
						case eTermType.TextMeshPFont	: ObjType = typeof(TMPro.TextMeshProFont); break;
#endif

						case eTermType.Object		: ObjType = typeof(Object); break;
					}

					bool bShowTranslationLabel = (Obj==null && !string.IsNullOrEmpty(Translation));
					if (bShowTranslationLabel)
					{
						GUI.backgroundColor=GUITools.DarkGray;
						GUILayout.BeginVertical("AS TextArea", GUILayout.Height(1));
						GUILayout.Space(2);
						
						GUI.backgroundColor = Color.white;
					}

					Object NewObj = EditorGUILayout.ObjectField(Obj, ObjType, true, GUILayout.ExpandWidth(true));
					if (Obj!=NewObj)
					{
						string sPath = AssetDatabase.GetAssetPath(NewObj);
						AddObjectPath( ref sPath, localizeCmp, NewObj );
						termdata.Languages[i] = sPath;
						EditorUtility.SetDirty(LocalizationEditorDB.mLanguageSource);
					}

					if (bShowTranslationLabel)
					{
						GUILayout.BeginHorizontal();
							GUI.color = Color.red;
							GUILayout.FlexibleSpace();
							GUILayout.Label (Translation, EditorStyles.miniLabel);
							GUILayout.FlexibleSpace();
							GUI.color = Color.white;
						GUILayout.EndHorizontal();
						GUILayout.EndVertical();
					}
				}
				
				GUILayout.EndHorizontal();
			}
			
			GUILayout.EndVertical();
		}

		static void AddObjectPath( ref string sPath, Localize localizeCmp, Object NewObj )
		{
			if (RemoveResourcesPath (ref sPath))
				return;

			// If its not in the Resources folder and there is no object reference already in the
			// Reference array, then add that to the Localization component or the Language Source
			bool AlreadyExist = false;
			if (localizeCmp!=null && System.Array.IndexOf(localizeCmp.TranslatedObjects, NewObj)>=0)
				AlreadyExist=true;
			else
				if (LocalizationEditorDB.mLanguageSource!=null && System.Array.IndexOf(LocalizationEditorDB.mLanguageSource.Assets, NewObj)>=0)
				AlreadyExist=true;
			
			if (AlreadyExist)
				return;

			if (localizeCmp!=null)
			{
				int Length = localizeCmp.TranslatedObjects.Length;
				System.Array.Resize( ref localizeCmp.TranslatedObjects, Length+1);
				localizeCmp.TranslatedObjects[Length] = NewObj;
				EditorUtility.SetDirty(localizeCmp);
			}
			else
			if (LocalizationEditorDB.mLanguageSource!=null)
			{
				int Length = LocalizationEditorDB.mLanguageSource.Assets.Length;
				System.Array.Resize( ref LocalizationEditorDB.mLanguageSource.Assets, Length+1);
				LocalizationEditorDB.mLanguageSource.Assets[Length] = NewObj;
				EditorUtility.SetDirty(LocalizationEditorDB.mLanguageSource);
			}
		}
		
		static void Translate ( string Key, ref TermData termdata, ref string sTarget, string TargetLanguageCode )
		{
			#if UNITY_WEBPLAYER
			ShowError ("Contacting google translation is not yet supported on WebPlayer" );
			#else
			// Translate first language that has something
			// If no language found, translation will fallback to autodetect language from key
			string SourceCode = "auto";
			string text = Key;
			
			for (int i=0, imax=termdata.Languages.Length; i<imax; ++i)
			{
				text = termdata.Languages[i];
				if (!string.IsNullOrEmpty(text) && LocalizationEditorDB.mLanguageSource.mLanguages[i].Code != TargetLanguageCode)
				{
					SourceCode = LocalizationEditorDB.mLanguageSource.mLanguages[i].Code;
					break;
				}
			}
			sTarget = GoogleTranslation.Translate( text, SourceCode, TargetLanguageCode );
			
			#endif
		}
		
		#endregion
	}
}
