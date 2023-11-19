using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "SkillsData", menuName = "ScriptableObjects/Skills/ShieldCharge", order = 2)]
public class ShieldCharge : SkillsData
{
    public override void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, GameObject parent, Action OnComplete = null)
    {
        switch (base.skillTarget)
        {
            case SkillTarget.Enemy:
                
                Path path = Pathfinder.Instance.GetPathBetween(ActivaterCharacter.characterTile, selectedTile, true);
                path.tiles.RemoveAt(path.tiles.Count - 1);
                
                ActivaterCharacter.canMove = true;
                ActivaterCharacter.StartMove(path, () =>
                {
                    ActivaterCharacter.Rotate(selectedTile.transform.position);
                    
                    int random = UnityEngine.Random.Range(1, 101);
                    if (random <= Skill.accuracy || Skill.accuracy == 100)
                    {
                        if (ActivaterCharacter is Player)
                        {
                            if (selectedTile.occupiedByEnemy)
                            {
                                selectedTile.occupyingEnemy.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                                Debug.Log($"HIT: {random} < {Skill.accuracy}");
                            }

                            if (selectedTile.OccupiedByCoverPoint)
                            {
                                selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                                Debug.Log($"HIT: {random} < {Skill.accuracy}");
                            }
                        }

                        if (ActivaterCharacter is Enemy)
                        {
                            if (selectedTile.occupiedByPlayer)
                            {
                                selectedTile.occupyingPlayer.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                                Debug.Log($"HIT: {random} < {Skill.accuracy}");
                            }
                            
                            if (selectedTile.OccupiedByCoverPoint)
                            {
                                selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                                Debug.Log($"HIT: {random} < {Skill.accuracy}");
                            }
                        }
                    }
                    else
                    {
                        Debug.Log($"MISSED: {random} > {Skill.accuracy}");
                    }
                }, false);
                
                
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
                OnComplete?.Invoke();
                break;
        }
    }
}
