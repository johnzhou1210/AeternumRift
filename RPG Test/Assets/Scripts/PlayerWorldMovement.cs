using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWorldMovement : MonoBehaviour
{
    [field: SerializeField] private InputAction playerControls;
    [field: SerializeField, Self] private CharacterController characterController;
    [field: SerializeField, Child] private SpriteRenderer spriteRenderer;

    [field: SerializeField] public float WalkSpeed { get; private set; } = 1f;

    private Vector2 moveDirection;

    private void OnValidate() {
        this.ValidateRefs();
    }

    private void OnEnable() {
        playerControls.Enable();
    }

    private void OnDisable() {
        playerControls.Disable();
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        /* y moveDirection is on 3d world Z axis */
        moveDirection = playerControls.ReadValue<Vector2>();

        if (moveDirection.x == 0) {
            return;
        }
        spriteRenderer.flipX = moveDirection.x < 0;
    }

    private void FixedUpdate() {
        characterController.Move(new Vector3(moveDirection.x, 0f, moveDirection.y) * Time.fixedDeltaTime * WalkSpeed);
    }

}
