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

    private bool isMenuOpen = false;

    void Start()
    {
        if (menuRoot != null)
        {
            menuRoot.SetActive(true);
            //OpenMenu();
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
        //if (isMenuOpen && menuRoot != null && headCamera != null)
        //{
        //    // check distance to see if the menu should auto-close
        //    float distance = Vector3.Distance(headCamera.position, menuRoot.transform.position);

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
        if (headCamera == null || menuRoot == null)
            return;

        // Calculate spawn position in front of the camera
        Vector3 forwardDirection = headCamera.forward;
        Vector3 spawnPos = headCamera.position + (forwardDirection * spawnDistance);

        // Adjust the menu to be slightly below eye level
        spawnPos.y -= 0.2f;

        // set menu position
        menuRoot.transform.position = spawnPos;

        // make the menu face the user
        menuRoot.transform.LookAt(2 * menuRoot.transform.position - headCamera.position);

        menuRoot.SetActive(true);
        isMenuOpen = true;
    }

    private void CloseMenu()
    {
        if (menuRoot != null)
        {
            menuRoot.SetActive(false);
        }
        isMenuOpen = false;
    }
}
