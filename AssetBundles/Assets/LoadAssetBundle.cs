using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

public class LoadAssetBundle : MonoBehaviour
{
    string uri = "https://drive.google.com/uc?export=download&id=1vTMGearlq7FCpqXN3-1b6nB0fDlynFl-";
    string manifestBundlePath = "Assets/AssetBundle/AssetBundle";
    AssetBundle bundle;

    void Start()
    {
        StartCoroutine(DownloadAndCacheAssetBundle());
    }


    IEnumerator DownloadAndCacheAssetBundle()
    {
        // Load the manifest
        AssetBundle manifestBundle = AssetBundle.LoadFromFile(manifestBundlePath);
        AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        // Create new cache
        string today = DateTime.Today.ToLongDateString();
        Directory.CreateDirectory(today);
        Cache newCache = Caching.AddCache(today);

        // Set current cache for writing to the new cache if the cache is valid
        if (newCache.valid)
            Caching.currentCacheForWriting = newCache;

        // Download the bundle
        Hash128 hash = manifest.GetAssetBundleHash("SkeletonSystem");
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri, hash, 0);
        yield return request.SendWebRequest();

        // Check for any errors during download
        if (request.error != null)
        {
            Debug.LogError("Error while downloading asset bundle: " + request.error);
            yield break;
        }

        // Get the downloaded asset bundle
        bundle = DownloadHandlerAssetBundle.GetContent(request);

        // Check if the asset bundle was loaded correctly
        if (bundle == null)
        {
            Debug.LogError("Failed to load asset bundle");
            yield break;
        }

        // Get all the cached versions
        List<Hash128> listOfCachedVersions = new List<Hash128>();
        Caching.GetCachedVersions(bundle.name, listOfCachedVersions);

        if (!AssetBundleContainsAssetIWantToLoad(bundle))     // Or any conditions you want to check on your new asset bundle
        {
            // If our criteria wasn't met, we can remove the new cache and revert back to the most recent one
            Caching.currentCacheForWriting = Caching.GetCacheAt(Caching.cacheCount);
            Caching.RemoveCache(newCache);

            for (int i = listOfCachedVersions.Count - 1; i > 0; i--)
            {
                // Load a different bundle from a different cache
                request = UnityWebRequestAssetBundle.GetAssetBundle(uri, listOfCachedVersions[i], 0);
                yield return request.SendWebRequest();
                bundle = DownloadHandlerAssetBundle.GetContent(request);

                // Check and see if the newly loaded bundle from the cache meets your criteria
                if (AssetBundleContainsAssetIWantToLoad(bundle))
                    break;
            }
        }

        // Load the asset from the bundle
        AssetBundleRequest assetRequest = bundle.LoadAssetAsync<GameObject>("SkeletonSystem");
        yield return assetRequest;

        // Check if the asset was loaded correctly
        if (assetRequest.asset == null)
        {
            Debug.LogError("Failed to load asset");
            yield break;
        }

        // Instantiate the asset
        GameObject obj = (GameObject)assetRequest.asset;
        Instantiate(obj);
    }


    bool AssetBundleContainsAssetIWantToLoad(AssetBundle bundle)
    {
        return (bundle.LoadAsset<GameObject>("SkeletonSystem") != null);     // This could be any conditional
    }

    void OnDestroy()
    {
        if (bundle != null)
        {
            bundle.Unload(true);
        }
    }
}



