using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HighlightMaster
{
    public static void HighlightUnitToggle(bool on, Unit u)
    {
        string name = on ? "Custom/GlowShader" : "Standard";
        SetMaterialsTo(u.gameObject.GetComponentsInChildren<Renderer>(), name);
    }

    public static void HighlightMovable(Player p)
    {
        foreach (Unit u in p.Inf)
        {
            HighlightUnitToggle(!u.Moved, u);
        }
        foreach (Unit u in p.Cav)
        {
            HighlightUnitToggle(!u.Moved, u);
        }
        foreach (Cannon c in p.Can)
        {
            HighlightUnitToggle(!c.Moved, c);
        }
    }

    public static void UnhighlightMovable(Player p)
    {
        foreach (Unit u in p.Inf)
        {
            if (!u.Moved)
                HighlightUnitToggle(false, u);
        }
        foreach (Unit u in p.Cav)
        {
            if (!u.Moved)
                HighlightUnitToggle(false, u);
        }
        foreach (Cannon c in p.Can)
        {
            if (!c.Moved)
                HighlightUnitToggle(false, c);
        }
    }

    public static void HighlightActiveCannons(Player p)
    {
        foreach (Cannon c in p.Can)
        {
            if (!c.Moved)
                HighlightUnitToggle(true, c);
        }
    }

    public static void UnhighlightCannons(Player p)
    {
        foreach (Cannon c in p.Can)
        {
            if (!c.Moved)
                HighlightUnitToggle(false, c);
        }
    }

    private static void SetMaterialsTo(Renderer[] rend, string name)
    {
        foreach (Renderer r in rend)
        {
            foreach (Material m in r.materials)
            {
                m.shader = Shader.Find(name);
            }
        }
    }


    public static void ToggleTileHighlights(bool on, List<HexCell> list)
    {
        if (on)
        {
            foreach (HexCell h in list)
            {
                HighlightTile(h);
            }
        }
        else
        {
            foreach (HexCell h in list)
            {
                UnhighlightTile(h);
            }
        }
    }

    public static void HighlightTile(HexCell h)
    {
        h.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Specular");

        if (h.R >= 2 && h.R <= 3)
        {
            h.gameObject.GetComponent<Renderer>().material.SetColor("_SpecColor", Color.cyan);
        }
        else if (h.R <= MapMaster.MapRadius - 3 && h.R >= MapMaster.MapRadius - 4)
        {
            h.gameObject.GetComponent<Renderer>().material.SetColor("_SpecColor", Color.magenta);
        }
    }

    public static void UnhighlightTile(HexCell h)
    {
        h.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");

        if (h.R >= 2 && h.R <= 3)
        {
            h.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
        else if (h.R <= MapMaster.MapRadius - 3 && h.R >= MapMaster.MapRadius - 4)
        {
            h.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }
    }
}
