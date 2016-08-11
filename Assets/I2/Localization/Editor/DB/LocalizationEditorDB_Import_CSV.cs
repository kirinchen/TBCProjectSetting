using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace I2.Loc
{	
	public partial class LocalizationEditorDB
	{
		public static void Import_CSV( string FileName )
		{ 
			List<string[]> CSV = LocalizationReader.ReadCSVfile (FileName);

			//--[ Languages ]--------------
			
			string[] Tokens = CSV[0];
			mLanguageSource.mLanguages.Clear();

			int nLanguages = Mathf.Max (Tokens.Length-3, 0); // -3  because:  Key, Type & Desc
			for (int i=0; i<nLanguages; ++i)
			{
				LanguageData lanData = new LanguageData();
				GoogleTranslation.UnPackCodeFromLanguageName(Tokens[i+3], out lanData.Name, out lanData.Code);
				mLanguageSource.mLanguages.Add (lanData);
             }

			//--[ Keys ]--------------
			
			mLanguageSource.mTerms.Clear();
			for (int i=1, imax=CSV.Count; i<imax; ++i)
			{
				Tokens = CSV[i];
				TermData termData = new TermData();

				string sKey = Tokens[0];
				LanguageSource.ValidateFullTerm(ref sKey);
				termData.Term = sKey;

				termData.TermType = GetTermType(Tokens[1]);
				termData.Description = Tokens[2];

				termData.Languages = new string[ Mathf.Min (nLanguages, Tokens.Length-3) ];
				Array.Copy(Tokens,3, termData.Languages, 0, termData.Languages.Length);

				mLanguageSource.mTerms.Add (termData);
			}
			LocalizationEditor.mSelectedCategories = LocalizationEditorDB.GetCategories();
		}
	}
}