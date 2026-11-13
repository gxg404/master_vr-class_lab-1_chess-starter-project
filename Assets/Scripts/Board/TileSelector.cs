using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State 1: waits until the current player grabs one of their pieces with a touch controller.
public class TileSelector : MonoBehaviour
{
    // true if a piece was grabbed that may not be moved (opponent's piece) -> put it back after release
    private bool needsResnap = false;

    public void EnterState()
    {
        enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        // selection via grabbing (replaces mouse + raycasting)
        List<GameObject> heldPieces = GameManager.instance.HeldPieces();

        if (GameManager.instance.IsGrabTriggerPressed())
        {
            foreach (GameObject piece in heldPieces)
            {
                if (GameManager.instance.DoesPieceBelongToCurrentPlayer(piece))
                {
                    // 1. the piece is selected (colored)
                    GameManager.instance.SelectPiece(piece);
                    // 2. TileSelector is disabled and piece passed as movingPiece to MoveSelector
                    ExitState(piece);
                    return;
                }
            }
        }

        if (heldPieces.Count > 0)
        {
            // only pieces of the other player are held
            needsResnap = true;
        }
        else if (needsResnap)
        {
            needsResnap = false;
            GameManager.instance.SnapAllPieces();
        }
    }

    private void ExitState(GameObject movingPiece)
    {
        this.enabled = false;
        MoveSelector move = GetComponent<MoveSelector>();
        move.EnterState(movingPiece);
    }
}
