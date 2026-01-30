using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class DisableTurnOnGrab : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ContinuousTurnProvider turnProvider;
    [SerializeField] private XRBaseInteractor interactor;

    private void OnEnable()
    {
        interactor.selectEntered.AddListener(OnGrab);
        interactor.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        interactor.selectEntered.RemoveListener(OnGrab);
        interactor.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Disable the turn provider when an object is picked up
        if (turnProvider != null) turnProvider.enabled = false;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Re-enable it when the object is dropped
        if (turnProvider != null) turnProvider.enabled = true;
    }
}