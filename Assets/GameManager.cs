using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    public Texture2D cursorTexture;
    public Texture2D cursorTextureGreen;

    public bool IsGameOver { get; private set; } = false;

    public int initialCreatureCount = 3;
    public GameObject CreaturePrefab;
    public Creature? TargetCreature { get; private set; } = null;
    private VisualElement uiRoot;
    private VisualElement gameOverContainer;

    private GrabBarController grabBarController;
    private HungerBarController hungerBarController;
    private TimerController timerController;

    private float secondsSinceLastSpawn = 0;
    private float spawnDelay = 2;
    private int maxCreatureCount = 6;
    public int currCreatureCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        uiRoot = GameObject.Find("Canvas").GetComponent<UIDocument>().rootVisualElement;
        gameOverContainer = uiRoot.Q("GameOverContainer");
        grabBarController = gameObject.GetComponent<GrabBarController>();
        hungerBarController = gameObject.GetComponent<HungerBarController>();
        timerController = gameObject.GetComponent<TimerController>();
        SetCursor(false);
        StartNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGameOver)
        {
            return;
        }

        secondsSinceLastSpawn += Time.deltaTime;
        if (secondsSinceLastSpawn > spawnDelay && currCreatureCount < maxCreatureCount)
        {
            SpawnCreature();
        }

        if (Input.GetMouseButtonUp(0) && TargetCreature)
        {
            OnReleaseMouse();
        }

        if (TargetCreature)
        {
            grabBarController.UpdateGrabBarPointer();
        }
    }


    void SpawnCreature()
    {
        // we'll want to check for collisions here
        Vector3 randomVector = new Vector3(Random.Range(-4f, 4f), Random.Range(-5f, 5f), 0);
        Instantiate(CreaturePrefab, randomVector, Quaternion.identity);
        secondsSinceLastSpawn = 0;
        currCreatureCount++;
    }

    public void StartNewGame()
    {
        IsGameOver = false;
        timerController.Reset();
        hungerBarController.Reset();
        SpawnCreature();
    }

    public void SetTargetCreature(Creature creature)
    {
        if (!TargetCreature && !IsGameOver)
        {
            TargetCreature = creature;
            creature.SetState(CreatureState.Grabbing);
            grabBarController.ShowGrabBar();
        }
    }
 
    public void OnReleaseMouse()
    {
        SetCursor(false);
        grabBarController.HideGrabBar();
        if (grabBarController.IsGrabPointerInZone() && TargetCreature.IsHovered)
        {
            TargetCreature.SetState(CreatureState.GrabbedSuccess);
            hungerBarController.ChangeHunger(TargetCreature.hungerValue);
        } else if (!grabBarController.IsGrabPointerInZone() && TargetCreature.IsHovered)
        {
            TargetCreature.SetState(CreatureState.GrabbedDeath);
        } else
        {
            TargetCreature.SetState(CreatureState.GrabbedMiss);
        }
        UnsetTargetCreature();
    }

    private void UnsetTargetCreature()
    {
        if (TargetCreature)
        {
            TargetCreature.SetState(CreatureState.Idle);
        }
        TargetCreature = null;
    }

    public void OnGameOver()
    {
        IsGameOver = true;
        grabBarController.HideGrabBar();
        UnsetTargetCreature();
        gameOverContainer.style.visibility = Visibility.Visible;
        Debug.Log("Game over!");
    }

    public void SetCursor(bool isGreen)
    {
        Vector2 hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        if (isGreen)
        {
            UnityEngine.Cursor.SetCursor(cursorTextureGreen, hotspot, CursorMode.ForceSoftware);
        } else
        {
            UnityEngine.Cursor.SetCursor(cursorTexture, hotspot, CursorMode.ForceSoftware);
        }
    }
}
