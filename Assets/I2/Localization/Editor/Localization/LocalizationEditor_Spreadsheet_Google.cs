using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Spreadsheets;

using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace I2.Loc
{
	public class DumbSecurityCertificatePolicy 
	{
		public static bool Validator(object sender, X509Certificate certificate, X509Chain chain,SslPolicyErrors policyErrors) 
		{ 
			return true; 
		}
	}

	public partial class LocalizationEditor
	{
		#region Variables
		
		static string[] mGoogleSpreadsheets = new string[0];
		static string[] mGoogleSpreadsheetKeys = new string[0];

		string mGooglePassword ="";
		
		#endregion
		
		#region GUI
		
		void OnGUI_Spreadsheet_Google()
		{
			GUILayout.Space(20);
			
			OnGUI_GoogleCredentials();
			
			OnGUI_ShowMsg();
			
			GUILayout.Space(20);

			GUI.backgroundColor = Color.Lerp(Color.gray, Color.white, 0.5f);
			GUILayout.BeginVertical("AS TextArea", GUILayout.Height (1));
			GUI.backgroundColor = Color.white;
				GUILayout.Space(10);
				OnGUI_GoogleSpreadsheetsInGDrive();
				GUILayout.Space(10);
			GUILayout.EndVertical();

			GUI.changed = false;
			bool OpenDataSourceAfterExport = EditorPrefs.GetBool("I2Loc OpenDataSourceAfterExport", true);
			
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				OpenDataSourceAfterExport = GUILayout.Toggle(OpenDataSourceAfterExport, "Open Spreadsheet after Export");
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			if (GUI.changed)
			{
				GUI.changed = false;
				EditorPrefs.SetBool("I2Loc OpenDataSourceAfterExport", OpenDataSourceAfterExport);
			}

			GUILayout.Space(10);
		}
		
		void OnGUI_GoogleCredentials()
		{
			//--[ User Name ]------------------
			GUI.changed = false;
			
			string mGoogleUserName = EditorPrefs.GetString("I2Loc Google UserName", "you@gmail.com");
			bool mSavePassword = EditorPrefs.GetBool("I2Loc Google Save Password", false);
			if (mSavePassword)
				mGooglePassword = EditorPrefs.GetString("I2Loc Google Password", "");
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("User:",  GUILayout.Width(80));
			mGoogleUserName = EditorGUILayout.TextField(mGoogleUserName);
			GUILayout.EndHorizontal();
			
			//--[ Password ]------------------
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Password:", GUILayout.Width(80));
			mGooglePassword = EditorGUILayout.PasswordField(mGooglePassword);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			mSavePassword = GUILayout.Toggle (mSavePassword, "Save Password");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			if (GUI.changed)
			{
				EditorPrefs.SetBool("I2Loc Google Save Password", mSavePassword);
				EditorPrefs.SetString("I2Loc Google UserName", mGoogleUserName);
				if (mSavePassword)
					EditorPrefs.SetString("I2Loc Google Password", mGooglePassword);
				
				// If the credentials have changed then clear the cached spreadsheet keys
				mGoogleSpreadsheets = new string[0];
				mGoogleSpreadsheetKeys = new string[0];
				
				GUI.changed = false;
				ClearErrors();
			}
		}
		
		void OnGUI_GoogleSpreadsheetsInGDrive()
		{
			SerializedProperty Prop_SpreadsheetKey = mSerializedObj_Source.FindProperty("Google_SpreadsheetKey");

			int mSpreadsheetIndex = System.Array.IndexOf(mGoogleSpreadsheetKeys, Prop_SpreadsheetKey.stringValue);

			//--[ Spreadsheets ]------------------
			GUILayout.BeginHorizontal();
				GUILayout.Space(10);
				GUILayout.Label ("In Google Drive:", GUILayout.Width(100));
				
				mSpreadsheetIndex = EditorGUILayout.Popup(mSpreadsheetIndex, mGoogleSpreadsheets,EditorStyles.toolbarPopup);
				if (mSpreadsheetIndex>=0)
					Prop_SpreadsheetKey.stringValue = mGoogleSpreadsheetKeys[mSpreadsheetIndex];

				if (GUILayout.Button("Refresh", EditorStyles.toolbarButton,GUILayout.ExpandWidth(false)))
					Google_FindSpreadsheets();

				GUILayout.Space(10);
			GUILayout.EndHorizontal();

			GUILayout.Space(5);

			GUILayout.BeginHorizontal();
				GUILayout.Space(10);
				GUILayout.Label ("Key:",  GUILayout.Width(100));

				string s = EditorGUILayout.TextField( Prop_SpreadsheetKey.stringValue );
				if (s!=Prop_SpreadsheetKey.stringValue)
					Prop_SpreadsheetKey.stringValue = s;

				GUILayout.Space(10);
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
			
			OnGUI_GoogleButtons_ImportExport( Prop_SpreadsheetKey.stringValue );
		}

		string GetGoogle_SpreadsheetFullKey( string Key )
		{
			if (string.IsNullOrEmpty(Key))
				return string.Empty;
			else
				return string.Format("https://spreadsheets.google.com/feeds/worksheets/{0}/private/full", Key);
		}

		string GetGoogle_SpreadsheetKeyFromURL( string KeyURL )
		{
			// get the characters in {0} from this Format or a similar one: 
			// https://spreadsheets.google.com/feeds/worksheets/{0}/private/full
			
			int iSheet = KeyURL.IndexOf("worksheets/");
			if (iSheet>=0)
			{
				iSheet += "worksheets/".Length;
				int iLast = KeyURL.IndexOf('/', iSheet);
				if (iLast<0) iLast = KeyURL.Length;
				return KeyURL.Substring(iSheet, iLast-iSheet);
			}
			return string.Empty;
		}
		

		void OnGUI_GoogleButtons_ImportExport( string SpreadsheetKey )
		{
			GUI.enabled = !string.IsNullOrEmpty(SpreadsheetKey);

			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();

				if (GUILayout.Button( "Import", GUILayout.Width(150))) 
				{
					ClearErrors();
					mSerializedObj_Source.ApplyModifiedProperties();

					string error = LocalizationEditorDB.Import_Google( SpreadsheetKey );
					if (!string.IsNullOrEmpty(error))
						ShowError(error);
					else 
						ParseTerms(true);
					mSerializedObj_Source.Update();
					#if UNITY_EDITOR
					UnityEditor.EditorUtility.SetDirty (this);
					#endif
				}

				GUILayout.FlexibleSpace();

				if (GUILayout.Button( "Export", GUILayout.Width(150))) 
				{
					ClearErrors();
					string error = LocalizationEditorDB.Export_Google( SpreadsheetKey );
					if (!string.IsNullOrEmpty(error))
						ShowError(error);
				}

				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUI.enabled = true;
		}
		#endregion
		
		
		
		#region Google FindSpreadsheets
		
		void Google_FindSpreadsheets()
		{
			try
			{
				SpreadsheetsService mGoogleService = Google_CreateService();
				
				SpreadsheetQuery query = new SpreadsheetQuery();
				SpreadsheetFeed feed = mGoogleService.Query(query);
				
				List<string> lSpreadsheets = new List<string>();
				List<string> lKeys = new List<string>();
				foreach (SpreadsheetEntry entry in feed.Entries)
				{
					lSpreadsheets.Add(entry.Title.Text);
					string sKey = GetGoogle_SpreadsheetKeyFromURL( entry.Links.FindService(GDataSpreadsheetsNameTable.WorksheetRel, null).HRef.ToString() );
					lKeys.Add (sKey );
				}
				
				mGoogleSpreadsheets = lSpreadsheets.ToArray();
				mGoogleSpreadsheetKeys = lKeys.ToArray();

				SerializedProperty Prop_SpreadsheetKey = mSerializedObj_Source.FindProperty("Google_SpreadsheetKey");
				if (string.IsNullOrEmpty(Prop_SpreadsheetKey.stringValue) && lSpreadsheets.Count>=0)
					Prop_SpreadsheetKey.stringValue = lKeys[0];
			}
			#if UNITY_WEBPLAYER
			catch(Exception)
			{
				ShowError ("Contacting google is not yet supported on WebPlayer" );
			}
			#else
			catch(Exception e)
			{
				ShowError (e.Message);
			}
			#endif
			
		}

		public static SpreadsheetsService Google_CreateService()
		{
			string mGoogleUserName = EditorPrefs.GetString("I2Loc Google UserName", "you@gmail.com");
			string mGooglePassword = EditorPrefs.GetString("I2Loc Google Password", "");
			
			ServicePointManager.ServerCertificateValidationCallback = DumbSecurityCertificatePolicy.Validator;
			SpreadsheetsService mGoogleService = new SpreadsheetsService("exampleCo-exampleApp-1");
			
			mGoogleService.setUserCredentials( mGoogleUserName, mGooglePassword);
			string Token = mGoogleService.QueryClientLoginToken();
			mGoogleService.SetAuthenticationToken( Token );

			return mGoogleService;
		}

		#endregion
	}
}