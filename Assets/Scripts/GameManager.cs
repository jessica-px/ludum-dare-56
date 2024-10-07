using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public enum GameLevel
{
    One,
    Two,
    Three,
    Four,
    Five,
    Six
}

public class GameManager : MonoBehaviour
{

    public Texture2D cursorTexture;
    public Texture2D cursorTextureGreen;
    public Texture2D cursorTextureFocus;
    public Texture2D cursorTextureFocusGreen;
    public Texture2D cursorTextureFocusRed;

    public bool IsGameOver { get; private set; } = false;
    public int CreaturesCaught { get; private set; } = 0;
    public int CreaturesSquished { get; private set; } = 0;

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
    private IdleAudioController idleAudioController;
    private CatchSFXController catchSFXController;
    private BgMusicController bgMusicController;

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
        idleAudioController = GameObject.Find("IdleSFX").GetComponent <IdleAudioController>();
        catchSFXController = GameObject.Find("CatchSFX").GetComponent<CatchSFXController>();
        bgMusicController = GameObject.Find("BgMusic").GetComponent<BgMusicController>();

        grabBarController = GameObject.Find("GrabBar").GetComponent<GrabBarController>();
        hungerBarController = gameObject.GetComponent<HungerBarController>();
        timerController = gameObject.GetComponent<TimerController>();
        playerHandController = GameObject.Find("PlayerHand").GetComponent<PlayerHandController>();

        SetCursor(false, null);
        StartNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGameOver)
        {
            return;
        }


        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Break();
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


    public GameLevel GetGameLevel()
    {
        // first 15 seconds
        if (timerController.TimeElapsed < 15)
        {
            return GameLevel.One;
        }
        // 15-30
        if (timerController.TimeElapsed < 30)
        {
            return GameLevel.Two;
        }
        // 30-45  seconds
        if (timerController.TimeElapsed < 45)
        {
            return GameLevel.Three;
        }
        // 45-60 seconds
        else if (timerController.TimeElapsed < 60)
        {
            return GameLevel.Four;
        }
        // 60-75 seconds
        else if (timerController.TimeElapsed < 75)
        {
            return GameLevel.Five;
        }
        // >75 seconds
        else
        {
            return GameLevel.Six;
        }
    }

    void UpdateSpawnDelay()
    {
        GameLevel level = GetGameLevel();
        switch (level)
        {
            case GameLevel.One:
                spawnDelay = 2f;
                break;
            case GameLevel.Three:
                spawnDelay = 1.5f;
                break;
            case GameLevel.Six:
                spawnDelay = 1f;
                break;
        }
    }

    GameObject GetRandomCreature()
    {
        GameLevel level = GetGameLevel();
        float random = Random.Range(0, 1f);

        switch (level)
        {
            // slow creatures only
            case GameLevel.One:
                return SlowCreaturePrefab;

            // 75% odds slow, 25% normal
            case GameLevel.Two: 
                if (random < .75)
                {
                    return SlowCreaturePrefab;
                }
                else
                {
                    return CreaturePrefab;
                }

            // 50% odds slow, 50% normal
            case GameLevel.Three:
                if (random < .5)
                {
                    return SlowCreaturePrefab;
                }
                else
                {
                    return CreaturePrefab;
                }

            // 20% slow, 60% normal, 20% fast
            case GameLevel.Four:
                if (random < .2)
                {
                    return SlowCreaturePrefab;
                }
                else if (random < .8)
                {
                    return CreaturePrefab;
                }
                else
                {
                    return FastCreaturePrefab;
                }
            // 60 % normal, 40 % fast
            case GameLevel.Five:
                if (random < .6)
                {
                    return CreaturePrefab;
                }
                else
                {
                    return FastCreaturePrefab;
                }
            //  20% normal, 40% fast, 40% extra fast
            default:
                if (random < .2)
                {
                    return CreaturePrefab;
                }
                else if (random < .6)
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
        bgMusicController.StartMusic();
        currCreatureCount = 0;
    }

    public void SetTargetCreature(Creature creature)
    {
        if (!TargetCreature && !IsGameOver)
        {
            TargetCreature = creature;
            creature.SetState(CreatureState.Targeted);
            grabBarController.ShowGrabBar();
            idleAudioController.PlayIdleClip();
        }
    }
 
    public void OnReleaseMouse()
    {
        grabBarController.HideGrabBar();

        // Success
        if (grabBarController.IsGrabPointerInZone() && TargetCreature.IsHovered)
        {
            CreaturesCaught++;
            TargetCreature.SetState(CreatureState.GrabbedSuccess);
            hungerBarController.ChangeHunger(TargetCreature.hungerValue);
            playerHandController.PlayGrabAnimation(true);
            catchSFXController.PlayRandomClip();
            timeSinceLastDeathOrSpawn = 0;
        }
        // Fail (pop the creature)
        else if (!grabBarController.IsGrabPointerInZone() && TargetCreature.IsHovered)
        {
            CreaturesSquished++;
            TargetCreature.SetState(CreatureState.GrabbedDeath);
            playerHandController.PlayGrabAnimation(false);
            audioController.PlaySoundEffect(SoundEffect.Death, .5f);
            timeSinceLastDeathOrSpawn = 0;
        }
        // Fail (miss)
        else
        {
            TargetCreature.SetState(CreatureState.Idle);
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
        SetCursor(false, null);
    }

    public void OnGameOver()
    {
        IsGameOver = true;
        grabBarController.HideGrabBar();
        UnsetTargetCreature();
        gameOverContainer.style.visibility = Visibility.Visible;
        bgMusicController.StartEndTag();
        Debug.Log("Game over!");
    }

    public void SetCursor(bool creatureIsHovered, CreatureState? state)
    {
        Vector2 hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);

        // focused reticule
        if (TargetCreature)
        {
            // red if not over target
            if (!creatureIsHovered || state != CreatureState.Targeted)
            {
                UnityEngine.Cursor.SetCursor(cursorTextureFocusRed, hotspot, CursorMode.ForceSoftware);
            }
            // green if over target and in grab zone
            else if (grabBarController.IsGrabPointerInZone())
            {
                UnityEngine.Cursor.SetCursor(cursorTextureFocusGreen, hotspot, CursorMode.ForceSoftware);
            }
            // white if over target and not in grab zone
            else
            {
                UnityEngine.Cursor.SetCursor(cursorTextureFocus, hotspot, CursorMode.ForceSoftware);
            }
        }

        // normal reticule
        else
        {
            if (creatureIsHovered)
            {
                UnityEngine.Cursor.SetCursor(cursorTextureGreen, hotspot, CursorMode.ForceSoftware);
            } else
            {
                UnityEngine.Cursor.SetCursor(cursorTexture, hotspot, CursorMode.ForceSoftware);
            }
        }
    }
}
