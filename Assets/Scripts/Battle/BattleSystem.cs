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

    internal void StartBattle(UnitData unitData)
    {
        state = BattleState.START;
        enemyUnit.unitData = unitData;
        StartCoroutine(SetUpBattle());
    }

    IEnumerator SetUpBattle()
    {
        enemyUnit.SetUnitHealth();
        enemyUnit.CleanseUnitStatusEffect();
        dialogue.text = $"A wild {enemyUnit.unitData.displayName} approaches...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);
        // **Implement calculation for reuduction in speed when unit is paralysis
        if (playerUnit.unitData.speed > enemyUnit.unitData.speed)
        {
            dialogue.text = $"You take initiative!";
            yield return new WaitForSeconds(2f);
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
        else if (playerUnit.unitData.speed < enemyUnit.unitData.speed)
        {
            dialogue.text = $"{enemyUnit.unitData.displayName} will take initiative!";
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
                dialogue.text = $"{enemyUnit.unitData.displayName} was just faster!";
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
            Debug.Log("Player has status effect");
            if (playerUnit.IsStatusEffectPassUnitTurn())
            {
                Debug.Log("Enemy passed their turn");
                UpdateUnitStatusDialogue(playerUnit);
                yield return new WaitForSeconds(2f);
                state = BattleState.ENEMYTURN;
                EnemyTurn();
                StopCoroutine(nameof(PlayerTurnManager));
            }
            playerUnit.IsUnitDeadFromStatusEffect();
            if (playerUnit.GetStatusEffectString() != "")
            {
                playerHUD.SetHP(playerUnit);
                Debug.Log("Damage Calculated");
                UpdateUnitStatusDialogue(playerUnit);
                yield return new WaitForSeconds(2f);
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
    public void SetDialogueText(bool setActive, string message)
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
        playerUnit.HealUnit(5);

        playerHUD.SetHP(playerUnit);
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
    private void MoveManager(MovesData moveData)
    {
        if (moveData == null)
        {
            Debug.Log("ERROR: Missing move data");
            return;
        }
        SetDialogueText(true, "");

        if (state == BattleState.PLAYERTURN)
        {
            switch (moveData.moveType)
            {
                case MoveType.NONE:
                    Debug.Log("move data: NONE");
                    Debug.Log("ERROR: No move assigned");
                    break;
                case MoveType.PHYSICAL:
                    Debug.Log("move data: PHYSICAL");
                    //Type, power, accuracy
                    StartCoroutine(UnitPhysicalAttack(playerUnit, enemyUnit, moveData));
                    break;
                case MoveType.SPECIAL:
                    Debug.Log("move data: SPECIAL");
                    StartCoroutine(UnitSpecialAttack(moveData));
                    //Type, power, accuracy
                    break;
                case MoveType.BUFF:
                    Debug.Log("move data: BUFF");
                    playerUnit.HealUnit(5);
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
                StartCoroutine(PlayerPoison(moveName, isStatusApplied));
                break;
            case StatusEffect.SLEEP:
                StartCoroutine(PlayerSleep(moveName, isStatusApplied));
                break;
            case StatusEffect.FROZEN:
                StartCoroutine(PlayerFrozen(moveName, isStatusApplied));
                break;
            case StatusEffect.PARALYSIS:
                StartCoroutine(PlayerParalysis(moveName, isStatusApplied));
                break;
        }
    }
    #region Status to apply to enemy
    IEnumerator UnitBurn(UnitInstance unitAttacking, UnitInstance unitDefending, string moveName, bool isStatusApplied)
    {
        dialogue.text = $"{unitAttacking.unitData.displayName} used {moveName}!";
        yield return new WaitForSeconds(2f);
        if (isStatusApplied)
        {
            dialogue.text = $"{unitDefending.unitData.displayName} has been burned!";
            //Debug.Log("Dialogue Updated to burn effect");
            yield return new WaitForSeconds(2f);
            enemyHUD.UpdateStatusEffect(enemyUnit);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            if (enemyUnit.GetUnitStatusEffect() == StatusEffect.BURN)
            {
                dialogue.text = $"{unitDefending.unitData.displayName} is already burned!";
            }
            else if (enemyUnit.GetUnitStatusEffect() != StatusEffect.NONE)
            {
                dialogue.text = $"{unitDefending.unitData.displayName} cannot have more than one status effects!";
            }
            else
            {
                dialogue.text = $"{unitAttacking.unitData.displayName} missed!";
            }
            yield return new WaitForSeconds(2f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator PlayerPoison(string moveName, bool isStatusApplied)
    {
        dialogue.text = $"You used {moveName}!";
        yield return new WaitForSeconds(2f);
        if (isStatusApplied)
        {
            enemyHUD.UpdateStatusEffect(enemyUnit);
            dialogue.text = $"{enemyUnit.name} has been poisoned!";
            //Debug.Log("Dialogue Updated to burn effect");
            yield return new WaitForSeconds(2f);
            enemyHUD.UpdateStatusEffect(enemyUnit);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            if (enemyUnit.GetUnitStatusEffect() == StatusEffect.POISON)
            {
                dialogue.text = $"{enemyUnit.name} is already poisoned!";
            }
            else if (enemyUnit.GetUnitStatusEffect() != StatusEffect.NONE)
            {
                dialogue.text = $"{enemyUnit.name} cannot have 2 status effects!";
            }
            else
            {
                dialogue.text = $"You missed!";
            }
            yield return new WaitForSeconds(2f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator PlayerSleep(string moveName, bool isStatusApplied)
    {
        dialogue.text = $"You used {moveName}!";
        yield return new WaitForSeconds(2f);
        if (isStatusApplied)
        {
            enemyHUD.UpdateStatusEffect(enemyUnit);
            dialogue.text = $"{enemyUnit.name} fell asleep!";
            //Debug.Log("Dialogue Updated to burn effect");
            yield return new WaitForSeconds(2f);
            enemyHUD.UpdateStatusEffect(enemyUnit);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            if (enemyUnit.GetUnitStatusEffect() != StatusEffect.SLEEP)
            {
                dialogue.text = $"{enemyUnit.name} is already asleep!";
            }
            else if (enemyUnit.GetUnitStatusEffect() != StatusEffect.NONE)
            {
                dialogue.text = $"{enemyUnit.name} cannot have 2 status effects!";
            }
            else
            {
                dialogue.text = $"You missed!";
            }
            yield return new WaitForSeconds(2f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator PlayerFrozen(string moveName, bool isStatusApplied)
    {
        dialogue.text = $"You used {moveName}!";
        yield return new WaitForSeconds(2f);
        if (isStatusApplied)
        {
            enemyHUD.UpdateStatusEffect(enemyUnit);
            dialogue.text = $"{enemyUnit.name} is frozen solid!";
            //Debug.Log("Dialogue Updated to burn effect");
            yield return new WaitForSeconds(2f);
            enemyHUD.UpdateStatusEffect(enemyUnit);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            if (enemyUnit.GetUnitStatusEffect() == StatusEffect.FROZEN)
            {
                dialogue.text = $"{enemyUnit.name} is already frozen!";
            }
            else if (enemyUnit.GetUnitStatusEffect() != StatusEffect.NONE)
            {
                dialogue.text = $"{enemyUnit.name} cannot have 2 status effects!";
            }
            else
            {
                dialogue.text = $"You missed!";
            }
            yield return new WaitForSeconds(2f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator PlayerParalysis(string moveName, bool isStatusApplied)
    {
        dialogue.text = $"You used {moveName}!";
        yield return new WaitForSeconds(2f);
        if (isStatusApplied)
        {
            enemyHUD.UpdateStatusEffect(enemyUnit);
            dialogue.text = $"{enemyUnit.name} is paralysed!";
            //Debug.Log("Dialogue Updated to burn effect");
            yield return new WaitForSeconds(2f);
            enemyHUD.UpdateStatusEffect(enemyUnit);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            if (enemyUnit.GetUnitStatusEffect() == StatusEffect.PARALYSIS)
            {
                dialogue.text = $"{enemyUnit.name} is already paralysed!";
            }
            else if (enemyUnit.GetUnitStatusEffect() != StatusEffect.NONE)
            {
                dialogue.text = $"{enemyUnit.name} cannot have 2 status effects!";
            }
            else
            {
                dialogue.text = $"You missed!";
            }
            yield return new WaitForSeconds(2f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    #endregion
    IEnumerator UnitPhysicalAttack(UnitInstance unitAttacking, UnitInstance unitDefending, MovesData moveData)
    {
        dialogue.text = $"{unitAttacking.unitData.displayName} used {moveData.moveName}!";
        yield return new WaitForSeconds(2f);
        bool isDead = unitAttacking.PhysicalAttack(unitDefending, moveData.elementalType, moveData.power);
        enemyHUD.SetHP(enemyUnit);
        if (unitAttacking.IsUnitCriticalAttack() == true)
        {
            dialogue.text = "Critical Hit!";
            yield return new WaitForSeconds(2f);
        }
        yield return new WaitForSeconds(2f);
        if (isDead && state == BattleState.PLAYERTURN)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator UnitSpecialAttack(MovesData moveData)
    {
        dialogue.text = $"You used {moveData.moveName}!";
        yield return new WaitForSeconds(2f);
        bool isDead = playerUnit.SpecialAttack(enemyUnit, moveData.elementalType, moveData.power);
        enemyHUD.SetHP(enemyUnit);
        if (playerUnit.IsUnitCriticalAttack() == true)
        {
            dialogue.text = "Critical Hit!";
            yield return new WaitForSeconds(2f);
        }
        yield return new WaitForSeconds(2f);
        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator EnemyTurn()
    {
        hideActionMoveMenu();
        Debug.Log("Enemy Turn Start");

        if (enemyUnit.GetUnitStatusEffect() != StatusEffect.NONE)
        {
            //enemyHUD.UpdateStatusEffect(enemyUnit);
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
                    enemyHUD.SetHP(enemyUnit);
                    Debug.Log("Damage Calculated");
                    UpdateUnitStatusDialogue(enemyUnit);
                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    enemyHUD.SetHP(enemyUnit);
                    UpdateUnitStatusDialogue(enemyUnit);
                    yield return new WaitForSeconds(2f);
                    state = BattleState.WON;
                    EndBattle();
                    yield break;
                }

            }
        }

        dialogue.text = $"{enemyUnit.unitData.displayName} attacks!";
        yield return new WaitForSeconds(1f);

        //Apply status effect to player.
        /* enemyUnit.ApplyStatusEffect(playerUnit, ElementTypes.FIRE, StatusEffect.BURN, 100);
        playerHUD.UpdateStatusEffect(playerUnit);*/

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
        }
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
            dialogue.text = $"{enemyUnit.unitData.displayName} has drop some items!";
        else
            Debug.LogError("THIS STATE SHOULD BE ONLY WON. In enemey drop BattleSystem Script");
        yield return new WaitForSeconds(2f);
        OnButtonSetInventoryActive();
        inventoryMenu.GetComponent<InventoryButtonManager>().OpenInventoryWithDrop();
    }
    private void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogue.text = $"You defeated {enemyUnit.unitData.displayName}!";
            StartCoroutine(EnemeyDrop());
        }
        else if (state == BattleState.LOST)
        {
            dialogue.text = $"You were defeated by {enemyUnit.unitData.displayName}!";
            StartCoroutine(ReturnToMenu());
        }
    }
    private void UpdateUnitStatusDialogue(UnitInstance unitInstance)
    {
        dialogue.text = $"{unitInstance.GetStatusEffectString()}";
    }
}
