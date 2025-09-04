using UnityEngine;

public class DelayedLineRenderer : MonoBehaviour
{
    [SerializeField] private GameObject axis;
    [SerializeField] private GameObject centerLine;

    private void Update()
    {
        // axis が有効チェック
        if (!axis.activeInHierarchy)
        {
            // centerLine を無効化する
            centerLine.SetActive(false);
        }
        // centerLine の座標と角度を axis に合わせる
        centerLine.transform.position = axis.transform.position;
        centerLine.transform.rotation = axis.transform.rotation;
        // centerLine を有効化する
        centerLine.SetActive(true);
    }
}
