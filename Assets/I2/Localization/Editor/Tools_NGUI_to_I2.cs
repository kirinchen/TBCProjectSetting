﻿//#define NGUI
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace I2.Loc
{
	public class Tools_NGUI_to_I2
	{
#if NGUI
		[MenuItem( "Tools/I2 Localization/Convert from NGUI", false, 1 )]
		public static void ConvertNGUI_to_I2()
		{
			UILocalize[] locals = (UILocalize[])Resources.FindObjectsOfTypeAll(typeof(UILocalize));
			for (int i=0, imax=locals.Length; i<imax; ++i)
			{
				UILocalize local = locals[i];
				GameObject GO = local.gameObject;
				if (!GUITools.ObjectExistInScene (GO))
					continue;

				Localize NewLocal = GO.AddComponent<Localize>();
				NewLocal.Term = local.key;
				Object.DestroyImmediate( local );
			}

/*			Localization[] NGUILocalizations = (Localization[])Resources.FindObjectsOfTypeAll(typeof(Localization));
			for (int i=0, imax=NGUILocalizations.Length; i<imax; ++i)
				if (GUITools.ObjectExistInScene (NGUILocalizations[i].gameObject))
				{
					Localization NGUILocalization = NGUILocalizations[i];
					LanguageSource I2Source = NGUILocalization.gameObject.AddComponent<LanguageSource>();

					for (int j=0, jmax=NGUILocalization.languages.Length; j<jmax; ++j)
					{
						I2Source.AddLanguage( NGUILocalization.languages[j].name, string.Empty, NGUILocalization.languages[j] );
					}

					Object.DestroyImmediate( NGUILocalization );
				}*/
		}
#endif
		[MenuItem("Tools/I2 Localization/Help", false, 2 )]
		[MenuItem("Help/I2 Localization")]
		public static void MainHelp()
		{
			Application.OpenURL("http://www.inter-illusion.com/assets/I2Localization.pdf");
		}
	}
}