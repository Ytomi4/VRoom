using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using VRM;
using UnityEngine;
using UnityEngine.UI;

public class ImportVRMAsync : HMDInputManager
{
    [SerializeField]
    private Button _buttonVRM = default;
    [SerializeField]
    private Button _buttonAssetBundle = default;

    public static Action AvatarLoaded;
    public static GameObject Avatar { get; private set; }

    private void Start()
    {
        Avatar = null;

        _buttonVRM?.onClick.AddListener(async () =>
        {
            var bytes = await Task.Run(() => ReadBytes());

            if(bytes != null)
            {
                var context = new VRMImporterContext();

                await Task.Run(() => context.ParseGlb(bytes));
                var meta = context.ReadMeta(false);

                context.LoadAsync(() => OnLoaded(context));
            }
        });

        _buttonAssetBundle?.onClick.AddListener(async () =>
        {
            //前のアバターの消去
            GameObject[] othreAvatars = GameObject.FindGameObjectsWithTag("Avatar");
            foreach (GameObject otherAvatar in othreAvatars)
            {
                Destroy(otherAvatar);
            }

            string path = await Task.Run(() =>OpenFileName.ShowDialog("all", "."));

            StartCoroutine(LoadBundleCoroutine(path));
        });
    }

    private byte[] ReadBytes()
    {
        string path = OpenFileName.ShowDialog("open vrm", "vrm");
        if(path != null)
        {
            Debug.Log(path);
            return File.ReadAllBytes(path);
        }
        return null;
    }

    private void OnLoaded(VRMImporterContext context)
    {
        GameObject[] othreAvatars = GameObject.FindGameObjectsWithTag("Avatar");
        foreach(GameObject otherAvatar in othreAvatars)
        {
            Destroy(otherAvatar);
        }

        Avatar = context.Root;

        Avatar.transform.SetParent(transform, false);
        Avatar.gameObject.tag = "Avatar";

        context.ShowMeshes();

        AvatarLoaded();
    }


    //LoadAssetBundleを読み込む（分けるべき?ImportVRMAsync.Avatarにまとめたかった）

    public string assetName = "Cygnet_MidiDress";

    private void LoadBundle(string path)
    {
        AssetBundle localAssetBundle = AssetBundle.LoadFromFile(path);
        
        if (localAssetBundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
        }

        GameObject asset = localAssetBundle.LoadAsset<GameObject>(assetName);
        Avatar = Instantiate(asset, transform);
        Debug.Log("Avatar Loaded");
        Debug.Log(Avatar.name);
        Avatar.gameObject.tag = "Avatar";

        localAssetBundle.Unload(false);
    }

    IEnumerator LoadBundleCoroutine(string path)
    {
        AssetBundleCreateRequest asyncBundleRequest = AssetBundle.LoadFromFileAsync(path);
        yield return asyncBundleRequest;

        AssetBundle localAssetBundle = asyncBundleRequest.assetBundle;

        if (localAssetBundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            yield break;
        }

        AssetBundleRequest assetRequest = localAssetBundle.LoadAssetAsync<GameObject>(assetName);
        yield return assetRequest;

        GameObject prefab = assetRequest.asset as GameObject;

        Avatar = Instantiate(prefab, transform);
        Avatar.gameObject.tag = "Avatar";

        localAssetBundle.Unload(false);
        
        AvatarLoaded();
    }
}
