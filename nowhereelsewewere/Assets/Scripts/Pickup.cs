using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pickup : MonoBehaviour {

    [Tooltip("This is the cube that the weapon is listed on so if character lands on this cube he will see weapon.")]
    public Cube parentCube;

    [Tooltip("The material that uses a shader to indicate pickup is able to be picked up.")]
    public Material outlineMaterial;
    [Tooltip("The mesh renderer that the outline material should be applied to.")]
    public MeshRenderer meshRenderer;

    void Start()
    {
        // Add this pickup to its parent cube if there is one and show outline so player knows they can pick this up.
        if(parentCube != null)
        {
            parentCube.pickUps.Add(this);
            // Show outline by creating new materials array and loading original materials array data with new outline material as well.
            Material[] newMaterials = new Material[meshRenderer.materials.Length + 1];
            for(int i=0;i<meshRenderer.materials.Length; i++)
            {
                newMaterials[i] = meshRenderer.materials[i];
            }
            newMaterials[1] = outlineMaterial;
            meshRenderer.materials = newMaterials;
        }
    }

    public void PickUp(WorldCharacter character)
    {
        // Turn off the pickup material.
        List<Material> newMaterials = new List<Material>();
        for (int i = 0; i < meshRenderer.materials.Length-1; i++)
        {
            Material currMat = meshRenderer.materials[i];
            newMaterials.Add(currMat);
        }
        meshRenderer.materials = newMaterials.ToArray();
        character.getInventory().storeObject(this);
    }
}
