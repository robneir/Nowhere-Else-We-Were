using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CubeManager : MonoBehaviour {

    public static CubeManager instance;

	private class DijkstraVisited
	{
		public bool visited = false;
		public int range = 0;
	};


    [Tooltip("This is the base cube height used for calculating height differences")]
    public float baseCubeSize = 19.5f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization.
    void Start()
    {
        // DO ALL PRE PASSED TO INIT CUBES ADJACENCIES
        Cube[] cubes = GameObject.FindObjectsOfType<Cube>();
        List<Cube> topCubes = new List<Cube>();
        for (int i = 0; i < cubes.Length; i++)
        {
            Cube currCube = cubes[i];
            if (currCube != null)
            {
                // If the cube is a top cube then add to top cube list.
                if (currCube.IsTopCube())
                {
                    currCube.ProcessMinMaxXZ();
                    topCubes.Add(currCube);
                }
            }
        }

        foreach (Cube outterC in topCubes)
        {
            Vector2 rightPoint = new Vector2(outterC.transform.position.x + CubeManager.instance.baseCubeSize, outterC.transform.position.z);
            Vector2 leftPoint = new Vector2(outterC.transform.position.x - CubeManager.instance.baseCubeSize, outterC.transform.position.z);
            Vector2 backPoint = new Vector2(outterC.transform.position.x, outterC.transform.position.z - CubeManager.instance.baseCubeSize);
            Vector2 forwardPoint = new Vector2(outterC.transform.position.x, outterC.transform.position.z + CubeManager.instance.baseCubeSize);
            Vector2[] pointVector = { forwardPoint, backPoint, leftPoint, rightPoint };
            foreach(Cube innerC in topCubes)
            {
                if(innerC != outterC)
                {
                    // Check to see if outter c and inner c are neighbors
                    for (int i = 0; i < pointVector.Length; i++)
                    {
                        // Check to see if the x and z values of the point are within one of the top cubes in the list.
                        if(innerC.CheckPointInsideMinMax(pointVector[i]))
                        {
                            // Add this inner cube as a neighbor then.
                            Cube.CubeDumby cubeDumby = new Cube.CubeDumby();
                            cubeDumby.cube = innerC;
                            // Calculate if we can move to the neighbor cube TODO:
                            if(Mathf.Abs(CubeManager.instance.GetHeightDifference(outterC, innerC)) <= 1 && innerC.IsMovable())
                            {
                                cubeDumby.canMoveTo = true;
                            }
                            else
                            {
                                cubeDumby.canMoveTo = false;
                            }
                            outterC.AddNeighbor(cubeDumby, i);
                        }
                    }
                }
            }
        }
    }


    public int GetHeightDifference(Cube a, Cube b)
    {
        int difference = (int)((a.transform.position.y - b.transform.position.y) / baseCubeSize);
        return difference;
    }

	public List<Cube> FindPathTowardsTarget(WorldCharacter mover, WorldCharacter target)
	{
		Cube targetCube = target.GetCurrentCube();
	
		return BFS(mover.GetCurrentCube(), int.MaxValue, mover.isPlayable, targetCube);
	}

	private List<Cube> GetNewPathToCube(Cube start, int range, bool moverGroup, Cube CubeHasOccupant)
	{
		List<Cube> ActualPossibleCubes = BFS(start, range, moverGroup);
		HashSet<Cube> cubesCanMove = new HashSet<Cube>();
		cubesCanMove.Add(start);
		foreach(Cube c in ActualPossibleCubes)
		{
			cubesCanMove.Add(c);
		}
		HashSet<Cube> cubesAlreadySearch = new HashSet<Cube>();
		Queue<Cube> cubesToSearch = new Queue<Cube>();
		cubesToSearch.Enqueue(CubeHasOccupant);
		Cube newTarget = null;
		while(cubesToSearch.Count > 0)
		{
			Cube cubeToSearch = cubesToSearch.Dequeue();
			if(cubeToSearch.GetOccupant() != null)
			{
				cubesAlreadySearch.Add(cubeToSearch);
				foreach(Cube.CubeDumby adjCDumby in cubeToSearch.GetAdjacentCubes())
				{
					if(adjCDumby.canMoveTo)
					{
						Cube adjC = adjCDumby.cube;
						if(!cubesAlreadySearch.Contains(adjC))
						{
							cubesToSearch.Enqueue(adjC);
						}
					}
				}
			}
			newTarget = cubeToSearch;
			break;
		}
		return BFS(start, range, moverGroup, newTarget);
	}

    // Breadth first search.
    public List<Cube> BFS(Cube start, int range, bool moverGroup, Cube end = null)
    {
		if(start == null)
		{
			return null;
		}
        List<Cube> pathToCube = new List<Cube>();
        // Add the starting cube as part of the path.
        //pathToCube.Insert(0, end);
        if (start == end)
        {
            return pathToCube;
        }

		// Dictionary used for breadth first search.
		Dictionary<Cube, DijkstraVisited> visitedDict = new Dictionary<Cube, DijkstraVisited>();

		Dictionary<Cube, Cube> predecessors = new Dictionary<Cube, Cube>();

		List<Cube> output = new List<Cube>();

		visitedDict.Clear();
		predecessors.Clear();

        // Perform breadth first search.
        Queue<Cube> cubesToBeSearched = new Queue<Cube>();
        // Enqueue starting cube to begin search.
        cubesToBeSearched.Enqueue(start);
		// Mark starting cube as visited.
		if(!visitedDict.ContainsKey(start))
		{
			visitedDict.Add(start, new DijkstraVisited());
		}
        visitedDict[start].visited = true;

        // While we still have unvisited cubes.
        while (cubesToBeSearched.Count > 0)
        {
            Cube currCube = cubesToBeSearched.Dequeue();
            if (end != null && currCube == end)
            {
				// Add the end cube to the path.
				pathToCube.Insert(0, currCube);

                // Iterate through predecessors and add to pathToCube.
                Cube prevCube = predecessors[currCube];
                while(prevCube != start)
                {
                    // Insert previous cube to the front of list
                    pathToCube.Insert(0, prevCube);
                    // Get the next previous cube
                    prevCube = predecessors[prevCube];
                }
				// Break out of outter while loop because we found the end cube and filled "pathToCube".
				while(pathToCube.Count > 0 && pathToCube[pathToCube.Count - 1] == end && end.GetOccupant() != null)
				{
					pathToCube.Remove(pathToCube[pathToCube.Count - 1]);
				}

				if(pathToCube.Count > 0 && pathToCube[pathToCube.Count - 1].GetOccupant() != null)
				{
					pathToCube = GetNewPathToCube(start, range, moverGroup, pathToCube[pathToCube.Count - 1]);
				}

				return pathToCube;
            }
            else
            {
                // Enqueue adjacent cubes.
                Cube.CubeDumby[] adjCubes = currCube.GetAdjacentCubes();
                for (int i = 0; i < adjCubes.Length; i++)
                {
                    Cube c = adjCubes[i].cube;
                    if (c != null)
                    {
						// "cubeInDictionary" is being reused here to see if this adjacent cube is in the dictionary.
						bool cubeInDictionary = visitedDict.ContainsKey(c);
                        if (!cubeInDictionary)
                        {
                            // Check to see if cube has not been visited and not null before enqueuing.
                            visitedDict.Add(c, new DijkstraVisited());
                        }
                        if(c.GetOccupant() == null || (c.GetOccupant() != null && ((end != null && c == end) || c.GetOccupant().GetComponent<WorldCharacter>().isPlayable == moverGroup)))
                        {
                            // Make sure height of the cube isnt too high..
                            if(adjCubes[i].canMoveTo && (visitedDict[currCube].range + (int)c.difficulty) <= range)
                            {
								if (!visitedDict[c].visited)
								{
									predecessors[c] = currCube;
									cubesToBeSearched.Enqueue(c);
									visitedDict[c].visited = true;
									visitedDict[c].range = visitedDict[currCube].range + (int)c.difficulty;
									output.Add(c);
								}
								else if(visitedDict[c].range > visitedDict[currCube].range + (int)c.difficulty)
								{
									predecessors[c] = currCube;
									cubesToBeSearched.Enqueue(c);
									visitedDict[c].visited = true;
									visitedDict[c].range = visitedDict[currCube].range + (int)c.difficulty;
									output.Add(c);
								}
							}
                        }
                    }
                }
            }
        }
		if(end == null)
		{
			return output;
		}
        return null;
    }

    // Returns the int which corresponds to the direction the cube is in.
    // If something went wrong it returns -1.
    public int FindAdjacentDirectionOfCube(Cube source, Cube sink)
    {
        Cube.CubeDumby[] adjCubes = source.GetAdjacentCubes();
        for(int i=0;i<adjCubes.Length;i++)
        {
            Cube cube = adjCubes[i].cube;
            if(cube != null && cube == sink)
            {
                return i;
            }
        }
        return -1;
    }

    // Highlights cubes passed in.
    public void SetSelectionStateOfCubes(List<Cube> cubes, Cube.SelectionState selectionState)
    {
		if(cubes == null)
		{
			return;
		}
        for (int i = 0; i < cubes.Count; i++)
        {
            Cube c = cubes[i];
            if (c != null)
            {
                c.SetSelectedState(selectionState);
            }
        }
    }

    private void GetAllAdjacentCubesInWeaponRangeHelper(Cube source, ref Dictionary<Cube, VisitedCube> allCubesInBreadthFirstSearch, ref List<Cube> resultCubes, uint[] range, uint largestRange, uint counter, Weapon.GROUP group)
    {
        // Base case stop when we have counted up to the largest value in the range list.
        if (counter > largestRange)
        {
            return;
        }

        List<Cube> adjCubesList = new List<Cube>();
        // Add all adjacent cubes to the list of cubes in range
        Cube.CubeDumby[] adjCubes = source.GetAdjacentCubes();
        for (int i = 0; i < adjCubes.Length; i++)
        {
            Cube adjCube = adjCubes[i].cube;
            if (adjCube != null)
            {
				if (!allCubesInBreadthFirstSearch.ContainsKey(adjCube))
				{
					VisitedCube stuff = new VisitedCube();
					stuff.visited = false;
					stuff.range = counter;
					allCubesInBreadthFirstSearch.Add(adjCube, stuff);
				}
				if(!allCubesInBreadthFirstSearch[adjCube].visited || (allCubesInBreadthFirstSearch[adjCube].visited && allCubesInBreadthFirstSearch[adjCube].range > counter))
				{
					allCubesInBreadthFirstSearch[adjCube].visited = true;
					allCubesInBreadthFirstSearch[adjCube].range = counter;
					if (group != Weapon.GROUP.MELEE || (group == Weapon.GROUP.MELEE && adjCubes[i].canMoveTo))
					{
						adjCubesList.Add(adjCube);
					}
				}
            }
        }

        // If the cubes that we obtained and matches one of the range values were not discovered then add them to final list.
        foreach (Cube c in adjCubesList)
        {
            // Add the cube to the resultCubes if the cube is of one of the ranges we want.
            for (int i = 0; i < range.Length; i++)
            {
                uint rangeValue = range[i];
                // If these cubes are within the range we want then add them to the resulting cubes.
                if (counter == rangeValue)
                {
					resultCubes.Add(c);
                }
            }
        }

        // Recursively call on all adjacent cubes
        counter = counter + 1;
        foreach (Cube c in adjCubesList)
        {
            if (c != null)
            {
				int extraRange = 0;
				uint[] newRange = range;
				if (group == Weapon.GROUP.RANGED)
				{
					extraRange = (GetHeightDifference(source, c));
					if (extraRange > 0)
					{
						HashSet<uint> newRanges = new HashSet<uint>();
						foreach (uint i in newRange)
						{
							newRanges.Add(i);
							if ((int)i + extraRange > 0)
							{
								newRanges.Add((uint)(i + extraRange));
							}
						}
						newRange = new uint[newRanges.Count];
						int index = 0;
						foreach (uint i in newRanges)
						{
							newRange[index] = i;
							index++;
						}
					}
					GetAllAdjacentCubesInWeaponRangeHelper(c, ref allCubesInBreadthFirstSearch, ref resultCubes, newRange, (int)largestRange + extraRange > 0 ? largestRange + (uint)extraRange : 0, counter, group);
				}
				else
				{
					GetAllAdjacentCubesInWeaponRangeHelper(c, ref allCubesInBreadthFirstSearch, ref resultCubes, range, largestRange, counter, group);
				}
            }
        }
    }

	private class VisitedCube
	{
		public bool visited = false;
		public uint range = 0;
	};

	// Used for finding cube range for weapon
	public List<Cube> GetAllAdjacentCubesInWeaponRange(Cube source, uint[] weaponRange, Weapon.GROUP WeaponGroup) 
    {
		
        Dictionary<Cube, VisitedCube> allCubesHitByBreadthFirstSearch = new Dictionary<Cube, VisitedCube>();
        List<Cube> cubesInAttackRange = new List<Cube>();
        uint largestRange = 0;
        for(int i=0;i< weaponRange.Length;i++)
        {
            uint currRange = weaponRange[i];
            if(currRange>largestRange)
            {
                largestRange = currRange;
            }
        }
		// Show range that occupant can move
		GetAllAdjacentCubesInWeaponRangeHelper(source, ref allCubesHitByBreadthFirstSearch, ref cubesInAttackRange, weaponRange, largestRange, 1, WeaponGroup);
		cubesInAttackRange.Remove(source);
        return cubesInAttackRange;
    }
}
