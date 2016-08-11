//#define UGUI
//#define NGUI
//#define DFGUI

using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace I2.Loc
{	


	public partial class LocalizationEditorDB
	{
		#region Variables

		public static LanguageSource mLanguageSource;
		public static string EmptyCategory = "Default";

		#endregion

		#region Source 

		public static bool HasLanguageSource(bool AutoFindSources)
		{
			if (mLanguageSource)
				return true;

			return FindSources();
		}

		public static void SetSource( LanguageSource Source )
		{
			mLanguageSource = Source;
		}

		public static bool FindSources()
		{
			LanguageSource[] Locs = (LanguageSource[])Resources.FindObjectsOfTypeAll(typeof(LanguageSource));
			if (Locs.Length>0)
				SetSource( Locs[0] );

			if (FindSourceInResources())
				return true;

			return mLanguageSource!=null;
		}

		public static bool FindSourceInResources()
		{
			GameObject Prefab = (Resources.Load("I2Languages") as GameObject);
			LanguageSource GlobalSource = (Prefab ? Prefab.GetComponent<LanguageSource>() : null);
			
			if (!GlobalSource)
				return false;

			SetSource( GlobalSource );
			return true;
		}

		#endregion
	}	
}