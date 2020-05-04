using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Spout;

public class RenderTextureSwitch : MonoBehaviour
{
    public RenderTexture[] textures = new RenderTexture[3];

    private SpoutSender spoutSender;

    private int i = 0;

    void Start()
    {
        spoutSender = GetComponent<SpoutSender>();
        spoutSender.sourceTexture = textures[0];

        HMDInputManager.RightGetTriggerButtonDown += TextureSwitch;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
            spoutSender.sourceTexture = textures[0];
        if (Input.GetKeyDown(KeyCode.Keypad2))
            spoutSender.sourceTexture = textures[1];
        if (Input.GetKeyDown(KeyCode.Keypad3))
            spoutSender.sourceTexture = textures[2];
            
    }

    private void TextureSwitch()
    {
        Debug.Log("textureSwitch");
        i++;
        if (i > textures.Length-1) i = 0;
        spoutSender.sourceTexture = textures[i];
    }
}
