using UnityEngine;

namespace IMGUIDebugDraw
{
    public class IMGUIColliderVisualizer : MonoBehaviour
    {
        Camera cam;
        Collider col;

        void Start()
        {
            cam = Camera.main;
            col = GetComponent<Collider>();
        }

        void OnGUI()
        {
            if (col is BoxCollider boxCol)
            {
                Draw.BoxCollider(cam, boxCol, Color.green);
            }
        }
    }
}
