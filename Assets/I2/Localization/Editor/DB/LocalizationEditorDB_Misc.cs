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
		#region Keys
		
		internal static List<string> GetCategories()
		{
			List<string> Categories = new List<string>();

			foreach (TermData data in mLanguageSource.mTerms)
			{
				string sCategory = GetCategoryFromFullTerm( data.Term );
				if (!Categories.Contains(sCategory))
					Categories.Add(sCategory);
			}
			Categories.Sort();
			return Categories;
		}

		internal static string GetKeyFromFullTerm( string FullTerm )
		{
			int Index = FullTerm.LastIndexOfAny("\\/".ToCharArray());
			return FullTerm.Substring(Index+1);
		}

		internal static string GetCategoryFromFullTerm( string FullTerm )
		{
			int Index = FullTerm.LastIndexOfAny("\\/".ToCharArray());
			if (Index<0) 
				return EmptyCategory;
			else
				return FullTerm.Substring(0, Index);
		}

		internal static void DeserializeFullTerm( string FullTerm, out string Key, out string Category )
		{
			int Index = FullTerm.LastIndexOfAny("\\/".ToCharArray());
			if (Index<0) 
			{
				Category = EmptyCategory;
				Key = FullTerm;
			}
			else 
			{
				Category = FullTerm.Substring(0, Index);
				Key = FullTerm.Substring(Index+1);
			}
		}


		#endregion

		#region Misc

		internal static eTermType GetTermType( string type )
		{
			for (int i=0, imax=(int)eTermType.Object; i<=imax; ++i)
				if (string.Equals( ((eTermType)i).ToString(), type, StringComparison.OrdinalIgnoreCase))
					return (eTermType)i;

			return eTermType.Text;
		}

		
		#endregion

	}
}