using UnityEditor;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

namespace I2.Loc
{
	public partial class LocalizationEditor
	{
		#region Variables
		const string ScriptLocalizationFileName = "/I2/Localization/Scripts/ScriptLocalization.cs";
		#endregion
		
		#region GUI Generate Script
		
		void OnGUI_Tools_Script()
		{
			OnGUI_KeysList (false, 200, false);

			//GUILayout.Space (5);
			
			GUI.backgroundColor = Color.Lerp (Color.gray, Color.white, 0.2f);
			GUILayout.BeginVertical("AS TextArea", GUILayout.Height(1));
			GUI.backgroundColor = Color.white;
			
			EditorGUILayout.HelpBox("This tool creates the ScriptLocalization.cs with the selected terms.\nThis allows for Compile Time Checking on the used Terms referenced in scripts", UnityEditor.MessageType.Info);
			
			GUILayout.Space (5);

			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Build Script with Selected Terms"))
				EditorApplication.update += BuildScriptWithSelectedTerms;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.EndVertical();
		}
		
		#endregion

		#region Generate Script File
		
		void BuildScriptWithSelectedTerms()
		{
			EditorApplication.update -= BuildScriptWithSelectedTerms;

			StringBuilder sb = new System.Text.StringBuilder();

			sb.AppendLine("// This class is Auto-Generated by the Script Tool in the Language Source");
			sb.AppendLine("using UnityEngine;");
			sb.AppendLine("");
			sb.AppendLine("namespace I2.Loc");
			sb.AppendLine("{");
			sb.AppendLine("	public static class ScriptLocalization");
			sb.AppendLine("	{");
			sb.AppendLine("		public static string Get( string Term ) { return LocalizationManager.GetTermTranslation(Term); }");
			sb.AppendLine();

			BuildScriptWithSelectedTerms( sb );

			sb.AppendLine();
			sb.AppendLine("	}");
			sb.AppendLine("}");

			string ScriptFile = Application.dataPath + ScriptLocalizationFileName;
			System.IO.File.WriteAllText(ScriptFile, sb.ToString(), System.Text.Encoding.UTF8);
			AssetDatabase.ImportAsset("Assets"+ScriptLocalizationFileName);
		}

		void BuildScriptWithSelectedTerms( StringBuilder sb )
		{
			List<string> Categories = LocalizationEditorDB.GetCategories();
			foreach (string Category in Categories)
			{
				List<string> CategoryTerms = GetSelectedTermsInCategory(Category);
				if (CategoryTerms.Count<=0)
					continue;

				sb.AppendLine();
				if (Category != LocalizationEditorDB.EmptyCategory)
				{
					sb.AppendLine("		public static class " + Category);
					sb.AppendLine("		{");
				}

				BuildScriptCategory( sb, Category, CategoryTerms );

				if (Category != LocalizationEditorDB.EmptyCategory)
				{
					sb.AppendLine("		}");
				}
			}
		}

		List<string> GetSelectedTermsInCategory( string Category )
		{
			List<string> list = new List<string>();
			foreach (string FullKey in mSelectedKeys)
			{
				string categ =  LocalizationEditorDB.GetCategoryFromFullTerm(FullKey);
				if (categ == Category && ShouldShowTerm(FullKey))
					list.Add (LocalizationEditorDB.GetKeyFromFullTerm(FullKey));
			}

			return list;
		}

		void BuildScriptCategory( StringBuilder sb, string Category, List<string> Terms )
		{
			if (Category==LocalizationEditorDB.EmptyCategory)
			{
				for (int i=0; i<Terms.Count; ++i)
					sb.AppendLine("		public static string "+Terms[i]+" \t\t{ get{ return Get (\""+Terms[i]+"\"); } }");
			}
			else
			for (int i=0; i<Terms.Count; ++i)
			{
				sb.AppendLine("			public static string "+Terms[i]+" \t\t{ get{ return Get (\""+Category+"/"+Terms[i]+"\"); } }");
			}
		}

		#endregion
	}
}