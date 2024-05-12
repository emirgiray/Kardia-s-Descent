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
      
        // path.tiles.AddRange(ActivaterCharacter.pathfinder.attackableTiles); 
        path.tiles.AddRange(ActivaterCharacter.SkillContainer.effectedTiles); 
      
        ActivaterCharacter.canMove = true;


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
        ActivaterCharacter.moveSpeed /= 1.5f;
        path.tiles.RemoveRange(last , path.tiles.Count - last);
        float defaultAnimSpeed = ActivaterCharacter.animator.speed;
        ActivaterCharacter.animator.speed = defaultAnimSpeed * 1.25f;
        ActivaterCharacter.StartMove(path,true, () =>
        {
            ActivaterCharacter.Rotate(enemyTile.transform.position);
            
            everythingUseful.Interact.GetComponent<MonoBehaviour>()
                .StartCoroutine(WaitUntilEnum(Skill, ActivaterCharacter, enemyTile, defaultAnimSpeed, OnComplete));
            //base.TryHit(Skill, ActivaterCharacter, selectedTile, parent, OnComplete = null);
                
        }, false);
    }
    
    
    
    private IEnumerator WaitUntilEnum(SkillContainer.Skills Skill, Character ActivaterCharacter, Tile selectedTile, float defaultAnimSpeed, Action OnComplete = null)
    {
        //Debug.Log($"wait started    ");
        yield return new WaitUntil(() => ActivaterCharacter.Moving == false);
        ActivaterCharacter.animator.SetTrigger("ShieldChargeAttack");
        yield return new WaitUntil(() => ActivaterCharacter.SkillContainer.GetImpact() == true);
        if (skillAudioEvent != null) skillAudioEvent.Play(ActivaterCharacter.transform);
        //Debug.Log($"Waitfinished");

        if (base.TryHit(Skill, ActivaterCharacter, selectedTile, OnComplete))
        {
            base.DoDamage(Skill, ActivaterCharacter, selectedTile, ActivaterCharacter.SkillContainer.coverDamageMultiplier * ActivaterCharacter.SkillContainer.otherDamageMultiplier, OnComplete);
            base.DoStun(Skill, ActivaterCharacter, selectedTile, OnComplete);
        }
        else
        {
            base.OnMiss(Skill, ActivaterCharacter, selectedTile, OnComplete);
        }
        ActivaterCharacter.moveSpeed *= 1.5f;
        ActivaterCharacter.animator.speed = defaultAnimSpeed;
            OnComplete?.Invoke();
    }
}