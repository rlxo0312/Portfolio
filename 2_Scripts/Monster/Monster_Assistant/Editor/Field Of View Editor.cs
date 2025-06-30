using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fieldOfView = (FieldOfView)target;

        if (fieldOfView == null || fieldOfView.VisibleTarget == null)
        {
            return;

        } 
        DrawFieldofview(fieldOfView);
        /*Handles.BeginGUI();

        Handles.color = Color.white;
        Handles.DrawWireArc(fieldOfView.transform.position, Vector3.up, Vector3.forward, 360, fieldOfView.enemy.viewRange);

        Vector3 viewAngleLeft = DirectionFromAngle(fieldOfView.viewAngle / 2 * -1);
        Vector3 viewAngleRight = DirectionFromAngle(fieldOfView.viewAngle / 2);

        Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + viewAngleLeft * fieldOfView.enemy.viewRange);
        Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + viewAngleRight * fieldOfView.enemy.viewRange);

        Handles.color = Color.red;
        foreach(Transform visibleTarget in fieldOfView.VisibleTarget)
        {
            if (visibleTarget != null)  
            {
                Handles.DrawLine(fieldOfView.transform.position, visibleTarget.position);
            }
        }
        Handles.EndGUI();*/
    } 

    private void DrawFieldofview(FieldOfView fieldOfView)
    {
        Handles.BeginGUI();

        Handles.color = Color.white;
        Handles.DrawWireArc(fieldOfView.transform.position, Vector3.up, Vector3.forward, 360, fieldOfView.fieldViewRange);

        Vector3 viewAngleLeft = DirectionFromAngle(fieldOfView.viewAngle / 2 * -1);
        Vector3 viewAngleRight = DirectionFromAngle(fieldOfView.viewAngle / 2);

        Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + viewAngleLeft * fieldOfView.fieldViewRange);
        Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + viewAngleRight * fieldOfView.fieldViewRange);

        Handles.color = Color.red;
        foreach (Transform visibleTarget in fieldOfView.VisibleTarget)
        {
            if (visibleTarget != null)
            {
                Handles.DrawLine(fieldOfView.transform.position, visibleTarget.position);
            }
        }
        Handles.EndGUI();
    }

    private Vector3 DirectionFromAngle(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
