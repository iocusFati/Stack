using UnityEngine;

namespace Utils
{
    public static class TransformExtension
    {
        public static void LookAtAxis(this Transform transform, Vector3 at, Vector3 axis)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
            float angle = Vector3.SignedAngle(transform.forward.ExceptY(),
                (at - transform.position).ExceptY(), axis);
            transform.rotation = Quaternion.AngleAxis(angle, axis);
        }

        public static Vector3 ExceptY(this Vector3 vector) =>
            new Vector3(vector.x, 0, vector.z);

        public static void RotWithoutAltChild(this Transform transform, Quaternion angle)
        {
            Quaternion initialRot = transform.rotation;
            Transform[] children = transform.gameObject.GetComponentsInChildren<Transform>();
            
            transform.rotation *= angle;
            
            foreach (var child in children) 
                child.rotation = Quaternion.Inverse(child.rotation) * initialRot;
        }

        public static float MaxAxis(this Vector3 vector) => 
            Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }
}