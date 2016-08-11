using UnityEngine;
using System.Collections.Generic;

namespace I2.Loc
{
	public class ResourceManager : MonoBehaviour 
	{
		#region Singleton
		public static ResourceManager pInstance
		{
			get {
				if (mInstance==null)
					mInstance = (ResourceManager)Object.FindObjectOfType(typeof(ResourceManager));

				if (mInstance==null)
				{
					GameObject GO = new GameObject("I2ResourceManager", typeof(ResourceManager));
					GO.hideFlags = GO.hideFlags | HideFlags.HideAndDontSave;	// Only hide it if this manager was autocreated
					mInstance = GO.GetComponent<ResourceManager>();
				}

				DontDestroyOnLoad(mInstance.gameObject);
				return mInstance;
			}
		}
		static ResourceManager mInstance;

		#endregion

		#region Assets

		public Object[] Assets;

		// This function tries finding an asset in the Assets array, if not found it tries loading it from the Resources Folder
		public Object GetAsset( string Name )
		{
			Object Obj = FindAsset( Name );
			if (Obj!=null)
				return Obj;

			return LoadFromResources( Name );
		}

		Object FindAsset( string Name )
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
			if (Assets==null)
				return false;
			return System.Array.IndexOf (Assets, Obj) >= 0;
		}

		#endregion

		#region Resources Cache

		// This cache is kept for a few moments and then cleared
		// Its meant to avoid doing several Resource.Load for the same Asset while Localizing 
		// (e.g. Lot of labels could be trying to Load the same Font)
		Dictionary<string, Object> mResourcesCache = new Dictionary<string, Object>(); // This is used to avoid re-loading the same object from resources in the same frame
		bool mCleaningScheduled = false;

		public Object LoadFromResources( string Path )
		{
			Object Obj;
			// Doing Resource.Load is very slow so we are catching the recently loaded objects
			if (mResourcesCache.TryGetValue(Path, out Obj) && Obj!=null)
			{
				return Obj;
			}

			Obj = Resources.Load(Path);
			mResourcesCache[Path] = Obj;

			if (!mCleaningScheduled)
			{
				Invoke("CleanResourceCache", 0.1f);
				mCleaningScheduled = true;
			}
			return Obj;
		}

		public void CleanResourceCache()
		{
			mResourcesCache.Clear();
			Resources.UnloadUnusedAssets();

			CancelInvoke();
			mCleaningScheduled = false;
		}

		#endregion
	}
}