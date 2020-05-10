using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BundledObjectLoaderAsync : MonoBehaviour
{
    public string assetName = "Cygnet_MidiDress";

    [SerializeField]
    private Button _button = default;

    private void Start()
    {
        _button?.onClick.AddListener(LoadBundle);
    }

    private void LoadBundle()
    {
        StartCoroutine(CignetLoad());
    }

    IEnumerator CignetLoad()
    {
        string path = OpenFileName.ShowDialog("all", "all");

        AssetBundleCreateRequest asyncBundleRequest = AssetBundle.LoadFromFileAsync(path);
        yield return asyncBundleRequest;

        AssetBundle localAssetBundle = asyncBundleRequest.assetBundle;

        if(localAssetBundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            yield break;
        }

        AssetBundleRequest assetRequest = localAssetBundle.LoadAssetAsync<GameObject>(assetName);
        yield return assetRequest;

        GameObject prefab = assetRequest.asset as GameObject;
        Instantiate(prefab);

        localAssetBundle.Unload(false);
    }
}
