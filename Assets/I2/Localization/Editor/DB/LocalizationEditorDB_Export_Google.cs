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
		#region Change Spreadsheet
		
		public static string Export_Google ( string SpreadsheetKey ) // returns the error string or empty if there is no error
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

				//--[ Export Categories ]-------------------

				Export_Google_CreateSheetsWithCategories( mGoogleService, Spreadsheet );

				if (EditorPrefs.GetBool("I2Loc OpenDataSourceAfterExport", true))
					OpenGoogleSpreadsheet( SpreadsheetKey );

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
				return e.Message;
			}
			#endif
		}

		public static string Export_Google_CreateSheetsWithCategories( SpreadsheetsService mGoogleService, SpreadsheetEntry Spreadsheet )
		{
			int nExportedCategories = 0;
			List<string> Categories = LocalizationEditorDB.GetCategories();
			for (int i=0, imax=Categories.Count; i<imax; ++i)
			{
				List<string> Terms = GetAllTerms(Categories[i]);
				if (Terms.Count==0) // skip empty categories
					continue;

				string WorksheetName = (string.IsNullOrEmpty(Categories[i]) ? EmptyCategory : Categories[i]);
				WorksheetEntry Worksheet;
				if (i<Spreadsheet.Worksheets.Entries.Count) // The sheet exists so just modify its data
				{
					//--[ First be sure that no other worksheet has this category name ]
					for (int j=i+1; j<Spreadsheet.Worksheets.Entries.Count; ++j)
						if (((WorksheetEntry)Spreadsheet.Worksheets.Entries[j]).Title.Text==WorksheetName)
						{
							Worksheet = (WorksheetEntry)Spreadsheet.Worksheets.Entries[j];
							Worksheet.Title.Text = "Temp"+j;
							Worksheet.Update();
						}

					//--[ Update Worksheet size and name ]-------------------
					Worksheet = (WorksheetEntry)Spreadsheet.Worksheets.Entries[nExportedCategories];
					Worksheet.Title.Text = WorksheetName;
					Worksheet.Cols = 3 + (uint)mLanguageSource.mLanguages.Count;  // Key + Type + Description
					Worksheet.Rows = 1 + (uint)Terms.Count;	 // Headers + Keys
					Worksheet.Update ();
				}
				else // create and set the sheet data
				{
					Worksheet = new WorksheetEntry();
					Worksheet.Title.Text = WorksheetName;
					Worksheet.Cols = 3 + (uint)mLanguageSource.mLanguages.Count;  // Key + Type + Description
					Worksheet.Rows = 1 + (uint)Terms.Count;	 // Headers + Keys
					mGoogleService.Insert( Spreadsheet.Worksheets, Worksheet );
				}

				Export_Google_ExportSheet( mGoogleService, (WorksheetEntry)Spreadsheet.Worksheets.Entries[nExportedCategories] , ref Terms);
				nExportedCategories++;
			}

			//--[ Delete extra worksheets ]---------------
			for (int i=Spreadsheet.Worksheets.Entries.Count-1; i>=nExportedCategories; --i)
			{
				WorksheetEntry Worksheet = (WorksheetEntry)Spreadsheet.Worksheets.Entries[i];
				mGoogleService.Delete (Worksheet.EditUri.ToString());
			}

			return string.Empty;
		}

		public static List<string> GetAllTerms( string Category )
		{
			List<string> Terms = new List<string>();
			foreach (TermData termData in mLanguageSource.mTerms)
				if (GetCategoryFromFullTerm( termData.Term ) == Category)
					Terms.Add(termData.Term);
			return Terms;
		}

		static void Export_Google_ExportSheet( SpreadsheetsService mGoogleService, WorksheetEntry Worksheet, ref List<string> Terms )
		{
			//--[ GetAll Cells ]-------------
			
			AtomLink cellLink = Worksheet.Links.FindService(GDataSpreadsheetsNameTable.CellRel, null);
			CellQuery cellQuery = new CellQuery(cellLink.HRef.ToString());
			cellQuery.ReturnEmpty = ReturnEmptyCells.yes;
			CellFeed cellFeed = mGoogleService.Query(cellQuery);
			
			//--[ Write Header ]----------------------
			WriteGoogleCell(0,0,"Keys", cellFeed);
			WriteGoogleCell(1,0,"Type", cellFeed);
			WriteGoogleCell(2,0,"Description", cellFeed);

			int nLanguages = mLanguageSource.mLanguages.Count;
			for (int i=0; i<nLanguages; ++i)
				WriteGoogleCell(3+i,0,GoogleTranslation.GetCodedLanguage(mLanguageSource.mLanguages[i].Name, mLanguageSource.mLanguages[i].Code), cellFeed);
			
			//--[ Write Keys ]----------------------
			
			for (int Row = 0, MaxRow=Terms.Count; Row<MaxRow; ++Row)
			{
				TermData termData = mLanguageSource.GetTermData(Terms[Row]);
				if (termData==null)
				    continue;

				WriteGoogleCell(0, Row+1, GetKeyFromFullTerm( Terms[Row] ), cellFeed);
				WriteGoogleCell(1, Row+1, termData.TermType.ToString(), cellFeed);
				WriteGoogleCell(2, Row+1, termData.Description, cellFeed);

				for (int i=0, imax=Mathf.Min (termData.Languages.Length, nLanguages); i<imax; ++i)
					WriteGoogleCell(3+i, Row+1, termData.Languages[i], cellFeed);
			}

			mGoogleService.Batch(cellFeed, new Uri(cellFeed.Batch));
		}
		
		static void WriteGoogleCell( int Col, int Row, string Value, CellFeed cellFeed )
		{
			CellEntry cell = (CellEntry)cellFeed.Entries[Row * (3+mLanguageSource.mLanguages.Count) + Col];
			cell.InputValue = Value;
			cell.BatchData = new GDataBatchEntryData(string.Format("R{0}C{1}",cell.Row,cell.Column), GDataBatchOperationType.update);
		}
		
		static void OpenGoogleSpreadsheet( string SpreadsheetKey )
		{
			string SpreadsheetUrl = "https://docs.google.com/spreadsheet/ccc?key=" + SpreadsheetKey;
			Application.OpenURL(SpreadsheetUrl);
		}

		#endregion

	}
}