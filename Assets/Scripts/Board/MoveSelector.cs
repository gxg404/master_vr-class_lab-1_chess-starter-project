using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State 2: a piece of the current player is held. Shows the tiles it may move to
// and executes the move as soon as the piece is released.
public class MoveSelector : MonoBehaviour
{
    public GameObject moveLocationPrefab;
    public GameObject tileHighlightPrefab;
    public GameObject attackLocationPrefab;

    private GameObject tileHighlight;
    private GameObject movingPiece;
    private OVRGrabbable movingGrabbable;
    private List<Vector2Int> moveLocations;
    private List<GameObject> locationHighlights = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        this.enabled = false;
        tileHighlight = SpawnMarker(tileHighlightPrefab, new Vector2Int(0, 0), 2);
        tileHighlight.SetActive(false);
    }

    public void EnterState(GameObject piece)
    {
        movingPiece = piece;
        movingGrabbable = piece.GetComponent<OVRGrabbable>();
        this.enabled = true;

        moveLocations = GameManager.instance.MovesForPiece(movingPiece);
        locationHighlights = new List<GameObject>();

        foreach (Vector2Int loc in moveLocations)
        {
            GameObject prefab = GameManager.instance.PieceAtGrid(loc) ? attackLocationPrefab : moveLocationPrefab;
            locationHighlights.Add(SpawnMarker(prefab, loc, 1));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (movingPiece == null || movingGrabbable == null)
        {
            ExitState();
            return;
        }

        Vector2Int gridPoint = Geometry.GridFromPoint(movingPiece.transform.position);

        // piece is still held -> highlight the valid tile below it
        if (movingGrabbable.isGrabbed)
        {
            if (moveLocations.Contains(gridPoint))
            {
                tileHighlight.SetActive(true);
                // one layer above the move markers to avoid z-fighting
                tileHighlight.transform.position = Geometry.MarkerPointFromGrid(gridPoint, 2);
            }
            else
            {
                tileHighlight.SetActive(false);
            }
            return;
        }

        // piece was released
        if (moveLocations.Contains(gridPoint))
        {
            GameManager.instance.MoveOrCapture(movingPiece, gridPoint);
            ExitStateNextPlayer();
        }
        else
        {
            // invalid tile, own tile or next to the board -> piece goes back, same player again
            ExitState();
        }
    }

    // Switches back to piece selection, stays on same player
    private void ExitState()
    {
        Cleanup();
        GameManager.instance.SnapAllPieces();
        GetComponent<TileSelector>().EnterState();
    }

    // Switches back to piece selection with the NEXT PLAYER
    private void ExitStateNextPlayer()
    {
        Cleanup();
        GameManager.instance.SnapAllPieces();
        GameManager.instance.NextPlayer();
        GetComponent<TileSelector>().EnterState();
    }

    // Markers are purely visual: their colliders are disabled so pieces don't bump into them
    private GameObject SpawnMarker(GameObject prefab, Vector2Int gridPoint, int layer)
    {
        GameObject marker = Instantiate(prefab, Geometry.MarkerPointFromGrid(gridPoint, layer), Quaternion.identity, gameObject.transform);
        foreach (Collider c in marker.GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }
        return marker;
    }

    private void Cleanup()
    {
        this.enabled = false;
        tileHighlight.SetActive(false);
        if (movingPiece != null)
        {
            GameManager.instance.DeselectPiece(movingPiece);
        }
        movingPiece = null;
        movingGrabbable = null;
        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }
        locationHighlights.Clear();
    }
}
