using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TurnOffShadows : MonoBehaviour
{
    /*A script I made to turn of shadows of multiple objects in the hierarchy without having to do it manually.
    Created out of necessity, there was a specific scene that had a bunch of assets that did not require shadows, being children of many different assets, would take hours to change it manually
    this reduced the work for a couple of minutes*/
    [Button] //Exposed button that calls the method below through inspector
    public void TurnOff()
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>(); //Grabs all the mesh renderers in the children of the object
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; //Disables shadow for all objects grabbed
        }
    }
}
