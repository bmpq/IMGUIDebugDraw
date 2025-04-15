using UnityEngine;

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
            Texture2D whiteTexture = Texture2D.whiteTexture;
            GUI.DrawTexture(new Rect(x, y, w + thickness, thickness), whiteTexture);
            GUI.DrawTexture(new Rect(x, y, thickness, h + thickness), whiteTexture);
            GUI.DrawTexture(new Rect(x + w, y, thickness, h + thickness), whiteTexture);
            GUI.DrawTexture(new Rect(x, y + h, w + thickness, thickness), whiteTexture);
        }

        public static void Line(Vector2 lineStart, Vector2 lineEnd, float thickness, Color color)
        {
            Vector2 vector = lineEnd - lineStart;
            float num = 57.29578f * Mathf.Atan(vector.y / vector.x);
            if (vector.x < 0f)
            {
                num += 180f;
            }
            if (thickness < 1f)
            {
                thickness = 1f;
            }
            int num2 = checked((int)Mathf.Ceil(thickness / 2f));
            GUIUtility.RotateAroundPivot(num, lineStart);
            GUI.DrawTexture(new Rect(lineStart.x, lineStart.y - (float)num2, vector.magnitude, thickness), Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 1f, color, 0f, 0f);
            GUIUtility.RotateAroundPivot(-num, lineStart);
        }

        public static void Circle(Vector2 center, float radius, Color color, float width, int segmentsPerQuarter)
        {
            float num = radius / 2f;
            Vector2 vector = new Vector2(center.x, center.y - radius);
            Vector2 endTangent = new Vector2(center.x - num, center.y - radius);
            Vector2 startTangent = new Vector2(center.x + num, center.y - radius);
            Vector2 vector2 = new Vector2(center.x + radius, center.y);
            Vector2 endTangent2 = new Vector2(center.x + radius, center.y - num);
            Vector2 startTangent2 = new Vector2(center.x + radius, center.y + num);
            Vector2 vector3 = new Vector2(center.x, center.y + radius);
            Vector2 startTangent3 = new Vector2(center.x - num, center.y + radius);
            Vector2 endTangent3 = new Vector2(center.x + num, center.y + radius);
            Vector2 vector4 = new Vector2(center.x - radius, center.y);
            Vector2 startTangent4 = new Vector2(center.x - radius, center.y - num);
            Vector2 endTangent4 = new Vector2(center.x - radius, center.y + num);

            Draw.BezierLine(vector, startTangent, vector2, endTangent2, color, width, segmentsPerQuarter);
            Draw.BezierLine(vector2, startTangent2, vector3, endTangent3, color, width, segmentsPerQuarter);
            Draw.BezierLine(vector3, startTangent3, vector4, endTangent4, color, width, segmentsPerQuarter);
            Draw.BezierLine(vector4, startTangent4, vector, endTangent, color, width, segmentsPerQuarter);
        }

        public static void BezierLine(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, int segments)
        {
            Vector2 lineStart = Draw.Bezier(start, startTangent, end, endTangent, 0f);
            checked
            {
                for (int i = 1; i < segments + 1; i++)
                {
                    Vector2 vector = Draw.Bezier(start, startTangent, end, endTangent, (float)i / (float)segments);
                    Draw.Line(lineStart, vector, width, color);
                    lineStart = vector;
                }
            }
        }

        private static Vector2 Bezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
        {
            float num = 1f - t;
            return num * num * num * s + 3f * num * num * t * st + 3f * num * t * t * et + t * t * t * e;
        }

        private static bool IsVisible(Camera camera, Vector3 worldPos)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(worldPos);
            return viewportPoint.z > 0; // If z is positive, the point is in front of the camera
        }

        private static Vector2 WorldToScreen(Camera camera, Vector3 worldPos)
        {
            Vector3 screenPoint = camera.WorldToScreenPoint(worldPos);
            screenPoint.y = (float)Screen.height - screenPoint.y;
            return screenPoint;
        }

        public static void Bounds(Camera cam, Bounds bounds, Color color)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            Vector3[] worldPoint = new Vector3[]
            {
                new Vector3(center.x + extents.x, center.y + extents.y, center.z + extents.z),
                new Vector3(center.x + extents.x, center.y + extents.y, center.z - extents.z),
                new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z),
                new Vector3(center.x + extents.x, center.y - extents.y, center.z + extents.z),
                new Vector3(center.x - extents.x, center.y + extents.y, center.z + extents.z),
                new Vector3(center.x - extents.x, center.y + extents.y, center.z - extents.z),
                new Vector3(center.x - extents.x, center.y - extents.y, center.z - extents.z),
                new Vector3(center.x - extents.x, center.y - extents.y, center.z + extents.z)
            };

            float thickness = 1.5f;

            if (IsVisible(cam, worldPoint[0]) && IsVisible(cam, worldPoint[1])) Draw.Line(WorldToScreen(cam, worldPoint[0]), WorldToScreen(cam, worldPoint[1]), thickness, color);
            if (IsVisible(cam, worldPoint[1]) && IsVisible(cam, worldPoint[2])) Draw.Line(WorldToScreen(cam, worldPoint[1]), WorldToScreen(cam, worldPoint[2]), thickness, color);
            if (IsVisible(cam, worldPoint[2]) && IsVisible(cam, worldPoint[3])) Draw.Line(WorldToScreen(cam, worldPoint[2]), WorldToScreen(cam, worldPoint[3]), thickness, color);
            if (IsVisible(cam, worldPoint[3]) && IsVisible(cam, worldPoint[0])) Draw.Line(WorldToScreen(cam, worldPoint[3]), WorldToScreen(cam, worldPoint[0]), thickness, color);
            if (IsVisible(cam, worldPoint[4]) && IsVisible(cam, worldPoint[5])) Draw.Line(WorldToScreen(cam, worldPoint[4]), WorldToScreen(cam, worldPoint[5]), thickness, color);
            if (IsVisible(cam, worldPoint[5]) && IsVisible(cam, worldPoint[6])) Draw.Line(WorldToScreen(cam, worldPoint[5]), WorldToScreen(cam, worldPoint[6]), thickness, color);
            if (IsVisible(cam, worldPoint[6]) && IsVisible(cam, worldPoint[7])) Draw.Line(WorldToScreen(cam, worldPoint[6]), WorldToScreen(cam, worldPoint[7]), thickness, color);
            if (IsVisible(cam, worldPoint[7]) && IsVisible(cam, worldPoint[4])) Draw.Line(WorldToScreen(cam, worldPoint[7]), WorldToScreen(cam, worldPoint[4]), thickness, color);
            if (IsVisible(cam, worldPoint[0]) && IsVisible(cam, worldPoint[4])) Draw.Line(WorldToScreen(cam, worldPoint[0]), WorldToScreen(cam, worldPoint[4]), thickness, color);
            if (IsVisible(cam, worldPoint[1]) && IsVisible(cam, worldPoint[5])) Draw.Line(WorldToScreen(cam, worldPoint[1]), WorldToScreen(cam, worldPoint[5]), thickness, color);
            if (IsVisible(cam, worldPoint[2]) && IsVisible(cam, worldPoint[6])) Draw.Line(WorldToScreen(cam, worldPoint[2]), WorldToScreen(cam, worldPoint[6]), thickness, color);
            if (IsVisible(cam, worldPoint[3]) && IsVisible(cam, worldPoint[7])) Draw.Line(WorldToScreen(cam, worldPoint[3]), WorldToScreen(cam, worldPoint[7]), thickness, color);
        }

        public static void BoxCollider(Camera cam, BoxCollider boxCollider, Color color)
        {
            Transform transform = boxCollider.transform;
            Vector3 center = transform.TransformPoint(boxCollider.center); // Collider's center is local
            Vector3 halfExtents = boxCollider.size / 2f;
            halfExtents.x *= boxCollider.transform.localScale.x;
            halfExtents.y *= boxCollider.transform.localScale.y;
            halfExtents.z *= boxCollider.transform.localScale.z;

            Vector3[] worldPoint = new Vector3[]
            {
            center + transform.rotation * new Vector3(halfExtents.x, halfExtents.y, halfExtents.z),
            center + transform.rotation * new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z),
            center + transform.rotation * new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z),
            center + transform.rotation * new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z),
            center + transform.rotation * new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z),
            center + transform.rotation * new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z),
            center + transform.rotation * new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z),
            center + transform.rotation * new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z)
            };

            float thickness = 1.5f;

            if (IsVisible(cam, worldPoint[0]) && IsVisible(cam, worldPoint[1])) Draw.Line(WorldToScreen(cam, worldPoint[0]), WorldToScreen(cam, worldPoint[1]), thickness, color);
            if (IsVisible(cam, worldPoint[1]) && IsVisible(cam, worldPoint[2])) Draw.Line(WorldToScreen(cam, worldPoint[1]), WorldToScreen(cam, worldPoint[2]), thickness, color);
            if (IsVisible(cam, worldPoint[2]) && IsVisible(cam, worldPoint[3])) Draw.Line(WorldToScreen(cam, worldPoint[2]), WorldToScreen(cam, worldPoint[3]), thickness, color);
            if (IsVisible(cam, worldPoint[3]) && IsVisible(cam, worldPoint[0])) Draw.Line(WorldToScreen(cam, worldPoint[3]), WorldToScreen(cam, worldPoint[0]), thickness, color);
            if (IsVisible(cam, worldPoint[4]) && IsVisible(cam, worldPoint[5])) Draw.Line(WorldToScreen(cam, worldPoint[4]), WorldToScreen(cam, worldPoint[5]), thickness, color);
            if (IsVisible(cam, worldPoint[5]) && IsVisible(cam, worldPoint[6])) Draw.Line(WorldToScreen(cam, worldPoint[5]), WorldToScreen(cam, worldPoint[6]), thickness, color);
            if (IsVisible(cam, worldPoint[6]) && IsVisible(cam, worldPoint[7])) Draw.Line(WorldToScreen(cam, worldPoint[6]), WorldToScreen(cam, worldPoint[7]), thickness, color);
            if (IsVisible(cam, worldPoint[7]) && IsVisible(cam, worldPoint[4])) Draw.Line(WorldToScreen(cam, worldPoint[7]), WorldToScreen(cam, worldPoint[4]), thickness, color);
            if (IsVisible(cam, worldPoint[0]) && IsVisible(cam, worldPoint[4])) Draw.Line(WorldToScreen(cam, worldPoint[0]), WorldToScreen(cam, worldPoint[4]), thickness, color);
            if (IsVisible(cam, worldPoint[1]) && IsVisible(cam, worldPoint[5])) Draw.Line(WorldToScreen(cam, worldPoint[1]), WorldToScreen(cam, worldPoint[5]), thickness, color);
            if (IsVisible(cam, worldPoint[2]) && IsVisible(cam, worldPoint[6])) Draw.Line(WorldToScreen(cam, worldPoint[2]), WorldToScreen(cam, worldPoint[6]), thickness, color);
            if (IsVisible(cam, worldPoint[3]) && IsVisible(cam, worldPoint[7])) Draw.Line(WorldToScreen(cam, worldPoint[3]), WorldToScreen(cam, worldPoint[7]), thickness, color);
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

            Vector3 xa = worldPos - new Vector3(size, 0f, 0f);
            Vector3 xb = worldPos + new Vector3(size, 0f, 0f);
            Vector3 ya = worldPos - new Vector3(0f, size, 0f);
            Vector3 yb = worldPos + new Vector3(0f, size, 0f);
            Vector3 za = worldPos - new Vector3(0f, 0f, size);
            Vector3 zb = worldPos + new Vector3(0f, 0f, size);

            float thickness = 1.5f;

            // Check if the points are in front of the camera before drawing
            if (IsVisible(camera, xa) && IsVisible(camera, xb)) Draw.Line(WorldToScreen(camera, xa), WorldToScreen(camera, xb), thickness, color);
            if (IsVisible(camera, ya) && IsVisible(camera, yb)) Draw.Line(WorldToScreen(camera, ya), WorldToScreen(camera, yb), thickness, color);
            if (IsVisible(camera, za) && IsVisible(camera, zb)) Draw.Line(WorldToScreen(camera, za), WorldToScreen(camera, zb), thickness, color);

            Vector2 letterOffset = new Vector2(0, 10);
            if (IsVisible(camera, xb)) Draw.Label(WorldToScreen(camera, xb) + letterOffset, "x", color);
            if (IsVisible(camera, yb)) Draw.Label(WorldToScreen(camera, yb) + letterOffset, "y", color);
            if (IsVisible(camera, zb)) Draw.Label(WorldToScreen(camera, zb) + letterOffset, "z", color);
        }
    }
}
