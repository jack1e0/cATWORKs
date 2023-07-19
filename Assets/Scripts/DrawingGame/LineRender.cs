using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LineRender : MonoBehaviour {
    private LineRenderer lr;
    private List<Vector2> points;
    public Color lineCol;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
        lineCol = lr.material.color;
    }

    public void UpdatePoint(Vector2 pos) {
        if (points == null) {
            points = new List<Vector2>();
            SetPoint(pos);
        } else if (Vector2.Distance(points.Last(), pos) > 0.1f) { // check that we are not adding same point twice
            SetPoint(pos);
        }
    }

    private void SetPoint(Vector2 point) {
        points.Add(point);
        lr.positionCount = points.Count;
        lr.SetPosition(points.Count - 1, point);
    }


}
