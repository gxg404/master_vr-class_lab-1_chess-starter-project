/*
 * Copyright (c) 2018 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */


using UnityEngine;

public class Board : MonoBehaviour
{
    public Material defaultMaterial;
    public Material selectedMaterial;

    // variables for animated movement
    private GameObject movingPiece = null;
    private Vector3 movingDirection = Vector3.zero;
    private Vector3 targetPosition;
    public float moveForce = 18f;
    public float stopDistance = 0.1f; // etwas toleranz

    public GameObject AddPiece(GameObject piece, int col, int row)
    {
        Vector2Int gridPoint = Geometry.GridPoint(col, row);
        GameObject newPiece = Instantiate(piece, Geometry.PointFromGrid(gridPoint),
            Quaternion.identity, gameObject.transform);
        return newPiece;
    }

    public void RemovePiece(GameObject piece)
    {
        Destroy(piece);
    }

    public void MovePiece(GameObject piece, Vector2Int gridPoint)
    {
        // adapted for linear piece movement/animation
        movingPiece = piece;
        targetPosition = Geometry.PointFromGrid(gridPoint);
        targetPosition.y = piece.transform.position.y;

        // activate action block until move is finished
        GameManager.instance.waitUntilMoveFinished = true;

        UpdateDirection();

        Debug.Log("Piece Position: " + piece.transform.position);
        Debug.Log("Target Position: " + targetPosition);
        Debug.Log("Direction: " + movingDirection);

        ConstantForce cf = piece.GetComponent<ConstantForce>();
        if (cf == null)
        {
            cf = piece.AddComponent<ConstantForce>();
        }

        cf.force = movingDirection * moveForce;
    }

    public void SelectPiece(GameObject piece)
    {
        MeshRenderer renderers = piece.GetComponentInChildren<MeshRenderer>();
        renderers.material = selectedMaterial;
    }

    public void DeselectPiece(GameObject piece)
    {
        MeshRenderer renderers = piece.GetComponentInChildren<MeshRenderer>();
        renderers.material = defaultMaterial;
    }

    void Update()
    {
        if (movingPiece != null)
        {
            // movement will continue until distance <= stopDistance
            Vector3 currentPos = new Vector3(movingPiece.transform.position.x, movingPiece.transform.position.y, movingPiece.transform.position.z);
            float distance = Vector3.Distance(currentPos, targetPosition);
            ConstantForce cf = movingPiece.GetComponent<ConstantForce>();

            if (distance <= stopDistance)
            {
                // remove force
                if (cf != null)
                {
                    Destroy(cf);
                }

                // stop rest-movement
                Rigidbody rb = movingPiece.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                // center
                movingPiece.transform.position = new Vector3(targetPosition.x, movingPiece.transform.position.y, targetPosition.z);

                // move piece logically
                Vector2Int gridPoint = Geometry.GridFromPoint(targetPosition);
                if (GameManager.instance.PieceAtGrid(gridPoint) == null)
                {
                    GameManager.instance.Move(movingPiece, gridPoint);
                }
                else
                {
                    GameManager.instance.CapturePieceAt(gridPoint);
                    GameManager.instance.Move(movingPiece, gridPoint);
                }

                movingPiece = null;
                GameManager.instance.waitUntilMoveFinished = false;
            } else
            {
                UpdateDirection();
                cf.force = movingDirection * moveForce;
            }
        }
    }

    // easier recalculation for per frame update of direction in case pieces crash and position is changed through external force
    private void UpdateDirection()
    {
        if (movingPiece != null)
        {
            movingDirection = (targetPosition - movingPiece.transform.position).normalized;
        }
    }
}
