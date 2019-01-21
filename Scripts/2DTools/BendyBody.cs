using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sf = UnityEngine.SerializeField;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class BendyBody : MonoBehaviour
{
    private enum Direction
    {
        Up,
        Right
    }

    [sf] private Transform reference;

    [Header("Properties")]
    [sf] private Direction direction = Direction.Up;
    [sf] private float length = 1;
    [sf] private int segments = 5;

    [Header("Settings")]
    [sf] private CurveAsset bendCurve;
    [sf] [Range(0, 1)] private float onAxisIntensity = 1;
    [sf] [Range(0, 1)] private float offAxisIntensity = 1;

    private LineRenderer line;
    private Vector2 axis;
    private Vector2 coAxis;

    private void Awake()
    {
        InitializeLine();
    }
    private void Update()
    {
        if (bendCurve)
            SetVertexPositions();
    }

    private void InitializeLine()
    {
        // Set LineRenderer reference
        line = GetComponent<LineRenderer>();
        // Set number of positions
        line.positionCount = segments;
        // Set other line properties
        line.useWorldSpace = false;
        line.alignment = LineAlignment.TransformZ;

        // Set axis & coaxis based on direction
        switch (direction)
        {
            case Direction.Up:
                axis = Vector2.up;
                coAxis = Vector2.right;
                break;
            case Direction.Right:
                axis = Vector2.right;
                coAxis = Vector2.up;
                break;
        }
    }
    private void SetVertexPositions()
    {
        for (int i = 0; i < segments; i++)
        {
            // Percentage along line
            float t = (float)i / (segments - 1);
            // Get value from bend curve
            float curve = bendCurve.Evaluate(t);
            // Get on, off axis values (local x, y position of reference)
            float onAxis = (direction == Direction.Right ? reference.localPosition.x : reference.localPosition.y) - length;
            float offAxis = direction == Direction.Right ? reference.localPosition.y : reference.localPosition.x;

            // Set line position
            line.SetPosition(i, t * axis * length + onAxis * onAxisIntensity * curve * axis + offAxis * offAxisIntensity * curve * coAxis);
        }
    }

    // Editor Functionality
    private void OnValidate()
    {
        InitializeLine();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(axis) * length);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(axis) * length);
    }
}
