using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace I2.Loc
{
	// Code to Export all Languages in the DB to the TextAssets
	public partial class LocalizationEditorDB
	{		
		#region DB to NGUI
		
/*		public static void DB_to_TextAssets()
		{
			//--[ Get All Keys sorted ]----------------------------

			List<string> Keys = new List<string>();
			Keys.AddRange( mDB_KeysData.Keys );
			Keys.Sort();

			//--[ Export Languages ]-------------------------------
			
			int nLanguages = LocalizationEditorDB.mDB_Languages.Count;
			List<TextAsset> Assets = new List<TextAsset>();
			for (int i=0; i<nLanguages; ++i)
			{
				string sFileName = GetPathForLanguage(mDB_Languages[i]);
				
				TextAsset asset = ExportTextAsset( sFileName, i, ref Keys );
				if (asset!=null)
				{
					Assets.Add(asset);
					if (mLanguageSource.GetLanguageIndex(mDB_Languages[i])<0)
						mLanguageSource.AddLanguage(mDB_Languages[i], mDB_LanguageCodes[i], asset);
				}
			}
			AssetDatabase.Refresh ();
			
			//--[ Delete the Languages that are not included in the DB ]------

			bool AnyAssetWasDeleted = false;
			for (int i=mLanguageSource.mLanguages.Count-1; i>=0; --i)
				if (!Assets.Contains (mLanguageSource.mLanguages[i].Asset))
				{
					string sPath = AssetDatabase.GetAssetPath (mLanguageSource.mLanguages[i].Asset);
					AssetDatabase.DeleteAsset(sPath);
					mLanguageSource.mLanguages.RemoveAt(i);
					AnyAssetWasDeleted = true;
				}
			if (AnyAssetWasDeleted)
				AssetDatabase.Refresh();
			
			mNeedsToSynchronizeTextAssets = false;
		}
		
		static TextAsset ExportTextAsset( string AssetDataPath, int languageIndex, ref List<string> Keys )
		{
			string sPath = AssetDataPath;
			string Root = Application.dataPath;
			
			//--[ Remove "Assets" directory if its on both paths ]---------------
			if ( Root.Substring(Root.LastIndexOf('/')+1) == sPath.Substring(0, sPath.IndexOf('/')) )
				sPath = sPath.Substring(sPath.IndexOf('/')+1);
			
			sPath = Path.Combine(Root, sPath).Replace ("\\", "/");
			
			StringBuilder Builder = new StringBuilder();
			
			bool bFirst = true;

			if (mDB_LanguageCodes[languageIndex]!=GoogleTranslation.GetLanguageCode(mDB_Languages[languageIndex]))
			{
				Builder.Append("LanguageCode: ");
				Builder.Append(mDB_LanguageCodes[languageIndex]);
				bFirst = false;
			}

			for (int i=0, imax=Keys.Count; i<imax; ++i)
			{
				string Key = Keys[i];
				KeyData keyData = mDB_KeysData[Key];

				if (keyData.Languages.Length <= languageIndex)
					continue;

				if (!bFirst) 
					Builder.Append ("\n");
				else
					bFirst = false;
					
				if (keyData.TermType!=eTermType.Text)
					Builder.AppendFormat("[{0}]", keyData.TermType.ToString());

				Builder.Append( Key );
				Builder.Append( " = " );
				Builder.Append( LocalizationReader.EncodeString(keyData.Languages[languageIndex]) );
					
				if (!string.IsNullOrEmpty(keyData.Description))
				{
					Builder.Append( " // " );
					Builder.Append( LocalizationReader.EncodeString(keyData.Description) );
				}
			}
			
			File.WriteAllText( sPath, Builder.ToString(),System.Text.Encoding.UTF8);
			
			AssetDataPath = sPath.Substring( Application.dataPath.Length, sPath.Length-Application.dataPath.Length );
			AssetDataPath = "Assets" + AssetDataPath;
			
			AssetDatabase.ImportAsset( AssetDataPath );
			return (TextAsset)AssetDatabase.LoadAssetAtPath( AssetDataPath, typeof(TextAsset) );
		}*/
		
		#endregion
	}
}