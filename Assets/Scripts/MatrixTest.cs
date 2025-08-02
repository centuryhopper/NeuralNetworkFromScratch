using System.Collections.Generic;
using System.Linq;
using Scripts.Utils;
using UnityEngine;

public class MatrixTest : MonoBehaviour
{
    public float[,,] array3d = {
        {
            { -3, 2, -1 },
            { 6, -6, 7 },
            { 3,-4, 4}
        },
        {
            { 1,2,3 },
            { 0,1,4 },
            { 5,6,0}
        },
        {
            { 2,-1,0 },
            { -1,2,-1 },
            { 0,-1,2}
        },
        {
            { 3,2,-1 },
            { 2,-2,4 },
            { -1,0.5f,-1}
        },
    };

    public float[,,] fourByfours = {
{
    {1,0,2,-1},
    {3,0,0,5},
    {2,1,4,-3},
    {1,0,5,0},
},

{
    {1,0,4,-6},
    {2,5,0,3},
    {-1,2,3,5},
    {2,1,-2,3},
},

{
    {1,2,3,4},
    {1,0,2,0},
    {0,1,2,3},
    {2,3,0,0},
},

    };

    private List<float[,]> Extract2DSlices(float[,,] array3D)
    {
        int depth = array3D.GetLength(0);
        int rows = array3D.GetLength(1);
        int cols = array3D.GetLength(2);

        return Enumerable.Range(0, depth)
            .Select(d =>
            {
                var slice = new float[rows, cols];
                Enumerable.Range(0, rows).ToList().ForEach(i =>
                    Enumerable.Range(0, cols).ToList().ForEach(j =>
                        slice[i, j] = array3D[d, i, j]
                    )
                );
                return slice;
            }).ToList();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var slices = Extract2DSlices(fourByfours);
        foreach (var slice in slices)
        {
            Matrix matrix = Matrix.From2dArray(slice);
            matrix.Print();
            var det = Matrix.DeterminantNaive(matrix);
            print($"determinant is {det}");
        }
        // print("###################");
        // var subMatrix = getSubMatrix(matrix, 0);
        // subMatrix.Print();
        // print("###################");
        // subMatrix = getSubMatrix(matrix, 1);
        // subMatrix.Print();
        // print("###################");
        // subMatrix = getSubMatrix(matrix, 2);
        // subMatrix.Print();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
