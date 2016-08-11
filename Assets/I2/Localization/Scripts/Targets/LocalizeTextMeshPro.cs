using UnityEngine;
using System.Collections;

namespace I2.Loc
{
	#if TextMeshPro
	public partial class Localize
	{
		TMPro.TextMeshPro 	mTarget_TMPLabel;

		public void RegisterEvents_TextMeshPro()
		{
			EventFindTarget += FindTarget_TMPLabel;
		}
		
		void FindTarget_TMPLabel() 	{ FindAndCacheTarget (ref mTarget_TMPLabel, SetFinalTerms_TMPLabel, DoLocalize_TMPLabel, true, true, false); }

		bool SetFinalTerms_TMPLabel(string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			string second = (mTarget_TMPLabel.font!=null ? mTarget_TMPLabel.font.name : string.Empty);
			return SetFinalTerms (mTarget_TMPLabel.text, second,		out primaryTerm, out secondaryTerm);
		}
		
		public void DoLocalize_TMPLabel(string MainTranslation, string SecondaryTranslation)
		{
			mTarget_TMPLabel.text = MainTranslation;

			//--[ Localize Font Object ]----------
			TMPro.TextMeshProFont newFont = GetSecondaryTranslatedObj<TMPro.TextMeshProFont>(ref MainTranslation, ref SecondaryTranslation);
			if (newFont!=null) 
			{
				mTarget_TMPLabel.font = newFont;
			}
			else
			{
				TMPro.TextMeshProFont newUIFont = GetSecondaryTranslatedObj<TMPro.TextMeshProFont>(ref MainTranslation, ref SecondaryTranslation);
				if (newUIFont!=null) 
					mTarget_TMPLabel.font = newUIFont;
			}
		}
	}
	#else
	public partial class Localize
	{
		public static void RegisterEvents_TextMeshPro()
		{
		}
	}
	#endif	
}