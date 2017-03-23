using UnityEngine;
using System.Collections;

public class TextureScroll : MonoBehaviour {

    [SerializeField]
    private float scrollSpeed;

    private MeshRenderer meshRenderer;

    private float currOffset;

	// Use this for initialization
	void Start () {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
    }
	
	// Update is called once per frame
	void Update () {
        currOffset += scrollSpeed * Time.deltaTime;
        meshRenderer.material.mainTextureOffset = new Vector2(currOffset, 0);
	}
}
