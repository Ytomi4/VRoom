using System;
using System.IO;
using System.Threading.Tasks;
using VRM;
using UnityEngine;
using UnityEngine.UI;

public class ImportVRMAsync : HMDInputManager
{
    [SerializeField]
    private Button _button;

    public static Action AvatarLoaded;
    public static GameObject _avatar { get; private set; }

    private void Start()
    {
        _avatar = null;

        _button?.onClick.AddListener(async () =>
        {
            var bytes = await Task.Run(() => ReadBytes());

            var context = new VRMImporterContext();

            await Task.Run(() => context.ParseGlb(bytes));
            var meta = context.ReadMeta(false);

            context.LoadAsync(() => OnLoaded(context));
        });
    }

    private byte[] ReadBytes()
    {
        string path = OpenFileName.ShowDialog("open vrm", "vrm");
        Debug.Log(path);
        return File.ReadAllBytes(path);
    }

    private void OnLoaded(VRMImporterContext context)
    {
        GameObject[] othreAvatars = GameObject.FindGameObjectsWithTag("Avatar");
        foreach(GameObject otherAvatar in othreAvatars)
        {
            Destroy(otherAvatar);
        }

        _avatar = context.Root;

        _avatar.transform.SetParent(transform, false);
        _avatar.gameObject.tag = "Avatar";

        context.ShowMeshes();

        AvatarLoaded();
    }
}
