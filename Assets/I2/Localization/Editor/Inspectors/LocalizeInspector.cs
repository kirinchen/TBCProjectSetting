//#define UGUI
//#define NGUI
//#define DFGUI

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace I2.Loc
{
	[CustomEditor(typeof(Localize))]
	public partial class LocalizeInspector : Editor
	{
		#region Variables

		SerializedObject mSerializedObj_Localize;

		bool mAllowEditKeyName = false;
		string mNewKeyName = "";

		bool mGUI_ShowReferences = false,
			 mGUI_ShowTems = true,
			 mGUI_ShowCallback = true;

		#endregion
		
		#region Inspector
		
		void OnEnable()
		{
			Localize loc = (Localize)target;
			mSerializedObj_Localize = new SerializedObject( loc );

			SetLanguageSource();

			mGUI_ShowReferences = (loc.TranslatedObjects!=null && loc.TranslatedObjects.Length>0);
			mGUI_ShowCallback = (loc.LocalizeCallBack.Target!=null);
			mGUI_ShowTems = true;
			LocalizationEditor.mKeysDesc_AllowEdit = false;
			GUI_SelectedTerm = 0;
			mNewKeyName = loc.Term;

			UpgradeManager.EnablePlugins();
		}

		void SetLanguageSource()
		{
			Localize loc = (Localize)target;
			
			if (LocalizationEditorDB.mLanguageSource==null || LocalizationEditorDB.mLanguageSource != loc.Source)
			{
				if (loc.Source) 
					LocalizationEditorDB.SetSource(loc.Source);
				else 
					LocalizationEditorDB.HasLanguageSource(true);
			}
		}

		void OnDisable()
		{
		}

		#endregion

		#region GUI
		
		public override void OnInspectorGUI()
		{
			mSerializedObj_Localize.Update();

			SetLanguageSource();

			if (!LocalizationEditorDB.mLanguageSource)
			{
				EditorGUILayout.HelpBox("Unable to find a Language Source.", MessageType.Warning);
			}
			else
			{
				GUILayout.Space(10);
					OnGUI_Target ();
				GUILayout.Space(10);
					OnGUI_Terms();

				//if (mGUI_ShowTems || mGUI_ShowReferences) GUILayout.Space(5);

					OnGUI_References();

				if (mGUI_ShowReferences || mGUI_ShowCallback) GUILayout.Space(10);

					Localize loc = target as Localize;

				//--[ Localize Callback ]----------------------
					string HeaderTitle = "On Localize Call:";
					if (!mGUI_ShowCallback && loc.LocalizeCallBack.Target!=null && !string.IsNullOrEmpty(loc.LocalizeCallBack.MethodName))
						HeaderTitle = string.Concat(HeaderTitle, " <b>",loc.LocalizeCallBack.Target.name, ".</b><i>", loc.LocalizeCallBack.MethodName, "</i>");
					mGUI_ShowCallback = GUITools.DrawHeader(HeaderTitle, mGUI_ShowCallback);
					if (mGUI_ShowCallback)
					{
						GUITools.BeginContents();
							GUITools.DrawEventCallBack( loc.LocalizeCallBack );
						GUITools.EndContents();
					}
			}
			OnGUI_Source ();

			mSerializedObj_Localize.ApplyModifiedProperties();
		}

		#endregion

		#region References

		void OnGUI_References()
		{
			if (mGUI_ShowReferences = GUITools.DrawHeader ("References", mGUI_ShowReferences))
			{
				GUITools.BeginContents();
				GUITools.DrawObjectsArray( mSerializedObj_Localize.FindProperty("TranslatedObjects") );
				GUITools.EndContents();
			}
		}

		#endregion


		#region Terms

		int GUI_SelectedTerm = 0;
		void OnGUI_Terms()
		{
			if (mGUI_ShowTems=GUITools.DrawHeader ("Terms", mGUI_ShowTems))
			{
				//--[ tabs: Main and Secondary Terms ]----------------
				int oldTab = GUI_SelectedTerm;
				if (((Localize)target).CanUseSecondaryTerm)
				{
					GUI_SelectedTerm = GUITools.DrawTabs (GUI_SelectedTerm, new string[]{"Main", "Secondary"});
				}
				else
				{
					GUI_SelectedTerm = 0;
					GUITools.DrawTabs (GUI_SelectedTerm, new string[]{"Main", ""});
				}

				GUITools.BeginContents();

					if (GUI_SelectedTerm==0) OnGUI_PrimaryTerm( oldTab!=GUI_SelectedTerm );
										else OnGUI_SecondaryTerm(oldTab!=GUI_SelectedTerm);

				GUITools.EndContents();

				//--[ Right To Left ]-------------
				GUILayout.BeginHorizontal();
					SerializedProperty Prop_IgnoreRTL = mSerializedObj_Localize.FindProperty("IgnoreRTL");
					GUI.changed = false;
					bool bIgnore = GUILayout.Toggle( Prop_IgnoreRTL.boolValue, " Ignore Right To Left Languages" );
					if (GUI.changed)
						Prop_IgnoreRTL.boolValue = bIgnore;

					GUILayout.FlexibleSpace();

					SerializedProperty Prop_Modifier = mSerializedObj_Localize.FindProperty( GUI_SelectedTerm==0 ? "PrimaryTermModifier" : "SecondaryTermModifier");
					GUI.changed=false;
					int val = EditorGUILayout.Popup(Prop_Modifier.enumValueIndex, Prop_Modifier.enumNames);
					if (GUI.changed)
					{
						Prop_Modifier.enumValueIndex = val;
						GUI.changed = false;
					}

				GUILayout.EndHorizontal();
			}
		}

		void OnGUI_PrimaryTerm( bool OnOpen )
		{
			SerializedProperty Prop_Term = mSerializedObj_Localize.FindProperty("mTerm");
			string Key = Prop_Term.stringValue;
			Localize localizeCmp = ((Localize)target);
			if (string.IsNullOrEmpty(Key))
			{
				string SecondaryTerm;
				((Localize)target).GetFinalTerms( out Key, out SecondaryTerm );
			}

			if (OnOpen) mNewKeyName = Key;
			if ( OnGUI_SelectKey( ref Key, string.IsNullOrEmpty(Prop_Term.stringValue)))
				Prop_Term.stringValue = Key;
			LocalizationEditor.OnGUI_Keys_Languages( Key, localizeCmp );
		}

		void OnGUI_SecondaryTerm( bool OnOpen )
		{
			SerializedProperty Prop_Term = mSerializedObj_Localize.FindProperty("mTermSecondary");
			string Key = Prop_Term.stringValue;
			Localize localizeCmp = ((Localize)target);

			if (string.IsNullOrEmpty(Key))
			{
				string ss;
				((Localize)target).GetFinalTerms( out ss, out Key );
			}
			
			if (OnOpen) mNewKeyName = Key;
			if ( OnGUI_SelectKey( ref Key, string.IsNullOrEmpty(Prop_Term.stringValue)))
				Prop_Term.stringValue = Key;
			LocalizationEditor.OnGUI_Keys_Languages( Key, localizeCmp );
		}

		bool OnGUI_SelectKey( ref string Term, bool Inherited )  // Inherited==true means that the mTerm is empty and we are using the Label.text instead
		{
			GUILayout.Space (5);
			GUILayout.BeginHorizontal();

			bool bChanged = false;
			mAllowEditKeyName = GUILayout.Toggle(mAllowEditKeyName, "Term:", EditorStyles.foldout, GUILayout.ExpandWidth(false));
			if (bChanged && mAllowEditKeyName)
				mNewKeyName = Term;

			bChanged = false;
			List<string> Terms = LocalizationEditorDB.mLanguageSource.GetTermsList();

			// If there is a filter, remove all terms not matching that filter
			if (mAllowEditKeyName && !string.IsNullOrEmpty(mNewKeyName)) 
			{
				string Filter = mNewKeyName.ToUpper();
				for (int i=Terms.Count-1; i>=0; --i)
					if (!Terms[i].ToUpper().Contains(Filter) && Terms[i]!=Term)
						Terms.RemoveAt(i);

			}
			Terms.Sort(System.StringComparer.OrdinalIgnoreCase);

			int Index = Terms.IndexOf( Term );
			if (Index == -1 && !Inherited)
			{
				Terms.Insert (0, Term);
				Index = 0;
			}
			Terms.Add ("<none>");

			if (Inherited)
				GUI.contentColor = Color.yellow*0.8f;

			GUI.changed = false;
			int newIndex = EditorGUILayout.Popup( Index, Terms.ToArray());

			GUI.contentColor = Color.white;
			if (/*newIndex != Index && newIndex>=0*/GUI.changed)
			{
				GUI.changed = false;
				Terms [Terms.Count - 1] = string.Empty;
				mNewKeyName = Term = Terms[newIndex];
				mAllowEditKeyName = false;
				bChanged = true;
			}

			TermData termData = LocalizationEditorDB.mLanguageSource.GetTermData(Term);
			if (termData!=null)
			{
				eTermType NewType = (eTermType)EditorGUILayout.EnumPopup(termData.TermType, GUILayout.Width(90));
				if (termData.TermType != NewType)
					termData.TermType = NewType;
			}
			
			GUILayout.EndHorizontal();
			
			if (mAllowEditKeyName)
			{
				GUILayout.BeginHorizontal(GUILayout.Height (1));
				GUILayout.BeginHorizontal(EditorStyles.toolbar);
				if(mNewKeyName==null) mNewKeyName = string.Empty;
				mNewKeyName = EditorGUILayout.TextField(mNewKeyName, new GUIStyle("ToolbarSeachTextField"), GUILayout.ExpandWidth(true));

				LanguageSource.ValidateFullTerm( ref mNewKeyName );

				if (GUILayout.Button (string.Empty, string.IsNullOrEmpty(mNewKeyName) ? "ToolbarSeachCancelButtonEmpty" : "ToolbarSeachCancelButton", GUILayout.ExpandWidth(false)))
					mNewKeyName = string.Empty;

				GUILayout.EndHorizontal();

				GUI.enabled = (!string.IsNullOrEmpty(mNewKeyName));
				if (GUILayout.Button ("Create", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
				{
					mNewKeyName = mNewKeyName.Trim();
					Term = mNewKeyName;
					LocalizationEditorDB.mLanguageSource.AddTerm( mNewKeyName, eTermType.Text );
					mAllowEditKeyName = false;
					bChanged = true;
					GUIUtility.keyboardControl = 0;
				}
				GUI.enabled = (termData!=null && !string.IsNullOrEmpty(mNewKeyName));
				if (GUILayout.Button (new GUIContent("Rename","Renames the term in the source and updates every object using it in the current scene"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
				{
					mNewKeyName = mNewKeyName.Trim();
					Term = mNewKeyName;
					mAllowEditKeyName = false;
					bChanged = true;
					LocalizationEditor.TermReplacements = new Dictionary<string, string>();
					LocalizationEditor.TermReplacements[ termData.Term ] = mNewKeyName;
					termData.Term = mNewKeyName;
					LocalizationEditor.ReplaceTermsInCurrentScene();
					GUIUtility.keyboardControl = 0;
					//ParseTerms(true);
				}
				GUI.enabled = true;
				GUILayout.EndHorizontal();

				OnGUI_SelectKey_PreviewTerms ( ref Term, Terms);
			}
			
			GUILayout.Space (5);
			return bChanged;
		}

		void OnGUI_SelectKey_PreviewTerms ( ref string Term, List<string> Terms)
		{
			if (Terms.Count<=0)
				return;

			if (Terms.Count==1 && Terms[0]==Term)
				return;


			GUI.backgroundColor = Color.gray;
			GUILayout.BeginVertical ("AS TextArea");
			for (int i = 0, imax = Mathf.Min (Terms.Count, 3); i < imax; ++i) {
				int nUses = -1;
				LocalizationEditor.mParsedTerms.TryGetValue (Terms [i], out nUses);
				string FoundText = Terms [i];
				if (nUses > 0)
					FoundText = string.Concat ("(", nUses, ") ", FoundText);
				if (GUILayout.Button (FoundText, EditorStyles.miniLabel)) {
					mNewKeyName = Term = Terms [i];
					GUIUtility.keyboardControl = 0;
				}
			}
			if (Terms.Count > 3)
				GUILayout.Label ("...");
			GUILayout.EndVertical ();
			GUI.backgroundColor = Color.white;
		}

		#endregion

		#region Target

		void OnGUI_Target()
		{
			List<string> TargetTypes = new List<string>();
			int CurrentTarget = -1;

			Localize Loc = ((Localize)target);
			Loc.FindTarget();
			TestTargetType<GUIText>		( ref TargetTypes, "GUIText", ref CurrentTarget );
			TestTargetType<TextMesh>	( ref TargetTypes, "TextMesh", ref CurrentTarget );
			TestTargetType<AudioSource>	( ref TargetTypes, "AudioSource", ref CurrentTarget );
			TestTargetType<GUITexture>	( ref TargetTypes, "GUITexture", ref CurrentTarget );

			#if UGUI
			TestTargetType<UnityEngine.UI.Text>		( ref TargetTypes, "UGUI Text", ref CurrentTarget );
			TestTargetType<UnityEngine.UI.Image>	( ref TargetTypes, "UGUI Image", ref CurrentTarget );
			#endif

			#if NGUI
				TestTargetType<UILabel>		( ref TargetTypes, "NGUI UILabel", ref CurrentTarget );
				TestTargetType<UISprite>	( ref TargetTypes, "NGUI UISprite", ref CurrentTarget );
				TestTargetType<UITexture>	( ref TargetTypes, "NGUI UITexture", ref CurrentTarget );
			#endif

			#if DFGUI
				TestTargetType<dfButton>		( ref TargetTypes, "DFGUI Button", ref CurrentTarget );
				TestTargetType<dfLabel>			( ref TargetTypes, "DFGUI Label", ref CurrentTarget );
				TestTargetType<dfPanel>			( ref TargetTypes, "DFGUI Panel", ref CurrentTarget );
				TestTargetType<dfSprite>		( ref TargetTypes, "DFGUI Sprite", ref CurrentTarget );
				TestTargetType<dfRichTextLabel>	( ref TargetTypes, "DFGUI RichTextLabel", ref CurrentTarget );
			#endif

			#if TK2D
			TestTargetType<tk2dTextMesh>		( ref TargetTypes, "2DToolKit Label", ref CurrentTarget );
			TestTargetType<tk2dBaseSprite>		( ref TargetTypes, "2DToolKit Sprite", ref CurrentTarget );
			#endif

			#if TextMeshPro
			TestTargetType<TMPro.TextMeshPro>	( ref TargetTypes, "TextMeshPro Label", ref CurrentTarget );
			#endif

			
			TestTargetTypePrefab	( ref TargetTypes, "Prefab", ref CurrentTarget );

			if (CurrentTarget==-1)
			{
				CurrentTarget = TargetTypes.Count;
				TargetTypes.Add("None");
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label ("Target:", GUILayout.Width (60));
			GUI.changed = false;
			int index = EditorGUILayout.Popup(CurrentTarget, TargetTypes.ToArray());
			if (GUI.changed)
			{
				switch (TargetTypes[index])
				{
					case "GUIText" 				:  Loc.mTarget = Loc.GetComponent<GUIText>(); break;
					case "TextMesh" 			:  Loc.mTarget = Loc.GetComponent<TextMesh>(); break;
					case "AudioSource" 			:  Loc.mTarget = Loc.GetComponent<AudioSource>(); break;
					case "GUITexture" 			:  Loc.mTarget = Loc.GetComponent<GUITexture>(); break;
					
					#if UGUI
					case "UGUI Text" 			:  Loc.mTarget = Loc.GetComponent<UnityEngine.UI.Text>(); break;
					case "UGUI Image" 			:  Loc.mTarget = Loc.GetComponent<UnityEngine.UI.Image>(); break;
					#endif
					
					#if NGUI
					case "NGUI UILabel" 		:  Loc.mTarget = Loc.GetComponent<UILabel>(); break;
					case "NGUI UISprite" 		:  Loc.mTarget = Loc.GetComponent<UISprite>(); break;
					case "NGUI UITexture" 		:  Loc.mTarget = Loc.GetComponent<UITexture>(); break;
					#endif
					
					#if DFGUI
					case "DFGUI Button" 		:  Loc.mTarget = Loc.GetComponent<dfButton>(); break;
					case "DFGUI Label" 			:  Loc.mTarget = Loc.GetComponent<dfLabel>(); break;
					case "DFGUI Panel" 			:  Loc.mTarget = Loc.GetComponent<dfPanel>(); break;
					case "DFGUI Sprite" 		:  Loc.mTarget = Loc.GetComponent<dfSprite>(); break;
					case "DFGUI RichTextLabel" 	:  Loc.mTarget = Loc.GetComponent<dfRichTextLabel>(); break;
					#endif

					#if TK2D
					case "2DToolKit Label" 		:  Loc.mTarget = Loc.GetComponent<tk2dTextMesh>(); break;
					case "2DToolKit Sprite"		:  Loc.mTarget = Loc.GetComponent<tk2dBaseSprite>(); break;
					#endif

					#if TextMeshPro
					case "TextMeshPro Label" 	:  Loc.mTarget = Loc.GetComponent<TMPro.TextMeshPro>(); break;
					#endif

					case "Prefab" 				:  Loc.mTarget = Loc.transform.GetChild(0).gameObject; break;
				}
				Loc.FindTarget();
			}
			GUILayout.EndHorizontal();
		}

		void TestTargetType<T>( ref List<string> TargetTypes, string TypeName, ref int CurrentTarget ) where T : Component
		{
			Localize Loc = ((Localize)target);
			if (Loc.GetComponent<T>()==null)
				return;
			TargetTypes.Add(TypeName);

			if ((Loc.mTarget as T) != null)
				CurrentTarget = TargetTypes.Count-1;
		}

		void TestTargetTypePrefab( ref List<string> TargetTypes, string TypeName, ref int CurrentTarget )
		{
			Localize Loc = ((Localize)target);

			if (Loc.transform.childCount==0)
				return;

			TargetTypes.Add(TypeName);
			
			if ((Loc.mTarget as GameObject) != null)
				CurrentTarget = TargetTypes.Count-1;
		}

		#endregion

		#region Source

		void OnGUI_Source()
		{
			GUILayout.BeginHorizontal();
				
				if (GUILayout.Button("Open Source", EditorStyles.toolbarButton, GUILayout.Width (100)))
				{
					Selection.activeObject = LocalizationEditorDB.mLanguageSource;

					string sTerm, sSecondary;
					((Localize)target).GetFinalTerms( out sTerm, out sSecondary );
					if (GUI_SelectedTerm==1) sTerm = sSecondary;
					LocalizationEditor.mKeyToExplore = sTerm;
				}

				GUILayout.Space (2);

				GUILayout.BeginHorizontal(EditorStyles.toolbar);
					EditorGUI.BeginChangeCheck ();
					if (!((Localize)target).Source)
						GUI.contentColor = Color.Lerp (Color.gray, Color.yellow, 0.1f);
					LanguageSource NewSource = EditorGUILayout.ObjectField( LocalizationEditorDB.mLanguageSource, typeof(LanguageSource), true) as LanguageSource;
					GUI.contentColor = Color.white;
					if (EditorGUI.EndChangeCheck())
					{
						((Localize)target).Source = NewSource;
						SetLanguageSource();
					}
				GUILayout.EndHorizontal();

			GUILayout.EndHorizontal();
		}

		#endregion
	}
}