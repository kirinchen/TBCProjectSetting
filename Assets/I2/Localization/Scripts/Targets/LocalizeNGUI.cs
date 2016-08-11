//#define NGUI

using UnityEngine;
using System.Collections;

namespace I2.Loc
{
	#if NGUI
	public partial class Localize
	{
		#region Cache

		UILabel 	mTarget_UILabel;
		UISprite 	mTarget_UISprite;
		UITexture	mTarget_UITexture;

		public void RegisterEvents_NGUI()
		{
			EventFindTarget += FindTarget_UILabel;
			EventFindTarget += FindTarget_UISprite;
			EventFindTarget += FindTarget_UITexture;
		}

		#endregion

		#region Find Target

		void FindTarget_UILabel() 	{ FindAndCacheTarget (ref mTarget_UILabel, SetFinalTerms_UIlabel, DoLocalize_UILabel, true, true, false); }
		void FindTarget_UISprite()	{ FindAndCacheTarget (ref mTarget_UISprite, SetFinalTerms_UISprite, DoLocalize_UISprite, true, false, false); }
		void FindTarget_UITexture() { FindAndCacheTarget (ref mTarget_UITexture, SetFinalTerms_UITexture, DoLocalize_UITexture, false, false, false); }

		#endregion

		#region SetFinalTerms

		bool SetFinalTerms_UIlabel(string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			string second = (mTarget_UILabel.ambigiousFont!=null ? mTarget_UILabel.ambigiousFont.name : string.Empty);
			return SetFinalTerms (mTarget_UILabel.text, second,		out primaryTerm, out secondaryTerm);
			
		}

		public bool SetFinalTerms_UISprite(string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			string second = (mTarget_UISprite.atlas!=null ? mTarget_UISprite.atlas.name : string.Empty);
			return SetFinalTerms (mTarget_UISprite.spriteName, 	second,	out primaryTerm, out secondaryTerm);
			
		}

		public bool SetFinalTerms_UITexture(string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			return SetFinalTerms (mTarget_UITexture.mainTexture.name, 	null,	out primaryTerm, out secondaryTerm);
			
		}

		#endregion

		#region DoLocalize

		public void DoLocalize_UILabel(string MainTranslation, string SecondaryTranslation)
		{
			UIInput input = NGUITools.FindInParents<UIInput>(mTarget_UILabel.gameObject);
			if (input != null && input.label == mTarget_UILabel) 
			{
				if (input.defaultText == MainTranslation) 
					return;
				input.defaultText = MainTranslation;
			}
			else 
			{
				if (mTarget_UILabel.text == MainTranslation) 
					return;
				mTarget_UILabel.text = MainTranslation;
			}
			
			//--[ Localize Font Object ]----------
			Font newFont = GetSecondaryTranslatedObj<Font>(ref MainTranslation, ref SecondaryTranslation);
			if (newFont!=null) 
			{
				mTarget_UILabel.ambigiousFont = newFont;
			}
			else
			{
				UIFont newUIFont = GetSecondaryTranslatedObj<UIFont>(ref MainTranslation, ref SecondaryTranslation);
				if (newUIFont!=null) 
					mTarget_UILabel.ambigiousFont = newUIFont;
			}
		}

		public void DoLocalize_UISprite(string MainTranslation, string SecondaryTranslation)
		{
			if (mTarget_UISprite.spriteName == MainTranslation)
				return;
			
			//--[ Localize Atlas ]----------
			UIAtlas newAtlas = GetSecondaryTranslatedObj<UIAtlas>(ref MainTranslation, ref SecondaryTranslation);
			if (newAtlas!=null) 
				mTarget_UISprite.atlas = newAtlas;
			
			mTarget_UISprite.spriteName = MainTranslation;
			mTarget_UISprite.MakePixelPerfect();
		}
		
		public void DoLocalize_UITexture(string MainTranslation, string SecondaryTranslation)
		{
			Texture Old = mTarget_UITexture.mainTexture;
			if (Old && Old.name==MainTranslation)
				return;
			
			mTarget_UITexture.mainTexture = (FindTranslatedObject(MainTranslation) as Texture);
			
			// If the old value is not in the translatedObjects, then unload it as it most likely was loaded from Resources
			//if (!HasTranslatedObject(Old))
			//	Resources.UnloadAsset(Old);
		}
		
		#endregion	
	}
	#else
	public partial class Localize
	{
		public static void RegisterEvents_NGUI()
		{
		}
	}
	#endif
}

