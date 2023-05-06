using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Map_1_Script : MonoBehaviour
{
    [SerializeField] public Button MenuButton;
    [SerializeField] public GameObject SettingPanell;
    [SerializeField] public Button ReturnButton;
    [SerializeField] public Button ReturnButton2;
    [SerializeField] public Button RespawnButton;
    [SerializeField] public Button MuteButton;
    [SerializeField] public AudioSource Bgm;
    [SerializeField] public GameObject RespawnPanel;
    [SerializeField] public GameObject Car;
    [SerializeField] public GameObject InstructionPanel;
    public Vector3 LastPosition;
    public float DistanceMoved = 0;
    [SerializeField] public Text Distance;
    [SerializeField] public Text TimeCountDown;
    public float CurrTime = 5;
    public bool TimeActivate = false;
    [SerializeField] public Text RespawnText;
    [SerializeField] public Text HighScoreText;
    [SerializeField] public Button ClearScoreButton;

    // Start is called before the first frame update
    void Start()
    {
        // calculate the distance moved
        LastPosition = Car.transform.position;

        // set high score
        if(!PlayerPrefs.HasKey("HighScore")){
            PlayerPrefs.SetFloat ("HighScore", 0);
        }

        Button menuBtn = MenuButton.GetComponent<Button>();
        menuBtn.onClick.AddListener(OpenPanel);

        Button muteBtn = MuteButton.GetComponent<Button>();
        muteBtn.onClick.AddListener(MuteBgm);

        Button returnBtn = ReturnButton.GetComponent<Button>();
        returnBtn.onClick.AddListener(SwitchScene);
        Button returnBtn2 = ReturnButton2.GetComponent<Button>();
        returnBtn2.onClick.AddListener(SwitchScene);

        Button respBtn = RespawnButton.GetComponent<Button>();
        respBtn.onClick.AddListener(ResetMap);

        Button cleScoBtn = ClearScoreButton.GetComponent<Button>();
        cleScoBtn.onClick.AddListener(ClearHighScore);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))   OpenPanel();

        CloseInstructionPanel();
        OpenRespawnPanel();
        DisplayScore();
        DisplayDistanceMoved();
        DisplayTimeCountDown();
    }


    void OpenPanel(){
        bool isActive = SettingPanell.activeSelf;
        SettingPanell.SetActive(!isActive);
    }

    void MuteBgm(){
        AudioSource player = Bgm.GetComponent<AudioSource>();
        Button muteBtn = MuteButton.GetComponent<Button>();
        if (player.isPlaying){
            //player.Stop();
            player.Pause();
            muteBtn.GetComponentInChildren<Text>().text = "Off";
        } else {
            player.UnPause();
            muteBtn.GetComponentInChildren<Text>().text = "On";
        }
    }

    void SwitchScene(){
        SceneManager.LoadScene("StartScene");
    }

    void OpenRespawnPanel(){
        if (Car != null){
            var healthManager = Car.gameObject.GetComponent<HealthManager>();
            if (healthManager.GetCurrentHealth() <= 0){
                RespawnPanel.SetActive(true);
            }
        }
    }

    void ResetMap(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void CloseInstructionPanel(){
        if (Input.GetKeyDown(KeyCode.UpArrow) || 
            Input.GetKeyDown(KeyCode.LeftArrow) || 
            Input.GetKeyDown(KeyCode.RightArrow) || 
            Input.GetKeyDown(KeyCode.DownArrow) ||
            Input.GetKeyDown(KeyCode.Escape)){
                InstructionPanel.SetActive(false);
            }
    }

    void DisplayDistanceMoved(){
        if (Car != null){
            if (TimeActivate){
                // calculate the distance moved
                DistanceMoved = Vector3.Distance(LastPosition, Car.transform.position);
                //LastPosition = Car.transform.position;
                Distance.fontSize = 32;
                Distance.text = DistanceMoved.ToString("f0") + "m";
            }
        }
    }

    void DisplayTimeCountDown(){
        if (!TimeActivate) {
            if (Input.GetKeyDown(KeyCode.UpArrow)){
                TimeActivate = true;
            }
        } else {
            TimeCountDown.text = CurrTime.ToString("f0") + "s";
            if (CurrTime > 0){
                CurrTime -= Time.deltaTime;
            } else {
                Destroy(Car);
                DisplayRespawnText();
                RespawnPanel.SetActive(true);
            }
        }
    }

    void DisplayRespawnText(){
        if (DistanceMoved >= PlayerPrefs.GetFloat("HighScore")){
            PlayerPrefs.SetFloat ("HighScore", DistanceMoved);
            RespawnText.text = "New Record!";
        } else {
            RespawnText.text ="Time's Up";
        }
    }

    void DisplayScore(){
        float tempScore = PlayerPrefs.GetFloat("HighScore");
        HighScoreText.text = "High Score: " + tempScore.ToString("f0") + "m";
    }

    void ClearHighScore(){
        //PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat ("HighScore", 0);
    }
}
