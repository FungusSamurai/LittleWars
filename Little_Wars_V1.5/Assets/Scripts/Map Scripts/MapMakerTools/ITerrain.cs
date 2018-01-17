using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITerrain { // Interface to designate a script as terrain
    void SetTiles(); // Any tiles the game object covers should have their passable value set to false
    void SetPosition(); // the game object should be centered above it's covered area (unless otherwise needing a different location) and always resting atop the tiles
    void SetCollider(); // the colliders and other physic aspects of the object should be turned on
    void ResetTiles(); // When removing, tiles the object covers are reset to passable = true
    void DestroySelf(); // Called when removing to destroy the game object.
}
