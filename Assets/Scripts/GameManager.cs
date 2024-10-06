using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum GamePhase
{
    Early,
    EarlyMid,
    Mid,
    Late,
    End
}

public class GameManager : MonoBehaviour
{

    public Texture2D cursorTexture;
    public Texture2D cursorTextureGreen;

    public bool IsGameOver { get; private set; } = false;

    public int initialCreatureCount = 3;
    public GameObject CreaturePrefab;
    public GameObject SlowCreaturePrefab;
    public GameObject FastCreaturePrefab;
    public GameObject ExtraFastCreaturePrefab;
    public Creature? TargetCreature { get; private set; } = null;
    private VisualElement uiRoot;
    private VisualElement gameOverContainer;
    private Button newGameButton;

    private GrabBarController grabBarController;
    private HungerBarController hungerBarController;
    private TimerController timerController;
    private PlayerHandController playerHandController;
    private AudioController audioController;

    private float timeSinceLastDeathOrSpawn = 0;
    public float spawnDelay = 3;
    private int maxCreatureCount = 3;
    public int currCreatureCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        uiRoot = GameObject.Find("Canvas").GetComponent<UIDocument>().rootVisualElement;
        gameOverContainer = uiRoot.Q("GameOverContainer");
        newGameButton = uiRoot.Q<Button>("NewGameButton");
        newGameButton.clicked += () => StartNewGame();

        audioController = gameObject.GetComponent<AudioController>();
        grabBarController = GameObject.Find("GrabBar").GetComponent<GrabBarController>();
        hungerBarController = gameObject.GetComponent<HungerBarController>();
        timerController = gameObject.GetComponent<TimerController>();
        playerHandController = GameObject.Find("PlayerHand").GetComponent<PlayerHandController>();
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

        timeSinceLastDeathOrSpawn += Time.deltaTime;
        bool noCreaturesLeft = currCreatureCount == 0;
        bool spawnDelayhasPassed = timeSinceLastDeathOrSpawn > spawnDelay;
        bool canSpawn = (noCreaturesLeft || spawnDelayhasPassed) && currCreatureCount < maxCreatureCount;
        if (canSpawn)
        {
            SpawnCreature();
        }

        if (Input.GetMouseButtonUp(0) && TargetCreature)
        {
            OnReleaseMouse();
        }

        UpdateSpawnDelay();
    }


    void SpawnCreature()
    {
        // we'll want to check for collisions here
        Vector3 randomVector = new Vector3(Random.Range(-2.5f, 2.5f), Random.Range(-.5f, -3f), 0);
        Instantiate(GetRandomCreature(), randomVector, Quaternion.identity);
        timeSinceLastDeathOrSpawn = 0;
    }


    public GamePhase GetGamePhase()
    {
        // first 10 seconds
        if (timerController.TimeElapsed < 10)
        {
            return GamePhase.Early;
        }
        // 10 - 30  seconds
        if (timerController.TimeElapsed < 30)
        {
            return GamePhase.EarlyMid;
        }
        // 30 - 40 seconds
        else if (timerController.TimeElapsed < 40)
        {
            return GamePhase.Mid;
        }
        // 40 - 60 seconds
        else if (timerController.TimeElapsed < 60)
        {
            return GamePhase.Late;
        }
        // >60 seconds
        else
        {
            return GamePhase.End;
        }
    }

    void UpdateSpawnDelay()
    {
        GamePhase phase = GetGamePhase();
        switch (phase)
        {
            case GamePhase.Early:
                spawnDelay = 2f;
                break;
            case GamePhase.EarlyMid:
                spawnDelay = 1.5f;
                break;
            case GamePhase.Mid:
                spawnDelay = 1.25f;
                break;
            case GamePhase.Late:
                spawnDelay = .9f;
                break;
            case GamePhase.End:
                spawnDelay = .8f;
                break;
        }
    }

    GameObject GetRandomCreature()
    {
        GamePhase phase = GetGamePhase();

        // early game: slow creatures only
        if (phase == GamePhase.Early)
        {
            return SlowCreaturePrefab;
        }

        // early mid game: 50% odds slow vs normal
        else if (phase == GamePhase.EarlyMid)
        {
            float percent = Random.Range(0, 1f);
            if (percent < .5)
            {
                return SlowCreaturePrefab;
            }
            else
            {
                return CreaturePrefab;
            }
        }
        // mid game: 50% normal, 25% slow, 25% fast
        else if (phase == GamePhase.Mid)
        {
            float percent = Random.Range(0, 1f);
            if (percent < .5)
            {
                return CreaturePrefab;
            }
            else if (percent < .75)
            {
                return SlowCreaturePrefab;
            }
            else
            {
                return FastCreaturePrefab;
            }
        }
        // late game: 40% normal, 20% slow, 40% fast
        else if (phase == GamePhase.Late)
        {
            float percent = Random.Range(0, 1f);
            if (percent < .4)
            {
                return CreaturePrefab;
            }
            else if (percent < .6)
            {
                return SlowCreaturePrefab;
            }
            else
            {
                return FastCreaturePrefab;
            }
        }

        // end game: 10% normal, 40% fast, 50% extra fast
        else 
        {
            float percent = Random.Range(0, 1f);
            if (percent < .1)
            {
                return CreaturePrefab;
            }
            else if (percent < .5)
            {
                return FastCreaturePrefab;
            }
            else
            {
                return ExtraFastCreaturePrefab;
            }
        }
    }

    public void StartNewGame()
    {
        IsGameOver = false;
        timerController.Reset();
        hungerBarController.Reset();
        gameOverContainer.style.visibility = Visibility.Hidden;
        SpawnCreature();
        currCreatureCount = 0;
    }

    public void SetTargetCreature(Creature creature)
    {
        if (!TargetCreature && !IsGameOver)
        {
            TargetCreature = creature;
            creature.SetState(CreatureState.Targeted);
            grabBarController.ShowGrabBar();
        }
    }
 
    public void OnReleaseMouse()
    {
        SetCursor(false);
        grabBarController.HideGrabBar();

        // Success
        if (grabBarController.IsGrabPointerInZone() && TargetCreature.IsHovered)
        {
            TargetCreature.SetState(CreatureState.GrabbedSuccess);
            hungerBarController.ChangeHunger(TargetCreature.hungerValue);
            playerHandController.PlayGrabAnimation(true);
            audioController.PlaySoundEffect(SoundEffect.Success, .2f);
            timeSinceLastDeathOrSpawn = 0;
        }
        // Fail (pop the creature)
        else if (!grabBarController.IsGrabPointerInZone() && TargetCreature.IsHovered)
        {
            TargetCreature.SetState(CreatureState.GrabbedDeath);
            playerHandController.PlayGrabAnimation(false);
            audioController.PlaySoundEffect(SoundEffect.Death, .5f);
            timeSinceLastDeathOrSpawn = 0;
        }
        // Fail (miss)
        else
        {
            TargetCreature.SetState(CreatureState.GrabbedMiss);
            audioController.PlaySoundEffect(SoundEffect.Miss, 0);
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
        grabBarController.HideGrabBar();
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
