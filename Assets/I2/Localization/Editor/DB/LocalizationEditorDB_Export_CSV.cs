using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace I2.Loc
{	
	public partial class LocalizationEditorDB
	{
		public static void Export_CSV( string FileName )
		{
			StringBuilder Builder = new StringBuilder();

			int nLanguages = (mLanguageSource.mLanguages.Count);
			Builder.Append ("Key,Type,Desc");
			
			for (int i=0; i<nLanguages; ++i)
			{
				Builder.Append (",");
				AppendString ( Builder, GoogleTranslation.GetCodedLanguage(mLanguageSource.mLanguages[i].Name, mLanguageSource.mLanguages[i].Code) );
			}
			Builder.Append ("\n");

			foreach (TermData termData in mLanguageSource.mTerms)
			{
				//--[ Key ] --------------
				AppendString( Builder, termData.Term );

				//--[ Type and Description ] --------------
				Builder.AppendFormat (",{0}", termData.TermType.ToString());
				Builder.Append (",");
				AppendString(Builder, termData.Description);

				//--[ Languages ] --------------
				for (int i=0; i<Mathf.Min (nLanguages, termData.Languages.Length); ++i)
				{
					Builder.Append (",");
					AppendString(Builder, termData.Languages[i]);
				}
				Builder.Append ("\n");
			}
			File.WriteAllText (FileName, Builder.ToString(),System.Text.Encoding.UTF8);
		}

		static void AppendString( StringBuilder Builder, string Text )
		{
			if (string.IsNullOrEmpty(Text))
				return;
			Text = Text.Replace ("\\n", "\n");
			if (Text.IndexOfAny(",\n\"".ToCharArray())>=0)
			{
				Text = Text.Replace("\"", "\"\"");
				Builder.AppendFormat("\"{0}\"", Text);
			}
			else 
				Builder.Append(Text);
		}
	}
}