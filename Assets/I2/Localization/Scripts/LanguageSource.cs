using UnityEngine;
using System.Collections.Generic;

namespace I2.Loc
{
	public enum eTermType 
	{ 
		Text, Font, Texture, AudioClip, GameObject, 
		#if UGUI
			Sprite, 
		#endif
		#if NGUI
			UIAtlas, UIFont,
		#endif
		#if DFGUI
			dfFont, dfAtlas, 
		#endif
		#if TK2D
			TK2dFont, TK2dCollection,
		#endif
		#if TextMeshPro
			TextMeshPFont,
		#endif
		Object 
	}

	[System.Serializable]
	public class TermData
	{
		public string 	 Term 			= string.Empty;
		public eTermType TermType 		= eTermType.Text;
		public string 	 Description	= string.Empty;
		public string[]  Languages 		= new string[0];
	};
	[System.Serializable]
	public class LanguageData
	{
		public string Name;
		public string Code;

		[System.NonSerialized]
		public bool Compressed = false;  // This will be used in the next version for only loading used Languages
	}

	[AddComponentMenu("I2/Localization/Source")]
	public class LanguageSource : MonoBehaviour
	{
		#region Variables

		public List<TermData> mTerms = new List<TermData>();
		public List<LanguageData> mLanguages = new List<LanguageData>();


		public Object[] Assets;	// References to Fonts, Atlasses and other objects the localization may need

		public bool NeverDestroy = true;  	// Keep between scenes (will call DontDestroyOnLoad )

		#endregion

		#region EditorVariables
		#if UNITY_EDITOR

		public string Spreadsheet_LocalFileName,
					  Google_SpreadsheetKey;

		#endif
		#endregion

		#region Language

		void Awake()
		{
			if (NeverDestroy)
			{
				if (ManagerHasASimilarSource())
				{
					Destroy (this);
					return;
				}
				else
					DontDestroyOnLoad (gameObject);
			}
			LocalizationManager.AddSource (this);
		}

		public string GetSourceName()
		{
			string s = gameObject.name;
			Transform tr = transform.parent;
			while (tr)
			{
				s = string.Concat(tr.name, "_", s);
				tr = tr.parent;
			}
			return s;
		}


		public int GetLanguageIndex( string language )
		{
			for (int i=0, imax=mLanguages.Count; i<imax; ++i)
				if (string.Compare(mLanguages[i].Name, language, System.StringComparison.OrdinalIgnoreCase)==0)
					return i;

			return -1;
		}

		public void AddLanguage( string LanguageName, string LanguageCode )
		{
			if (GetLanguageIndex(LanguageName)>=0)
				return;

			LanguageData Lang = new LanguageData();
				Lang.Name = LanguageName;
				Lang.Code = LanguageCode;
			mLanguages.Add (Lang);

			int NewSize = mLanguages.Count;
			for (int i=0, imax=mTerms.Count; i<imax; ++i)
				System.Array.Resize(ref mTerms[i].Languages, NewSize);
		}

		public void RemoveLanguage( string LanguageName )
		{
			int LangIndex = GetLanguageIndex(LanguageName);
			if (LangIndex<0)
				return;

			int nLanguages = mLanguages.Count;
			for (int i=0, imax=mTerms.Count; i<imax; ++i)
			{
				for (int j=LangIndex+1; j<nLanguages; ++j)
					mTerms[i].Languages[j-1] = mTerms[i].Languages[j];
				System.Array.Resize(ref mTerms[i].Languages, nLanguages-1);
			}
			mLanguages.RemoveAt(LangIndex);
		}

		public string GetTermTranslation (string term)
		{
			int Index = GetLanguageIndex(LocalizationManager.CurrentLanguage);
			if (Index<0) 
				return string.Empty;

			TermData data = GetTermData(term);
			if (data!=null)
					return data.Languages[Index];

			return string.Empty;
		}

		public TermData AddTerm( string term )
		{
			TermData termData = GetTermData(term);
			if (termData==null)
			{
				termData = new TermData();
				termData.Term = term;
				termData.Languages = new string[mLanguages.Count];
				mTerms.Add(termData);
				#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty (this);
				#endif
			}
			return termData;
		}

		public TermData GetTermData( string term )
		{
			for (int i=0, imax=mTerms.Count; i<imax; ++i)
				if (mTerms[i].Term==term)
					return mTerms[i];
			return null;
		}

		public bool ContainsTerm(string term)
		{
			return (GetTermData(term)!=null);
		}

		public List<string> GetTermsList ()
		{
			List<string> Terms = new List<string>();
			for (int i=0, imax=mTerms.Count; i<imax; ++i)
				Terms.Add(mTerms[i].Term);
			return Terms;
		}

		public  TermData AddTerm( string NewTerm, eTermType termType )
		{
			ValidateFullTerm( ref NewTerm );
			NewTerm = NewTerm.Trim ();

			// Don't duplicate Terms
			TermData data = GetTermData(NewTerm);
			if (data==null) 
			{
				data = new TermData();
				data.Term = NewTerm;
				data.TermType = termType;
				data.Languages = new string[ mLanguages.Count ];
				mTerms.Add (data);
			}

			return data;
		}

		public void RemoveTerm( string term )
		{
			for (int i=0, imax=mTerms.Count; i<imax; ++i)
				if (mTerms[i].Term==term)
				{
					mTerms.RemoveAt(i);
					return;
				}
		}

		public static void ValidateFullTerm( ref string Term )
		{
			Term = Term.Replace('\\', '/');
			/*int First = Term.IndexOf('/');
			if (First<0)
				return;
			
			int second;
			while ( (second=Term.LastIndexOf('/')) != First )
				Term = Term.Remove( second,1);*/
		}

/*		public void AddLanguageKeys ()
		{
			int LanguageIndex = GetLanguageIndex( LocalizationManager.CurrentLanguage );
			if (LanguageIndex<0)
				return;

			LocalizationManager.CurrentLanguageCode = mLanguages [LanguageIndex].LanguageCode;
			TextAsset asset = mLanguages[LanguageIndex].Asset;
			Dictionary<string,string> Dict = LocalizationReader.ReadTextAsset(asset);
			if (LocalizationManager.Terms.Count<=0)
			{
				LocalizationManager.Terms = Dict;
			}
			else
			{
				foreach (KeyValuePair<string,string> kvp in Dict)
					LocalizationManager.Terms[kvp.Key] = kvp.Value;
			}
			//Debug.Log ("Adding LanguageKeys for " + LocalizationManager.CurrentLanguage + " " + asset.text);
		}*/

		public bool IsEqualTo( LanguageSource Source )
		{
			if (Source.mLanguages.Count != mLanguages.Count)
				return false;

			for (int i=0, imax=mLanguages.Count; i<imax; ++i)
				if (Source.GetLanguageIndex( mLanguages[i].Name ) < 0)
					return false;

			return true;
		}

		internal bool ManagerHasASimilarSource()
		{
			for (int i=0, imax=LocalizationManager.Sources.Count; i<imax; ++i)
			{
				LanguageSource source = (LocalizationManager.Sources[i] as LanguageSource);
				if (source!=null && source.IsEqualTo(this) && source!=this)
					return true;
			}
			return false;
		}

		#endregion

		#region Assets
		
		public Object FindAsset( string Name )
		{
			if (Assets!=null)
			{
				for (int i=0, imax=Assets.Length; i<imax; ++i)
					if (Assets[i]!=null && Assets[i].name == Name)
						return Assets[i];
			}
			return null;
		}
		
		public bool HasAsset( Object Obj )
		{
			return System.Array.IndexOf (Assets, Obj) >= 0;
		}
		
		#endregion
	}
}