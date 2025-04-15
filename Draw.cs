﻿using UnityEngine;

namespace IMGUIDebugDraw
{
    public class Draw
    {
        public static GUIStyle StringStyle { get; set; } = new GUIStyle(GUI.skin.label);

        public static void Label(Vector2 position, string label, Color color, bool centered = true)
        {
            StringStyle.normal.textColor = color;
            Label(position, label, centered);
        }

        public static void Label(Vector2 position, string label, bool centered = true)
        {
            GUIContent content = new GUIContent(label);
            Vector2 size = StringStyle.CalcSize(content);
            GUI.Label(new Rect(centered ? (position - size / 2f) : position, size), content, StringStyle);
        }

        public static void Label(Camera cam, Vector3 worldPos, string label, Color color)
        {
            if (!IsVisible(cam, worldPos)) return;
            Vector2 screenPos = WorldToScreen(cam, worldPos);
            Label(screenPos, label, color);
        }

        public static void Label(Camera cam, Vector3 worldPos, string label)
        {
            if (!IsVisible(cam, worldPos)) return;
            Vector2 screenPos = WorldToScreen(cam, worldPos);
            Label(screenPos, label);
        }

        public static void Crosshair(Vector2 position, float size, Color color, float thickness = 1f)
        {
            GUI.color = color;
            Texture2D whiteTexture = Texture2D.whiteTexture;

            GUI.DrawTexture(new Rect(position.x - size / 2f, position.y - thickness / 2f, size, thickness), whiteTexture);
            GUI.DrawTexture(new Rect(position.x - thickness / 2f, position.y - size / 2f, thickness, size), whiteTexture);
        }

        public static void Crosshair(float size, Color color, float thickness = 1f)
        {
            Crosshair(new Vector2(Screen.width / 2f, Screen.height / 2f), size, color, thickness);
        }

        public static void Box(float x, float y, float w, float h, float thickness, Color color)
        {
            GUI.color = color;
            Texture2D whiteTexture = Texture2D.whiteTexture;
            GUI.DrawTexture(new Rect(x, y, w + thickness, thickness), whiteTexture);
            GUI.DrawTexture(new Rect(x, y + h, w + thickness, thickness), whiteTexture);
            GUI.DrawTexture(new Rect(x, y + thickness, thickness, h - thickness), whiteTexture);
            GUI.DrawTexture(new Rect(x + w, y + thickness, thickness, h - thickness), whiteTexture);
        }

        public static void Line(Vector2 lineStart, Vector2 lineEnd, float thickness, Color color)
        {
            if (lineStart == lineEnd) return;

            GUI.color = color;

            Vector2 vector = lineEnd - lineStart;
            float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

            if (thickness < 1f)
            {
                thickness = 1f;
            }
            float halfThickness = thickness / 2f;

            Matrix4x4 matrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(angle, lineStart);
            GUI.DrawTexture(new Rect(lineStart.x, lineStart.y - halfThickness, vector.magnitude, thickness), Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.matrix = matrix;
        }

        public static void Circle(Vector2 center, float radius, Color color, float width, int segmentsPerQuarter = 10)
        {
            segmentsPerQuarter = Mathf.Max(1, segmentsPerQuarter);

            Vector2 top = new Vector2(center.x, center.y - radius);
            Vector2 right = new Vector2(center.x + radius, center.y);
            Vector2 bottom = new Vector2(center.x, center.y + radius);
            Vector2 left = new Vector2(center.x - radius, center.y);

            float tangentLength = radius * 0.55228f;

            Draw.BezierLine(top, top + Vector2.right * tangentLength, right, right + Vector2.down * tangentLength, color, width, segmentsPerQuarter);
            Draw.BezierLine(right, right + Vector2.up * tangentLength, bottom, bottom + Vector2.left * tangentLength, color, width, segmentsPerQuarter);
            Draw.BezierLine(bottom, bottom + Vector2.right * tangentLength, left, left + Vector2.down * tangentLength, color, width, segmentsPerQuarter);
            Draw.BezierLine(left, left + Vector2.up * tangentLength, top, top + Vector2.left * tangentLength, color, width, segmentsPerQuarter);
        }

        public static void BezierLine(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, int segments)
        {
            Vector2 lineStart = Draw.EvaluateCubicBezier(start, startTangent, end, endTangent, 0f);
            for (int i = 1; i <= segments; i++)
            {
                float t = (float)i / segments;
                Vector2 lineEnd = Draw.EvaluateCubicBezier(start, startTangent, end, endTangent, t);
                Draw.Line(lineStart, lineEnd, width, color);
                lineStart = lineEnd;
            }
        }

        private static Vector2 EvaluateCubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 p = uuu * p0; // (1-t)^3 * p0
            p += 3f * uu * t * p1; // 3 * (1-t)^2 * t * p1
            p += 3f * u * tt * p2; // 3 * (1-t) * t^2 * p2
            p += ttt * p3; // t^3 * p3

            return p;
        }

        private static bool IsVisible(Camera camera, Vector3 worldPos)
        {
            if (camera == null)
            {
                Debug.LogError("IMGUIDebugDraw: Camera is null!");
                return false;
            }
            Vector3 viewportPoint = camera.WorldToViewportPoint(worldPos);

            return viewportPoint.z > camera.nearClipPlane &&
                   viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
                   viewportPoint.y >= 0 && viewportPoint.y <= 1;
        }

        private static Vector2 WorldToScreen(Camera camera, Vector3 worldPos)
        {
            if (camera == null)
            {
                Debug.LogError("IMGUIDebugDraw: Camera is null! Cannot convert WorldToScreen.");
                return Vector2.zero;
            }
            Vector3 screenPoint = camera.WorldToScreenPoint(worldPos);

            screenPoint.y = Screen.height - screenPoint.y;
            return new Vector2(screenPoint.x, screenPoint.y);
        }

        public static void Bounds(Camera cam, Bounds bounds, Color color)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            Vector3[] corners = new Vector3[8];
            corners[0] = center + new Vector3(extents.x, extents.y, extents.z);
            corners[1] = center + new Vector3(extents.x, extents.y, -extents.z);
            corners[2] = center + new Vector3(extents.x, -extents.y, -extents.z);
            corners[3] = center + new Vector3(extents.x, -extents.y, extents.z);
            corners[4] = center + new Vector3(-extents.x, extents.y, extents.z);
            corners[5] = center + new Vector3(-extents.x, extents.y, -extents.z);
            corners[6] = center + new Vector3(-extents.x, -extents.y, -extents.z);
            corners[7] = center + new Vector3(-extents.x, -extents.y, extents.z);

            float thickness = 1.5f;

            DrawEdge(cam, corners[0], corners[1], thickness, color);
            DrawEdge(cam, corners[1], corners[2], thickness, color);
            DrawEdge(cam, corners[2], corners[3], thickness, color);
            DrawEdge(cam, corners[3], corners[0], thickness, color);

            DrawEdge(cam, corners[4], corners[5], thickness, color);
            DrawEdge(cam, corners[5], corners[6], thickness, color);
            DrawEdge(cam, corners[6], corners[7], thickness, color);
            DrawEdge(cam, corners[7], corners[4], thickness, color);

            DrawEdge(cam, corners[0], corners[4], thickness, color);
            DrawEdge(cam, corners[1], corners[5], thickness, color);
            DrawEdge(cam, corners[2], corners[6], thickness, color);
            DrawEdge(cam, corners[3], corners[7], thickness, color);
        }


        public static void BoxCollider(Camera cam, BoxCollider boxCollider, Color color)
        {
            if (boxCollider == null) return;

            Transform transform = boxCollider.transform;
            Vector3 center = boxCollider.center;
            Vector3 size = boxCollider.size;

            Vector3 halfExtents = size / 2f;
            Vector3 scale = transform.lossyScale;
            halfExtents.x *= scale.x;
            halfExtents.y *= scale.y;
            halfExtents.z *= scale.z;

            Vector3 worldCenter = transform.TransformPoint(center);
            Quaternion worldRotation = transform.rotation;

            Vector3[] corners = new Vector3[8];
            corners[0] = worldCenter + worldRotation * new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
            corners[1] = worldCenter + worldRotation * new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            corners[2] = worldCenter + worldRotation * new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
            corners[3] = worldCenter + worldRotation * new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);
            corners[4] = worldCenter + worldRotation * new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);
            corners[5] = worldCenter + worldRotation * new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            corners[6] = worldCenter + worldRotation * new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
            corners[7] = worldCenter + worldRotation * new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);

            float thickness = 1.5f;

            DrawEdge(cam, corners[0], corners[1], thickness, color);
            DrawEdge(cam, corners[1], corners[2], thickness, color);
            DrawEdge(cam, corners[2], corners[3], thickness, color);
            DrawEdge(cam, corners[3], corners[0], thickness, color);

            DrawEdge(cam, corners[4], corners[5], thickness, color);
            DrawEdge(cam, corners[5], corners[6], thickness, color);
            DrawEdge(cam, corners[6], corners[7], thickness, color);
            DrawEdge(cam, corners[7], corners[4], thickness, color);

            DrawEdge(cam, corners[0], corners[4], thickness, color);
            DrawEdge(cam, corners[1], corners[5], thickness, color);
            DrawEdge(cam, corners[2], corners[6], thickness, color);
            DrawEdge(cam, corners[3], corners[7], thickness, color);
        }

        private static void DrawEdge(Camera cam, Vector3 worldStart, Vector3 worldEnd, float thickness, Color color)
        {
            if (cam == null) return;
            Vector3 viewStart = cam.WorldToViewportPoint(worldStart);
            Vector3 viewEnd = cam.WorldToViewportPoint(worldEnd);

            if (viewStart.z > cam.nearClipPlane - 0.01f && viewEnd.z > cam.nearClipPlane - 0.01f)
            {
                bool startIn = viewStart.x >= -0.1f && viewStart.x <= 1.1f && viewStart.y >= -0.1f && viewStart.y <= 1.1f;
                bool endIn = viewEnd.x >= -0.1f && viewEnd.x <= 1.1f && viewEnd.y >= -0.1f && viewEnd.y <= 1.1f;

                if (startIn || endIn)
                {
                    Vector2 screenStart = WorldToScreen(cam, worldStart);
                    Vector2 screenEnd = WorldToScreen(cam, worldEnd);
                    Line(screenStart, screenEnd, thickness, color);
                }
            }
        }

        public static void PlainAxes(Camera camera, Vector3 worldPos, Color color)
        {
            // draws 3d axis lines on world point:

            //      |  
            //      |/
            // -----·-----
            //     /|
            //      |

            float size = 0.2f;
            float thickness = 1.5f;
            Vector2 letterOffset = new Vector2(0, -10);
            float labelVisibilityThreshold = 0.1f;

            Vector3 xEnd = worldPos + Vector3.right * size;
            Vector3 yEnd = worldPos + Vector3.up * size;
            Vector3 zEnd = worldPos + Vector3.forward * size;

            DrawEdge(camera, worldPos - Vector3.right * size, xEnd, thickness, color);
            DrawEdge(camera, worldPos - Vector3.up * size, yEnd, thickness, color);
            DrawEdge(camera, worldPos - Vector3.forward * size, zEnd, thickness, color);

            // Draw Labels only if endpoint is visible and not too close to screen edge
            if (camera != null)
            {
                Vector3 viewX = camera.WorldToViewportPoint(xEnd);
                if (viewX.z > camera.nearClipPlane && viewX.x > labelVisibilityThreshold && viewX.x < 1 - labelVisibilityThreshold && viewX.y > labelVisibilityThreshold && viewX.y < 1 - labelVisibilityThreshold)
                {
                    Label(WorldToScreen(camera, xEnd) + letterOffset, "x", color);
                }

                Vector3 viewY = camera.WorldToViewportPoint(yEnd);
                if (viewY.z > camera.nearClipPlane && viewY.x > labelVisibilityThreshold && viewY.x < 1 - labelVisibilityThreshold && viewY.y > labelVisibilityThreshold && viewY.y < 1 - labelVisibilityThreshold)
                {
                    Label(WorldToScreen(camera, yEnd) + letterOffset, "y", color);
                }

                Vector3 viewZ = camera.WorldToViewportPoint(zEnd);
                if (viewZ.z > camera.nearClipPlane && viewZ.x > labelVisibilityThreshold && viewZ.x < 1 - labelVisibilityThreshold && viewZ.y > labelVisibilityThreshold && viewZ.y < 1 - labelVisibilityThreshold)
                {
                    Label(WorldToScreen(camera, zEnd) + letterOffset, "z", color);
                }
            }
        }


            float thickness = 1.5f;


        }
    }
}
