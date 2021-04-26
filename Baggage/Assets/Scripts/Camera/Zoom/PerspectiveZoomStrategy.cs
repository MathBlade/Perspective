using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveZoomStrategy : IZoomStrategy
{
    public PerspectiveZoomStrategy(Camera cam, Vector2 offset, float startingZoomLevel)
    {
        baseZoomLevel = startingZoomLevel;
        normalizedCameraPosition = new Vector3(0f, Mathf.Abs(offset.y), -Mathf.Abs(offset.x)).normalized;
        currentZoomLevel = startingZoomLevel;
        PositionCamera(cam);

    }

    public void ZoomIn(Camera cam, float delta, float nearZoomLimit)
    {
        if (currentZoomLevel <= nearZoomLimit) return;
        currentZoomLevel = Mathf.Max(currentZoomLevel - delta, nearZoomLimit);
        PositionCamera(cam);
    }

    public void ZoomOut(Camera cam, float delta, float farZoomLimit)
    {
        if (currentZoomLevel >= farZoomLimit) return;
        currentZoomLevel = Mathf.Min(currentZoomLevel + delta, farZoomLimit);
        PositionCamera(cam);
    }

    void PositionCamera(Camera cam) => cam.transform.localPosition = normalizedCameraPosition * currentZoomLevel;
    
    public void Reset(Camera cam) => cam.transform.localPosition = normalizedCameraPosition * baseZoomLevel;

    Vector3 normalizedCameraPosition;
    float currentZoomLevel;
    float baseZoomLevel;
}
