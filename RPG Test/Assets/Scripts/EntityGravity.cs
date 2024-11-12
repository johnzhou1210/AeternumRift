using KBCore.Refs;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntityGravity : MonoBehaviour
{
    public static float GRAVITY = -9.8f;
    [field: SerializeField, Self] private CharacterController controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnValidate() {
        this.ValidateRefs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate() {
        controller.Move(new Vector3(0, GRAVITY, 0));
    }

}
