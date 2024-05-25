using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Summon", menuName = "ScriptableObjects/Skills/Summon", order = 0)]

public class Summon : SkillsData
{
    public GameObject summonPrefab;
    public override void ActivateSkill(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, float multipliars, Action OnComplete = null) //skill logic goes here
    {
        ActivaterCharacter.Interact.GetComponent<MonoBehaviour>()
            .StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, selectedTile, multipliars, OnComplete));
    }

    private IEnumerator WaitUntilEnum(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, float multipliars, Action OnComplete = null)
    {
        if (skillAudioEvent != null) skillAudioEvent.Play(selectedTile.transform);
        if (skillStartVFX != null ) skillStartVFX.SpawnVFX(selectedTile.transform);
        
        yield return new WaitUntil(() => ActivaterCharacter.SkillContainer.GetImpact() == true);
        
        if (skillImpactVFX != null ) skillImpactVFX.SpawnVFX(selectedTile.transform);
        
        var spawned = Instantiate(summonPrefab, selectedTile.transform.position + Vector3.up , Quaternion.identity);
        var spawnedCharacter = spawned.GetComponent<Character>();

        if (spawnedCharacter is Player)
        {
            
        }
        else if (spawnedCharacter is Enemy)
        {
            var stateController = spawnedCharacter.GetComponent<StateController>();
            //stateController.currentState = stateController.defaultCombatStartState; // inf loop
            
            spawnedCharacter.StartCombat();
           // everythingUseful.TurnSystem.RoundInfoScript.ForceObjectIndex(spawnedCharacter.GetCharacterCard(), 9999999);
        }
        
        OnComplete?.Invoke();
        yield return new WaitUntil(() => ActivaterCharacter.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("idle"));
    }
}
