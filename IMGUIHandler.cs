using System.Collections.Generic;
using UnityEngine;

namespace IMGUIDebugDraw
{
    public class IMQueue : MonoBehaviour
    {
        static IMQueue instance;

        private Queue<Vector3> renderQueue;

        private Camera cam;

        public static void Point(Vector3 point)
        {
            if (instance == null)
            {
                instance = new GameObject("IMGUIDebugDrawIMQueue").AddComponent<IMQueue>();
                DontDestroyOnLoad(instance.gameObject);
            }
            instance.gameObject.SetActive(true);
            instance.enabled = true;

            if (instance.renderQueue == null)
                instance.renderQueue = new Queue<Vector3>();

            instance.renderQueue.Enqueue(point);
        }

        void LateUpdate()
        {
            renderQueue.Clear();
        }

        void OnGUI()
        {
            cam = Camera.main;

            //Draw.PlainAxes(cam, Vector3.zero, Color.cyan);

            if (renderQueue.Count > 0)
            {
                Vector3 point = renderQueue.Dequeue();
                Draw.PlainAxes(cam, point, Color.cyan);
            }
        }
    }
}
