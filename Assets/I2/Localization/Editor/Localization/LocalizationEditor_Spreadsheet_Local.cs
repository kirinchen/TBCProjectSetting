using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace I2.Loc
{
	public partial class LocalizationEditor
	{
		enum eLocalSpreadsheeet { CSV, XLS, XLSX, NONE };

		void OnGUI_Spreadsheet_Local()
		{
			SerializedProperty Prop_LocalFileName = mSerializedObj_Source.FindProperty("Spreadsheet_LocalFileName");

			GUILayout.Space(10);
			GUILayout.BeginVertical();

				GUILayout.BeginHorizontal();
					GUILayout.Label ("File:", GUILayout.ExpandWidth(false));

					Prop_LocalFileName.stringValue = EditorGUILayout.TextField(Prop_LocalFileName.stringValue);
					/*if (GUILayout.Button("...", "toolbarbutton", GUILayout.ExpandWidth(false)))
					{
						string sFileName = Prop_LocalFileName.stringValue;

						string sPath = string.Empty;
						try {
						sPath = System.IO.Path.GetDirectoryName(sFileName);
						}
						catch( System.Exception e){}

						if (string.IsNullOrEmpty(sPath))
							sPath = Application.dataPath + "/";

						sFileName = System.IO.Path.GetFileName(sFileName);
						if (string.IsNullOrEmpty(sFileName))
							sFileName = "Localization.csv";

						string FullFileName = EditorUtility.SaveFilePanel("Select CSV File", sPath, sFileName, "csv");
						//string FullFileName = EditorUtility.OpenFilePanel("Select CSV,  XLS or XLSX File", sFileName, "csv;*.xls;*.xlsx");

						if (!string.IsNullOrEmpty(FullFileName))
						{
							Prop_LocalFileName.stringValue = TryMakingPathRelativeToProject(FullFileName);
						}
					}*/
				GUILayout.EndHorizontal();

				//--[ Find current extension ]---------------
				eLocalSpreadsheeet CurrentExtension = eLocalSpreadsheeet.NONE;
				//string FileNameLower = Prop_LocalFileName.stringValue.ToLower();
				/*if (FileNameLower.EndsWith(".csv"))  */CurrentExtension = eLocalSpreadsheeet.CSV;
			    /*if (FileNameLower.EndsWith(".xls"))  CurrentExtension = eLocalSpreadsheeet.XLS;
			    if (FileNameLower.EndsWith(".xlsx")) CurrentExtension = eLocalSpreadsheeet.XLSX;*/

				GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					switch (CurrentExtension)
					{
						case eLocalSpreadsheeet.NONE :
								{
									string 	FileTypesDesc = "Select or Drag any file of the following types:\n\n";
											FileTypesDesc+= "*.csv  (Comma Separated Values)";
											//FileTypesDesc+= "\n*.xls  (Excel 97-2003)";
											//FileTypesDesc+= "\n*.xlsx (Excel Open XML format)";
									EditorGUILayout.HelpBox(FileTypesDesc, MessageType.None);
								}
								break;

						case eLocalSpreadsheeet.CSV 	: EditorGUILayout.HelpBox("Comma Separated Values", MessageType.None); break;
						case eLocalSpreadsheeet.XLS 	: EditorGUILayout.HelpBox("Excel 97-2003", MessageType.None); break;
						case eLocalSpreadsheeet.XLSX 	: EditorGUILayout.HelpBox("Excel Open XML format", MessageType.None); break;
					}
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

			GUILayout.EndVertical();

			//--[ Allow Dragging files ]-----------------
			if (GUILayoutUtility.GetLastRect().Contains (Event.current.mousePosition) && IsValidDraggedLoadSpreadsheet())
			{
				if (Event.current.type == EventType.DragUpdated)
					DragAndDrop.visualMode = DragAndDropVisualMode.Link;
				
				if (Event.current.type == EventType.DragPerform)
				{
					Prop_LocalFileName.stringValue = TryMakingPathRelativeToProject( DragAndDrop.paths[0] );
					DragAndDrop.AcceptDrag();
					Event.current.Use();
				}
			}

			GUILayout.Space(10);

			OnGUI_Spreadsheet_Local_ImportExport( CurrentExtension, Prop_LocalFileName.stringValue );

			OnGUI_ShowMsg();
		}

		bool IsValidDraggedLoadSpreadsheet()
		{
			if (DragAndDrop.paths==null || DragAndDrop.paths.Length!=1)
				return false;

			string sPath = DragAndDrop.paths[0].ToLower();
			if (sPath.EndsWith(".csv")) return true;
			else return false;
			//if (sPath.EndsWith(".xls")) return true;
			//if (sPath.EndsWith(".xlsx")) return true;

			/*int iChar = sPath.LastIndexOfAny( "/\\.".ToCharArray() );
			if (iChar<0 || sPath[iChar]!='.')
				return true;
			return false;*/
		}

		string TryMakingPathRelativeToProject( string FileName )
		{
			string ProjectPath = Application.dataPath.ToLower();
			string FileNameLower = FileName.ToLower();

			if (FileNameLower.StartsWith(ProjectPath))
				FileName = FileName.Substring(ProjectPath.Length+1);
			else
			if (FileNameLower.StartsWith("assets/"))
				FileName = FileName.Substring("assets/".Length);
			return FileName;
		}

		void OnGUI_Spreadsheet_Local_ImportExport( eLocalSpreadsheeet CurrentExtension, string File )
		{
			GUI.enabled = (CurrentExtension!=eLocalSpreadsheeet.NONE);
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			
			if (GUILayout.Button( "Import", GUILayout.Width(150)))
			{
				try
				{
					mSerializedObj_Source.ApplyModifiedProperties();
					mSerializedObj_Source.Update();
					LocalizationEditor.ClearErrors();

					if (string.IsNullOrEmpty(File))
						File = Application.dataPath + "/Localization.csv";
					else
					if (!System.IO.Path.IsPathRooted(File))
						File = string.Concat(Application.dataPath, "/", File);

					File = EditorUtility.OpenFilePanel("Select CSV,  XLS or XLSX File", File, "csv;*.xls;*.xlsx");
					if (!string.IsNullOrEmpty(File))
					{
						LocalizationEditorDB.mLanguageSource.Spreadsheet_LocalFileName = TryMakingPathRelativeToProject(File);
						switch (CurrentExtension)
						{
							case eLocalSpreadsheeet.CSV : LocalizationEditorDB.Import_CSV(File); break;
						}
						ParseTerms(true);
						#if UNITY_EDITOR
							UnityEditor.EditorUtility.SetDirty (this);
						#endif
					}
				}
				catch (System.Exception ex) 
				{ 
					LocalizationEditor.ShowError("Unable to import file");
					Debug.LogError(ex.Message); 
				}
			}
			
			GUILayout.FlexibleSpace();
			
			if (GUILayout.Button( "Export", GUILayout.Width(150))) 
			{
				try
				{
					LocalizationEditor.ClearErrors();

					string sPath = string.Empty;
					if (!System.IO.Path.IsPathRooted(File))
						File = string.Concat(Application.dataPath, "/", File);

					try {
						sPath = System.IO.Path.GetDirectoryName(File);
					}
					catch( System.Exception){}
					
					if (string.IsNullOrEmpty(sPath))
						sPath = Application.dataPath + "/";
					
					File = System.IO.Path.GetFileName(File);
					if (string.IsNullOrEmpty(File))
						File = "Localization.csv";
					
					File = EditorUtility.SaveFilePanel("Select CSV", sPath, File, "csv");
					if (!string.IsNullOrEmpty(File))
					{
						LocalizationEditorDB.mLanguageSource.Spreadsheet_LocalFileName = TryMakingPathRelativeToProject(File);

						switch (CurrentExtension)
						{
							case eLocalSpreadsheeet.CSV : LocalizationEditorDB.Export_CSV(File); break;
						}
					}
				}
				catch (System.Exception)
				{
					LocalizationEditor.ShowError("Unable to export file\nCheck it is not READ-ONLY and that\nits not opened in an external viewer");
				}

			}
			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUI.enabled = true;
		}
	}
}