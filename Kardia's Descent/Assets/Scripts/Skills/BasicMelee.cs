using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicMelee", menuName = "ScriptableObjects/Skills/BasicMelee", order = 0)]
public class BasicMelee : SkillsData
{
    public override void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile,
        GameObject parent, Action OnComplete = null) //skill logic goes here
    {
        Interact.Instance.GetComponent<MonoBehaviour>()
            .StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, selectedTile, parent, OnComplete));
        /*int random = UnityEngine.Random.Range(1, 101);
        if (random <= Skill.accuracy ||
            Skill.accuracy == 100) //todo do this in a different way, maybe a method in a class
        {
            if (ActivaterCharacter is Player)
            {
                if (selectedTile.occupiedByEnemy)
                {
                    selectedTile.occupyingEnemy.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                    skillVFX.SpawnVFX(selectedTile.occupyingEnemy.transform);
                    Debug.Log($"HIT: {random} < {Skill.accuracy}");
                }

                if (selectedTile.OccupiedByCoverPoint)
                {
                    selectedTile.occupyingCoverPoint.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                    skillVFX.SpawnVFX(selectedTile.occupyingCoverPoint.transform);
                    Debug.Log($"HIT: {random} < {Skill.accuracy}");
                }
            }

            if (ActivaterCharacter is Enemy)
            {
                if (selectedTile.occupiedByPlayer)
                {
                    selectedTile.occupyingPlayer.GetComponent<SGT_Health>().HealthDecrease(Skill.damage);
                    skillVFX.SpawnVFX(selectedTile.occupyingPlayer.transform);
                    Debug.Log($"HIT: {random} < {Skill.accuracy}");
                }
            }
        }
        else
        {
            Debug.Log($"MISSED: {random} > {Skill.accuracy}");
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