using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialSetter : MonoBehaviour
{
    public bool isEnabled = true;

    [SerializeField]
    [Tooltip("Material wanted for showing disabled versus enabled state.")]
    private Material disabledMat;

    [SerializeField]
    [Tooltip("All the meshes that we want affected by this script.")]
    private List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>();

    private List<Material> originalMaterials = new List<Material>();

    // Use this for initialization
    void Start()
    {
        // Add all original materials to list to then use later when reseting materials to enabled state.
        foreach (SkinnedMeshRenderer s in meshes)
        {
            originalMaterials.Add(s.material);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowDisabledMaterial()
    {
        isEnabled = false;
        foreach (SkinnedMeshRenderer s in meshes)
        {
            s.material = disabledMat;
        }
    }

    public void ShowEnabledMaterial()
    {
        isEnabled = true;
        for (int i=0; i< meshes.Count;i++)
        {
            // Check to make sure we have a matching material for this mesh.
            if (i < originalMaterials.Count)
            {
                // Set original material.
                Material originalMat = originalMaterials[i];
                meshes[i].material = originalMat;
            }
            else
            {
                Debug.Log("No matching enabled material for this mesh.");
            }
        }
    }
}
