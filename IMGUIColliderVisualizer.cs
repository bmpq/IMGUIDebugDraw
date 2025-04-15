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
            Draw.Label(cam, transform.position, name);

            if (col is BoxCollider boxCol)
            {
                Draw.BoxCollider(cam, boxCol, Color.green);
            }
            else if (col is SphereCollider sphereCol)
            {
                Draw.SphereCollider(cam, sphereCol, Color.blue);
            }
            else if (col is CapsuleCollider capsuleCol)
            {
                Draw.CapsuleCollider(cam, capsuleCol, Color.blue);
            }
        }
    }
}
