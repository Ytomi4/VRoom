using System;
using System.IO;
using System.Threading.Tasks;
using VRM;
using UnityEngine;
using RootMotion.FinalIK;


public class ImportVRM : HMDInputManager
{
    public static Action AvatarLoaded;

    public static GameObject _avatar { get; private set; }

    private void Start()
    {
        _avatar = null;
    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(100,100,200,40),"VRM Import"))
        {
            string path = OpenFileName.ShowDialog("open vrm", "vrm");

            var bytes = File.ReadAllBytes(path);

            var context = new VRMImporterContext();

            context.ParseGlb(bytes);
            var meta = context.ReadMeta(false);

            context.LoadAsync(_ => OnLoaded(context));
        }
    }

    private void OnLoaded(VRMImporterContext context)
    {
        _avatar = context.Root;

        _avatar.transform.SetParent(transform, false);
        _avatar.gameObject.tag = "Avatar";

        context.ShowMeshes();


        AvatarLoaded();
    }
}
