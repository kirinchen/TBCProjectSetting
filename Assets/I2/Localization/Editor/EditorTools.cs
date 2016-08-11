using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace I2.Loc
{	
	public class GUITools
	{
		static public Color LightGray = Color.Lerp(Color.gray, Color.white, 0.5f);
		static public Color DarkGray = Color.Lerp(Color.gray, Color.white, 0.2f);

		#region Header

		static public bool DrawHeader (string text, string key)
		{
			bool state = EditorPrefs.GetBool(key, true);

			bool newState = DrawHeader (text, state);

			if (state!=newState) EditorPrefs.SetBool(key, newState);
			return newState;
		}

		static public bool DrawHeader (string text, bool state)
		{
			GUIStyle Style = new GUIStyle(EditorStyles.foldout);
			Style.richText = true;
			EditorStyles.foldout.richText = true;
			if (state)
			{
				GUI.backgroundColor=DarkGray;
				GUILayout.BeginVertical("AS TextArea", GUILayout.Height(1));
				if (!string.IsNullOrEmpty(text))
					state = GUILayout.Toggle(state, text, Style, GUILayout.ExpandWidth(true));
				GUILayout.Space(2);
				
				GUI.backgroundColor = Color.white;
			}
			else
			{
				GUILayout.BeginVertical(EditorStyles.toolbarButton);
				state = GUILayout.Toggle(state, text, Style, GUILayout.ExpandWidth(true));
				GUILayout.EndVertical();
			}
			return state;
		}

		static public void CloseHeader()
		{
			GUILayout.EndHorizontal();
		}


		#endregion

		#region Content
	
		static public void BeginContents ()
		{
			EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
			GUILayout.Space(2f);
			EditorGUILayout.BeginVertical();
			GUILayout.Space(2f);
		}
	
		static public void EndContents () { EndContents(true); }
		static public void EndContents ( bool closeHeader )
		{
			GUILayout.Space(2f);
			EditorGUILayout.EndVertical();
			GUILayout.Space(3f);
			GUILayout.EndHorizontal();

			if (closeHeader) CloseHeader();
		}

		#endregion

		#region Tabs

		static public int DrawTabs( int Index, string[] Tabs )
		{
			GUILayout.BeginHorizontal();
			for (int i=0; i<Tabs.Length; ++i)
			{
				if ( GUILayout.Toggle(Index==i, Tabs[i], "dragtab") && Index!=i) 
					Index=i;
			}
			GUILayout.EndHorizontal();
			return Index;
		}

		static public int DrawShadowedTabs( int Index, string[] Tabs )
		{
			GUI.backgroundColor=Color.Lerp (Color.gray, Color.white, 0.2f);
			GUILayout.BeginVertical("AS TextArea", GUILayout.Height(1));
				GUI.backgroundColor=Color.white;
				GUILayout.Space(2);
				Index = DrawTabs( Index, Tabs );
			GUILayout.EndVertical();
			return Index;
		}

		#endregion

		#region Object Array

		static public void DrawObjectsArray( SerializedProperty PropArray )
		{
			GUILayout.BeginVertical();

				int DeleteElement = -1, MoveUpElement = -1;

				for (int i=0, imax=PropArray.arraySize; i<imax; ++i)
				{
					SerializedProperty Prop = PropArray.GetArrayElementAtIndex(i);
					GUILayout.BeginHorizontal();

						//--[ Delete Button ]-------------------
						if (GUILayout.Button(GUI.skin.GetStyle("WinBtnCloseWin").normal.background, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
				    		DeleteElement = i;

						GUILayout.Space(2);
				    	//--[ Object ]--------------------------
						GUILayout.BeginHorizontal(EditorStyles.toolbar);
							GUI.changed = false;
							Object Obj = EditorGUILayout.ObjectField( Prop.objectReferenceValue, typeof(Object), true, GUILayout.ExpandWidth(true));
							if (Obj==null)
								DeleteElement = i;
							else
							if (GUI.changed)
								Prop.objectReferenceValue = Obj;
						GUILayout.EndHorizontal();

						//--[ MoveUp Button ]-------------------
						if (i==0)
						{
							if (imax>1)
								GUILayout.Space (18);
						}
						else
						{
							if (GUILayout.Button( "\u25B2", EditorStyles.toolbarButton, GUILayout.Width(18)))
								MoveUpElement = i;
						}

					GUILayout.EndHorizontal();
				}

				GUILayout.BeginHorizontal(EditorStyles.toolbar);
					Object NewObj = EditorGUILayout.ObjectField( null, typeof(Object), true, GUILayout.ExpandWidth(true));
					if (NewObj) 
					{
						int Index = PropArray.arraySize;
						PropArray.InsertArrayElementAtIndex( Index );
						PropArray.GetArrayElementAtIndex(Index).objectReferenceValue = NewObj;
					}
				GUILayout.EndHorizontal();

				if (DeleteElement>=0)
				{
					PropArray.DeleteArrayElementAtIndex( DeleteElement );
					PropArray.DeleteArrayElementAtIndex( DeleteElement );
				}

				if (MoveUpElement>=0)
					PropArray.MoveArrayElement(MoveUpElement, MoveUpElement-1);

			GUILayout.EndVertical();
		}

		#endregion

		#region Event CallBack

		static public void DrawEventCallBack( EventCallback CallBack )
		{
			if (CallBack==null)
				return;

			GUILayout.BeginHorizontal();
				GUILayout.Label("Target:", GUILayout.ExpandWidth(false));
				CallBack.Target = EditorGUILayout.ObjectField( CallBack.Target, typeof(MonoBehaviour), true) as MonoBehaviour;
			GUILayout.EndHorizontal();
			
			if (CallBack.Target!=null)
			{
				MethodInfo[] Infos = CallBack.Target.GetType().GetMethods();
				List<string> Methods = new List<string>();

				for (int i = 0, imax=Infos.Length; i<imax; ++i)
				{
					MethodInfo mi = Infos[i];

					if (IsValidMethod(mi))
						Methods.Add (mi.Name);
				}

				int Index = Methods.IndexOf(CallBack.MethodName);
				
				int NewIndex = EditorGUILayout.Popup(Index, Methods.ToArray(), GUILayout.ExpandWidth(true));
				if (NewIndex!=Index)
					CallBack.MethodName = Methods[ NewIndex ];
			}
		}

		static bool IsValidMethod( MethodInfo mi )
		{
			if (mi.DeclaringType == typeof(MonoBehaviour) || mi.ReturnType != typeof(void))
				return false;

			ParameterInfo[] Params = mi.GetParameters ();
			if (Params.Length == 0)	return true;
			if (Params.Length > 1)  return false;

			if (Params [0].ParameterType.IsSubclassOf (typeof(UnityEngine.Object)))	return true;
			if (Params [0].ParameterType == typeof(UnityEngine.Object))	return true;
			return false;
		}


		#endregion

		#region Misc
		
		public static bool ObjectExistInScene( GameObject Obj )
		{
			//if (Obj.transform.root != Obj.transform)
			//	continue;
			
			// We are only interested in GameObjects that are visible in the Hierachy panel and are persitent 
			if ((Obj.hideFlags & (HideFlags.DontSave|HideFlags.HideInHierarchy)) > 0)
				return false;
			
			// We are not interested in Prefab, unless they are Prefab Instances
			PrefabType pfType = PrefabUtility.GetPrefabType(Obj);
			if(pfType == PrefabType.Prefab || pfType == PrefabType.ModelPrefab)
				return false;
			
			// If the database contains the object then its not an scene object, 
			// but the previous test should get rid of them, so I will just comment this 
			// unless an false positive object is found in the future
			/*if (AssetDatabase.Contains(Obj))
					return false;*/
			
			return true;
		}
		#endregion
	}
}
