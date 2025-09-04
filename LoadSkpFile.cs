using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using TextSketchUpFormat;

namespace TextSketchUpFormat
{
    public partial class LoadSkp
    {
        public void LoadSkpFile(string dataFile,
            Vector3 position, Quaternion rotation, string colorName = "Yellow", float lineSize = 0.01f,
            Action<GameObject> onFinished = null)
        {
            // 初期カラーは未指定の場合は黄色を設定する
            // Màu ban đầu được đặt là màu vàng.
            var lastMaterial = GetMaterial(colorName);
            // TSKPデータを解析して線分情報を作成する
            // Phân tích dữ liệu TSKP để tạo thông tin đoạn thẳng.
            _edges = Regex.Split(dataFile, @"\r\n|\n|\r")
                .Select(x => x.Split(','))
                .Select(x =>
                {
                    // Color で始まる行はマテリアルの変更を表す
                    // Các dòng bắt đầu bằng "Color" đại diện cho việc thay đổi vật liệu.
                    if (x.Length == 2 && x[0].ToLower().StartsWith("color"))
                    {
                            //StatusText = $"Color = {x[1]}";
                            lastMaterial = GetMaterial(x[1]);
                    }
                    // 6個の要素を持たない行は無視する
                    // Các dòng không chứa 6 phần tử sẽ bị bỏ qua.
                    if (x.Length != 6)
                    {
                            //StatusText = $"x.Length = {x.Length}";
                            return new LoadSkpEdge { CreateSurface = true };
                    }
                    // 6個の要素を持つ行は線分を表す
                    // Các dòng có 6 phần tử đại diện cho đoạn thẳng.
                    var values = x.Select(y => float.TryParse(y, out var value) ? value : 0f).ToArray();
                    //StatusText = $"({values[0]}, {values[1]}, {values[2]}) - ({values[3]}, {values[4]}, {values[5]}))";
                    // TSKPファイルの線分情報を生成する
                    // Tạo thông tin đoạn thẳng của tệp TSKP.
                    return new LoadSkpEdge
                    {
                        CreateSurface = false,
                        Material = lastMaterial,
                        Start = new Vector3(values[0], values[1], values[2]),
                        End = new Vector3(values[3], values[4], values[5])
                    };
                })
                .ToList();

            StatusText = $"Edges = {_edges.Count}";
            int i = 0;
            // 生成された線分情報を元に、LineRenderer を生成する
            // Tạo LineRenderer dựa trên thông tin đoạn thẳng được tạo ra.
            _lineRenderers = _edges
                .Where(x => !x.CreateSurface)
                .Select(x =>
                {
                    var lrgo = new GameObject();
                    lrgo.transform.parent = transform;
                    lrgo.transform.position = Vector3.zero;
                    // Y軸に180度回転させた値を設定する
                    // Đặt giá trị đã quay 180 độ quanh trục Y.
                    lrgo.transform.rotation = Quaternion.Euler(0, 0, 0);
                    var lr = lrgo.AddComponent<LineRenderer>();
                    lr.positionCount = 2;
                    lr.startWidth = lineSize;
                    lr.endWidth = lineSize;
                    lr.useWorldSpace = false;
                    // lrgo の LineRenderer は親オブジェクトの座標を基本として描画される
                    // そのため、親オブジェクトの座標を基準にした座標を設定する必要がある
                    // ここでは、x, y, z の順番を変えている
                    // LineRenderer của lrgo được vẽ dựa trên tọa độ của đối tượng cha
                    // Do đó, cần thiết phải thiết lập tọa độ dựa trên tọa độ của đối tượng cha
                    // Ở đây, thứ tự của x, y, z đã được thay đổi.
                    lr.SetPosition(0, new Vector3(x.Start.x, x.Start.z, x.Start.y));
                    lr.SetPosition(1, new Vector3(x.End.x, x.End.z, x.End.y));
                    lr.material = x.Material;
                    i++;
                    //StatusText = $"current i = {i}";
                    return lr;
                })
                .ToList();
            StatusText = $"total = {_lineRenderers.Count}";
            // 読み込み完了フラグセット
            // Đặt cờ hoàn thành đọc.
            onFinished?.Invoke(this.gameObject);
            // オブジェクトの位置と回転角度をセットする
            // Đặt vị trí và góc quay của đối tượng.
            transform.position = position;
            transform.rotation = rotation;
        }

    }

}