using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ImageTimer WheatTimer;
    [SerializeField] private ImageTimer EatingTimer;
    [SerializeField] private Image RaidTimerImg;
    [SerializeField] private Image VillagerTimerImg;
    [SerializeField] private Image KnightTimerImg;

    [SerializeField] private AudioClip eatSound;
    [SerializeField] private AudioClip wheatGatherSound;
    [SerializeField] private AudioClip newRaidStart;
    [SerializeField] private AudioClip plusResourceSound;
    [SerializeField] private AudioSource audio;
    [SerializeField] private AudioSource inGameMusic;
    [SerializeField] private AudioSource gameOverMusic;
    [SerializeField] private AudioSource winMusic;
    [SerializeField] private AudioSource backGroundAnimals;

    [SerializeField] private Button villagerButton;
    [SerializeField] private Button knightButton;

    [SerializeField] private Text wheatCountText;
    [SerializeField] private Text villagerCountText;
    [SerializeField] private Text knightCountText;
    [SerializeField] private Text raidCountText;
    [SerializeField] private Text nextRaidEnemyCount;

    [SerializeField] private Text WinScreenWheatStatistics;
    [SerializeField] private Text WinScreenVillagerStatistics;
    [SerializeField] private Text WinScreenPancakeStatistics;
    [SerializeField] private Text WinScreenRaidsStatistics;

    [SerializeField] private Text GameOverScrWheatStatistics;
    [SerializeField] private Text GameOverScrVillagerStatistics;
    [SerializeField] private Text GameOverScrPancakeStatistics;
    [SerializeField] private Text GameOverScrRaidsStatistics;

    [SerializeField] private int wheatCount;
    [SerializeField] private int villagerCount;
    [SerializeField] private int knightCount;

    [SerializeField] private int wheatPerVillager;
    [SerializeField] private int wheatToKnights;
    [SerializeField] private int raidCount = 1;
    [SerializeField] private int villagersHired = 0;
    [SerializeField] private int raidsSurvived = 0;
    [SerializeField] private int pancakesEaten = 0;
    [SerializeField] private int wheatGathered = 0;

    [SerializeField] private int villagerCost;
    [SerializeField] private int knightCost;

    [SerializeField] private float villagerCreateTime;
    [SerializeField] private float knightCreateTime;
    [SerializeField] private float raidMaxTime;
    [SerializeField] private int raidIncrease;
    [SerializeField] private int nextRaid;

    private float villagerTimer = -2;
    private float knightTimer = -2;
    private float raidTimer;

    private bool villagerIsCreating;
    private bool knightIsCreating;
    private bool audioIsPlaying;
    private bool gameIsPaused;
    private bool gameIsMuted;
    private bool dialogueIsActive;
    private bool dialogueTwoIsActive;
    private bool dialogueThreeIsActive;

    public GameObject MainMenu;
    public GameObject GameScreen;
    public GameObject GameOverScreen;
    public GameObject WinScreen;
    public GameObject PauseScreen;
    public GameObject AudioMuted;
    public GameObject StartDialogue;
    public GameObject StartDialogue2;
    public GameObject StartDialogue3;
    public GameObject GhostImage;
    public GameObject GhostImgPreview;

    
    void Start()
    {
        UpdateText();
        raidTimer = raidMaxTime;
        backGroundAnimals.ignoreListenerPause = true;
        dialogueIsActive = false;
    }

    
    void Update()
    {
        raidTimer -= Time.deltaTime;
        RaidTimerImg.fillAmount = raidTimer / raidMaxTime;

        GhostAppear();
        DialogueLogic();

        GamePause();
        MuteAudio();

        WheatCountManager();
        HireButtonLogic();

        RaidTimerLogic();
        WheatTimerLogic();
        EatingTimerLogic();
        
        UpdateText();

        VillagerButtonTimer();
        KnightButtonTimer();

        GameOverLogic();
        WinGameLogic();
    }

    private void UpdateText()
    {
        wheatCountText.text = wheatCount.ToString();
        villagerCountText.text = villagerCount.ToString();
        knightCountText.text = knightCount.ToString();

        WinScreenWheatStatistics.text = wheatGathered.ToString();
        WinScreenVillagerStatistics.text = villagersHired.ToString();
        WinScreenRaidsStatistics.text = raidsSurvived.ToString();
        WinScreenPancakeStatistics.text = pancakesEaten.ToString();

        GameOverScrWheatStatistics.text = wheatGathered.ToString(); 
        GameOverScrVillagerStatistics.text = villagersHired.ToString();
        GameOverScrPancakeStatistics.text = pancakesEaten.ToString();
        GameOverScrRaidsStatistics.text = (raidsSurvived - 1).ToString();
    }

    private void EndGameUpdate()
    {
        DestroyObject(GameScreen);
        MainMenu.SetActive(true);
    }

    public void CreateVillager()
    {
        wheatCount -= villagerCost;
        villagerTimer = villagerCreateTime;
        villagerIsCreating = true;
    }

    public void CreateKnight()
    {
        wheatCount -= knightCost;
        knightTimer = knightCreateTime;
        knightIsCreating = true;
    }

    public void WheatCountManager()
    {
        if (wheatCount < 0) wheatCount = 0;
        if (wheatCount > 999) wheatCount = 999;
    }

    public void HireButtonLogic()
    {
        if (villagerIsCreating || wheatCount < villagerCost)
        {
            villagerButton.interactable = false;
        }
        else
        {
            villagerButton.interactable = true;
        }

        if (knightIsCreating || wheatCount < knightCost)
        {
            knightButton.interactable = false;
        }
        else
        {
            knightButton.interactable = true;
        }
    }

    public void GameOverMusicPlay()
    {
         inGameMusic.Stop();
         if (audioIsPlaying && !gameOverMusic.isPlaying)
         {
            gameOverMusic.Play();
         }   
    }

    public void WinMusicPlay()
    {
        inGameMusic.Stop();
        if (audioIsPlaying && !winMusic.isPlaying && !gameOverMusic.isPlaying)
        {
            winMusic.Play();
        }
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GamePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (knightCount >= 0 && raidCount <= 10)
            gameIsPaused = !gameIsPaused;

            if (gameIsPaused)
            {
                Time.timeScale = 0f;
                AudioListener.pause = true;
                PauseScreen.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                AudioListener.pause = false;
                PauseScreen.SetActive(false);
            }

            Debug.Log("Esc button pressed");
        }
    }

    public void MuteAudio()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            gameIsMuted = !gameIsMuted;

            if (gameIsMuted)
            {
                AudioListener.volume = 0f;
                AudioMuted.SetActive(true);
            }
            else
            {
                AudioListener.volume = 1.0f;
                AudioMuted.SetActive(false);
            }
        }
    }
    public void RaidTimerToDefault()
    {
        raidMaxTime = 16;
        raidTimer = raidMaxTime;
    }

    private void RaidTimerLogic()
    {
        if (raidTimer <= 0)
        {
            raidTimer = raidMaxTime;
            knightCount -= nextRaid;
            raidCount += 1;
            raidsSurvived += 1;
            raidCountText.text = raidCount.ToString();
            audio.PlayOneShot(newRaidStart, 0.9f);

            if (raidCount > 3)
            {
                nextRaid += raidIncrease;
            }

            if (raidCount > 6)
            {
                nextRaid -= 1;
            }
            nextRaidEnemyCount.text = nextRaid.ToString();
        }
    }

    private void WheatTimerLogic()
    {
        if (WheatTimer.Tick)
        {
            wheatCount += villagerCount * wheatPerVillager;
            wheatGathered += villagerCount * wheatPerVillager;
            audio.PlayOneShot(wheatGatherSound, 0.5f);
        }
    }

    private void EatingTimerLogic()
    {
        if (EatingTimer.Tick)
        {
            wheatCount -= knightCount * wheatToKnights;
            audio.PlayOneShot(eatSound, 0.5f);
            pancakesEaten += 1;
        }
    }

    private void VillagerButtonTimer()
    {
        if (villagerTimer > 0)
        {
            villagerTimer -= Time.deltaTime;
            VillagerTimerImg.fillAmount = villagerTimer / villagerCreateTime;
        }
        else if (villagerTimer > -1)
        {
            VillagerTimerImg.fillAmount = 0;
            villagerIsCreating = false;
            villagerCount += 1;
            villagersHired += 1;
            villagerTimer = -2;
            audio.PlayOneShot(plusResourceSound, 0.5f);
        }
    }

    private void KnightButtonTimer()
    {
        if (knightTimer > 0)
        {
            knightTimer -= Time.deltaTime;
            KnightTimerImg.fillAmount = knightTimer / knightCreateTime;
        }
        else if (knightTimer > -1)
        {
            KnightTimerImg.fillAmount = 0;
            knightIsCreating = false;
            knightCount += 1;
            knightTimer = -2;
            audio.PlayOneShot(plusResourceSound, 0.5f);
        }
    }

    private void GameOverLogic()
    {
        if (knightCount < 0)
        {
            GameOverScreen.SetActive(true);
            audioIsPlaying = true;
            GameOverMusicPlay();
            EndGameUpdate();
            backGroundAnimals.volume = 0;
        }
    }

    private void WinGameLogic()
    {
        if (raidCount > 10 && knightCount >= 0)
        {
            WinScreen.SetActive(true);
            audioIsPlaying = true;
            WinMusicPlay();
            EndGameUpdate();
            backGroundAnimals.volume = 0;
        }
    }

    private void DialogueLogic()
    {
        if (raidCount >= 2 && !dialogueIsActive)
        {
            StartDialogue.SetActive(true);
            dialogueIsActive = true;
        }
        if (raidCount >= 3 && dialogueIsActive)
        {
            StartDialogue.SetActive(false);
            dialogueIsActive = false;
        }
        if (raidCount >=5 && !dialogueTwoIsActive)
        {
            StartDialogue2.SetActive(true);
            dialogueTwoIsActive = true;
        }
        if (raidCount >=6 && dialogueTwoIsActive)
        {
            StartDialogue2.SetActive(false);
            dialogueTwoIsActive = false;
        }
        if (raidCount >= 9 && !dialogueThreeIsActive)
        {
            StartDialogue3.SetActive(true);
            dialogueThreeIsActive = true;
        }
        if (raidCount >=10 && dialogueThreeIsActive)
        {
            StartDialogue3.SetActive(false);
            dialogueThreeIsActive = false;
        }
    }

    public void DialogueTurnOff()
    {
        if (dialogueIsActive)
        {
            StartDialogue.SetActive(false);
        }
        if (dialogueTwoIsActive)
        {
            StartDialogue2.SetActive(false);
        }
        if (dialogueThreeIsActive)
        {
            StartDialogue3.SetActive(false);
        }
    }

    private void GhostAppear()
    {
        if (raidCount >=4)
        {
            GhostImgPreview.SetActive(false);
            GhostImage.SetActive(true);
        }
    }
}

