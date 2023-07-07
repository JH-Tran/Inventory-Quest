using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, ESCAPED }
public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public GameObject movesMenu;
    public GameObject actionMenu;
    public GameObject inventoryMenu;
    public GameObject dialogueGamObject;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    [SerializeField] private UnitInstance playerUnit;
    [SerializeField] private UnitInstance enemyUnit;

    public Text dialogue;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;
    public EncounterSystem encounterSystem;
    //Variables for drop rate
    public InventoryController inventoryController;
    private bool isBossEnemy = false;

    public void StartBattle(UnitData unitData, bool isBoss = false, int bossLevelIncrease = 0)
    {
        state = BattleState.START;
        enemyUnit.unitData = unitData;
        enemyUnit.Initalise();
        if (isBoss == true)
        {
            enemyUnit.AddLevel(bossLevelIncrease);
        }
        isBossEnemy = isBoss;
        StartCoroutine(SetUpBattle());
    }

    IEnumerator SetUpBattle()
    {
        enemyUnit.SetUnitHealth();
        enemyUnit.RemoveUnitStatusEffect();
        dialogue.text = $"A wild {enemyUnit.unitData.DisplayName} approaches...";
        //Debug.Log($"Player health {playerUnit.currentUnitHealth}");
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);
        // **Implement calculation for reuduction in speed when unit is paralysis
        if (playerUnit.unitData.Speed > enemyUnit.unitData.Speed)
        {
            dialogue.text = $"You take initiative!";
            yield return new WaitForSeconds(2f);
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
        else if (playerUnit.unitData.Speed < enemyUnit.unitData.Speed)
        {
            dialogue.text = $"{enemyUnit.unitData.DisplayName} will take initiative!";
            yield return new WaitForSeconds(2f);

            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            // Coinflip the first person start if speeds are the same
            var random = new System.Random();
            if (random.Next(2) == 1)
            {
                dialogue.text = $"You were just faster!";
                yield return new WaitForSeconds(2f);
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
            else
            {
                dialogue.text = $"{enemyUnit.unitData.DisplayName} was just faster!";
                yield return new WaitForSeconds(2f);
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
    }
    private void PlayerTurn()
    {
        StartCoroutine(PlayerTurnManager());
    }
    IEnumerator PlayerTurnManager()
    {
        if (playerUnit.GetUnitStatusEffect() != StatusEffect.NONE)
        {
            StatusEffect temp = playerUnit.GetUnitStatusEffect();
            Debug.Log("Player has status effect");
            if (playerUnit.IsStatusEffectPassUnitTurn())
            {
                Debug.Log("Player passed their turn");
                UpdateUnitStatusDialogue(playerUnit);
                yield return new WaitForSeconds(2f);
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
                yield break;
            }
            else if (playerUnit.GetUnitStatusEffect() == StatusEffect.BURN || playerUnit.GetUnitStatusEffect() == StatusEffect.POISON)
            {
                if (!playerUnit.IsUnitDeadFromStatusEffect())
                {
                    StartCoroutine(playerHUD.SetHealthSmooth(playerUnit));
                    Debug.Log("Player Status Damage Calculated != DEAD");
                    UpdateUnitStatusDialogue(playerUnit);
                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    StartCoroutine(playerHUD.SetHealthSmooth(playerUnit));
                    UpdateUnitStatusDialogue(playerUnit);
                    Debug.Log("Player Status Damage Calculated = DEAD");
                    yield return new WaitForSeconds(2f);
                    state = BattleState.LOST;
                    EndBattle();
                    yield break;
                }
            }
            else
            {
                if (playerUnit.GetStatusEffectString() != "")
                {
                    UpdateUnitStatusDialogue(playerUnit);
                    playerHUD.UpdateStatusEffect(playerUnit);
                    yield return new WaitForSeconds(2f);
                }
            }
        }
        OnButtonShowActionMenu();
    }
    public void OnButtonRun()
    {
        state = BattleState.ESCAPED;
        hideActionMoveMenu();
        SetDialogueText(true, "You ran away!");
        StartCoroutine(ReturnToMenu());
    }
    public void OnButtonShowActionMenu()
    {
        SetDialogueText(true, "Choose an action:");
        actionMenu.SetActive(true);
        movesMenu.SetActive(false);
    }
    public void OnButtonSetInventoryActive()
    {
        if (inventoryMenu.activeSelf)
        {
            inventoryMenu.SetActive(false);
        }
        else
        {
            inventoryMenu.SetActive(true);
        }
    }
    public void OnButtonShowMovesMenu()
    {
        SetDialogueText(false, "");
        actionMenu.SetActive(false);
        movesMenu.SetActive(true);
    }
    public void hideActionMoveMenu()
    {
        SetDialogueText(true, "");
        actionMenu.SetActive(false);
        movesMenu.SetActive(false);
    }
    public void SetDialogueText(bool setActive = false, string message = "")
    {
        dialogueGamObject.SetActive(setActive);
        dialogue.text = message;
    }
    //BETA CONTROLS: Test Moves Effects
    #region Test Player Moves
    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN) return;
        hideActionMoveMenu();
        StartCoroutine(PlayerHeal());
    }
    IEnumerator PlayerHeal()
    {
        playerUnit.HealUnit(playerUnit.maxHp/5);

        StartCoroutine(playerHUD.SetHealthSmooth(playerUnit));
        dialogue.text = "You feel renewed strength!";

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    #endregion

    public void OnMove1Button()
    {
        if (playerUnit.moveList[0] == null) { return; }
        if (state != BattleState.PLAYERTURN) return;
        hideActionMoveMenu();
        MoveManager(playerUnit.moveList[0]);
    }
    public void OnMove2Button()
    {
        if (playerUnit.moveList[1] == null) { return; }
        if (state != BattleState.PLAYERTURN) return;
        hideActionMoveMenu();
        MoveManager(playerUnit.moveList[1]);
    }
    public void OnMove3Button()
    {
        if (playerUnit.moveList[2] == null) { return; }
        if (state != BattleState.PLAYERTURN) return;
        hideActionMoveMenu();
        MoveManager(playerUnit.moveList[2]);
    }
    public void OnMove4Button()
    {
        if (playerUnit.moveList[3] == null) { return; }
        if (state != BattleState.PLAYERTURN) return;
        hideActionMoveMenu();
        MoveManager(playerUnit.moveList[3]);
    }

    //MOVES TO IMPLEMENT
    //BUFF && DEBUFF
    private void MoveManager(MovesData moveData)
    {
        if (moveData == null)
        {
            Debug.Log("ERROR: Missing move data");
            return;
        }
        SetDialogueText(true, "");
        Debug.Log($"Game turn: {state}");

        if (state == BattleState.PLAYERTURN)
        {
            switch (moveData.moveType)
            {
                case MoveType.NONE:
                    Debug.Log("ERROR: No move assigned");
                    break;
                case MoveType.PHYSICAL:
                    Debug.Log("move data: PHYSICAL");
                    //Type, power, accuracy
                    StartCoroutine(UnitAttack(playerUnit, enemyUnit, moveData));
                    break;
                case MoveType.SPECIAL:
                    Debug.Log("move data: SPECIAL");
                    StartCoroutine(UnitAttack(playerUnit, enemyUnit, moveData));
                    //Type, power, accuracy
                    break;
                case MoveType.BUFF:
                    Debug.Log("move data: BUFF");
                    playerUnit.HealUnit(playerUnit.maxHp/4);
                    playerHUD.SetHP(playerUnit);
                    state = BattleState.ENEMYTURN;
                    StartCoroutine(EnemyTurn());
                    //Type, status, accuracy
                    break;
                case MoveType.DEBUFF:
                    Debug.Log("move data: DEBUFF");
                    //Type, status, accuracy
                    break;
                case MoveType.STATUS:
                    Debug.Log("move data: STATUS");
                    //Type, status, accuracy
                    ApplyStaticEffect(playerUnit, enemyUnit, moveData.moveName, moveData.elementalType, moveData.statusEffect, moveData.statusAccuracy);
                    break;
            }
        }
        else if (state == BattleState.ENEMYTURN)
        {
            switch (moveData.moveType)
            {
                case MoveType.NONE:
                    Debug.Log("ERROR: No move assigned.");
                    break;
                case MoveType.PHYSICAL:
                    Debug.Log("move data: PHYSICAL");
                    //Type, power, accuracy
                    StartCoroutine(UnitAttack(enemyUnit, playerUnit, moveData));
                    break;
                case MoveType.SPECIAL:
                    Debug.Log("move data: SPECIAL");
                    StartCoroutine(UnitAttack(enemyUnit, playerUnit,moveData));
                    //Type, power, accuracy
                    break;
                case MoveType.BUFF:
                    Debug.Log("move data: BUFF");
                    enemyUnit.HealUnit(2);
                    state = BattleState.ENEMYTURN;
                    StartCoroutine(EnemyTurn());
                    //Type, status, accuracy
                    break;
                case MoveType.DEBUFF:
                    Debug.Log("move data: DEBUFF");
                    //Type, status, accuracy
                    break;
                case MoveType.STATUS:
                    Debug.Log("move data: STATUS");
                    //Type, status, accuracy
                    ApplyStaticEffect(enemyUnit, playerUnit, moveData.moveName, moveData.elementalType, moveData.statusEffect, moveData.statusAccuracy);
                    break;
            }
        }

    }
    public void ApplyStaticEffect(UnitInstance unitAttacking, UnitInstance unitDefending, string moveName, ElementTypes elementType, StatusEffect statusEffect, int accuracy)
    {
        bool isStatusApplied = unitAttacking.ApplyStatusEffect(unitDefending, elementType, statusEffect, accuracy);
        hideActionMoveMenu();
        switch (statusEffect)
        {
            case StatusEffect.NONE:
                Debug.LogError("No Status effect assigned");
                break;
            case StatusEffect.BURN:
                StartCoroutine(UnitBurn(unitAttacking, unitDefending, moveName, isStatusApplied));
                break;
            case StatusEffect.POISON:
                StartCoroutine(UnitPoison(unitAttacking, unitDefending, moveName, isStatusApplied));
                break;
            case StatusEffect.SLEEP:
                StartCoroutine(UnitSleep(unitAttacking, unitDefending, moveName, isStatusApplied));
                break;
            case StatusEffect.FROZEN:
                StartCoroutine(UnitFrozen(unitAttacking, unitDefending, moveName, isStatusApplied));
                break;
            case StatusEffect.PARALYSIS:
                StartCoroutine(UnitParalysis(unitAttacking, unitDefending, moveName, isStatusApplied));
                break;
        }
    }
    #region Status to apply to enemy
    IEnumerator UnitBurn(UnitInstance unitAttacking, UnitInstance unitDefending, string moveName, bool isStatusApplied)
    {
        dialogue.text = $"{unitAttacking.unitData.DisplayName} used {moveName}!";
        yield return new WaitForSeconds(2f);
        if (isStatusApplied)
        {
            enemyHUD.UpdateStatusEffect(enemyUnit);
            playerHUD.UpdateStatusEffect(playerUnit);
            dialogue.text = $"{unitDefending.unitData.DisplayName} has been burned!";
        }
        else
        {
            if (unitDefending.GetUnitStatusEffect() == StatusEffect.BURN)
            {
                dialogue.text = $"{unitDefending.unitData.DisplayName} is already burned!";
            }
            else if (unitDefending.GetUnitStatusEffect() != StatusEffect.NONE)
            {
                dialogue.text = $"{unitDefending.unitData.DisplayName} cannot have more than one status effects!";
            }
            else
            {
                dialogue.text = $"{unitAttacking.unitData.DisplayName} missed!";
            }
        }
        yield return new WaitForSeconds(2f);
        PassTurn(false);
    }
    IEnumerator UnitPoison(UnitInstance unitAttacking, UnitInstance unitDefending, string moveName, bool isStatusApplied)
    {
        dialogue.text = $"{unitAttacking.unitData.DisplayName} used {moveName}!";
        yield return new WaitForSeconds(2f);
        if (isStatusApplied)
        {
            enemyHUD.UpdateStatusEffect(enemyUnit);
            playerHUD.UpdateStatusEffect(playerUnit);
            dialogue.text = $"{unitDefending.unitData.DisplayName} has been poisoned!";
        }
        else
        {
            if (unitDefending.GetUnitStatusEffect() == StatusEffect.POISON)
            {
                dialogue.text = $"{unitDefending.unitData.DisplayName} is already poisoned!";
            }
            else if (unitDefending.GetUnitStatusEffect() != StatusEffect.NONE)
            {
                dialogue.text = $"{unitDefending.unitData.DisplayName} cannot have 2 status effects!";
            }
            else
            {
                dialogue.text = $"{unitAttacking.unitData.DisplayName} missed!";
            }
        }
        yield return new WaitForSeconds(2f);
        PassTurn(false);
    }
    IEnumerator UnitSleep(UnitInstance unitAttacking, UnitInstance unitDefending, string moveName, bool isStatusApplied)
    {
        dialogue.text = $"{unitAttacking.unitData.DisplayName} used {moveName}!";
        yield return new WaitForSeconds(2f);
        if (isStatusApplied)
        {
            enemyHUD.UpdateStatusEffect(enemyUnit);
            playerHUD.UpdateStatusEffect(playerUnit);
            dialogue.text = $"{unitDefending.unitData.DisplayName} fell asleep!";
        }
        else
        {
            if (unitDefending.GetUnitStatusEffect() == StatusEffect.SLEEP)
            {
                dialogue.text = $"{unitDefending.unitData.DisplayName} is already asleep!";
            }
            else if (unitDefending.GetUnitStatusEffect() != StatusEffect.NONE)
            {
                dialogue.text = $"{unitDefending.unitData.DisplayName} cannot have 2 status effects!";
            }
            else
            {
                dialogue.text = $"{unitAttacking.unitData.DisplayName} missed!";
            }
        }
        yield return new WaitForSeconds(2f);
        PassTurn(false);
    }
    IEnumerator UnitFrozen(UnitInstance unitAttacking, UnitInstance unitDefending, string moveName, bool isStatusApplied)
    {
        dialogue.text = $"{unitAttacking.unitData.DisplayName} used {moveName}!";
        yield return new WaitForSeconds(2f);
        if (isStatusApplied)
        {
            enemyHUD.UpdateStatusEffect(enemyUnit);
            playerHUD.UpdateStatusEffect(playerUnit);
            dialogue.text = $"{unitDefending.unitData.DisplayName} is frozen solid!";
        }
        else
        {
            if (unitDefending.GetUnitStatusEffect() == StatusEffect.FROZEN)
            {
                dialogue.text = $"{unitDefending.unitData.DisplayName} is already frozen!";
            }
            else if (unitDefending.GetUnitStatusEffect() != StatusEffect.NONE)
            {
                dialogue.text = $"{unitDefending.unitData.DisplayName} cannot have 2 status effects!";
            }
            else
            {
                dialogue.text = $"{unitAttacking.unitData.DisplayName} missed!";
            }
        }
        yield return new WaitForSeconds(2f);
        PassTurn(false);
    }
    IEnumerator UnitParalysis(UnitInstance unitAttacking, UnitInstance unitDefending, string moveName, bool isStatusApplied)
    {
        dialogue.text = $"{unitAttacking.unitData.DisplayName} used {moveName}!";
        yield return new WaitForSeconds(2f);
        if (isStatusApplied)
        {
            enemyHUD.UpdateStatusEffect(enemyUnit);
            playerHUD.UpdateStatusEffect(playerUnit);
            dialogue.text = $"{unitDefending.unitData.DisplayName} is paralysed!";
        }
        else
        {
            if (enemyUnit.GetUnitStatusEffect() == StatusEffect.PARALYSIS)
            {
                dialogue.text = $"{unitDefending.unitData.DisplayName} is already paralysed!";
            }
            else if (enemyUnit.GetUnitStatusEffect() != StatusEffect.NONE)
            {
                dialogue.text = $"{unitDefending.unitData.DisplayName} cannot have 2 status effects!";
            }
            else
            {
                dialogue.text = $"{unitAttacking.unitData.DisplayName} missed!";
            }
        }
        yield return new WaitForSeconds(2f);
        PassTurn(false);
    }
    #endregion
    IEnumerator UnitAttack(UnitInstance unitAttacking, UnitInstance unitDefending, MovesData moveData)
    {
        dialogue.text = $"{unitAttacking.unitData.DisplayName} used {moveData.moveName}!";
        yield return new WaitForSeconds(2f);
        //IMPLEMENT: Damage dealt to defender unit
        bool isDead = false;
        if(moveData.moveType == MoveType.PHYSICAL)
        {
            isDead = unitAttacking.PhysicalAttack(unitDefending, moveData.elementalType, moveData.power);
        }
        else if (moveData.moveType == MoveType.SPECIAL)
        {
            isDead = unitAttacking.SpecialAttack(unitDefending, moveData.elementalType, moveData.power);
        }

        StartCoroutine(enemyHUD.SetHealthSmooth(enemyUnit));
        StartCoroutine(playerHUD.SetHealthSmooth(playerUnit));
        yield return new WaitForSeconds(2f);
        ElementEffectiveness MoveEffectivenessToUnitDefending = unitDefending.GetMoveEffectiveness();
        if (MoveEffectivenessToUnitDefending == ElementEffectiveness.SUPEREFFECTIVE)
        {
            dialogue.text = "It was super effective!";
            yield return new WaitForSeconds(2f);
        }
        else if (MoveEffectivenessToUnitDefending == ElementEffectiveness.NOTEFFECTIVE)
        {
            dialogue.text = "It was not effective!";
            yield return new WaitForSeconds(2f);
        }

        if (unitDefending.IsUnitCriticalAttack() == true)
        {
            dialogue.text = "Critical Hit!";
            yield return new WaitForSeconds(2f);
        }
        PassTurn(isDead);
    }
    IEnumerator EnemyTurn()
    {
        hideActionMoveMenu();
        Debug.Log("Enemy Turn Start");

        if (enemyUnit.GetUnitStatusEffect() != StatusEffect.NONE)
        {
            Debug.Log("Enemy Turn has status effect");
            if (enemyUnit.IsStatusEffectPassUnitTurn())
            {
                Debug.Log("Enemy passed their turn");
                UpdateUnitStatusDialogue(enemyUnit);
                yield return new WaitForSeconds(2f);
                state = BattleState.PLAYERTURN;
                PlayerTurn();
                yield break;
            }
            else if (enemyUnit.GetUnitStatusEffect() == StatusEffect.BURN || enemyUnit.GetUnitStatusEffect() == StatusEffect.POISON)
            {
                if (!enemyUnit.IsUnitDeadFromStatusEffect())
                {
                    StartCoroutine(enemyHUD.SetHealthSmooth(enemyUnit));
                    Debug.Log("Enemy Status Damage Calculated");
                    UpdateUnitStatusDialogue(enemyUnit);
                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    StartCoroutine(enemyHUD.SetHealthSmooth(enemyUnit));
                    Debug.Log("Enemy Status Damage Calculated = DEAD");
                    UpdateUnitStatusDialogue(enemyUnit);
                    yield return new WaitForSeconds(2f);
                    state = BattleState.WON;
                    EndBattle();
                    yield break;
                }
            }
            else
            {
                if (playerUnit.GetStatusEffectString() != "")
                {
                    UpdateUnitStatusDialogue(enemyUnit);
                    enemyHUD.UpdateStatusEffect(enemyUnit);
                    yield return new WaitForSeconds(2f);
                }
            }
        }

        dialogue.text = $"{enemyUnit.unitData.DisplayName} is thinking!";
        yield return new WaitForSeconds(2f);
        MoveManager(enemyUnit.GetRandomMove());

        #region TESTING ENEMIES FUNCTION WITHOUT THEIR MOVESET
        /*      //Apply status effect to player.
        *//* enemyUnit.ApplyStatusEffect(playerUnit, ElementTypes.FIRE, StatusEffect.BURN, 100);
        playerHUD.UpdateStatusEffect(playerUnit);*//*

        bool isDead = enemyUnit.PhysicalAttack(playerUnit, ElementTypes.NORMAL, 20);
        playerHUD.SetHP(playerUnit);

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }*/
        #endregion
    }
    IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(2f);
        if (state == BattleState.LOST)
            dialogue.text = "You were defeated!";
        else if (state == BattleState.ESCAPED)
            dialogue.text = "You did not receive any reward!";
        yield return new WaitForSeconds(2f);
        encounterSystem.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    IEnumerator EnemeyDrop()
    {
        yield return new WaitForSeconds(2f);
        if (state == BattleState.WON)
            dialogue.text = $"{enemyUnit.unitData.DisplayName} has drop some items!";
        else
            Debug.LogError("THIS STATE SHOULD BE ONLY WON. In enemey drop BattleSystem Script");
        yield return new WaitForSeconds(2f);
        InventoryDrop();

    }
    IEnumerator PlayerExperience()
    {
        yield return new WaitForSeconds(2f);
        //Experience Gaon
        int baseExperience = enemyUnit.unitData.BaseExperience;
        int enemyLevel = enemyUnit.Level;
        float bossBonus = (enemyUnit.unitData.IsBoss) ? 1.5f : 1f;
        int experienceGain = Mathf.FloorToInt((baseExperience * enemyLevel * bossBonus) / 7);
        dialogue.text = $"{playerUnit.unitData.DisplayName} has gain {experienceGain} exp";
        playerUnit.experience += experienceGain;
        yield return playerHUD.SetExperienceSmooth(playerUnit);
        //Check for level up
        while (playerUnit.CheckForLevelUp())
        {
            playerHUD.SetLevel(playerUnit);
            dialogue.text = $"{playerUnit.unitData.DisplayName} grew to level {playerUnit.Level}";
            yield return playerHUD.SetExperienceSmooth(playerUnit, true);
        }
        //Enemy Drop items
        StartCoroutine(EnemeyDrop());

    }
    private void EndBattle()
    {
        if (state == BattleState.WON)
        {
            if (isBossEnemy)
            {
                dialogue.text = $"{playerUnit.unitData.DisplayName} has slain {enemyUnit.unitData.DisplayName}!";
                encounterSystem.AddBossDefeated();
            }
            else
            {
                dialogue.text = $"{playerUnit.unitData.DisplayName} defeated {enemyUnit.unitData.DisplayName}!";
                encounterSystem.AddEnemiesDefeated();
            }
            StartCoroutine(PlayerExperience());
        }
        else if (state == BattleState.LOST)
        {
            dialogue.text = $"{enemyUnit.unitData.DisplayName} deafeated {playerUnit.unitData.DisplayName}!";
            encounterSystem.AddPlayerDeath();
            StartCoroutine(ReturnToMenu());
        }
    }
    private void UpdateUnitStatusDialogue(UnitInstance unitInstance)
    {
        SetDialogueText(true, $"{unitInstance.GetStatusEffectString()}");    
    }
    private void PassTurn(bool isDefendingUnitDead)
    {
        if (isDefendingUnitDead && state == BattleState.PLAYERTURN)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else if (isDefendingUnitDead && state == BattleState.ENEMYTURN)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else if (!isDefendingUnitDead && state == BattleState.PLAYERTURN)
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        else if (!isDefendingUnitDead && state == BattleState.ENEMYTURN)
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }
    private void InventoryDrop()
    {
        OnButtonSetInventoryActive();
        inventoryMenu.GetComponent<InventoryButtonManager>().OpenInventoryAfterBattle();
        inventoryController.InsertRandomItemInDropGrid();
        if (enemyUnit.IsUnitDropReward())
        {
            Debug.Log("GAIN BONUS DROP REWARD");
            inventoryController.InsertItemInDropGrid(null, enemyUnit.GetRandomMove());
        }
        gameObject.SetActive(false);
    }
}
