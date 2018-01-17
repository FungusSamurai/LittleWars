using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlights {

    bool highlighting = false;

    //for use with hexcells
    //use int to determine what color/shader you want
    public void HighlightCombatTile(HexCell h)
    {
        h.GetComponent<Renderer>().material.shader = (Shader)Resources.Load("Shaders/Highlight");
    }

    public void Highlighth(HexCell g, int i)
    {
            switch (i)
            {
                case (0):
                if (g.Passable)
                {
                    g.GetComponent<Renderer>().material.shader = (Shader)Resources.Load("Shaders/Highlight");
                }
                    break;
                case (1):
                    g.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                    break;
                case (2):
                    g.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                    g.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                    break;
                case (3):
                    g.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                    g.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    break;
                case (4):
                if (g.Passable)
                {
                    g.GetComponent<Renderer>().material.shader = Shader.Find("Specular");
                    g.GetComponent<Renderer>().material.SetColor("_SpecColor", Color.cyan);
                }
                    break;
                case (5):
                if (g.Passable)
                {
                    g.GetComponent<Renderer>().material.shader = Shader.Find("Specular");
                    g.GetComponent<Renderer>().material.SetColor("_SpecColor", Color.magenta);
                }
                    break;
        }
    }
    //for use with game objects
    public void Highlightg(GameObject g, int i)
    {
        switch (i)
        {
            case (0):
                g.GetComponent<Renderer>().material.shader = (Shader)Resources.Load("Shaders/Highlight");
                break;
            case (1):
                g.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                break;
            case (2):
                g.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                g.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                break;
            case (3):
                g.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                g.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                break;
            case (4):
                g.GetComponent<Renderer>().material.shader = Shader.Find("Specular");
                g.GetComponent<Renderer>().material.SetColor("_SpecColor", Color.cyan);
                break;
            case (5):
                g.GetComponent<Renderer>().material.shader = Shader.Find("Specular");
                g.GetComponent<Renderer>().material.SetColor("_SpecColor", Color.magenta);
                break;
        }
    }

    public static void HighlightGameObject(GameObject g, int i)
    {
        switch (i)
        {
            case (0):
                g.GetComponent<Renderer>().material.shader = (Shader)Resources.Load("Shaders/Highlight");
                break;
            case (1):
                g.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                break;
            case (2):
                g.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                g.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                break;
            case (3):
                g.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                g.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                break;
            case (4):
                g.GetComponent<Renderer>().material.shader = Shader.Find("Specular");
                g.GetComponent<Renderer>().material.SetColor("_SpecColor", Color.cyan);
                break;
            case (5):
                g.GetComponent<Renderer>().material.shader = Shader.Find("Specular");
                g.GetComponent<Renderer>().material.SetColor("_SpecColor", Color.magenta);
                break;
        }
    }

    public void Highlightu(Unit g, int i)
    {
        switch (i)
        {
            case (0):
                SetMaterialsTo(g.GetComponentsInChildren<Renderer>(), "Custom/GlowShader");
                break;
            case (1):
                SetMaterialsTo(g.GetComponentsInChildren<Renderer>(), "Standard");
                break;
            case (2):
                g.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                g.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                break;
            case (3):
                g.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                g.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                break;
            case (4):
                g.GetComponent<Renderer>().material.shader = Shader.Find("Specular");
                g.GetComponent<Renderer>().material.SetColor("_SpecColor", Color.cyan);
                break;
            case (5):
                g.GetComponent<Renderer>().material.shader = Shader.Find("Specular");
                g.GetComponent<Renderer>().material.SetColor("_SpecColor", Color.magenta);
                break;
        }
    }

    public void SetMaterialsTo(Renderer[] rend, string name)
    {
        foreach (Renderer r in rend)
        {
            foreach (Material m in r.materials)
            {
                m.shader = Shader.Find(name);
            }
        }
    }

    public void HighlightCannon(Cannon c)
    {
        SetMaterialsTo(c.GetComponentsInChildren<Renderer>(), "Custom/GlowShader");
    }

    public void UnhighlightCannon(Cannon c)
    {
        SetMaterialsTo(c.GetComponentsInChildren<Renderer>(), "Standard");
    }

    public void HandleHighlights(int range, List<HexCell> ListOfTiles, Player[] players)
    {
        if (!highlighting) //If no tiles are currently being highlighted (Brennan)
        {
            foreach (HexCell c in ListOfTiles)
            {
                if (players[0].backLine.Contains(c))
                {
                    Highlighth(c, 4);
                }
                else if (players[1].backLine.Contains(c))
                {
                    Highlighth(c, 5);
                }
                else
                {
                    Highlighth(c, 0);
                }
            }
            highlighting = true; //Tiles are now currently being highlighted (Brennan)
        }

        else //If tiles are being highlighted (Brennan)
        {
            foreach (HexCell c in ListOfTiles)
            {
                if (players[0].backLine.Contains(c))
                {
                    Highlighth(c, 2);
                }
                else if (players[1].backLine.Contains(c))
                {
                    Highlighth(c, 3);
                }
                else
                {
                    Highlighth(c, 1);
                }
            }

            highlighting = false; //Tiles are no longer being highlighted (Brennan)
        }
    }

    public void HighlightUnits(bool s, Player currentPlayer)
    {
        if (s)
        {
            foreach (Unit u in currentPlayer.Inf)
            {
                Highlightu(u, 0);
            }
            foreach (Unit u in currentPlayer.Cav)
            {
                Highlightu(u, 0);
            }
        }
        else
        {
            foreach (Unit u in currentPlayer.Inf)
            {
                Highlightu(u, 1);
            }
            foreach (Unit u in currentPlayer.Cav)
            {
                Highlightu(u, 1);
            }
        }
    }
}
