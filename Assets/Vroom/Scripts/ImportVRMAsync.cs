using System;
using System.IO;
using System.Threading.Tasks;
using VRM;
using UnityEngine;
using UnityEngine.UI;

public class ImportVRMAsync : HMDInputManager
{
    [SerializeField]
    private Button _button = default;

    public static Action AvatarLoaded;
    public static GameObject Avatar { get; private set; }

    private void Start()
    {
        Avatar = null;

        _button?.onClick.AddListener(async () =>
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
}
