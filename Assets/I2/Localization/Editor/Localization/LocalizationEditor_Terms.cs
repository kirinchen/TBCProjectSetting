using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace I2.Loc
{
	public partial class LocalizationEditor
	{
		#region Variables
		
		Vector2 mScrollPos_Keys = Vector2.zero;
		
		public static string mKeyToExplore;  								// Key that should show all the language details
		string KeyList_Filter = "";
		//float mRowSize=-1;
		//float ScrollHeight;

		public static List<string> mSelectedKeys = new List<string>(); 	// Selected Keys in the list of mParsedKeys
		public static List<string> mSelectedCategories = new List<string>();


		enum eFlagsViewKeys
		{
			Used = 1<<1,
			Missing = 1<<2, 
			NotUsed = 1<<3
		};
		static int mFlagsViewKeys = ((int)eFlagsViewKeys.Used | (int)eFlagsViewKeys.NotUsed | (int)eFlagsViewKeys.Missing);

		public static string mTermsList_NewTerm = null;
		#endregion
		
		#region GUI Key List
		void OnGUI_KeysList(bool AllowExpandKey = true, float Height = 300.0f, bool ShowTools=true)
		{
			//GUILayout.Space (5);

			//--[ List Filters ]--------------------------------------
			
			OnGUI_ShowMsg();
			
			GUILayout.BeginHorizontal();
			mFlagsViewKeys = OnGUI_FlagToogle("Used","Shows All Terms referenced in the parsed scenes", 				mFlagsViewKeys, (int)eFlagsViewKeys.Used);
			mFlagsViewKeys = OnGUI_FlagToogle("Not Used", "Shows all Terms from the Source that are not been used", 	mFlagsViewKeys, (int)eFlagsViewKeys.NotUsed);
			mFlagsViewKeys = OnGUI_FlagToogle("Missing","Shows all Terms Used but not defined in the Source", 			mFlagsViewKeys, (int)eFlagsViewKeys.Missing);

			OnGUI_SelectedCategories();

			GUILayout.EndHorizontal();
			
			//--[ Keys List ]-----------------------------------------
			mScrollPos_Keys = GUILayout.BeginScrollView( mScrollPos_Keys, "AS TextArea", GUILayout.MinHeight(Height), GUILayout.MaxHeight(Screen.height), GUILayout.ExpandHeight(false));

			List<string> ParsedTermsList = new List<string>(mParsedTerms.Keys);

			// Parse according to Key without Category
			ParsedTermsList.Sort ( delegate(string x, string y) 
			{
				return LocalizationEditorDB.GetKeyFromFullTerm(x).CompareTo( LocalizationEditorDB.GetKeyFromFullTerm(y) );
			});
			bool bAnyValidUsage = false;

			/*mRowSize = EditorStyles.toolbar.fixedHeight;
			if (Event.current!=null && Event.current.type == EventType.Layout)
				ScrollHeight = mScrollPos_Keys.y;*/

			for (int i=0, imax=ParsedTermsList.Count; i<imax; ++i)
			{
				string sKey, sCategory;
				string FullKey = ParsedTermsList[i];

				int nUses = mParsedTerms[FullKey];
				bAnyValidUsage = bAnyValidUsage | (nUses>=0);

				if (!ShouldShowTerm( FullKey, nUses ))
					continue;

				/*if (mRowSize>0)
				{
					float YPosMin = mRowSize*i - ScrollHeight;
					float YPosMax = YPosMin + mRowSize;
					if (YPosMax<-2*mRowSize || YPosMin>Height+mRowSize)
					{
						GUILayout.Space(mRowSize);
						continue;
					}
				}*/

				LocalizationEditorDB.DeserializeFullTerm( FullKey, out sKey, out sCategory );

				GUILayout.BeginHorizontal(GUILayout.Height (1));

				//--[ Toggle ]---------------------
				
				GUILayout.BeginHorizontal("Toolbar");
				OnGUI_SelectableToogleListItem( FullKey, ref mSelectedKeys, "OL Toggle" );
				GUILayout.EndHorizontal();
				
				bool bEnabled = mSelectedKeys.Contains(FullKey);
				
				//--[ Number of Objects using this key ]---------------------
				
				if (nUses>=0)
				{
					if (nUses==0) 
					{
						GUI.color = Color.Lerp(Color.gray, Color.white, 0.5f);
						GUILayout.Label(nUses.ToString(), "toolbarbutton", GUILayout.Width(30));
					}
					else
					{
						if (GUILayout.Button(nUses.ToString(), "toolbarbutton", GUILayout.Width(30)))
							SelectObjectsUsingKey(FullKey);
					}
				}
				else
				{
					GUI.color = Color.Lerp(Color.red, Color.white, 0.6f);
					if (GUILayout.Button("", "toolbarbutton", GUILayout.Width(30)))
					{
						mCurrentToolsMode = eToolsMode.Parse;
						mCurrentViewMode = eViewMode.Tools;
					}
				}
				GUI.color = Color.white;
				
				TermData termData = LocalizationEditorDB.mLanguageSource.GetTermData(FullKey);
				bool bKeyIsMissing = (termData==null);
				if (bKeyIsMissing)
				{
					Rect rect = GUILayoutUtility.GetLastRect();
					float Width = 15;
					rect.xMin = rect.xMax+1;
					rect.xMax = rect.xMin + rect.height;
					GUI.DrawTexture( rect, GUI.skin.GetStyle("CN EntryWarn").normal.background);
					GUILayout.Space (Width);
				}
				if (mKeyToExplore==FullKey)
				{
					GUI.backgroundColor = Color.Lerp(Color.blue, Color.white, 0.8f);
					if (GUILayout.Button(new GUIContent(sKey, EditorStyles.foldout.onNormal.background), "AS TextArea"))
					{
						mKeyToExplore = string.Empty;
						ClearErrors();
					}
					GUI.backgroundColor = Color.white;
				}
				else
				{
					GUIStyle LabelStyle = EditorStyles.label;
					
					if (!bKeyIsMissing && !TermHasAllTranslations(termData))
					{
						LabelStyle = new GUIStyle( EditorStyles.label ); 
						LabelStyle.fontStyle = FontStyle.Italic;
						GUI.color = Color.Lerp(Color.white, Color.yellow, 0.5f);
					}
					
					if (!bEnabled) GUI.contentColor = Color.Lerp(Color.gray, Color.white, 0.3f);
					if (GUILayout.Button(sKey, LabelStyle))
					{
						SelectTerm( FullKey );
						ClearErrors ();
					}

					if (!bEnabled) GUI.contentColor = Color.white;
					GUI.color = Color.white;
				}

				//--[ Category ]--------------------------

				if (sCategory!=LocalizationEditorDB.EmptyCategory)
				{
					if (mKeyToExplore==FullKey)
					{
						if (GUILayout.Button( sCategory, EditorStyles.toolbarButton, new GUILayoutOption[]{ GUILayout.ExpandWidth(false), GUILayout.MinWidth(100)}))
							OpenTool_ChangeCategoryOfSelectedTerms();
					}
					else
					if (GUILayout.Button( sCategory, EditorStyles.miniLabel, GUILayout.ExpandWidth(false)))
					{
						SelectTerm( FullKey );
						ClearErrors();
					}
				}

				GUILayout.EndHorizontal();

				//--[ Key Details ]-------------------------------
				
				if (AllowExpandKey && mKeyToExplore==FullKey)
				{
					OnGUI_KeyList_ShowKeyDetails();
				}
			}

			OnGUI_KeysList_AddKey();

			GUILayout.EndScrollView();
			
			OnGUI_Keys_ListSelection();    // Selection Buttons
			
			if (!bAnyValidUsage)
				EditorGUILayout.HelpBox("Use (Tools\\Parse Terms) to find how many times each of the Terms are used", UnityEditor.MessageType.Info);

			if (ShowTools)
			{
				GUILayout.BeginHorizontal();
				GUI.enabled = (mSelectedKeys.Count>0 || !string.IsNullOrEmpty(mKeyToExplore));
					if (GUILayout.Button ("Add keys to Source")) 		 AddTermsToSource();
					if (GUILayout.Button ("Remove keys from Source")) 	 RemoveTermsFromSource();

					GUILayout.FlexibleSpace ();

					if (GUILayout.Button ("Change Category")) OpenTool_ChangeCategoryOfSelectedTerms();
				GUI.enabled = true;
				GUILayout.EndHorizontal();
			}
		}

		bool TermHasAllTranslations( TermData data )
		{
			for (int i=0, imax=data.Languages.Length; i<imax; ++i)
				if (string.IsNullOrEmpty(data.Languages[i]))
					return false;
			return true;
		}

		void OnGUI_KeysList_AddKey()
		{
			GUILayout.BeginHorizontal();
				GUI.color = Color.Lerp(Color.gray, Color.white, 0.5f);
				bool bWasEnabled = (mTermsList_NewTerm!=null);
				bool bEnabled = !GUILayout.Toggle (!bWasEnabled, "+", EditorStyles.toolbarButton, GUILayout.Width(30));
				GUI.color = Color.white;

				if (bWasEnabled  && !bEnabled) mTermsList_NewTerm = null;
				if (!bWasEnabled &&  bEnabled) mTermsList_NewTerm = string.Empty;

				if (bEnabled)
				{
					GUILayout.BeginHorizontal(EditorStyles.toolbar);
					mTermsList_NewTerm = EditorGUILayout.TextField(mTermsList_NewTerm, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
					GUILayout.EndHorizontal();

					LanguageSource.ValidateFullTerm( ref mTermsList_NewTerm );
					if (LocalizationEditorDB.mLanguageSource.ContainsTerm(mTermsList_NewTerm) || string.IsNullOrEmpty(mTermsList_NewTerm))
						GUI.enabled = false;
	
					if (GUILayout.Button ("Create Key", "toolbarbutton", GUILayout.ExpandWidth(false)))
					{
						AddTerm(mTermsList_NewTerm);
						SelectTerm( mTermsList_NewTerm );
						ClearErrors();
						mTermsList_NewTerm = null;
					}
					GUI.enabled = true;
				}
			GUILayout.EndHorizontal();
		}

		void OpenTool_ChangeCategoryOfSelectedTerms()
		{
			mCurrentViewMode = eViewMode.Tools;
			mCurrentToolsMode = eToolsMode.Categorize;
			if (!string.IsNullOrEmpty(mKeyToExplore) && !mSelectedKeys.Contains(mKeyToExplore))
				mSelectedKeys.Add(mKeyToExplore);
			mSelectedKeys.Sort();
		}

		void OnGUI_SelectedCategories()
		{
			List<string> mCategories = LocalizationEditorDB.GetCategories();

			if (mCategories.Count==0)
				return;

			//--[ Compress Mask ]-------------------
			int Mask = 0;
			for (int i=0, imax=mCategories.Count; i<imax; ++i)
				if (mSelectedCategories.Contains( mCategories[i] ))
					Mask |= (1<<i);
			
			//--[ GUI ]-----------------------------
			GUI.changed = false;
			Mask = EditorGUILayout.MaskField(Mask, mCategories.ToArray(), EditorStyles.toolbarDropDown, GUILayout.Width(100));

			//--[ Decompress Mask ]-------------------
			if (GUI.changed)
			{
				GUI.changed = false;
				mSelectedCategories.Clear();
				for (int i=0, imax=mCategories.Count; i<imax; ++i)
					if ( (Mask & (1<<i)) > 0 )
						mSelectedCategories.Add (mCategories[i]);
			}
		}

		// Bottom part of the Key list (buttons: All, None, Used,...  to select the keys)
		void OnGUI_Keys_ListSelection()
		{
			GUILayout.BeginHorizontal("toolbarbutton");
			
			if (GUILayout.Button(new GUIContent("All","Selects All Terms in the list"), "toolbarbutton", GUILayout.ExpandWidth(false)))  { mSelectedKeys.Clear(); mSelectedKeys.AddRange(mParsedTerms.Keys); }
			if (GUILayout.Button(new GUIContent("None","Clears the selection"), "toolbarbutton", GUILayout.ExpandWidth(false))) { mSelectedKeys.Clear(); }
			GUILayout.Space (5);
			
			GUI.enabled = ((mFlagsViewKeys & (int)eFlagsViewKeys.Used)>1);
			if (GUILayout.Button(new GUIContent("Used","Selects All Terms referenced in the parsed scenes"), "toolbarbutton", GUILayout.ExpandWidth(false))) 
			{ 
				mSelectedKeys.Clear(); 
				foreach (KeyValuePair<string,int> kvp in mParsedTerms)
					if (kvp.Value > 0)
						mSelectedKeys.Add ( kvp.Key );
			}
			GUI.enabled = ((mFlagsViewKeys & (int)eFlagsViewKeys.NotUsed)>1);
			if (GUILayout.Button(new GUIContent("Not Used", "Selects all Terms from the Source that are not been used"), "toolbarbutton", GUILayout.ExpandWidth(false))) 
			{ 
				mSelectedKeys.Clear(); 
				foreach (KeyValuePair<string,int> kvp in mParsedTerms)
					if (kvp.Value == 0)
						mSelectedKeys.Add ( kvp.Key );
			}

			GUI.enabled = ((mFlagsViewKeys & (int)eFlagsViewKeys.Missing)>1);
			if (GUILayout.Button(new GUIContent("Missing","Selects all Terms Used but not defined in the Source"), "toolbarbutton", GUILayout.ExpandWidth(false)))
			{ 
				mSelectedKeys.Clear(); 
				foreach (KeyValuePair<string,int> kvp in mParsedTerms)
					if (!LocalizationEditorDB.mLanguageSource.ContainsTerm( kvp.Key ))
						mSelectedKeys.Add ( kvp.Key );
			}
			GUI.enabled = true;
			GUI.SetNextControlName ("TermsFilter");
			KeyList_Filter = GUILayout.TextField(KeyList_Filter, "ToolbarSeachTextField", GUILayout.ExpandWidth(true));
			if (GUILayout.Button (string.Empty, string.IsNullOrEmpty(KeyList_Filter) ? "ToolbarSeachCancelButtonEmpty" : "ToolbarSeachCancelButton", GUILayout.ExpandWidth(false)))
				KeyList_Filter = string.Empty;
			
			GUILayout.EndHorizontal();
		}
		
		
		#endregion

		#region Filtering

		public bool ShouldShowTerm (string FullTerm)
		{
			int nUses = 0;
			if (!mParsedTerms.TryGetValue(FullTerm, out nUses))
				return false;
			
			return ShouldShowTerm (FullTerm, nUses);
		}
		
		public bool ShouldShowTerm (string FullTerm, int nUses )
		{
			string Key, Category;
			LocalizationEditorDB.DeserializeFullTerm( FullTerm, out Key, out Category );
			return ShouldShowTerm (Key, Category, nUses );
		}
		
		public bool ShouldShowTerm (string Term, string Category, int nUses )
		{
			if (!string.IsNullOrEmpty(Category) && !mSelectedCategories.Contains(Category)) 
				return false;
			
			if (!StringContainsFilter(Term, KeyList_Filter)) 
				return false;
			
			
			if (nUses<0) return true;
			bool bIsMissing = !LocalizationEditorDB.mLanguageSource.ContainsTerm (Term);

			if ((mFlagsViewKeys & (int)eFlagsViewKeys.Missing)>0 && bIsMissing) return true;
			if ((mFlagsViewKeys & (int)eFlagsViewKeys.Missing)==0 && bIsMissing) return false;

			if ((mFlagsViewKeys & (int)eFlagsViewKeys.Used)>0 && nUses>0) return true;
			if ((mFlagsViewKeys & (int)eFlagsViewKeys.NotUsed)>0 && nUses==0) return true;
			return false;
		}
		
		bool StringContainsFilter( string Term, string Filter )
		{
			if (string.IsNullOrEmpty(Filter))
				return true;
			Term = Term.ToLower();
			string[] Filters = Filter.ToLower().Split(";,".ToCharArray());
			for (int i=0, imax=Filters.Length; i<imax; ++i)
				if (Term.Contains(Filters[i]))
					return true;
			
			return false;
		}

		#endregion
		
		#region Add/Remove Keys to DB
		
		void AddTermsToSource()
		{
			for (int i=mSelectedKeys.Count-1; i>=0; --i)
			{
				string key = mSelectedKeys[i];

				if (!ShouldShowTerm(key))
					continue;

				AddTerm(key);
				mSelectedKeys.RemoveAt(i);
			}
		}
		
		void RemoveTermsFromSource()
		{
			for (int i=mSelectedKeys.Count-1; i>=0; --i)
			{
				string key = mSelectedKeys[i];
				
				if (!ShouldShowTerm(key)) 
					continue;

				DeleteTerm(key);
			}
		}

		#endregion
		
		#region Select Objects in Current Scene


		public static void SelectTerm( string Key, bool SwitchToKeysTab=false )
		{
			mKeyToExplore = Key;
			mKeysDesc_AllowEdit = false;
			if (SwitchToKeysTab)
				mCurrentViewMode = eViewMode.Keys;
		}


		void SelectObjectsUsingKey( string Key )
		{
			List<GameObject> SelectedObjs = new List<GameObject>();
			
			Localize[] Locals = (Localize[])Resources.FindObjectsOfTypeAll(typeof(Localize));
			
			if (Locals==null)
				return;
			
			for (int i=0, imax=Locals.Length; i<imax; ++i)
			{
				Localize localize = Locals[i];
				if (localize==null || localize.gameObject==null || !GUITools.ObjectExistInScene(localize.gameObject))
					continue;

				string Term, SecondaryTerm;
				localize.GetFinalTerms( out Term, out SecondaryTerm );

				if (Key==Term || Key==SecondaryTerm)
					SelectedObjs.Add (localize.gameObject);
			}

			if (SelectedObjs.Count>0)
				Selection.objects = SelectedObjs.ToArray();
			else
				ShowWarning("The selected Terms are not used in this Scene. Try opening other scenes"); 
		}

		#endregion

	}
}