using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace I2.Loc
{
	// Code to Generate the DB from the TextAssets
	public partial class LocalizationEditorDB
	{
		#region TextAsset to DB
		
		public static void Import_TextAsset( TextAsset asset, int iLanguage=-1 )
		{
			if (asset==null || mLanguageSource==null)
				return;

			if (iLanguage>=mLanguageSource.mLanguages.Count || iLanguage<0)
				iLanguage = mLanguageSource.GetLanguageIndex(asset.name);

			if (iLanguage<0)
			{
				iLanguage = mLanguageSource.mLanguages.Count;
				mLanguageSource.AddLanguage(asset.name, GoogleTranslation.GetLanguageCode(asset.name));
			}

			string Text = Encoding.UTF8.GetString (asset.bytes);
			Text = Text.Replace("\r\n", "\n");
			System.IO.StringReader reader = new System.IO.StringReader(Text);

			string s;
			bool bFirstLine = true;

			while ( (s=reader.ReadLine()) != null )
			{
				s = s.Trim();
				if (string.IsNullOrEmpty(s))  // Allow empty lines
					continue;

				if (bFirstLine && s.StartsWith("LanguageCode:"))
				{
					mLanguageSource.mLanguages[iLanguage].Code = s.Substring("LanguageCode:".Length).Trim();
				}
				else
				{
					TextAsset_ReadLine(s, iLanguage);
				}
				bFirstLine = false;
			}
		}

		static void TextAsset_ReadLine( string s, int iLanguage )
		{
			string Key, Value, Category, termType, Comment;
			if (!LocalizationReader.TextAsset_ReadLine(s, out Key, out Value, out Category, out Comment, out termType))
				return;

			//--[ Type ]---------
			eTermType TermType = eTermType.Text;
			for (eTermType i=(eTermType)0, imax=eTermType.Object; i<imax; i++)
				if (string.Equals(termType, i.ToString(),StringComparison.OrdinalIgnoreCase))
				{
					TermType = i;
					break;
				}

			TermData termData = mLanguageSource.GetTermData(Key);
			if (termData==null)
				termData =mLanguageSource.AddTerm(Key);

			termData.TermType = TermType;
			termData.Description = Comment;
			termData.Languages[iLanguage] = Value;
		}

		#endregion

	}
	
}