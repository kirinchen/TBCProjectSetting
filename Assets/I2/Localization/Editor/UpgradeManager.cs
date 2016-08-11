using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace I2.Loc
{
	public class UpgradeManager : AssetPostprocessor 
	{
		static void OnPostprocessAllAssets( string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths )
		{
			EditorApplication.delayCall += CheckPlugins;
		}
		
		static void CheckPlugins()
		{
			EditorApplication.delayCall -= CheckPlugins;
			
			EnablePlugins();			
		}

		[MenuItem( "Tools/I2 Localization/Enable Plugins", false, 0 )]
		public static void EnablePlugins()
		{
			foreach (BuildTargetGroup target in System.Enum.GetValues(typeof(BuildTargetGroup)))
			{
				EnablePlugins( target );
			}
		}

		static void EnablePlugins( BuildTargetGroup Platform )
		{
			string Settings = PlayerSettings.GetScriptingDefineSymbolsForGroup(Platform );
			
			bool HasChanged = false;
			List<string> symbols = new List<string>( Settings.Split(';'));

			HasChanged |= UpdateSettings("NGUI",  "NGUIDebug",  	  		 "", ref symbols);
			HasChanged |= UpdateSettings("DFGUI", "dfPanel", 	  		 "", ref symbols);
			//HasChanged |= UpdateSettings("UGUI",  "UnityEngine.UI.Text", "UnityEngine.UI", ref symbols);
			HasChanged |= UpdateSettings("TK2D",  "tk2dTextMesh", 		 "", ref symbols);
			HasChanged |= UpdateSettings("TextMeshPro",  "TMPro.TextMeshPro", 		 "", ref symbols);

			if (HasChanged)
			{
				Settings = string.Empty;
				for (int i=0,imax=symbols.Count; i<imax; ++i)
				{
					if (i>0) Settings += ";";
					Settings += symbols[i];
				}
				PlayerSettings.SetScriptingDefineSymbolsForGroup(Platform, Settings );
			}
		}
		
		static bool UpdateSettings( string mPlugin, string mType, string AssemblyType, ref List<string> symbols)
		{
			bool hasPluginClass = false;
			System.Reflection.Assembly assembly = typeof(Localize).Assembly;//(string.IsNullOrEmpty(AssemblyType) ? typeof(Localize).Assembly : System.Reflection.Assembly.Load(AssemblyType));
			if (assembly!=null && assembly.GetType(mType)!=null)
				hasPluginClass = true;

			bool hasPluginDef = (symbols.IndexOf(mPlugin)>=0);
			
			if (hasPluginClass != hasPluginDef)
			{
				if (hasPluginClass) symbols.Add(mPlugin);
				else symbols.Remove(mPlugin);
				return true;
			}
			return false;
			
		}
	}
}