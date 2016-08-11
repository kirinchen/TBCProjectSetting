using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace I2.Loc
{
	
	public partial class LocalizationEditor
	{
		#region Variables

		public static Dictionary<string, int> mParsedTerms = new Dictionary<string,int>(); // All Terms resulted from parsing the scenes and collecting the Localize.Term and how many times the terms are used

		#endregion
		
		#region GUI Parse Keys
		
		void OnGUI_Tools_ParseTerms()
		{
			OnGUI_ScenesList();

			GUI.backgroundColor = Color.Lerp (Color.gray, Color.white, 0.2f);
			GUILayout.BeginVertical("AS TextArea", GUILayout.Height(1));
			GUI.backgroundColor = Color.white;

			GUILayout.Space (5);

				EditorGUILayout.HelpBox("This tool searches all Terms used in the selected scenes and updates the usage counter in the Terms Tab", UnityEditor.MessageType.Info);

				GUILayout.Space (5);

				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace();
					if (GUILayout.Button("Parse Localized Terms"))
						EditorApplication.update += ParseTermsInSelectedScenes;
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

			GUILayout.EndVertical();
		}
		
		#endregion


		#region ParseKeys

		void ParseTermsInSelectedScenes()
		{
			EditorApplication.update -= ParseTermsInSelectedScenes;
			ParseTerms(false);
		}
		
		void ParseTerms( bool OnlyCurrentScene, bool OpenTermsTab = true )
		{ 
			mIsParsing = true;

			mParsedTerms.Clear();
			mSelectedKeys.Clear ();
			
			if (!OnlyCurrentScene)
				ExecuteActionOnSelectedScenes( FindTermsInCurrentScene );
			else 
				FindTermsInCurrentScene();
			
			FindTermsNotUsed();
			
			if (mParsedTerms.Count<=0)
			{
				ShowInfo ("No terms where found during parsing");
				return;
			}

			if (OpenTermsTab) 
			{
				mFlagsViewKeys = ((int)eFlagsViewKeys.Used | (int)eFlagsViewKeys.NotUsed | (int)eFlagsViewKeys.Missing);
				mCurrentViewMode = eViewMode.Keys;
			}
			mIsParsing = false;
		}
		
		void FindTermsInCurrentScene()
		{
			Localize[] Locals = (Localize[])Resources.FindObjectsOfTypeAll(typeof(Localize));
			
			if (Locals==null)
				return;
			
			for (int i=0, imax=Locals.Length; i<imax; ++i)
			{
				Localize localize = Locals[i];
				if (localize==null || localize.gameObject==null || !GUITools.ObjectExistInScene(localize.gameObject))
					continue;
				
				string Term, SecondaryTerm;
				Term = localize.Term;
				SecondaryTerm = localize.SecondaryTerm;
				//localize.GetFinalTerms( out Term, out SecondaryTerm );

				AddTermToParsedList(Term);
				AddTermToParsedList(SecondaryTerm);
			}
		}

		void AddTermToParsedList( string Term )
		{
			if (string.IsNullOrEmpty(Term))
				return;

			if (mParsedTerms.ContainsKey(Term))
				mParsedTerms[Term]++;
			else
				mParsedTerms[Term] = 1;
		}
		
		void FindTermsNotUsed()
		{
			// every Term that is in the DB but not in mParsedTerms

			foreach (TermData termData in LocalizationEditorDB.mLanguageSource.mTerms)
			{
				if ( !mParsedTerms.ContainsKey(termData.Term) )
					mParsedTerms[termData.Term] = 0;
			}
		}

		#endregion
	}
}
