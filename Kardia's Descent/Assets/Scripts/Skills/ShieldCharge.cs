using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "SkillsData", menuName = "ScriptableObjects/Skills/ShieldCharge", order = 2)]
public class ShieldCharge : SkillsData
{
    //[SerializeField] private int stunDuration = 1 ;
    
    public override void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, Action OnComplete = null)
    {
        // ActivaterCharacter.animator.ResetTrigger("Attack");

        Path path = new Path();
        path.tiles = new List<Tile>();
        /* path.tiles.Add(ActivaterCharacter.characterTile);
         path.tiles.AddRange (ActivaterCharacter.pathfinder.attackableTiles); */
        path.tiles.AddRange(ActivaterCharacter.pathfinder.attackableTiles); 
        // Path path = ActivaterCharacter.pathfinder.GetPathBetween(ActivaterCharacter, ActivaterCharacter.characterTile, selectedTile /*, true*/);
        // Path path = ActivaterCharacter.pathfinder.PathBetween(ActivaterCharacter,  selectedTile , ActivaterCharacter.characterTile /*, true*/);
        
        /*if (path.tiles[path.tiles.Count - 1].Occupied) //stop one tile before the destination tile
        {
            path.tiles.RemoveAt(path.tiles.Count - 1);
        }*/
        
        ActivaterCharacter.canMove = true;

        /*if (path.tiles[path.tiles.Count - 1] == ActivaterCharacter.characterTile) //if the destination tile is the character tile
        {
            ActivaterCharacter.Rotate(selectedTile.transform.position);
            
            Interact.Instance.GetComponent<MonoBehaviour>()
                .StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, selectedTile, OnComplete));
            //base.TryHit(Skill, ActivaterCharacter, selectedTile, parent, OnComplete = null);
        }
        else*/
        {
            int last = path.tiles.Count - 1;
            Tile enemyTile = path.tiles[last];
            
            //change path.tiles to create a new path that gets the first enemy on the line
            for (int i = 0; i < path.tiles.Count; i++) //maybe start i from 1
            {
                if (ActivaterCharacter is Player && path.tiles[i].occupiedByEnemy)
                {
                    enemyTile = path.tiles[i]; //store the enemy tile for hit miss calculation
                    last = i;
                }
                if (ActivaterCharacter is Enemy && path.tiles[i].occupiedByPlayer)
                {
                    enemyTile = path.tiles[i]; 
                    last = i;
                }
            }
            ActivaterCharacter.movedata.MoveTime /= 1.5f;
            path.tiles.RemoveRange(last , path.tiles.Count - last);
            ActivaterCharacter.StartMove(path,true, () =>
            {
                
                ActivaterCharacter.Rotate(enemyTile.transform.position);
            
                everythingUseful.Interact.GetComponent<MonoBehaviour>()
                    .StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, enemyTile, OnComplete));
                //base.TryHit(Skill, ActivaterCharacter, selectedTile, parent, OnComplete = null);
                
            }, false);
        }
        
        /*if (accuracy <= 100)
        {
            int random = UnityEngine.Random.Range(1, 101);
            if (random <= Skill.accuracy)
            {
                if (ActivaterCharacter is Player)
                {
                    if (selectedTile.occupiedByEnemy)
                    {
                        Path path2 = Pathfinder.Instance.GetPathBetween(ActivaterCharacter.characterTile, selectedTile, true);

                        ActivaterCharacter.StartMove(path2, () =>
                        {
                            ActivaterCharacter.Rotate(selectedTile.transform.position);
                            if (selectedTile.occupyingEnemy.GetComponent<SGT_Health>() != null)
                            {
                                selectedTile.occupyingEnemy.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                            }
                            Debug.Log($"HIT: {random} < {Skill.accuracy}");
                        }, false);


                    }
                    if (selectedTile.OccupiedByCoverPoint)
                    {
                        selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                        Debug.Log($"HIT: {random} < {Skill.accuracy}");
                    }

                    if (!selectedTile.occupiedByEnemy && !selectedTile.OccupiedByCoverPoint && !selectedTile.occupiedByPlayer)
                    {

                        Path path2 = Pathfinder.Instance.GetPathBetween(ActivaterCharacter.characterTile, selectedTile, true);

                        ActivaterCharacter.canMove = true;
                        ActivaterCharacter.StartMove(path2, () =>
                        {
                            ActivaterCharacter.Rotate(selectedTile.transform.position);
                            Debug.Log($"HIT: {random} < {Skill.accuracy}");
                        }, false);
                    }
                }

                if (ActivaterCharacter is Enemy)
                {
                    if (selectedTile.occupiedByPlayer)
                    {
                        selectedTile.occupyingPlayer.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                        Debug.Log($"HIT: {random} < {Skill.accuracy}");
                    }
                }



            }
            else
            {
                Debug.Log($"MISSED: {random} > {Skill.accuracy}");
            }
        }
        else
        {
            if (ActivaterCharacter is Player)
            {
                if (selectedTile.occupiedByEnemy)
                {
                    selectedTile.occupyingEnemy.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                    Debug.Log($"HIT with full accuracy");
                }

                if (selectedTile.OccupiedByCoverPoint)
                {
                    selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                    Debug.Log($"HIT with full accuracy");
                }
            }

            if (ActivaterCharacter is Enemy)
            {
                if (selectedTile.occupiedByPlayer)
                {
                    selectedTile.occupyingPlayer.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                    Debug.Log($"HIT with full accuracy");
                }
            }
        }*/

        //add delay of 0.1f seconds here
        // OnComplete?.Invoke();
    }
    public IEnumerator WaitUntilEnum(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, Action OnComplete = null)
    {
        //Debug.Log($"wait started    ");
        yield return new WaitUntil(() => ActivaterCharacter.SkillContainer.GetImpact() == true);
        if (skillAudioEvent != null) skillAudioEvent.Play(ActivaterCharacter.transform);
        //Debug.Log($"Waitfinished");

        if (base.TryHit(Skill, ActivaterCharacter, selectedTile, OnComplete))
        {
            base.DoDamage(Skill, ActivaterCharacter, selectedTile, OnComplete);
            base.DoStun(Skill, ActivaterCharacter, selectedTile, OnComplete);
        }
        else
        {
            base.OnMiss(Skill, ActivaterCharacter, selectedTile, OnComplete);
        }
        ActivaterCharacter.movedata.MoveTime *= 1.5f;
            /*int random = UnityEngine.Random.Range(1, 101);
            if (random <= Skill.accuracy || Skill.accuracy == 100)
            {
                if (ActivaterCharacter is Player)
                {
                    if (selectedTile.occupiedByEnemy)
                    {
                        foreach (var fx in skillHitVFX)
                        {
                            fx.SpawnVFX(selectedTile.occupyingEnemy.transform);
                        }
                        selectedTile.occupyingEnemy.Stun(true, skillEffectDuration);
                        //selectedTile.occupyingEnemy.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                        selectedTile.occupyingEnemy.GetComponent<DamageHandler>().TakeDamage(Skill.damage, ActivaterCharacter);

                        //Debug.Log($"HIT: {random} < {Skill.accuracy}");
                        //base.OnHit();
                    }

                    if (selectedTile.OccupiedByCoverPoint)
                    {
                        foreach (var fx in skillHitVFX)
                        {
                            fx.SpawnVFX(selectedTile.occupyingCoverPoint.transform);
                        }
                        
                        if(selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>() != null) selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);  
                        //Debug.Log($"HIT: {random} < {Skill.accuracy}");
                    }
                }

                if (ActivaterCharacter is Enemy)
                {
                    if (selectedTile.occupiedByPlayer)
                    {
                        foreach (var fx in skillHitVFX)
                        {
                            fx.SpawnVFX(selectedTile.occupyingPlayer.transform);
                        }

                        selectedTile.occupyingPlayer.Stun(true, skillEffectDuration);
                        //selectedTile.occupyingPlayer.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                        selectedTile.occupyingPlayer.GetComponent<DamageHandler>().TakeDamage(Skill.damage, ActivaterCharacter);
                        //Debug.Log($"HIT: {random} < {Skill.accuracy}");
                        //base.OnHit();
                    }

                    if (selectedTile.OccupiedByCoverPoint)
                    {
                        foreach (var fx in skillHitVFX)
                        {
                            fx.SpawnVFX(selectedTile.occupyingCoverPoint.transform);
                        }
                        
                        if(selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>() != null) selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                        //Debug.Log($"HIT: {random} < {Skill.accuracy}");
                    }
                }
                
                
            }
            else
            {
                //Debug.Log($"MISSED: {random} > {Skill.accuracy}");
                if (selectedTile.occupiedByEnemy)
                {
                    skillMissVFX.SpawnVFX(selectedTile.occupyingEnemy.transform);
                    selectedTile.occupyingEnemy.GetComponent<SGT_Health>().Miss();
                }
                if (selectedTile.occupiedByPlayer)
                {
                    skillMissVFX.SpawnVFX(selectedTile.occupyingPlayer.transform);
                    selectedTile.occupyingPlayer.GetComponent<SGT_Health>().Miss();
                }
                
                
            }*/
            OnComplete?.Invoke();
    }
}