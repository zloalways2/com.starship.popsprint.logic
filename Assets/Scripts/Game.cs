using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Game : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject levelsPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject endLvl;
    [SerializeField] private GameObject statisticsPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private GameObject menuBtn;

    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject stats;

    [SerializeField] private TextMeshProUGUI lvlTitle;
    [SerializeField] private TextMeshProUGUI timerTxt;

    [SerializeField] private TextMeshProUGUI wlTxt;
    [SerializeField] private TextMeshProUGUI rnTxt;
    [SerializeField] private TextMeshProUGUI [] scoreTxt;

    Bubble _bubble;
    Timer _time;
    StatsUIManager statsUI;

    public bool inGame;
    public string lvlName;
    GameObject lastBtnLvl;

    [SerializeField] private Slider slide_1, slide_2;
    [SerializeField] private AudioMixerGroup mixer;
    public AudioSource aEffsects;
    float vol;

    // Start is called before the first frame update
    void Start()
    {
        _bubble = FindObjectOfType<Bubble>();
        _time = timer.GetComponent<Timer>();
        statsUI = stats.GetComponent<StatsUIManager>();
        inGame = false;
        LoadEffect();
        LoadMusic();
    }

    // Update is called once per frame
    void Update()
    {
        if (_time.min <= 0 && _time.sec <=0 && inGame == true)
        {
            _time.startTimer = false;
            _bubble.saveStat(false);
            _bubble.clearAll();
            gamePanel.SetActive(false);
            endLvl.SetActive(true);
            inGame = false;
            scorePanel.SetActive(false);
            menuBtn.SetActive(false);
            wlTxt.text = "DEFEAT";
            rnTxt.text = "Try Again";
            scoreTxt[0].text = "TIME IS";
            scoreTxt[1].text = "OUT";
        }
    }

    void exitOnMainMenu(GameObject go)
    {
        menuPanel.SetActive(false);
        go.SetActive(true);
        menuBtn.SetActive(true);
        aEffsects.Play();
    }

    public void clickPlay()
    {
        exitOnMainMenu(levelsPanel);
        _bubble.HilightLevelBtn();
    }

    public void clickInfo()
    {
        exitOnMainMenu(infoPanel);
    }

    public void clickOptions()
    {
        exitOnMainMenu(optionsPanel);
    }

    public void clickSettingBtn()
    {
        _time.startTimer = false;
        _bubble.HideBubbles();
        gamePanel.SetActive(false);
        scorePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void clickStatistics()
    {
        exitOnMainMenu(statisticsPanel);
        statsUI.DisplayStats();
    }

    public void clickExit()
    {
        exitOnMainMenu(exitPanel);
    }

    public void clickExitYes()
    {
        Application.Quit();
    }

    public void clickBack()
    {
        if (inGame == true)
        {
            _time.startTimer = true;
            _bubble.ShowBubbles();
            gamePanel.SetActive(true);
            scorePanel.SetActive(true);
            optionsPanel.SetActive(false);
        }
        else
            exitMainMenu();
    }

    public void clickChoiseLvl(GameObject btn)
    {
        levelsPanel.SetActive(false);
        gamePanel.SetActive(true);
        scorePanel.SetActive(true);
        inGame = true;
        lvlName = btn.GetComponentInChildren<TextMeshProUGUI>().text;
        lvlTitle.text = "Level " + lvlName;
        lastBtnLvl = btn;

        _bubble.GenerateBubbles();
        _time.startTimer = true;
        _time.min = 2;
        _time.sec = 0;
        menuBtn.SetActive(true);
        aEffsects.Play();
    }

    public GameObject FindLevelBtn(string levelName)
    {
        Transform[] levelBtns = levelsPanel.GetComponentsInChildren<Transform>();
        foreach (var level in levelBtns)
        {
            if (level.name == levelName)
            {
                return level.gameObject;
            }
        }
        return null;
    }

    public void clickNT()
    {
        if (lvlName == "35")
        {
            exitMainMenu();
            return;
        }

        if (rnTxt.text == "Next")
        {
            int tmp;
            int.TryParse(lvlName, out tmp);
            tmp++;
            lvlName = tmp.ToString();
        }

        endLvl.SetActive(false);
        clickChoiseLvl(FindLevelBtn("LevelChoiceBtn " + lvlName));
        aEffsects.Play();
    }

    public void clickHome()
    {
        exitMainMenu();
        aEffsects.Play();
    }

    public void clickClearData()
    {

    }

    public void endLevel() 
    {
        if (_time.min > 0 || _time.sec > 0 && inGame == false)
        {
            _bubble.clearAll();
            endLvl.SetActive(true);
            _time.startTimer = false;
            scorePanel.SetActive(false);
            menuBtn.SetActive(false);
            wlTxt.text = "WIN";
            rnTxt.text = "Next";
            scoreTxt[0].text = "SCORE";
            scoreTxt[1].text = _bubble.score.ToString();
            lastBtnLvl.GetComponent<Image>().color = Color.green;
        }
    }

    public void exitMainMenu()
    {
        _bubble.clearAll();
        menuPanel.SetActive(true);
        levelsPanel.SetActive(false);
        gamePanel.SetActive(false);
        endLvl.SetActive(false);
        statisticsPanel.SetActive(false);
        optionsPanel.SetActive(false);
        exitPanel.SetActive(false);
        infoPanel.SetActive(false);
        scorePanel.SetActive(false);
        menuBtn.SetActive(false);
        aEffsects.Play();
    }

    void LoadMusic()
    {
        if (PlayerPrefs.HasKey("music"))
        {
            slide_1.value = PlayerPrefs.GetFloat("music");
            Options.Value(ref vol, slide_1.value);
            mixer.audioMixer.SetFloat("Music", vol);
        }
        else
        {
            slide_1.value = 10;
            Options.Value(ref vol, slide_1.value);
            mixer.audioMixer.SetFloat("Music", vol);
        }
    }

    void LoadEffect()
    {
        if (PlayerPrefs.HasKey("sound"))
        {
            slide_2.value = PlayerPrefs.GetFloat("sound");
            Options.Value(ref vol, slide_2.value);
            mixer.audioMixer.SetFloat("Sound", vol);
        }
        else
        {
            slide_1.value = 10;
            Options.Value(ref vol, slide_1.value);
            mixer.audioMixer.SetFloat("Sound", vol);
        }
    }
}
