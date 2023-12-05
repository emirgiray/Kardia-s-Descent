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
    
    public override void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile,
        GameObject parent, Action OnComplete = null)
    {
        Path path = Pathfinder.Instance.GetPathBetween(ActivaterCharacter.characterTile, selectedTile /*, true*/);
        // path.tiles.RemoveAt(path.tiles.Count - 1);

        ActivaterCharacter.canMove = true;

        if (path.tiles[path.tiles.Count - 1] == ActivaterCharacter.characterTile) //if the destination tile is the character tile
        {
            ActivaterCharacter.Rotate(selectedTile.transform.position);
            
            Interact.Instance.GetComponent<MonoBehaviour>()
                .StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, selectedTile, parent, OnComplete));
            //base.TryHit(Skill, ActivaterCharacter, selectedTile, parent, OnComplete = null);
        }
        else
        {
            ActivaterCharacter.StartMove(path, () =>
            {
                ActivaterCharacter.Rotate(selectedTile.transform.position);
            
                Interact.Instance.GetComponent<MonoBehaviour>()
                    .StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, selectedTile, parent, OnComplete));
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
    public IEnumerator WaitUntilEnum(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile,
        GameObject parent, Action OnComplete = null)
    {
        Debug.Log($"wait started    ");
        yield return new WaitUntil(() => ActivaterCharacter.GetComponent<SkillContainer>().GetImpact() == true);
        Debug.Log($"Waitfinished");
                    int random = UnityEngine.Random.Range(1, 101);
            if (random <= Skill.accuracy || Skill.accuracy == 100)
            {
                if (ActivaterCharacter is Player)
                {
                    if (selectedTile.occupiedByEnemy)
                    {
                        skillVFX.SpawnVFX(selectedTile.occupyingEnemy.transform);
                        selectedTile.occupyingEnemy.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                        selectedTile.occupyingEnemy.Stun(true, skillEffectDuration);

                        Debug.Log($"HIT: {random} < {Skill.accuracy}");
                        //base.OnHit();
                    }

                    if (selectedTile.OccupiedByCoverPoint)
                    {
                        skillVFX.SpawnVFX(selectedTile.occupyingCoverPoint.transform);
                        selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                        Debug.Log($"HIT: {random} < {Skill.accuracy}");
                    }
                }

                if (ActivaterCharacter is Enemy)
                {
                    if (selectedTile.occupiedByPlayer)
                    {
                        skillVFX.SpawnVFX(selectedTile.occupyingPlayer.transform);
                        selectedTile.occupyingPlayer.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                        selectedTile.occupyingPlayer.Stun(true, skillEffectDuration);
                        Debug.Log($"HIT: {random} < {Skill.accuracy}");
                        //base.OnHit();
                    }

                    if (selectedTile.OccupiedByCoverPoint)
                    {
                        skillVFX.SpawnVFX(selectedTile.occupyingCoverPoint.transform);
                        selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                        Debug.Log($"HIT: {random} < {Skill.accuracy}");
                    }
                }
            }
            else
            {
                Debug.Log($"MISSED: {random} > {Skill.accuracy}");
            }
            OnComplete?.Invoke();
    }
}