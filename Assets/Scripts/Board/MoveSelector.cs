using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MoveSelector : MonoBehaviour
{
    public GameObject moveLocationPrefab;
    public GameObject tileHighlightPrefab;
    public GameObject attackLocationPrefab;

    private GameObject tileHighlight;
    private GameObject movingPiece;
    private List<Vector2Int> moveLocations;
    private List<GameObject> locationHighlights;

    // Start is called before the first frame update
    void Start()
    {
        this.enabled = false;
        tileHighlight = Instantiate(tileHighlightPrefab, Geometry.PointFromGrid(new Vector2Int(0, 0)), Quaternion.identity, gameObject.transform);
        tileHighlight.SetActive(false);
    }

    public void EnterState(GameObject piece)
    {
        movingPiece = piece;
        this.enabled = true;

        moveLocations = GameManager.instance.MovesForPiece(movingPiece);
        locationHighlights = new List<GameObject>();

        foreach (Vector2Int loc in moveLocations)
        {
            GameObject highlight;
            if (GameManager.instance.PieceAtGrid(loc))
            {
                highlight = Instantiate(attackLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
            }
            else
            {
                highlight = Instantiate(moveLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
            }
            locationHighlights.Add(highlight);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 point = hit.point;
            Vector2Int gridPoint = Geometry.GridFromPoint(point);

            tileHighlight.SetActive(true);
            tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);
            
            if (Input.GetMouseButtonDown(0))
            {
                // if the moving piece is clicked again
                if (movingPiece == GameManager.instance.PieceAtGrid(gridPoint))
                {
                    ExitState();
                    return;
                }

                // return if the clicked-point is not in the move-locations
                if (!moveLocations.Contains(gridPoint))
                {
                    return;
                }

                if (GameManager.instance.waitUntilMoveFinished)
                {
                    Debug.Log("Wait till oponents move is done!");
                }
                else
                {
                    GameManager.instance.board.MovePiece(movingPiece, gridPoint);
                    ExitStateNextPlayer();
                }
            }
        }
        else
        {
            tileHighlight.SetActive(false);
        }
    }

    // Switches back to piece selection if chosen piece is clicked again, so another piece can be selected...
    // Stays on same player
    private void ExitState()
    {
        this.enabled = false;
        tileHighlight.SetActive(false);
        GameManager.instance.DeselectPiece(movingPiece);
        movingPiece = null;
        TileSelector selector = GetComponent<TileSelector>();
        selector.EnterState();
        foreach(GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }
    }

    // Switches back to piece selection
    // Switches to NEXT PLAYER
    private void ExitStateNextPlayer()
    {
        this.enabled = false;
        tileHighlight.SetActive(false);
        GameManager.instance.DeselectPiece(movingPiece);
        movingPiece = null;
        TileSelector selector = GetComponent<TileSelector>();
        GameManager.instance.NextPlayer();
        selector.EnterState();
        foreach(GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }
    }
}