using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Spreadsheets;

namespace I2.Loc
{	
	public partial class LocalizationEditorDB
	{
		#region Variables
		
		#endregion

		#region  Import Spreadsheet
		
		public static string Import_Google( string SpreadsheetKey ) // returns the error string or empty if there is no error
		{
			try
			{
				SpreadsheetsService mGoogleService = LocalizationEditor.Google_CreateService();
				
				//--[ Get Spreadsheet ]-------------------
				SpreadsheetQuery querySpreadsheet = new SpreadsheetQuery("https://spreadsheets.google.com/feeds/spreadsheets/private/full/"+SpreadsheetKey);
				SpreadsheetFeed feedSpreadsheet = mGoogleService.Query(querySpreadsheet);

				if (feedSpreadsheet==null || feedSpreadsheet.Entries.Count==0)
					return "Spreadsheet not found";
				
				SpreadsheetEntry Spreadsheet = (SpreadsheetEntry)feedSpreadsheet.Entries[0];

				//--[ Import Data ]----------------------

				List<string> ImportedLanguages = new List<string>();
				mLanguageSource.mTerms.Clear();

				for (int i=0, imax=Spreadsheet.Worksheets.Entries.Count; i<imax; ++i)
				{
					//--[ Read Cells ]-------------

					WorksheetEntry Worksheet = (WorksheetEntry)Spreadsheet.Worksheets.Entries[i];
					AtomLink cellFeedLink = Worksheet.Links.FindService(GDataSpreadsheetsNameTable.CellRel, null);
					CellQuery query3 = new CellQuery(cellFeedLink.HRef.ToString());
					CellFeed feed3 = mGoogleService.Query(query3);

					Import_Google_FromWorksheet( feed3, Worksheet.Title.Text, ImportedLanguages );
				}

				//--[ Delete languages not imported ]--------------
				for (int i=mLanguageSource.mLanguages.Count-1; i>=0; --i)
					if (!ImportedLanguages.Contains(mLanguageSource.mLanguages[i].Name))
					{
						mLanguageSource.RemoveLanguage(mLanguageSource.mLanguages[i].Name);
					}

				LocalizationEditor.mSelectedCategories = LocalizationEditorDB.GetCategories();
				return string.Empty;
			}
			#if UNITY_WEBPLAYER
			catch(Exception)
			{
				return "Contacting google is not yet supported on WebPlayer";
			}
			#else
			catch(Exception e)
			{
				Debug.LogError(e.Message);
				return "Import Failed, Check the console for more info";
			}
			#endif
		}

		static void Import_Google_FromWorksheet( CellFeed cellFeed, string Category, List<string> ImportedLanguages )
		{
			//--[ Find Sheet Size ]-----------------
			int nColumns=0, nRows=0;
			
			foreach (CellEntry curCell in cellFeed.Entries)
			{
				if (nColumns < curCell.Column) nColumns = (int)curCell.Column;
				if (nRows < curCell.Row) 	   nRows 	= (int)curCell.Row;
			}
			

			//mLanguages = new string[nColumns];
			//mDB_Keys = new string[nRows-1]; // don't dount the header row

			//--[ cellFeeds.Entries could be unsorted so move all that into a temporal array ]-----------
			string[,] Cells = new string[nColumns, nRows];
			foreach (CellEntry curCell in cellFeed.Entries)
			{
				Cells[curCell.Column-1, curCell.Row-1] = curCell.Value ?? string.Empty;
			}
			
			//--[ Create existing languages ]------------
			for (int i=3; i<nColumns; ++i)
				if (!string.IsNullOrEmpty(Cells[i,0]))
				{
					string lan, cod;
					GoogleTranslation.UnPackCodeFromLanguageName(Cells[i,0], out lan, out cod);
					mLanguageSource.AddLanguage(lan, cod);
					if (!ImportedLanguages.Contains(lan))
						ImportedLanguages.Add(lan);
				}

			
			//--[ Create Keys ]------------

			for (int row=1; row<nRows; ++row)
			{
				string key;
				if (Category=="Default") 
					key = Cells[0, row] ?? string.Empty;
				else 
					key = string.Concat(Category, "/", Cells[0, row] ?? string.Empty);

				LanguageSource.ValidateFullTerm( ref key );
				TermData termData = mLanguageSource.AddTerm (key);

				termData.TermType = GetTermType( Cells[1, row] ?? string.Empty );
				termData.Description = Cells[2, row] ?? string.Empty;

				// Translations
				for (int i=3; i<nColumns; ++i)
				{
					string CellValue = Cells[i,0] ?? string.Empty;
					string lan, cod;
					GoogleTranslation.UnPackCodeFromLanguageName(CellValue, out lan, out cod);

					int Index = mLanguageSource.GetLanguageIndex(lan);
					if (Index>=0)  // if language exists
						termData.Languages[ Index ] = Cells[i,row] ?? string.Empty;
				}
			}
		}
		
		#endregion
	}
}
