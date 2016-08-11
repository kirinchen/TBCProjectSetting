using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace I2.Loc
{
	[CustomEditor(typeof(LanguageSource))]
	public partial class LocalizationEditor : Editor
	{
		#region Variables
		
		SerializedObject mSerializedObj_Source;

		static bool mIsParsing = false;  // This is true when the editor is opening several scenes to avoid reparsing objects

		#endregion
		
		#region Variables GUI
		
		GUIStyle Style_ToolBar_Big, Style_ToolBarButton_Big;
		
		public GUISkin CustomSkin;

		static Vector3 mScrollPos_Languages;
		static string mLanguages_NewLanguage = "";

		#endregion

		#region Inspector

		void OnEnable()
		{
			bool IsNewSource = (LocalizationEditorDB.mLanguageSource != (LanguageSource)target);
			LocalizationEditorDB.SetSource( (LanguageSource)target );
			mSerializedObj_Source = new SerializedObject( LocalizationEditorDB.mLanguageSource );
				
			if (!mIsParsing)
			{
				if (!string.IsNullOrEmpty(LocalizationEditorDB.mLanguageSource.Spreadsheet_LocalFileName))
					mSpreadsheetMode = eSpreadsheetMode.Local;
				else
					mSpreadsheetMode = eSpreadsheetMode.Google;

				mCurrentViewMode = (((LanguageSource)target).mLanguages.Count>0 ? eViewMode.Keys : eViewMode.Languages);

				if (IsNewSource || mSelectedCategories.Count==0)
				{
					mSelectedKeys.Clear ();
					mSelectedCategories.Clear();
					mSelectedCategories.AddRange( LocalizationEditorDB.GetCategories() );
					if (mSelectedScenes.Count==0)
						mSelectedScenes.Add (EditorApplication.currentScene);
				}
				ParseTerms(true);
			}
			UpgradeManager.EnablePlugins();
		}

		public override void OnInspectorGUI()
		{
			mIsParsing = false;
			mSerializedObj_Source.Update();

				InitializeStyles();

				GUILayout.Space(10);

				OnGUI_Main();

			mSerializedObj_Source.ApplyModifiedProperties();
		}

		/*void OnDisable()
		{
			if (!mIsParsing)
				mParsedTerms.Clear ();
		}*/


		#endregion
	}
}