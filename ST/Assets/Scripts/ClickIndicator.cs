using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ClickIndicator : MonoBehaviour
{
    [SerializeField] private GameObject circle;
    [SerializeField] private float fadeOutSpeed;

    private Color circleColor;
    private Image circleImage;
    private Camera mainCamera;
    private Coroutine currectCoroutine;

    public static ClickIndicator Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        circleImage = circle.GetComponent<Image>();
        circleColor = circleImage.color;
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found. Please ensure there is a camera tagged as 'MainCamera' in the scene.");
        }
    }

    private void Update()
    {
        // Check for mouse click or touch input
        if (Input.GetMouseButton(0) || (Input.touchCount > 0))
        {
            circle.SetActive(true);
            circleImage.color = circleColor;

            // Get the position in world coordinates
            Vector3 inputPosition;
            
            if (Input.touchCount > 0)
            {
                // Get touch position and convert to world coordinates
                inputPosition = Input.GetTouch(0).position;
            }
            else
            {
                // Get mouse position and convert to world coordinates
                inputPosition = Input.mousePosition;
            }

            // Move the circle to the target position
            circle.transform.position = inputPosition;
        }
        else
        {
            // Start fading out the circle when there is no input and coroutine is not already running
            if (currectCoroutine == null)
            {
                currectCoroutine = StartCoroutine(FadeOutCircle());
            }
        }
    }

    private IEnumerator FadeOutCircle()
    {
        while (circleImage.color.a > 0)
        {
            Color color = circleImage.color;
            color.a -= fadeOutSpeed * Time.deltaTime;
            circleImage.color = color;
            yield return null;
        }
        circle.SetActive(false);
        currectCoroutine = null;
    }
}
