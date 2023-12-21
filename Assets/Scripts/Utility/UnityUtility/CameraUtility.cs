using System;
using UnityEngine;

namespace Utility.UnityUtility
{
    public static class CameraUtility
    {
        //borderX and borderY is border percentage thickness in ViewPort from 0 to 0.5
        public static Vector3 ClampPointToViewportWithBorder(Camera camera, Vector3 pointToClamp, float borderX = 0.15f, float borderY = 0.15f)
        {
            Vector3 viewPoint = camera.WorldToViewportPoint(pointToClamp);
            float clampX = Math.Clamp(viewPoint.x, borderX, 1 - borderX);
            float clampY = Math.Clamp(viewPoint.y, borderY, 1 - borderY);
            viewPoint = new Vector3(clampX, clampY, viewPoint.z);
            Vector3 worldPoint = camera.ViewportToWorldPoint(viewPoint);
            return worldPoint;
        }
    }
}