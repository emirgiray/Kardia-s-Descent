using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

//[RequireComponent(typeof(PathIllustrator))]
public class Pathfinder : MonoBehaviour
{
    //public static Pathfinder Instance { get; private set; }
    [SerializeField] private bool illustratePath = false;
    [ShowIf("illustratePath")]
    public PathIllustrator illustrator;
    public List<Tile> attackableTiles = new List<Tile>();

    /// <summary>
    /// Main pathfinding function, marks tiles as being in frontier, while keeping a copy of the frontier
    /// in "currentFrontier" for later clearing
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="destination"></param>
    /// <param name="forAIPathfinding">this is for AI pathfinding bc without addng this ai cant get the destination since its occupied</param>
    /// <returns></returns>
    public Path FindPath(Character activator, Tile origin, Tile destination, bool forAIPathfinding = false)
    {
        
        List<Tile> openSet = new List<Tile>();
        List<Tile> closedSet = new List<Tile>();
        
        openSet.Add(origin);
        origin.costFromOrigin = 0;
        
        /*foreach (Tile tile in openSet)
        {
            tile.Highlight(Color.red);
        }*/

        float tileDistance = origin.GetComponent<MeshFilter>().sharedMesh.bounds.extents.z * 2;
        
        while (openSet.Count > 0)
        {
            openSet.Sort((x, y) => x.TotalCost.CompareTo(y.TotalCost));
            Tile currentTile = openSet[0];

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);
            
            //Destination reached
            if (currentTile == destination)
            {  
                return PathBetween(activator, destination, origin, forAIPathfinding);
            }
            else //does this work???
            {
                if (destination.Occupied && Vector3.Distance(currentTile.transform.position, destination.transform.position) <= (origin.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * PathfinderVariables.Instance.HEXAGONAL_OFFSET))
                {
                    return PathBetween(activator, currentTile, origin, forAIPathfinding);
                }
            }

            

            foreach (Tile neighbor in NeighborTiles(currentTile, forAIPathfinding))
            {
                /*neighbor.Highlight(Color.yellow);*/
                
                if(closedSet.Contains(neighbor))
                    continue;
                
                float costToNeighbor = currentTile.costFromOrigin + neighbor.terrainCost + tileDistance;
                if (costToNeighbor < neighbor.costFromOrigin || !openSet.Contains(neighbor))
                {
                    neighbor.costFromOrigin = costToNeighbor;
                    neighbor.costToDestination = Vector3.Distance(destination.transform.position, neighbor.transform.position);
                    neighbor.parent = currentTile;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
        
    }

    /// <summary>
    /// Returns a list of all neighboring hexagonal tiles and ladders
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="forAIPathfinding">this is for AI pathfinding bc without adding this ai cant get the destination since its occupied</param>
    /// <param name="forAttackRange">this is for attack range bc without addng this character cant get the destination since its occupied</param>
    /// <returns></returns>
    public List<Tile> NeighborTiles(Tile origin, bool ignoreOccupied = false, bool forAttackTiles = false)
    {
        
        List<Tile> tiles = new List<Tile>();
        Vector3 direction = Vector3.forward * (origin.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * PathfinderVariables.Instance.HEXAGONAL_OFFSET);
        float rayLength = 4f;
        float rayHeightOffset = 1f;

        //Rotate a raycast in 60 degree steps and find all adjacent tiles
        for (int i = 0; i < 6; i++)
        {
            direction = Quaternion.Euler(0f, 60f, 0f) * direction;

            Vector3 aboveTilePos = (origin.transform.position + direction).With(y: origin.transform.position.y + rayHeightOffset);

            if (Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit, rayLength, PathfinderVariables.Instance.tileMask))
            {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                if (hitTile.selectable)
                {
                    if (hitTile.Occupied == false)
                    {
                        tiles.Add(hitTile);
                    }
                    else
                    {
                        if (ignoreOccupied)
                        {
                            if (hitTile.occupyingCoverPoint == null)
                            {
                                tiles.Add(hitTile);
                            }

                            if (forAttackTiles)
                            {
                                tiles.Add(hitTile);
                            }

                        }
                    }
                }

                /*foreach (var tile in tiles)
                {
                    tile.Highlight(Color.white);
                }*/
                
                    
            }

            Debug.DrawRay(aboveTilePos, Vector3.down * rayLength, Color.blue);
        }

        //Additionally add connected tiles such as ladders
        if (origin.connectedTile != null)
            tiles.Add(origin.connectedTile);

        /*foreach (var tile in tiles)
        {
            tile.Highlight(Color.blue);
        }*/

        return tiles;
    }

    /// <summary>
    /// Called by Interact.cs to create a path between two tiles on the grid 
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    private Path PathBetween(Character activator, Tile dest, Tile source, bool forAIPathfinding = false)
    {
        Path path = MakePath(dest, source);

        /*if (illustratePath)
        {
            /*illustrator.IllustratePath(path);#1#
        }*/
        
        return path;
    }

    public void ClearIllustratedPath()
    {
        if (illustratePath)
        {
            illustrator.ClearIllustratedPath();
        }
    }

    public void EnableIllustratePath(bool value)
    {
        if (illustratePath)
        {
            illustrator.EnableIllustratePath(value);
        }
    }

    /// <summary>
    /// Creates a path between two tiles
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    private Path MakePath(Tile destination, Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        Tile current = destination;

        while (current != origin)
        {
            tiles.Add(current);
            if (current.parent != null)
                current = current.parent;
            else
                break;
        }

        tiles.Add(origin);
        tiles.Reverse();

        Path path = new Path();
        path.tiles = tiles;

        return path;
    }

    /// <summary>
    /// Returns a list of all tiles within range of a character
    /// </summary>
    /// <param name="selectedCharacterCharacterTile"></param>
    /// <param name="selectedCharacterMoveRange"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public List<Tile> GetReachableTiles(Tile selectedCharacterCharacterTile, int selectedCharacterMoveRange/*, Tile origin*/)
    {
        
        
        /*List<Tile> tiles = NeighborTiles(selectedCharacterCharacterTile);
       List<Tile> reachableTiles = new List<Tile>();
       foreach (Tile tile in tiles)
       {
           Path path = FindPath(selectedCharacterCharacterTile, tile);
           if (path != null && path.tiles.Length <= selectedCharacterMoveRange + 1)
           {
               reachableTiles.Add(tile);
           }
       }

       return reachableTiles;*/
        
        List<Tile> tiles = new List<Tile>();
        tiles.Add(selectedCharacterCharacterTile);
        List<Tile> frontier = new List<Tile>();
        frontier.Add(selectedCharacterCharacterTile);

        int currentRange = 0;
        while (currentRange < selectedCharacterMoveRange )
        {
            List<Tile> newFrontier = new List<Tile>();

            foreach (Tile tile in frontier)
            {
                foreach (Tile neighbor in NeighborTiles(tile))
                {
                    if (neighbor.Occupied == false && !tiles.Contains(neighbor))
                    {
                        newFrontier.Add(neighbor);
                        tiles.Add(neighbor);
                    }
                }
            }

            frontier = newFrontier;
            currentRange++;
        }

        return tiles;
        
        /*const float HEXAGONAL_OFFSET = 1.75f;
        List<Tile> tiles = new List<Tile>();
        Vector3 direction = Vector3.forward * (origin.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * HEXAGONAL_OFFSET);
        float rayLength = 4f;
        float rayHeightOffset = 1f;

        //Rotate a raycast in 60 degree steps and find all adjacent tiles
        for (int i = 0; i < 6; i++)
        {
            direction = Quaternion.Euler(0f, 60f, 0f) * direction;

            Vector3 aboveTilePos = (origin.transform.position + direction).With(y: origin.transform.position.y + rayHeightOffset);

            if (Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit, rayLength * selectedCharacterMoveRange, tileMask))
            {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                if (hitTile.Occupied == false)
                    tiles.Add(hitTile);
            }

            Debug.DrawRay(aboveTilePos, Vector3.down * rayLength * selectedCharacterMoveRange, Color.blue);
        }

        //Additionally add connected tiles such as ladders
        if (origin.connectedTile != null)
            tiles.Add(origin.connectedTile);

        return tiles;*/
        
    }

     private List<Tile> effectedTiles;
    private List<Tile> innerEffectedTiles;
    private List<Tile> outerEffectedTiles;
    public List<Tile> GetAttackableTiles(Character character ,SkillContainer.Skills selectedSkill, Tile characterTile, Tile targetTile, out Action OnCompleteAdded)
    { 
        if (character.TryGetComponent(out StateController stateController))
        {
            stateController.forcedTargetPlayerTile = null;
        }
        OnCompleteAdded = null;
        switch (selectedSkill.skillData.skillTargetType)
        {
            case SkillsData.SkillTargetType.AreaAroundSelf: //default
                
                attackableTiles = GetAttackableTilesDefault(characterTile, selectedSkill);
                return attackableTiles;
                break;
            
            case SkillsData.SkillTargetType.AreaAroundTarget:
                
                attackableTiles = GetAttackableTilesArea(characterTile, selectedSkill, 
                    targetTile, out effectedTiles, out innerEffectedTiles, out outerEffectedTiles);
               
                // other than the origin tile keep effected tiles around the target in a list
                character.SkillContainer.effectedTiles = effectedTiles;
                character.SkillContainer.innerEffectedTiles = innerEffectedTiles;
                character.SkillContainer.outerEffectedTiles = outerEffectedTiles;
                return attackableTiles;
                break;
            
            case SkillsData.SkillTargetType.Suicide:
                
                attackableTiles = GetAttackableTilesArea(characterTile, selectedSkill, 
                    targetTile, out effectedTiles, out innerEffectedTiles, out outerEffectedTiles);
                // other than the origin tile keep effected tiles around the target in a list
                character.SkillContainer.effectedTiles = effectedTiles;
                character.SkillContainer.innerEffectedTiles = innerEffectedTiles;
                character.SkillContainer.outerEffectedTiles = outerEffectedTiles;

                if (stateController)
                {
                    stateController.forcedTargetPlayerTile = characterTile;
                    OnCompleteAdded = () =>
                    {
                        // EndTurn();
                        character.health.HealthDecrease(character.health.Max);
                        character.diedOnSelfTurn = true;
                    };
                }
                
                return attackableTiles;
                break;
            
            case SkillsData.SkillTargetType.Line:
                
                attackableTiles = GetAttackableTilesLine(character, characterTile, targetTile, selectedSkill, out effectedTiles);
                character.SkillContainer.effectedTiles = effectedTiles;
                return attackableTiles;
                break;
            
            case SkillsData.SkillTargetType.Cone:
                throw new NotImplementedException();
                return attackableTiles;
                break;
            
            case SkillsData.SkillTargetType.Cleave:
                
                attackableTiles = GetAttackableTilesCleave(characterTile, selectedSkill, targetTile, 
                    selectedSkill.skillData.cleaveStartLeft, selectedSkill.skillData.cleaveTimes, out effectedTiles);
                
                character.SkillContainer.effectedTiles = effectedTiles;
                return attackableTiles;
                break;
        }

        if (stateController)
        {
            stateController.forcedTargetPlayerTile = null;
        }
        return attackableTiles;
        
    }
    
      public List<Tile> GetAttackableTilesDefault(Tile selectedCharacterCharacterTile, SkillContainer.Skills skill)
    {
        List<Tile> tiles = new List<Tile>();
        tiles.Add(selectedCharacterCharacterTile);
        List<Tile> frontier = new List<Tile>();
        frontier.Add(selectedCharacterCharacterTile);

        int currentRange = 0;
        while (currentRange < skill.range )
        {
            List<Tile> newFrontier = new List<Tile>();

            foreach (Tile tile in frontier)
            {
                foreach (Tile neighbor in NeighborTiles(tile, true, true))
                {
                    if (skill.skillData.skillType == SkillsData.SkillType.Ranged)
                    {
                        if (/*neighbor.Occupied == false && */!tiles.Contains(neighbor))
                        {
                            newFrontier.Add(neighbor);
                            tiles.Add(neighbor);
                            //also add verticle tiles with raycasting up
                            //Debug.DrawRay(neighbor.transform.position, Vector3.up * tileVerticalityLenght, Color.yellow);
                            if (Physics.Raycast(neighbor.transform.position, Vector3.up, out RaycastHit hit, PathfinderVariables.Instance.tileVerticalityLenght, PathfinderVariables.Instance.tileMask))
                            {
                                newFrontier.Add(hit.transform.GetComponent<Tile>());
                                tiles.Add(hit.transform.GetComponent<Tile>());
                            }
                        }
                    }
                    else //if the skill is melee
                    {
                        if (!tiles.Contains(neighbor))
                        {
                            newFrontier.Add(neighbor);
                            tiles.Add(neighbor);
                       
                        }
                    }
                }
            }
            
            frontier = newFrontier;
            currentRange++;
        }
        
        //remove the ones directly below
        List<Tile> tilesToRemove = new List<Tile>();
        
        foreach (var tile in tiles)
        {
            if (Physics.Raycast(tile.transform.position, Vector3.down, out RaycastHit hit, PathfinderVariables.Instance.tileVerticalityLenght +1 , PathfinderVariables.Instance.tileMask))
            {
                /*Debug.Log($"tile: {selectedCharacterCharacterTile.name} = {selectedCharacterCharacterTile.transform.position.y}" +
                          $"tile: {hit.transform.GetComponent<Tile>().name} =  {hit.transform.GetComponent<Tile>().transform.position.y}");*/
                if (selectedCharacterCharacterTile.transform.position.y != hit.transform.GetComponent<Tile>().transform.position.y)
                {
                    tilesToRemove.Add(hit.transform.GetComponent<Tile>());
                }
                
            }
        }

        foreach (var tile in tilesToRemove)
        {
            tiles.Remove(tile);
        }

        // Debug.Log($"tiles: {tiles.Count}");
        return tiles;
    }
      
    public List<Tile> GetAttackableTilesLine(Character activatorCharacter, Tile originTile, Tile targetTile, SkillContainer.Skills skill, out List<Tile> effectedTilesList)
    {
        // Path path = FindPath(originTile.occupyingCharacter, originTile, targetTile);
        Vector3 dir = (targetTile.transform.position - originTile.transform.position).normalized;
        // Debug.Log($"dir: {dir}");
        Path path = new Path();
        path.tiles = new List<Tile>();
        path.tiles.Add(originTile);
        //path.tiles.Add(originTile);
        Tile currentTile = originTile;
        for (int i = 0; i < skill.range; i++)
        {
            foreach (var neigbour in NeighborTiles(currentTile, true))
            {
                if ((neigbour.transform.position - originTile.transform.position).normalized == dir && !path.tiles.Contains(neigbour))
                {
                    if (activatorCharacter is Player && !neigbour.occupiedByPlayer)
                    {
                        path.tiles.Add(neigbour);
                        currentTile = neigbour;
                    }
                    if (activatorCharacter is Enemy && !neigbour.occupiedByEnemy)
                    {
                        path.tiles.Add(neigbour);
                        currentTile = neigbour;
                    }
                }
            }
        }
        
        
        List<Tile> tiles = new List<Tile>();
        tiles = path.tiles;
        //remove if not in the same direction
        
        List<Tile> tilesToRemove = new List<Tile>();
        foreach (var tile in tilesToRemove)
        {
            tiles.Remove(tile);
        }
        if (tiles.Count == 1 && tiles[0] == targetTile && !NeighborTiles(originTile, true, true).Contains(targetTile))
        {
            tiles.Remove(tiles[0]);
        }
        
        effectedTilesList = tiles;
        
        // calculate the cleave effected tiles around the impact tile
        // this finds a relative forward direction from the selected character to the impact tile
        // then it finds the tiles on the left or right of the forward direction
        List<Tile> tempAttackablesTilesList = new List<Tile>();
        List<Tile> tempAttackablesOuterTilesList = new List<Tile>();
        Vector3 direction = Vector3.forward * (originTile.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * PathfinderVariables.Instance.HEXAGONAL_OFFSET);
        float rayLength = 4f;
        float rayHeightOffset = 1f;
        for (int i = 0; i < 3; i++)
        {
            direction = Quaternion.Euler(0f, 120, 0f) * direction;
            Vector3 aboveTilePos = (originTile.transform.position + direction).With(y: originTile.transform.position.y + rayHeightOffset);
            if (Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit2, rayLength, PathfinderVariables.Instance.tileMask))
            {
                Tile hitTile = hit2.transform.GetComponent<Tile>();
                tempAttackablesTilesList.Add(hitTile);
                
                Vector3 newDir = (hitTile.transform.position - originTile.transform.position).normalized;
                Tile currentTile2 = hitTile;
                for (int j = 0; j < skill.range - 1; j++)
                {
                    foreach (var neigbour in NeighborTiles(currentTile2, true, true))
                    {
                        if ((neigbour.transform.position - originTile.transform.position).normalized == newDir && !tempAttackablesTilesList.Contains(neigbour))
                        {
                            tempAttackablesOuterTilesList.Add(neigbour);
                            currentTile2 = neigbour;
                        }
                    }
                }
                
            }

        }
        
        tempAttackablesTilesList.AddRange(tempAttackablesOuterTilesList);
        attackableTiles = tempAttackablesTilesList;
        
        return attackableTiles;
    }

    public List<Tile> GetAttackableTilesArea(Tile selectedCharacterCharacterTile, SkillContainer.Skills skill, Tile impactOriginTile, out List<Tile> effectedTilesList, out List<Tile> innerEffectedTilesList, out List<Tile> outerEffectedTilesList)
    {
        List<Tile> attackableTiles = new List<Tile>();
        attackableTiles.Add(selectedCharacterCharacterTile);
        List<Tile> frontier = new List<Tile>();
        frontier.Add(selectedCharacterCharacterTile);

        int currentRange = 0;
        while (currentRange < skill.range )
        {
            List<Tile> newFrontier = new List<Tile>();

            foreach (Tile tile in frontier)
            {
                foreach (Tile neighbor in NeighborTiles(tile, true, true))
                {
                    if (skill.skillData.skillType == SkillsData.SkillType.Ranged)
                    {
                        if (/*neighbor.Occupied == false && */!attackableTiles.Contains(neighbor))
                        {
                            newFrontier.Add(neighbor);
                            attackableTiles.Add(neighbor);
                            //also add verticle tiles with raycasting up
                            //Debug.DrawRay(neighbor.transform.position, Vector3.up * tileVerticalityLenght, Color.yellow);
                            if (Physics.Raycast(neighbor.transform.position, Vector3.up, out RaycastHit hit, PathfinderVariables.Instance.tileVerticalityLenght, PathfinderVariables.Instance.tileMask))
                            {
                                newFrontier.Add(hit.transform.GetComponent<Tile>());
                                attackableTiles.Add(hit.transform.GetComponent<Tile>());
                            }
                        }
                    }
                }
            }
            
            frontier = newFrontier;
            currentRange++;
        }
        
        //remove the ones directly below
        List<Tile> tilesToRemove = new List<Tile>();
        
        foreach (var tile in attackableTiles)
        {
            if (Physics.Raycast(tile.transform.position, Vector3.down, out RaycastHit hit, PathfinderVariables.Instance.tileVerticalityLenght +1 , PathfinderVariables.Instance.tileMask))
            {
                /*Debug.Log($"tile: {selectedCharacterCharacterTile.name} = {selectedCharacterCharacterTile.transform.position.y}" +
                          $"tile: {hit.transform.GetComponent<Tile>().name} =  {hit.transform.GetComponent<Tile>().transform.position.y}");*/
                if (selectedCharacterCharacterTile.transform.position.y != hit.transform.GetComponent<Tile>().transform.position.y)
                {
                    tilesToRemove.Add(hit.transform.GetComponent<Tile>());
                }
                
            }
        }

        foreach (var tile in tilesToRemove)
        {
            attackableTiles.Remove(tile);
        }
        
        // calculate the effected tiles around the impact tile (selectedTile)
        // all the effected tiles
        effectedTilesList = GetNeighbouringTiles(impactOriginTile, skill.skillData.impactRange, skill.skillData.impactRange == 1); //if the impact range is 1, include the origin tile bc it was buggy
        
        //effectedTilesList.Add(impactOriginTile);
        // the inner effected tiles for full damage
        innerEffectedTilesList = GetNeighbouringTiles(impactOriginTile, skill.skillData.innerImpactRadius, true);
        //innerEffectedTilesList.Add(impactOriginTile);
        // the outer effected tiles for less damage
        List<Tile> temp = new List<Tile>();
        
        foreach (var tile in effectedTilesList)
        {
            if (!innerEffectedTilesList.Contains(tile))
            {
                temp.Add(tile);
            }
        }
        
        outerEffectedTilesList = temp;

        return attackableTiles;

    }


    public List<Tile> GetAttackableTilesCleave(Tile selectedCharacterCharacterTile, SkillContainer.Skills skill, Tile targetTile, bool startFromLeft, int cleaveTimes, out List<Tile> effectedTilesList)
    {
        List<Tile> attackableTiles = new List<Tile>();
        attackableTiles.Add(selectedCharacterCharacterTile);
        List<Tile> frontier = new List<Tile>();
        frontier.Add(selectedCharacterCharacterTile);

        int currentRange = 0;
        while (currentRange < skill.range)
        {
            List<Tile> newFrontier = new List<Tile>();

            foreach (Tile tile in frontier)
            {
                foreach (Tile neighbor in NeighborTiles(tile, true, true))
                {
                    if (skill.skillData.skillType == SkillsData.SkillType.Ranged)
                    {
                        if ( /*neighbor.Occupied == false && */!attackableTiles.Contains(neighbor))
                        {
                            newFrontier.Add(neighbor);
                            attackableTiles.Add(neighbor);
                            //also add verticle tiles with raycasting up
                            //Debug.DrawRay(neighbor.transform.position, Vector3.up * tileVerticalityLenght, Color.yellow);
                            if (Physics.Raycast(neighbor.transform.position, Vector3.up, out RaycastHit hit,
                                    PathfinderVariables.Instance.tileVerticalityLenght,
                                    PathfinderVariables.Instance.tileMask))
                            {
                                newFrontier.Add(hit.transform.GetComponent<Tile>());
                                attackableTiles.Add(hit.transform.GetComponent<Tile>());
                            }
                        }
                    }
                }
            }

            frontier = newFrontier;
            currentRange++;
        }

        //remove the ones directly below
        List<Tile> tilesToRemove = new List<Tile>();

        foreach (var tile in attackableTiles)
        {
            if (Physics.Raycast(tile.transform.position, Vector3.down, out RaycastHit hit,
                    PathfinderVariables.Instance.tileVerticalityLenght + 1, PathfinderVariables.Instance.tileMask))
            {
                /*Debug.Log($"tile: {selectedCharacterCharacterTile.name} = {selectedCharacterCharacterTile.transform.position.y}" +
                          $"tile: {hit.transform.GetComponent<Tile>().name} =  {hit.transform.GetComponent<Tile>().transform.position.y}");*/
                if (selectedCharacterCharacterTile.transform.position.y !=
                    hit.transform.GetComponent<Tile>().transform.position.y)
                {
                    tilesToRemove.Add(hit.transform.GetComponent<Tile>());
                }

            }
        }

        foreach (var tile in tilesToRemove)
        {
            attackableTiles.Remove(tile);
        }
        
        // calculate the cleave effected tiles around the impact tile
        // this finds a relative forward direction from the selected character to the impact tile
        // then it finds the tiles on the left or right of the forward direction
        Vector3 forwardDir = (targetTile.transform.position - selectedCharacterCharacterTile.transform.position).normalized;
        Vector3 direction = forwardDir * (targetTile.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * PathfinderVariables.Instance.HEXAGONAL_OFFSET);
        float rayLength = 4f;
        float rayHeightOffset = 1f;

        List<Tile> tempEffectedTilesList = new List<Tile>();
        for (int i = 0; i < cleaveTimes; i++)
        {
            // direction = Quaternion.Euler(0f, startFromLeft ? 240 : -240, 0f) * direction; //first iteration
            //Vector3 aboveTilePos = (targetTile.transform.position + direction).With(y: targetTile.transform.position.y + rayHeightOffset); //first iteration, cleaveTimes = 1 works great but cleaveTimes = 2,3 is off but interesting
            direction = i == 0 ? Quaternion.Euler(0f, startFromLeft ? 240 : -240, 0f) * direction : Quaternion.Euler(0f, startFromLeft ? 300 : -120, 0f) * direction;
            var aboveTilePos = i == 0 ? (targetTile.transform.position + direction).With(y: targetTile.transform.position.y + rayHeightOffset) : (tempEffectedTilesList[tempEffectedTilesList.Count - 1].transform.position + direction).With(y: tempEffectedTilesList[tempEffectedTilesList.Count - 1].transform.position.y + rayHeightOffset);
            
            if (Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit2, rayLength, PathfinderVariables.Instance.tileMask))
            {
                Tile hitTile = hit2.transform.GetComponent<Tile>();
                tempEffectedTilesList.Add(hitTile);
            }
        }

        effectedTilesList = tempEffectedTilesList;
        effectedTilesList.Add(targetTile);
        return attackableTiles;
    }

    public List<Tile> GetNeighbouringTiles(Tile origin, int range, bool includeOrigin)
    {
        List<Tile> tiles = new List<Tile>();
        List<Tile> frontier = new List<Tile>();
        frontier.Add(origin);

        int currentRange = 0;
        while (currentRange < range)
        {
            List<Tile> newFrontier = new List<Tile>();

            foreach (Tile tile in frontier)
            {
                foreach (Tile neighbor in NeighborTiles(tile, true, true))
                {
                    if (!tiles.Contains(neighbor))
                    {
                        newFrontier.Add(neighbor);
                        tiles.Add(neighbor);
                    }
                }
            }

            frontier = newFrontier;
            currentRange++;
        }

        if (includeOrigin)
        {
            tiles.Add(origin);
        }
        return tiles;
    }
      
    [Button]
    public List<Tile> GetTilesInBetween(Character activator,  Tile origin, Tile destination, bool forAIPathfinding = false)
    {
        //return the list of tiles between the two tiles
        Path path = FindPath(activator, origin, destination, forAIPathfinding);
        //print(path);
        List<Tile> tiles = new List<Tile>();
        //print(tiles);
        foreach (Tile tile in path.tiles)
        {
            if (tile != origin && tile != destination)
            {
                tiles.Add(tile);
                //tile.Highlight(Color.white);
            }
            
        }

        return tiles;
       
    }
    [Button]
    public Path GetPathBetween(Character activator, Tile origin, Tile destination, bool forAIPathfinding = false)
    {
        //return the path between the two tiles
        /*Path path = Pathfinder.Instance.FindPath(origin,
            Pathfinder.Instance.GetTilesInBetween(origin,
                destination, forAIPathfinding)[Pathfinder.Instance.GetTilesInBetween(origin,
                destination, forAIPathfinding).Count - 1]);*/

        Path path = FindPath(activator, origin, destination, forAIPathfinding); 
        
        // path.tiles = path.tiles.Skip(1).ToArray();
        /*List<Tile> list = new List<Tile>(path.tiles);
        list.RemoveAt(0);
        path.tiles = list.ToArray();*/
        
        /*Path path = FindPath(origin, destination, forAIPathfinding);*/
        /*foreach (var VARIABLE in path.tiles)
        {
            VARIABLE.Highlight(Color.white);
        }*/
        return path;
       
    }

    [Button]
    public bool CheckCoverPoint(Tile attacking, Tile defending, bool sphereCast, float sphereRaidus = 0.5f)
    {
       //check if the defending character is in cover
       Vector3 attackingPos = new Vector3(attacking.transform.position.x, attacking.transform.position.y + PathfinderVariables.Instance.characterYOffset, attacking.transform.position.z);
       Vector3 defendingPos = new Vector3(defending.transform.position.x, defending.transform.position.y + PathfinderVariables.Instance.characterYOffset, defending.transform.position.z);
       
      
       if (!sphereCast)
       { 
           //Debug.DrawRay(defendingPos, (attackingPos - defendingPos).normalized * coverPointRayLenght,  Color.red);
           if (Physics.Raycast(defendingPos, (attackingPos - defendingPos).normalized, out RaycastHit hit, PathfinderVariables.Instance.coverPointRayLenght, PathfinderVariables.Instance.coverPointMask))
           {
               //Debug.Log($"defender {defending} is in cover");
               return true;
           }
       
           if (defending.occupiedByPlayer && defending.isCoveredByCoverPoint)
           {
               if (Physics.Raycast(defendingPos, (attackingPos - defendingPos).normalized, out RaycastHit hit2, PathfinderVariables.Instance.coverPointRayLenght, PathfinderVariables.Instance.tankCoverPointMask))
               {
                   //Debug.Log($"defender {defending} is in tank cover");
                   return true;
               }
           }
       }
       else
       {
           if (Physics.SphereCast(defendingPos, sphereRaidus, (attackingPos - defendingPos).normalized, out RaycastHit hit, PathfinderVariables.Instance.coverPointRayLenght, PathfinderVariables.Instance.coverPointMask))
           {
               return true;
           }

           if (defending.occupiedByPlayer && defending.isCoveredByCoverPoint)
           {
               if (Physics.SphereCast(defendingPos, sphereRaidus, (attackingPos - defendingPos).normalized, out RaycastHit hit2, PathfinderVariables.Instance.coverPointRayLenght, PathfinderVariables.Instance.tankCoverPointMask))
               {
                   return true;
               }
           }
       }
       
       
       //Debug.Log($"defender {defending}  is NOT in cover");
       return false;
    }

    /*[SerializeField] private Tile origin;
    [SerializeField] private Tile target1;
    [SerializeField] private Tile target2;

    [SerializeField] private Path tempPath;
    [Button]
    public Path Findpath1()
    {
        foreach (var VARIABLE in tempPath.tiles)
        {
            VARIABLE.ClearHighlight();
        }
        
        
         tempPath = GetPathBetween(origin, target1);
        foreach (var VARIABLE in tempPath.tiles)
        {
            VARIABLE.Highlight(Color.white);
        }
        return tempPath;
    }
    
    [Button]
    public Path Findpath2()
    { 
        
        foreach (var VARIABLE in tempPath.tiles)
        {
            VARIABLE.ClearHighlight();
        }

        tempPath = GetPathBetween(origin, target2);
        
        foreach (var VARIABLE in tempPath.tiles)
        {
            VARIABLE.Highlight(Color.white);
        }
        return tempPath;
    }*/
}
