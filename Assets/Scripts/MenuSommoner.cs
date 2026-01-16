using UnityEngine;
using UnityEngine.InputSystem;

public class MenuSommoner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("GameObject with UIDocument")]
    public GameObject menuObject;

    [Tooltip("Main (VR) Camera")]
    public Transform headCamera;

    [Header("Settings")]
    [Tooltip("Distance in front of the camera to place the menu")]
    public float spawnDistance = 1.5f;

    [Tooltip("Distance to travel before the menu closes")]
    public float autoCloseDistance = 3.0f;

    [Header("Input Actions")]
    public InputActionProperty toggleMenuAction;

    private bool isMenuOpen = false;

    void Start()
    {
        // Menu is hidden at the start
        if (menuObject != null)
        {
            menuObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        toggleMenuAction.action.performed += OnToggleMenu;
        toggleMenuAction.action.Enable();
    }

    private void OnDisable()
    {
        toggleMenuAction.action.performed -= OnToggleMenu;
        toggleMenuAction.action.Disable();
    }

    void Update()
    {
        //if (isMenuOpen && menuObject != null && headCamera != null)
        //{
        //    // check distance to see if the menu should auto-close
        //    float distance = Vector3.Distance(headCamera.position, menuObject.transform.position);

        //    if (distance > autoCloseDistance)
        //    {
        //        CloseMenu();
        //    }
        //}
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

    private void OpenMenu()
    {
        if (headCamera == null || menuObject == null)
            return;

        // Calculate spawn position in front of the camera
        Vector3 forwardDirection = headCamera.forward;
        Vector3 spawnPos = headCamera.position + (forwardDirection * spawnDistance);

        // Adjust the menu to be slightly below eye level
        spawnPos.y -= 0.2f;

        // set menu position
        menuObject.transform.position = spawnPos;

        // make the menu face the user
        menuObject.transform.LookAt(2 * menuObject.transform.position - headCamera.position);

        menuObject.SetActive(true);
        isMenuOpen = true;
    }

    private void CloseMenu()
    {
        if (menuObject != null)
        {
            menuObject.SetActive(false);
        }
        isMenuOpen = false;
    }
}
