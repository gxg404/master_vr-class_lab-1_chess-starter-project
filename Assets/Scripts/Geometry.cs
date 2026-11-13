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

public class Geometry
{
    // board is centered at the world origin, 8x8 tiles
    public const float TileSize = 0.1f;
    public const float BoardHalfSize = 4 * TileSize; // distance from center to board edge
    // y of the board surface (piece pivot). Measured from the board collider in Board.Awake,
    // this is only the fallback value.
    public static float BoardSurfaceY = 1.3225f;
    // markers are lifted slightly above the surface so they are not hidden inside the board
    public const float MarkerOffset = 0.002f;

    static public Vector3 PointFromGrid(Vector2Int gridPoint)
    {
        // center of the tile
        float x = -BoardHalfSize + TileSize * (gridPoint.x + 0.5f);
        float z = -BoardHalfSize + TileSize * (gridPoint.y + 0.5f);
        return new Vector3(x, BoardSurfaceY, z);
    }

    // position for highlight markers on a tile; layer > 0 stacks markers above each other
    static public Vector3 MarkerPointFromGrid(Vector2Int gridPoint, int layer = 1)
    {
        return PointFromGrid(gridPoint) + Vector3.up * (MarkerOffset * layer);
    }

    static public Vector2Int GridPoint(int col, int row)
    {
        return new Vector2Int(col, row);
    }

    static public Vector2Int GridFromPoint(Vector3 point)
    {
        // inverse of PointFromGrid (was still using a tile size of 1 before the rescale)
        int col = Mathf.FloorToInt((point.x + BoardHalfSize) / TileSize);
        int row = Mathf.FloorToInt((point.z + BoardHalfSize) / TileSize);
        return new Vector2Int(col, row);
    }
}
