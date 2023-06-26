using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogueBox : MonoBehaviour
{
    [SerializeField] UnitInstance player;
    [SerializeField] List<Text> moveTexts;

    public void Awake()
    {
        SetMoveNames(player.moveList);
    }

    public void UpdatePlayerMoves()
    {
        SetMoveNames(player.moveList);
    }

    public void SetMoveNames(List<MovesData> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (moves[i] == null)
                moveTexts[i].text = "-";
            else if (i < moves.Count)
                moveTexts[i].text = moves[i].moveName;
            else
                moveTexts[i].text = "-";
        }
    }
}
