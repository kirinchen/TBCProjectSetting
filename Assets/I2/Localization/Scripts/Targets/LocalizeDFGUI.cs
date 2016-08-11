//#define DFGUI

using UnityEngine;
using System.Collections;

namespace I2.Loc
{
	#if DFGUI
	public partial class Localize
	{
		#region Cache

		dfButton		mTarget_dfButton;
		dfLabel			mTarget_dfLabel;
		dfPanel			mTarget_dfPanel;
		dfSprite		mTarget_dfSprite;
		dfRichTextLabel	mTarget_dfRichTextLabel;

		// This components are missing as they need group localizations
		//dfListbox	mTarget_dfListbox;
		//dfDropdown  mTarget_dfDropDown;

		public void RegisterEvents_DFGUI()
		{
			EventFindTarget += FindTarget_dfButton;
			EventFindTarget += FindTarget_dfLabel;
			EventFindTarget += FindTarget_dfPanel;
			EventFindTarget += FindTarget_dfSprite;
			EventFindTarget += FindTarget_dfRichTextLabel;
		}
		
		#endregion
		
		#region Find Target
		
		void FindTarget_dfButton() 		  { FindAndCacheTarget (ref mTarget_dfButton, SetFinalTerms_dfButton, DoLocalize_dfButton, true, true, false); }
		void FindTarget_dfLabel()		  { FindAndCacheTarget (ref mTarget_dfLabel, SetFinalTerms_dfLabel, DoLocalize_dfLabel, true, true, false); }
		void FindTarget_dfPanel() 		  { FindAndCacheTarget (ref mTarget_dfPanel, SetFinalTerms_dfPanel, DoLocalize_dfPanel, true, true, false); }
		void FindTarget_dfSprite() 		  { FindAndCacheTarget (ref mTarget_dfSprite, SetFinalTerms_dfSprite, DoLocalize_dfSprite, true, false, false); }
		void FindTarget_dfRichTextLabel() { FindAndCacheTarget (ref mTarget_dfRichTextLabel, SetFinalTerms_dfRichTextLabel, DoLocalize_dfRichTextLabel, true, true, false); }

		#endregion
		
		#region SetFinalTerms
		
		bool SetFinalTerms_dfButton(string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			string second = (mTarget_dfButton.Font!=null ? mTarget_dfButton.Font.name : string.Empty);
			return SetFinalTerms (mTarget_dfButton.Text, second,		out primaryTerm, out secondaryTerm);
		}

		bool SetFinalTerms_dfLabel(string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			string second = (mTarget_dfLabel.Font!=null ? mTarget_dfLabel.Font.name : string.Empty);
			return SetFinalTerms (mTarget_dfLabel.Text, second,		out primaryTerm, out secondaryTerm);
		}

		public bool SetFinalTerms_dfPanel(string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			string second = (mTarget_dfPanel.Atlas!=null ? mTarget_dfPanel.Atlas.name : string.Empty);
			return SetFinalTerms (mTarget_dfPanel.BackgroundSprite, 	second,	out primaryTerm, out secondaryTerm);
			
		}
		
		public bool SetFinalTerms_dfSprite(string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			string second = (mTarget_dfSprite.Atlas!=null ? mTarget_dfSprite.Atlas.name : string.Empty);
			return SetFinalTerms (mTarget_dfSprite.SpriteName, 	second,	out primaryTerm, out secondaryTerm);
		}
		
		bool SetFinalTerms_dfRichTextLabel(string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			string second = (mTarget_dfRichTextLabel.Font!=null ? mTarget_dfRichTextLabel.Font.name : string.Empty);
			return SetFinalTerms (mTarget_dfRichTextLabel.Text, second,		out primaryTerm, out secondaryTerm);
		}

		#endregion
		
		#region DoLocalize

		public void DoLocalize_dfButton(string MainTranslation, string SecondaryTranslation)
		{
			if (mTarget_dfButton.Text == MainTranslation)
				return;
			
			//--[ Localize Font Object ]----------
			dfFont newFont = GetSecondaryTranslatedObj<dfFont>(ref MainTranslation, ref SecondaryTranslation);
			if (newFont!=null) 
				mTarget_dfButton.Font = newFont;
			
			//--[ Localize Text ]----------
			mTarget_dfButton.Text = MainTranslation;
		}

		public void DoLocalize_dfLabel(string MainTranslation, string SecondaryTranslation)
		{
			if (mTarget_dfLabel.Text == MainTranslation && MainTranslation!=null)
				return;
			
			//--[ Localize Font Object ]----------
			dfFont newFont = GetSecondaryTranslatedObj<dfFont>(ref MainTranslation, ref SecondaryTranslation);
			if (newFont!=null) 
				mTarget_dfLabel.Font = newFont;
			
			//--[ Localize Text ]----------
			mTarget_dfLabel.Text = MainTranslation;
		}

		
		public void DoLocalize_dfRichTextLabel(string MainTranslation, string SecondaryTranslation)
		{
			if (mTarget_dfRichTextLabel.Text == MainTranslation)
				return;
			
			//--[ Localize Font Object ]----------
			dfDynamicFont newFont = GetSecondaryTranslatedObj<dfDynamicFont> (ref MainTranslation, ref SecondaryTranslation);
			if (newFont!=null) 
				mTarget_dfRichTextLabel.Font = newFont;
			
			//--[ Localize Text ]----------
			mTarget_dfRichTextLabel.Text = MainTranslation;
		}


		public void DoLocalize_dfPanel(string MainTranslation, string SecondaryTranslation)
		{
			if (mTarget_dfPanel.BackgroundSprite == MainTranslation)
				return;
			
			//--[ Localize Atlas ]----------
			dfAtlas newAtlas = GetSecondaryTranslatedObj<dfAtlas>(ref MainTranslation, ref SecondaryTranslation);
			if (newAtlas!=null) 
				mTarget_dfPanel.Atlas = newAtlas;
			
			mTarget_dfPanel.BackgroundSprite = MainTranslation;
			//mTarget_dfSprite.MakePixelPerfect();
		}

		public void DoLocalize_dfSprite(string MainTranslation, string SecondaryTranslation)
		{
			if (mTarget_dfSprite.SpriteName == MainTranslation)
				return;
			
			//--[ Localize Atlas ]----------
			dfAtlas newAtlas = GetSecondaryTranslatedObj<dfAtlas>(ref MainTranslation, ref SecondaryTranslation);
			if (newAtlas!=null) 
				mTarget_dfSprite.Atlas = newAtlas;
			
			mTarget_dfSprite.SpriteName = MainTranslation;
			mTarget_dfSprite.MakePixelPerfect();
		}

		#endregion	
	}
	#else
	public partial class Localize
	{
		public static void RegisterEvents_DFGUI()
		{
		}
	}
	#endif
}

