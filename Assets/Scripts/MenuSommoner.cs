using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuSommoner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("GameObject with UIDocument")]
    public GameObject menuRoot;

    [Tooltip("Main (VR) Camera")]
    public Transform headCamera;

    [Header("Settings")]
    [Tooltip("Distance in front of the camera to place the menu")]
    public float spawnDistance = 1.5f;

    [Tooltip("Distance to travel before the menu closes")]
    public float autoCloseDistance = 3.0f;

    [Header("Input Actions")]
    public InputActionProperty toggleMenuAction;
    public InputActionProperty resetMenuPositions;

    private Dictionary<Transform, Vector3> initialLocalPositions =
        new Dictionary<Transform, Vector3>();

    private Dictionary<Transform, Quaternion> initialLocalRotations =
        new Dictionary<Transform, Quaternion>();

    private bool isMenuOpen = false;

    void Start()
    {
        if (menuRoot != null)
        {
            SaveOriginalPositions();
            menuRoot.SetActive(true);
        }
    }

    private void OnEnable()
    {
        toggleMenuAction.action.performed += OnToggleMenu;
        toggleMenuAction.action.Enable();

        resetMenuPositions.action.performed += OnResetMenu;
        resetMenuPositions.action.Enable();
    }

    private void OnDisable()
    {
        toggleMenuAction.action.performed -= OnToggleMenu;
        toggleMenuAction.action.Disable();

        resetMenuPositions.action.performed -= OnResetMenu;
        resetMenuPositions.action.Disable();
    }

    private void SaveOriginalPositions()
    {
        foreach (Transform child in menuRoot.transform)
        {
            initialLocalPositions[child] = child.localPosition;
            initialLocalRotations[child] = child.localRotation;
        }
    }

    public void ResetPositions()
    {
        // Iterate through the saved keys (the transforms)
        foreach (Transform child in initialLocalPositions.Keys)
        {
            if (child != null)
            {
                // --- CRITICAL FIX START ---
                // If the object was detached (Retain Transform Parent unchecked),
                // we must force it back to be a child of menuRoot.
                if (child.parent != menuRoot.transform)
                {
                    child.SetParent(menuRoot.transform);
                }
                // --- CRITICAL FIX END ---

                child.localPosition = initialLocalPositions[child];
                child.localRotation = initialLocalRotations[child];

                if (child.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }
    }

    void Update()
    {
        // Only run checks if the menu is currently open
        if (isMenuOpen && headCamera != null && menuRoot != null)
        {
            // Calculate distance between the player's head and the menu anchor
            float distance = Vector3.Distance(headCamera.position, menuRoot.transform.position);

            // If the player moves further than the allowed distance, close the menu
            if (distance > autoCloseDistance)
            {
                CloseMenu();
            }
        }
    }
    private void OnToggleMenu(InputAction.CallbackContext context)
    {
        if (isMenuOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    private void OnResetMenu(InputAction.CallbackContext context)
    {
        ResetPositions();
    }

    private void OpenMenu()
    {
        if (headCamera == null || menuRoot == null)
            return;

        // Reset positions BEFORE opening to ensure stray items are attached and visible
        // Remove this line if you want items to stay where you left them until manual reset
        ResetPositions();

        Vector3 forwardDirection = headCamera.forward;
        Vector3 spawnPos = headCamera.position + (forwardDirection * spawnDistance);
        spawnPos.y -= 0.2f;

        menuRoot.transform.position = spawnPos;
        menuRoot.transform.LookAt(2 * menuRoot.transform.position - headCamera.position);

        menuRoot.SetActive(true);
        isMenuOpen = true;
    }


    private void CloseMenu()
    {
        if (menuRoot != null)
        {
            // --- OPTIONAL FIX ---
            // If you want items to disappear when the menu closes, 
            // you must pull them back into the menuRoot hierarchy first.
            foreach (Transform child in initialLocalPositions.Keys)
            {
                if (child != null && child.parent != menuRoot.transform)
                {
                    child.SetParent(menuRoot.transform);
                }
            }

            menuRoot.SetActive(false);
        }
        isMenuOpen = false;
    }
}