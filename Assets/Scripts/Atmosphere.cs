using UnityEngine;

namespace DefaultNamespace
{
    public class Atmosphere : MonoBehaviour
    {
        private Material material;
        private float value;
        float speed = 2.5f;

        void Start ()
        {
            material = GetComponent<MeshRenderer> ().sharedMaterial;
        }

        void Update ()
        {
            value = Mathf.PingPong (Time.time * speed, 5);
            material.SetFloat ("_Strength", value);
            Debug.Log (value);
        }
    }
}