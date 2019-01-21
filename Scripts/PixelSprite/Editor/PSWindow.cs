using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using PixelSprite;

public class PSWindow : EditorWindow
{
    [MenuItem("Window/Pixel Sprite")]
    static void Init()
    {
        PSWindow w = (PSWindow)EditorWindow.GetWindow(typeof(PSWindow));
        w.Show();
    }

    PixelRenderer[] renderers;
    MainCamera maincam;

    bool test;

    private void OnGUI()
    {
        if (GUILayout.Button("Reset"))
        {
            var maincams = FindObjectsOfType<MainCamera>();
            foreach (var v in maincams)
            {
                v.ResetAll();
            }

            test = true;
        }
    }

    public void Update()
    {
        renderers = FindObjectsOfType<PixelRenderer>();
        maincam = FindObjectOfType<MainCamera>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].OnWillRenderObject();
        }

        maincam.OnPreRender();
        maincam.camera.Render();
    }
}
