﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace I2.Loc
{
	public partial class LocalizationEditor
	{

		#region Variables
		
		Vector2 mScrollPos_BuildScenes = Vector2.zero;
		
		static List<string> mSelectedScenes = new List<string>();

		public enum eToolsMode { Parse, Categorize, Merge, NoLocalized, Script };
		public eToolsMode mCurrentToolsMode = eToolsMode.Parse;
		
		#endregion

		#region GUI

		void OnGUI_Tools()
		{
			eToolsMode OldMode = mCurrentToolsMode;
			mCurrentToolsMode = (eToolsMode)GUITools.DrawShadowedTabs ((int)mCurrentToolsMode, new string[]{"Parse", "Categorize", "Merge", "No Localized", "Script"});
			if (mCurrentToolsMode!=OldMode)
				ClearErrors();

			switch (mCurrentToolsMode)
			{
				case eToolsMode.Parse 		: OnGUI_Tools_ParseTerms(); break;
				case eToolsMode.Categorize 	: OnGUI_Tools_Categorize(); break;
				case eToolsMode.Merge 		: OnGUI_Tools_MergeTerms(); break;
				case eToolsMode.NoLocalized : OnGUI_Tools_NoLocalized(); break;
			 	case eToolsMode.Script		: OnGUI_Tools_Script(); break;
			}
		}

		#endregion
	}
}