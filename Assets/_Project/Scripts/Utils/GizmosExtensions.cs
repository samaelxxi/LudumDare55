using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmosExtensions
{
    public static void DrawArrow(Vector3 pos, Vector3 direction, float arrowLength, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
        var arrowTip = pos + direction * arrowLength;
        Gizmos.DrawLine(pos, arrowTip);

        Camera c = Camera.current;
        if (c == null) return;
        Vector3 right = Quaternion.LookRotation(direction, c.transform.forward) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction, c.transform.forward) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawLine(arrowTip, arrowTip + right * arrowHeadLength);
        Gizmos.DrawLine(arrowTip, arrowTip + left * arrowHeadLength);
    }
}
