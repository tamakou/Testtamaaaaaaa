using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawBackgroundController : MonoBehaviour
{
    [Tooltip("Draw debug cube.")]
    [SerializeField] private bool _debugDraw;
    [SerializeField] private Material _tagMaterial = null;
    private TagDrawer _drawer;
    [Tooltip("Size of debug graphic.")]
    [SerializeField] float _debugWireCubeSize = 0.05f;

    public Transform Graphics;
    private IEnumerator Start()
    {
        if (_tagMaterial == null)
        {
            _tagMaterial = new Material(Shader.Find("Unlit/Color"));
            _tagMaterial.color = Color.yellow;
        }
        yield return null;
    }
    private void OnEnable()
    {
        if (_debugDraw)
        {
            _drawer = new TagDrawer(_tagMaterial);
        }
    }
    private void OnDisable()
    {
        _drawer?.Dispose();
    }
    private void Update()
    {
        if (_debugDraw)
        {
            _drawer?.Draw(Graphics.transform.position, Graphics.transform.rotation, _debugWireCubeSize);
        }
    }
}
