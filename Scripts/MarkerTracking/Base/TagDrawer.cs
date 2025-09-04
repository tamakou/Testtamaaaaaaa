using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Draws simple wireframe cube
/// </summary>
/// <remarks>
///     <para>
///     Credit: Keijiro Takahashi's April Tags for Unity: <a href="https://github.com/keijiro/jp.keijiro.apriltag"> Github Repository Link</a>.
///     </para>
/// </remarks>
sealed class TagDrawer : System.IDisposable
{
    public bool IsDisposed { get; private set; }
    private Mesh _mesh;
    private Material _sharedMaterial;


    public TagDrawer(Material material)
    {
        _mesh = BuildMesh();
        _sharedMaterial = material;
        IsDisposed = false;
    }

    public void Dispose()
    {
        Object.Destroy(_mesh);
        _mesh = null;
        _sharedMaterial = null;
        IsDisposed = true;
    }

    public void Draw(Vector3 position, Quaternion rotation, float scale)
    {
        var xform = Matrix4x4.TRS(position, rotation, Vector3.one * scale);
        Graphics.DrawMesh(_mesh, xform, _sharedMaterial, 0);
    }
    static int gridNumber = 100;
    static float gridDistance = 0.15f;
    static Mesh BuildMesh()
    {
        List<Vector3> listPosition = new List<Vector3>();
        for (int i = 0; i < gridNumber; i++)
        {
            for (int j = 0; j < gridNumber; j++)
            {
                listPosition.Add(new Vector3(i * gridDistance - (gridDistance * gridNumber) / 2, 0, j * gridDistance - (gridDistance * gridNumber) / 2));
            }
        }
        List<int> ind = new List<int>();


        for (int i = 0; i < listPosition.Count; i += gridNumber)
        {
            ind.Add(i);
            ind.Add(i + gridNumber - 1);
        }

        for (int i = 0; i < gridNumber; i++)
        {
            ind.Add(i);
            ind.Add(gridNumber * gridNumber - gridNumber + i);
        }
        var mesh = new Mesh();
        mesh.vertices = listPosition.ToArray();
        mesh.SetIndices(ind.ToArray(), MeshTopology.Lines, 0);
        return mesh;
    }
}
