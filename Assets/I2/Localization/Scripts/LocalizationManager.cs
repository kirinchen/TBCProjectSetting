using UnityEngine;
using System.Collections.Generic;

namespace I2.Loc
{
	public static class LocalizationManager
	{
		#region Variables: CurrentLanguage
		
		public static string CurrentLanguage
		{
			get { 
				if (string.IsNullOrEmpty(mCurrentLanguage))
				{
					RegisterSceneSources();
					RegisterSourceInResources();
				    SelectStartupLanguage();
				}

				return mCurrentLanguage; 
			}
			set {
				if (mCurrentLanguage != value && HasLanguage(value))
				{
					PlayerPrefs.SetString ("I2 Language", value);
					mCurrentLanguage = value;
					LocalizeAll();
				}
			}
		}
		public static string CurrentLanguageCode
		{
			get { return mLanguageCode; }
			set {
				mLanguageCode = value; 
				IsRight2Left = IsRTL (mLanguageCode);
			}
		}

		static string mCurrentLanguage;
		static string mLanguageCode;
		public static bool IsRight2Left = false;

		static void SelectStartupLanguage()
		{
			// Use the system language if there is a source with that language, 
			// or pick any of the languages provided by the sources

			string SavedLanguage = PlayerPrefs.GetString ("I2 Language", string.Empty);
			string SysLanguage = Application.systemLanguage.ToString();

			// Try selecting the System Language
			// But fallback to the first language found  if the System Language is not available in any source

			if (HasLanguage (SavedLanguage))
			{
				CurrentLanguage = SavedLanguage;
				return;
			}

			// Check if the device language is supported. 
			//Also recognize when not region is set ("English (United State") will be used if sysLanguage is "English")
			string ValidLanguage = GetLanguageThatContains(SysLanguage);
			if (!string.IsNullOrEmpty(ValidLanguage))
			{
				CurrentLanguage = ValidLanguage;
				return;
			}

			//--[ Use first language ]-----------
			for (int i=0, imax=Sources.Count; i<imax; ++i)
				if (Sources[i].mLanguages.Count>0)
				{
					CurrentLanguage = Sources[i].mLanguages[0].Name;
					return;
				}
		}

		#endregion

		#region Variables: Misc

		//public static Dictionary<string, string> Terms = new Dictionary<string, string>();
		public static List<LanguageSource> Sources = new List<LanguageSource>();

		public delegate void OnLocalizeCallback ();
		public static event OnLocalizeCallback OnLocalizeEvent;

		#endregion

		#region Localization

		public static string GetTermTranslation (string Term)
		{
			/*string value = string.Empty;
			Terms.TryGetValue(Term, out value);
			return value;*/
			for (int i=0, imax=Sources.Count; i<imax; ++i)
			{
				string val = Sources[i].GetTermTranslation (Term);
				if (!string.IsNullOrEmpty(val))
					return val;
			}
			return string.Empty;
		}

		internal static void LocalizeAll()
		{
			Localize[] Locals = (Localize[])Resources.FindObjectsOfTypeAll( typeof(Localize) );
			for (int i=0, imax=Locals.Length; i<imax; ++i)
			{
				Localize local = Locals[i];
				//if (ObjectExistInScene (local.gameObject))
				local.OnLocalize();
			}
			if (OnLocalizeEvent != null)
				OnLocalizeEvent ();
			ResourceManager.pInstance.CleanResourceCache();
		}

		#endregion

		#region Sources

		static void RegisterSceneSources()
		{
			LanguageSource[] SceneSources = (LanguageSource[])Resources.FindObjectsOfTypeAll( typeof(LanguageSource) );
			for (int i=0, imax=SceneSources.Length; i<imax; ++i)
				if (!Sources.Contains(SceneSources[i]))
				{
					Sources.Add( SceneSources[i] );
				}
		}		

		static void RegisterSourceInResources()
		{
			// Find the Source that its on the Resources Folder
			GameObject Prefab = (ResourceManager.pInstance.GetAsset("I2Languages") as GameObject);
			LanguageSource GlobalSource = (Prefab ? Prefab.GetComponent<LanguageSource>() : null);
			
			if (GlobalSource && !Sources.Contains(GlobalSource))
			{
				Sources.Add( GlobalSource );
			}
		}		

		internal static void AddSource ( LanguageSource Source )
		{
			if (Sources.Contains (Source))
				return;

			Sources.Add( Source );
		}

		internal static void RemoveSource (LanguageSource Source )
		{
			Sources.Remove( Source );
		}

		public static bool HasLanguage( string Language )
		{
			for (int i=0, imax=Sources.Count; i<imax; ++i)
				if (Sources[i].GetLanguageIndex(Language)>=0)
					return true;
			return false;
		}

		public static string GetLanguageThatContains( string Filter )
		{
			for (int i=0, imax=Sources.Count; i<imax; ++i)
				for (int j=0, jmax=Sources[i].mLanguages.Count; j<jmax; ++j)
					if (Sources[i].mLanguages[j].Name.IndexOf(Filter, System.StringComparison.OrdinalIgnoreCase) >=0 )
					{
						return Sources[i].mLanguages[j].Name;
					}
			return string.Empty;
		}

		public static List<string> GetAllLanguages ()
		{
			List<string> Languages = new List<string> ();
			for (int i=0, imax=Sources.Count; i<imax; ++i)
				for (int j=0, jmax=Sources[i].mLanguages.Count; j<jmax; ++j)
				{
					if (!Languages.Contains(Sources[i].mLanguages[j].Name))
						Languages.Add(Sources[i].mLanguages[j].Name);
				}
			return Languages;
		}

		#endregion

		public static Object FindAsset (string value)
		{
			for (int i=0, imax=Sources.Count; i<imax; ++i)
			{
				Object Obj = Sources[i].FindAsset(value);
				if (Obj)
					return Obj;
			}
			return null;
		}

		#region Left to Right Languages

		static string[] LanguagesRTL = {"ar-DZ", "ar","ar-BH","ar-EG","ar-IQ","ar-JO","ar-KW","ar-LB","ar-LY","ar-MA","ar-OM","ar-QA","ar-SA","ar-SY","ar-TN","ar-AE","ar-YE",
										"he","ur","ji"};

		static bool IsRTL(string Code)
		{
			return System.Array.IndexOf(LanguagesRTL, Code)>=0;
		}

		#endregion

#if UNITY_EDITOR
		// This function should only be called from within the Localize Inspector to temporaly preview that Language

		public static void PreviewLanguage(string NewLanguage)
		{
			mCurrentLanguage = NewLanguage;
		}
#endif
	}
}