﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace I2.Loc
{
	public partial class LocalizationEditor
	{

		#region Variables

		public enum eViewMode { ImportExport, Keys, Languages, Tools, References };
		public static eViewMode mCurrentViewMode = eViewMode.Keys;
		
		enum eSpreadsheetMode { Local, Google };
		eSpreadsheetMode mSpreadsheetMode = eSpreadsheetMode.Google;


		public static string mLocalizationMsg = "";
		public static MessageType mLocalizationMessageType = MessageType.None;

		#endregion

		#region Editor

		/*[MenuItem("Window/Localization", false)]
		public static void OpenLocalizationEditor()
		{
			EditorWindow.GetWindow<LocalizationEditor>(false, "Localization", true);
		}*/

		#endregion

		#region GUI

		void InitializeStyles()
		{
			Style_ToolBar_Big = new GUIStyle(EditorStyles.toolbar);
			Style_ToolBar_Big.fixedHeight = Style_ToolBar_Big.fixedHeight*1.5f;

			Style_ToolBarButton_Big = new GUIStyle(EditorStyles.toolbarButton);
			Style_ToolBarButton_Big.fixedHeight = Style_ToolBarButton_Big.fixedHeight*1.5f;
		}


		void OnGUI_Main()
		{
			GUILayout.BeginHorizontal();
				//OnGUI_ToggleEnumBig( "Spreadsheets", ref mCurrentViewMode, eViewMode.ImportExport, GUI.skin.GetStyle("CN EntryWarn").normal.background, "External Spreadsheet File or Service" );
				OnGUI_ToggleEnumBig( "Spreadsheets", ref mCurrentViewMode, eViewMode.ImportExport, null, "External Spreadsheet File or Service" );
				OnGUI_ToggleEnumBig( "Terms", ref mCurrentViewMode, eViewMode.Keys, null, null );
				OnGUI_ToggleEnumBig( "Languages", ref mCurrentViewMode, eViewMode.Languages, null, null );
				OnGUI_ToggleEnumBig( "Tools", ref mCurrentViewMode, eViewMode.Tools, null, null );
				OnGUI_ToggleEnumBig( "Assets", ref mCurrentViewMode, eViewMode.References, null, null );
			GUILayout.EndHorizontal();
			//GUILayout.Space(10);

			switch (mCurrentViewMode)
			{
				case eViewMode.ImportExport 			: OnGUI_ImportExport(); break;
				case eViewMode.Keys 					: OnGUI_KeysList(); break;
				case eViewMode.Languages 				: OnGUI_Languages(); break;
				case eViewMode.Tools 					: OnGUI_Tools(); break;
				case eViewMode.References 				: OnGUI_References(); break;
			}
		}

		void OnGUI_ImportExport()
		{
			eSpreadsheetMode OldMode = mSpreadsheetMode;
			mSpreadsheetMode = (eSpreadsheetMode)GUITools.DrawShadowedTabs ((int)mSpreadsheetMode, new string[]{"Local", "Google"});
			if (mSpreadsheetMode != OldMode)
				ClearErrors();

			GUITools.BeginContents();
			switch (mSpreadsheetMode)
			{
				case eSpreadsheetMode.Local 	: OnGUI_Spreadsheet_Local();  break;
				case eSpreadsheetMode.Google	: OnGUI_Spreadsheet_Google(); break;
			}
			GUITools.EndContents(false);
		}

		void OnGUI_References()
		{
			EditorGUILayout.HelpBox("These are the assets that are referenced by the Terms and not in the Resources folder", MessageType.Info);
			SerializedProperty Prop = mSerializedObj_Source.FindProperty("Assets");
			GUITools.DrawObjectsArray( Prop );
		}	

		#endregion

		#region Misc

		void OnGUI_ToggleEnumBig<Enum>( string text, ref Enum currentMode, Enum newMode, Texture texture, string tooltip) 	{ OnGUI_ToggleEnum<Enum>( text, ref currentMode, newMode, texture, tooltip, Style_ToolBarButton_Big); }
		void OnGUI_ToggleEnumSmall<Enum>( string text, ref Enum currentMode, Enum newMode, Texture texture, string tooltip) { OnGUI_ToggleEnum<Enum>( text, ref currentMode, newMode, texture, tooltip, EditorStyles.toolbarButton); }
		void OnGUI_ToggleEnum<Enum>( string text, ref Enum currentMode, Enum newMode, Texture texture, string tooltip, GUIStyle style)
		{
			GUI.changed = false;
			if (GUILayout.Toggle( currentMode.Equals(newMode), new GUIContent(text, texture, tooltip), style, GUILayout.ExpandWidth(true)))
			{ 
				currentMode = newMode;
				if (GUI.changed)
					ClearErrors();
			}			
		}
		
		int OnGUI_FlagToogle( string Text, string tooltip, int flags, int bit )
		{
			bool State = ((flags & bit)>0);
			bool NewState = GUILayout.Toggle(State, new GUIContent(Text, tooltip), "toolbarbutton");
			if (State!=NewState)
			{
				if (!NewState && flags==bit)
					return flags;
				
				flags = (NewState ? (flags | bit)  : (flags & ~bit));
			}
			
			return flags;
		}
		
		void OnGUI_SelectableToogleListItem( string Element, ref List<string> Selections, string Style )
		{
			bool WasEnabled = Selections.Contains(Element);
			bool IsEnabled = GUILayout.Toggle( WasEnabled, "", Style, GUILayout.ExpandWidth(false) );
			
			if (IsEnabled && !WasEnabled)
				Selections.Add(Element);
			else
				if (!IsEnabled && WasEnabled)
					Selections.Remove(Element);
		}

		static bool RemoveResourcesPath( ref string sPath )
		{
			int Ind1 = sPath.IndexOf("\\Resources\\");
			int Ind2 = sPath.IndexOf("\\Resources/");
			int Ind3 = sPath.IndexOf("/Resources\\");
			int Ind4 = sPath.IndexOf("/Resources/");
			int Index = Mathf.Max (Ind1, Ind2, Ind3, Ind4);
			bool IsResource = false;
			if (Index>=0)
			{
				sPath = sPath.Substring(Index+11);
				IsResource = true;
			}
			else
			{
				// If its not in the Resources, then it has to be in the References
				// Therefore, the path has to be stripped and let only the name
				Index = sPath.LastIndexOfAny("\\/".ToCharArray());
				if (Index>0)
					sPath = sPath.Substring(Index+1);
			}

			string Extension = System.IO.Path.GetExtension(sPath);
			if (!string.IsNullOrEmpty(Extension))
				sPath = sPath.Substring(0, sPath.Length-Extension.Length);

			return IsResource;
		}

		#endregion

		#region Error Management
		
		static void OnGUI_ShowMsg()
		{
			if (!string.IsNullOrEmpty(mLocalizationMsg))
			{
				EditorGUILayout.HelpBox(mLocalizationMsg, mLocalizationMessageType);
			}
		}
		
		static void ShowError  ( string Error ) { ShowMessage ( Error, MessageType.Error ); }
		static void ShowInfo   ( string Msg   ) { ShowMessage ( Msg, MessageType.Info ); }
		static void ShowWarning( string Msg   ) { ShowMessage ( Msg, MessageType.Warning ); }
		
		static void ShowMessage( string Msg, MessageType msgType )
		{
			mLocalizationMsg = Msg;
			mLocalizationMessageType = msgType;
			switch (msgType)
			{
				case MessageType.Error 	 : Debug.LogError(Msg); break;
				case MessageType.Warning : Debug.LogWarning(Msg); break;
				default 	 			 : Debug.Log(Msg); break;
			}
		}
		
		
		static void ClearErrors()
		{
			mLocalizationMsg = "";
		}
		
		#endregion
	}
}