using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SuspectController : MonoBehaviour, IDraggable
{
    public bool isActive { get; private set; }

    [SerializeField] private Animator animator;

    private Camera mainCamera;
    private Vector3 offset;
    private GameObject wig;
    private Plane dragPlane; // A virtual plane in the 3D world for dragging
    private float initialZ;
    private float initialY;

    private float guessTriggerPoint = 0.3f;
    private float swipeRotationSpeed = 45f;

    private Image confirmIcon;
    private Image cancelIcon;
    private RotationSlider rotationSlider;
    private SuspectsManager suspectsManager;
    private SuspectAudioHandler suspectAudioHandler;


    void Awake()
    {
        mainCamera = Camera.main;
        confirmIcon = GameObject.Find("ConfirmIcon").GetComponent<Image>();
        cancelIcon = GameObject.Find("CancelIcon").GetComponent<Image>();
        suspectAudioHandler = GetComponent<SuspectAudioHandler>();
        rotationSlider = GameObject.Find("RotationSlider").GetComponent<RotationSlider>();
        rotationSlider.RotationSliderComponent.onValueChanged.AddListener(OnRotationSlider);
    }

    public void OnRotationSlider(float value)
    {
        if (!isActive) return;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, -value, transform.rotation.eulerAngles.z);
    }

    public void Activate(SuspectsManager suspectsManager)
    {
        this.suspectsManager = suspectsManager;
        isActive = true;

        if (wig == null && (SortingLevelManager.Instance.CurrentGameState == GameState.Round1 || SortingLevelManager.Instance.CurrentGameState == GameState.Round2))
        {
            // Check if the suspect has a wig
            foreach (Transform child in transform)
            {
                if (child.name.Contains("Wig"))
                {
                    wig = child.gameObject;
                    break;
                }
            }
        }
    }

    public void Deactivate()
    {
        SetWigAsSuspectChild();
        wig = null;
        isActive = false;
    }

    public void StartDrag(Vector3 hitPoint)
    {
        if (!isActive || SortingLevelManager.Instance.CurrentGameState == GameState.Win 
            || SortingLevelManager.Instance.CurrentGameState == GameState.Loose)
        {
            return;
        }

        rotationSlider.ResetSliderValue();

        animator.ResetTrigger("Down");
        animator.SetTrigger("Up");

        // Create a plane at the object's current position perpendicular to the camera's forward direction
        dragPlane = new Plane(mainCamera.transform.forward, hitPoint);

        // Calculate offset between the hit point and object's center
        offset = transform.position - hitPoint;
        initialZ = transform.position.z;
        initialY = transform.position.y;

        // Set wig to go up
        if (wig != null && (SortingLevelManager.Instance.CurrentGameState == GameState.Round1 
            || SortingLevelManager.Instance.CurrentGameState == GameState.Round2))
        {
            wig.transform.rotation = Quaternion.Euler(-90, 0, -90);
            wig.transform.position += new Vector3(0, 0.25f, 0);
        }
    }

    public void DragObject(Ray ray)
    {
        if (!isActive || SortingLevelManager.Instance.CurrentGameState == GameState.Win 
            || SortingLevelManager.Instance.CurrentGameState == GameState.Loose)
        {
            return;
        }

        if (transform.rotation.eulerAngles.y != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);
        }

        // Calculate the distance between the ray origin and the plane
        float distance;
        if (dragPlane.Raycast(ray, out distance))
        {
            // Update the object's position
            Vector3 hitPoint = ray.GetPoint(distance);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 0.25f);
            transform.position = new Vector3(hitPoint.x + offset.x, initialY, initialZ);
            // Rotate the object based on the x-coordinate
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, -transform.position.x * swipeRotationSpeed);

            if (transform.position.x < -guessTriggerPoint)
            {
                ShowSwipePanel(Icons.Release);
            }
            else if (transform.position.x > guessTriggerPoint)
            {
                ShowSwipePanel(Icons.Confirm);
            }
            else
            {
                HideSwipePanel();
            }
        }
    }

    public void EndDrag()
    {
        if (!isActive || SortingLevelManager.Instance.CurrentGameState == GameState.Win || SortingLevelManager.Instance.CurrentGameState == GameState.Loose)
        {
            return;
        }

        HideSwipePanel();

        if (transform.position.x < -guessTriggerPoint || transform.position.x > guessTriggerPoint)
        {
            StartCoroutine(MoveOutOfFrame());
            suspectAudioHandler.PlayRandomSwipeSound();
        }
        else
        {
            StartCoroutine(MoveBack());
        }
    }

    private void SetWigAsSuspectChild()
    {
        if (wig != null)
        {
            wig.transform.SetParent(transform);
            wig.transform.localPosition = Vector3.zero;
            // -90 , 0, -90 is the default rotation for the wig
            wig.transform.rotation = Quaternion.Euler(-90, 0, -90);
        }
    }

    private void ShowSwipePanel(Icons icon)
    {
        if (confirmIcon.isActiveAndEnabled || cancelIcon.isActiveAndEnabled)
        {
            return;
        }
        if (icon == Icons.Confirm)
        {
            confirmIcon.enabled = true;
        }
        else if (icon == Icons.Release)
        {
            cancelIcon.enabled = true;
        }
    }

    private void HideSwipePanel()
    {
        if (confirmIcon.isActiveAndEnabled || cancelIcon.isActiveAndEnabled)
        {
            confirmIcon.enabled = false;
            cancelIcon.enabled = false;
        }
        
    }

    private IEnumerator MoveOutOfFrame()
    {
        if (suspectsManager.CheckForCorrectGuess(transform.position.x > guessTriggerPoint) == false)
        {
            StartCoroutine(MoveBack());
            yield break;
        }

        // Find and destroy the collectable if it exists
        ICollectable collectable = transform.GetComponentInChildren<ICollectable>(true);
        if (collectable != null)
        {
            collectable.Despawn();
        }

        isActive = false;
        float t = 0;
        float speed = 10.0f;
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = new Vector3(0, initialY, initialZ);
        if (transform.position.x < -guessTriggerPoint)
        {
            targetPosition.x = -2f;
        }
        else if (transform.position.x > guessTriggerPoint)
        {
            targetPosition.x = 2f;
        }

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }
        suspectsManager.NextSuspect();

        if (transform.position.x <= -2f)
        {
            SetWigAsSuspectChild();
            suspectsManager.DeleteSuspect(gameObject);
        }
        else
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            Deactivate();
            animator.ResetTrigger("Up");
            animator.SetTrigger("Down");
            SortingLevelManager.Instance.CheckIfRoundIsOver();
        }
    }

    private IEnumerator MoveBack()
    {
        isActive = false;

        animator.ResetTrigger("Up");
        animator.SetTrigger("Down");

        float t = 0;
        float speed = 5.0f;
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = new Vector3(0, initialY, initialZ);
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
            yield return null;
        }
        SetWigAsSuspectChild();
        isActive = true;
    }

    private void OnDisable()
    {
        rotationSlider.RotationSliderComponent.onValueChanged.RemoveListener(OnRotationSlider);
    }
}

enum Icons
{
    Confirm,
    Release
}