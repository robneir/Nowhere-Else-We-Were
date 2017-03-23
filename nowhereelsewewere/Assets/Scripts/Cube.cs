using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Cube : MonoBehaviour {

    [SerializeField]
    [Tooltip("Distance raycast for finding adjacent cubes will travel")]
    public static float adjacentCubeRaycastDistance = 14.0f;

    [SerializeField]
    [Tooltip("Layer mask for raycast for determining this cube's neighbours.")]
    private LayerMask defaultLayer;

    [SerializeField]
    [Tooltip("Layer mask for raycasting up to see if a cube is on top of this cube.")]
    private LayerMask colliderLayer;

    [SerializeField]
    [Tooltip("If the cube should be able to be moved to. As in nothing blocking cube on top of it.")]
    private bool isMovable = true;

    [Tooltip("Sprite to display when cube within range for move")]
    private SpriteRenderer gridSprite;

    [SerializeField]
    [Tooltip("Offset for grid sprite on top of cube so it is above cube instead of inside it.")]
    private Vector3 gridSpriteOffset;

    [SerializeField]
    [Tooltip("Offset applied to anything that is on this cube.")]
    private Vector3 occupantOffset = new Vector3(0.0f, 9.0f, 0.0f);

    [SerializeField]
    [Tooltip("Name of the type of cube")]
    private string cubeType;

    [Tooltip("1 for normal terrain, more for more difficult.")]
    public uint difficulty = 1;

	[Tooltip("0 for normal terrain, any more will add dodge to the character on this cube.")]
	public uint hitDifficulty = 0;

    [HideInInspector]
    public List<Pickup> pickUps = new List<Pickup>();

    // Whoever/whatever character or enemy that is occupying the cube
    private GameObject occupant;

    private bool isTopCube = false;

    public enum SelectionState
    {
        Selected,
        InMoveRange,
        InPath,
        InAttackRange,
        Attacking,
        NotSelected
    }

    private SelectionState selectionState;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    
    // Enum must be filled only with integers from (0-(maxAdjacentCubes-1))
    public enum AdjacentDirections
    {
        FORWARD = 0,
        BACK = 1,
        LEFT = 2,
        RIGHT = 3,
    }

    public struct CubeDumby
    {
        public Cube cube;
        public bool canMoveTo;
    }
    // Cube array to hold all cubes that are adjacent to this cube
    /*  indices: these indices correspond to the "Adjacents" enum
        0 = forward
        1 = back
        2 = left
        3 = right
    */
    private CubeDumby[] adjCubes = new CubeDumby[4];

    void Awake()
    {
        ProcessIsTopCube();
    }

	// Use this for initialization
	void Start ()
    {
        gridSpriteOffset= new Vector3(0, 10.1f, 0.0f);
        // Instantiate grid sprite
        SpriteRenderer gridSpritePrefab = Resources.Load<SpriteRenderer>("Grid_Sprite");
        gridSprite = Instantiate(gridSpritePrefab, this.transform, false) as SpriteRenderer;
        gridSprite.transform.position = this.transform.position + gridSpriteOffset;
        SetSelectedState(SelectionState.NotSelected);
    }

    public void SetOccupant(GameObject occupant)
    {
		this.occupant = occupant;
    }

    public GameObject GetOccupant()
    {
        return occupant;
    }

    // Raycast up and set selectable and destroy trigger collider if cube not selectable
    private bool ProcessIsTopCube()
    {
        RaycastHit hit;
        // Check to see if there is a cube on top of this cube.
        if (Physics.Raycast(this.transform.position, Vector3.up, out hit, adjacentCubeRaycastDistance, colliderLayer))
        {
            GameObject gameObjectHit = hit.collider.gameObject.transform.parent.gameObject;
            if (this.gameObject != gameObjectHit && gameObjectHit.tag == "Cube")
            {
                this.isTopCube = false;
                return false;
            }
        }
        // Cube is a top cube if gets here.
        this.isTopCube = true;
        return true;
    }

    // Processes the bounds of each cube and should be called only if the cube is a top cube.
    public void ProcessMinMaxXZ()
    {
        minX = this.transform.position.x - CubeManager.instance.baseCubeSize / 2;
        maxX = this.transform.position.x + CubeManager.instance.baseCubeSize / 2;
        minZ = this.transform.position.z - CubeManager.instance.baseCubeSize / 2;
        maxZ = this.transform.position.z + CubeManager.instance.baseCubeSize / 2;
    }

    public bool CheckPointInsideMinMax(Vector2 point)
    {
        if(point.x <= maxX && 
            point.x >= minX && 
            point.y >= minZ && 
            point.y <= maxZ)
        {
            return true;
        }
        return false;
    }

    /*  indices: these indices correspond to the "Adjacents" enum
        0 = forward
        1 = back
        2 = left
        3 = right
    */
    public void AddNeighbor(CubeDumby cube, int neighborIndex)
    {
        if(neighborIndex >= 4 && neighborIndex < 0)
        {
            return;
        }
        adjCubes[neighborIndex] = cube;
    }

    /* 
    Gets adjacent cube in specified direction.
    Will return null if no cube in that direction.
    @param direction - enum that chooses direction to get the adjacent cube
    */
    public CubeDumby GetAdjacentCube(AdjacentDirections direction)
    {
        return adjCubes[(int)direction];
    } 

    public CubeDumby[] GetAdjacentCubes()
    {
        return adjCubes;
    }

    public void SetSelectedState(SelectionState selected)
    {
        selectionState = selected;
		if (gridSprite)
		{
			SpriteRenderer spriteRenderer = gridSprite.GetComponent<SpriteRenderer>();
			if (spriteRenderer)
			{
				spriteRenderer.gameObject.SetActive(true);
				switch (selectionState)
				{
					case SelectionState.Selected:
						spriteRenderer.color = Constants.instance.selectedColor;
						break;
					case SelectionState.InMoveRange:
						spriteRenderer.color = Constants.instance.inMoveRangeColor;
						break;
					case SelectionState.InPath:
						spriteRenderer.color = Constants.instance.inPathColor;
						break;
					case SelectionState.InAttackRange:
						spriteRenderer.color = Constants.instance.inAttackRangeColor;
						break;
					case SelectionState.Attacking:
						spriteRenderer.color = Constants.instance.attackingColor;
						break;
					case SelectionState.NotSelected:
                        //spriteRenderer.gameObject.SetActive(false);
                        spriteRenderer.color = Constants.instance.notSelectedColor;
						break;
				}
			}
		}
    }

    public bool IsMovable()
    {
        return isMovable;
    }

    public bool IsTopCube()
    {
        return isTopCube;
    }

    public Vector3 GetCubeOffset()
    {
        return occupantOffset;
    }

	public string GetCubeType()
	{
		return cubeType;
	}
}
