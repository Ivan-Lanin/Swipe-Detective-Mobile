using UnityEngine;

public class SortingLevelInputHandler : MonoBehaviour
{
    private Camera mainCamera;
    private IDraggable currentDraggableObject;

    public bool allowDragging = true;
    public bool allowCollecting = true;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Please ensure there is a camera tagged as 'MainCamera' in the scene.");
        }
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            HandleTouchInput(Input.GetTouch(0));
        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            HandleMouseInput();
        }
    }

    private void HandleTouchInput(Touch touch)
    {
        Ray ray = mainCamera.ScreenPointToRay(touch.position);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                HandleInputBegan(ray);
                break;

            case TouchPhase.Moved:
                HandleDrag(ray);
                break;

            case TouchPhase.Ended:
                HandleDragEnd();
                break;
        }
    }

    private void HandleMouseInput()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            HandleInputBegan(ray);
        }

        if (currentDraggableObject == null) return;

        if (Input.GetMouseButton(0))
        {
            HandleDrag(ray);
        }

        if (Input.GetMouseButtonUp(0))
        {
            HandleDragEnd();
        }
    }

    private void HandleInputBegan(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (allowDragging) TryHandleDraggable(hit);
            if (allowCollecting) TryHandleCollectable(hit);
        }
    }

    private void TryHandleDraggable(RaycastHit hit)
    {
        IDraggable draggableComponent = hit.transform.GetComponent<IDraggable>();
        if (draggableComponent != null)
        {
            if (!draggableComponent.isActive)
            {
                currentDraggableObject = null;
                return;
            }

            currentDraggableObject = draggableComponent;
            currentDraggableObject.StartDrag(hit.point);
        }
    }

    private void TryHandleCollectable(RaycastHit hit)
    {
        ICollectable collectableComponent = hit.transform.GetComponent<ICollectable>();
        if (collectableComponent != null)
        {
            collectableComponent.Collect();
        }
    }

    private void HandleDrag(Ray ray)
    {
        currentDraggableObject?.DragObject(ray);
    }

    private void HandleDragEnd()
    {
        if (currentDraggableObject == null) return;

        currentDraggableObject.EndDrag();
        currentDraggableObject = null;
    }
}
