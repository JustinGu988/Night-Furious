using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public Button startButton;
    public Button settingButton;
    public Button quitButton;
    public Button muteButton;
    public AudioSource Bgm;
    public GameObject SettingPanell;
    public GameObject StartPanel;
    public Button ImageCity;
    public Button ImageHalloween;


    // Start is called before the first frame update
    void Start()
    {
        Button startBtn = startButton.GetComponent<Button>();
        //startBtn.GetComponentInChildren<Text>().text = " ";
		    startBtn.onClick.AddListener(OpenStartPanel);

        Button settingBtn = settingButton.GetComponent<Button>();
        settingBtn.onClick.AddListener(OpenSettingPanel);

        Button quitBtn = quitButton.GetComponent<Button>();
        quitBtn.onClick.AddListener(QuitGame);

        Button muteBtn = muteButton.GetComponent<Button>();
        muteBtn.onClick.AddListener(MuteBgm);

        Button city = ImageCity.GetComponent<Button>();
        city.onClick.AddListener(SwitchSceneCity);

        Button hall = ImageHalloween.GetComponent<Button>();
        hall.onClick.AddListener(SwitchSceneHall);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SwitchSceneCity(){
        SceneManager.LoadScene("Map_1_Scene");
    }

    void SwitchSceneHall(){
        SceneManager.LoadScene("Map_2_Scene");
    }

    void MuteBgm(){
        AudioSource player = Bgm.GetComponent<AudioSource>();
        Button muteBtn = muteButton.GetComponent<Button>();
        if (player.isPlaying){
            //player.Stop();
            player.Pause();
            muteBtn.GetComponentInChildren<Text>().text = "Off";
        } else {
            player.UnPause();
            muteBtn.GetComponentInChildren<Text>().text = "On";
        }
    }

    void OpenSettingPanel(){
        StartPanel.SetActive(false);
        bool isActive = SettingPanell.activeSelf;
        SettingPanell.SetActive(!isActive);
    }

    void OpenStartPanel(){
        bool isActive = StartPanel.activeSelf;
        StartPanel.SetActive(!isActive);
    }

    void QuitGame(){
        Application.Quit();
    }

    void StartBtnColor(){
        var red = new Color (250, 65, 39);
        var black = new Color (61, 61, 61);
        Button StartBtn = startButton.GetComponent<Button>();
        var CurrColor = StartBtn.GetComponent<Button>().colors;

        if(CurrColor.normalColor == red){
            CurrColor.normalColor = black;
            StartBtn.GetComponent<Button>().colors = CurrColor;
        }
        else{
            CurrColor.normalColor = red;
            StartBtn.GetComponent<Button>().colors = CurrColor;
        }
    }
}
