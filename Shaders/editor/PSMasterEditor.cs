using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class PSMasterEditor : CustomMaterialEditor
{
    protected override void CreateToggleList()
    {
        Toggles.Add(new FeatureToggle("Outline", "outline", "OUTLINE", ""));
        Toggles.Add(new FeatureToggle("Color Palette", "palette", "PALETTE", ""));
        Toggles.Add(new FeatureToggle("Grid Snap", "grid", "GRIDSNAP", ""));
        Toggles.Add(new FeatureToggle("Alpha Threshold", "alpha", "CUTOUT", ""));
        Toggles.Add(new FeatureToggle("Pixel Snap", "pixel", "PIXELSNAP", ""));
    }
}