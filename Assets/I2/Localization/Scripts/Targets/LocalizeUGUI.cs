//#define UGUI

using UnityEngine;
using System.Collections;

namespace I2.Loc
{
	#if UGUI
	public partial class Localize
	{
		#region Cache

		UnityEngine.UI.Text 		mTarget_uGUI_Text;
		//UISprite 	mTarget_UISprite;
		UnityEngine.UI.Image 	mTarget_uGUI_Image;

		public void RegisterEvents_UGUI()
		{
			EventFindTarget += FindTarget_uGUI_Text;
			EventFindTarget += FindTarget_uGUI_Image;
		}
		
		#endregion
		
		#region Find Target
		
		void FindTarget_uGUI_Text() 	{ FindAndCacheTarget (ref mTarget_uGUI_Text, SetFinalTerms_uGUI_Text, DoLocalize_uGUI_Text, true, true, false); }
		//void FindTarget_UISprite()	{ FindAndCacheTarget (ref mTarget_UISprite, SetFinalTerms_UISprite, DoLocalize_UISprite, true, false, false); }
		void FindTarget_uGUI_Image() 	{ FindAndCacheTarget (ref mTarget_uGUI_Image, SetFinalTerms_uGUI_Image, DoLocalize_uGUI_Image, false, false, false); }
		
		#endregion
		
		#region SetFinalTerms
		
		bool SetFinalTerms_uGUI_Text(string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			string second = (mTarget_uGUI_Text.font!=null ? mTarget_uGUI_Text.font.name : string.Empty);
			return SetFinalTerms (mTarget_uGUI_Text.text, second,		out primaryTerm, out secondaryTerm);
			
		}
		
		/*public bool SetFinalTerms_UISprite(string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			return SetFinalTerms (mTarget_UISprite.spriteName, 	mTarget_UISprite.atlas.name,	out primaryTerm, out secondaryTerm);
			
		}*/
		
		public bool SetFinalTerms_uGUI_Image(string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			return SetFinalTerms (mTarget_uGUI_Image.mainTexture.name, 	null,	out primaryTerm, out secondaryTerm);
			
		}
		
		#endregion
		
		#region DoLocalize

		public static T FindInParents<T> (Transform tr) where T : Component
		{
			if (!tr) 
				return null;

			T comp = tr.GetComponent<T>();
			while (!comp && tr)
			{
				comp = tr.GetComponent<T>();
				tr = tr.parent;
			}
			return comp;
		}
		
		public void DoLocalize_uGUI_Text(string MainTranslation, string SecondaryTranslation)
		{
			/*UnityEngine.UI.InputField input = FindInParents<UnityEngine.UI.InputField>(mTarget_uGUI_Text.transform);
			if (input != null && input.text == mTarget_uGUI_Text) 
			{
				if (input.defaultText == MainTranslation) 
					return;
				input.defaultText = MainTranslation;
			}
			else */
			{
				if (mTarget_uGUI_Text.text == MainTranslation) 
					return;
			}
			mTarget_uGUI_Text.text = MainTranslation;

			//--[ Localize Font Object ]----------
			Font newFont = GetSecondaryTranslatedObj<Font>(ref MainTranslation, ref SecondaryTranslation);
			if (newFont!=null) mTarget_uGUI_Text.font = newFont;
		}
		/*
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
		}*/
		
		public void DoLocalize_uGUI_Image(string MainTranslation, string SecondaryTranslation)
		{
			Sprite Old = mTarget_uGUI_Image.sprite;
			if (Old && Old.name==MainTranslation)
				return;
			
			mTarget_uGUI_Image.sprite = (FindTranslatedObject(MainTranslation) as Sprite);
			
			// If the old value is not in the translatedObjects, then unload it as it most likely was loaded from Resources
			//if (!HasTranslatedObject(Old))
			//	Resources.UnloadAsset(Old);
		}
		
		#endregion	
	}
	#else
	public partial class Localize
	{
		public static void RegisterEvents_UGUI()
		{
		}
	}
	#endif
}

