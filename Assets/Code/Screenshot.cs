using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Screenshot : MonoBehaviour {

    [Header("Press SPACE to capture screenshot.")]
    public int ResolutionFactor = 2;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            string filename = string.Format("Assets/Screenshots/Screenshot | {0} | {1}.png", SceneManager.GetActiveScene().name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            if (!Directory.Exists("Assets/Screenshots")) {
                Directory.CreateDirectory("Assets/Screenshots");
            }
            TakeTransparentScreenshot(GetComponent<Camera>(), Screen.width * ResolutionFactor, Screen.height * ResolutionFactor, filename);
        }
    }

    public static void TakeTransparentScreenshot(Camera cam, int width, int height, string savePath) {
        // Depending on your render pipeline, this may not work.
        var bak_cam_targetTexture = cam.targetTexture;
        var bak_cam_clearFlags = cam.clearFlags;
        var bak_RenderTexture_active = RenderTexture.active;

        var tex_transparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // Must use 24-bit depth buffer to be able to fill background.
        var render_texture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        var grab_area = new Rect(0, 0, width, height);

        RenderTexture.active = render_texture;
        cam.targetTexture = render_texture;
        cam.clearFlags = CameraClearFlags.SolidColor;

        // Simple: use a clear background
        cam.backgroundColor = Color.clear;
        cam.Render();
        tex_transparent.ReadPixels(grab_area, 0, 0);
        tex_transparent.Apply();

        // Encode the resulting output texture to a byte array then write to the file
        byte[] pngShot = ImageConversion.EncodeToPNG(tex_transparent);
        File.WriteAllBytes(savePath, pngShot);

        cam.clearFlags = bak_cam_clearFlags;
        cam.targetTexture = bak_cam_targetTexture;
        RenderTexture.active = bak_RenderTexture_active;
        RenderTexture.ReleaseTemporary(render_texture);
        Texture2D.Destroy(tex_transparent);
    }

}