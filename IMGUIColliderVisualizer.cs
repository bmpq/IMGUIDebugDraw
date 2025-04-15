using UnityEngine;

namespace IMGUIDebugDraw
{
    [RequireComponent(typeof(Collider))]
    public class IMGUIColliderVisualizer : MonoBehaviour
    {
        public Color color = Color.green;

        Camera cam;
        Collider col;

        void Start()
        {
            cam = Camera.main;
            col = GetComponent<Collider>();
        }

        void OnGUI()
        {
            Draw.Label(cam, transform.position, name);

            if (col is BoxCollider boxCol)
            {
                Draw.BoxCollider(cam, boxCol, color);
            }
            else if (col is SphereCollider sphereCol)
            {
                Draw.SphereCollider(cam, sphereCol, color);
            }
            else if (col is CapsuleCollider capsuleCol)
            {
                Draw.CapsuleCollider(cam, capsuleCol, color);
            }
        }
    }
}
