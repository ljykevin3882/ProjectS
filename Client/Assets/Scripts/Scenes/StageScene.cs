using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StageScene : MonoBehaviour
{
    private Button Stage0_Button, Stage1_Button, Stage2_Button, Stage3_Button, Back_Button,Home_Button ;
    void Start()
    {
        Stage0_Button = GameObject.Find("Stage0").GetComponent<Button>();
        Stage0_Button.onClick.AddListener(Stage0);
        Stage1_Button = GameObject.Find("Stage1").GetComponent<Button>();
        Stage1_Button.onClick.AddListener(Stage1);
        Stage2_Button = GameObject.Find("Stage2").GetComponent<Button>();
        Stage2_Button.onClick.AddListener(Stage2);
        Stage3_Button = GameObject.Find("Stage3").GetComponent<Button>();
        Stage3_Button.onClick.AddListener(Stage3);
        Back_Button = GameObject.Find("Button_Back").GetComponent<Button>();
        Back_Button.onClick.AddListener(GoToPlayModeScene);
        Home_Button = GameObject.Find("Button_Home").GetComponent<Button>();
        Home_Button.onClick.AddListener(GoToLobbyScene);
    }
    private void Stage0()
    {
        Managers.Scene.LoadScene("SingleGame");
        Stage.currentStage = 0;
    }
    private void Stage1()
    {
        Managers.Scene.LoadScene("SingleGame");
        Stage.currentStage = 1;
    }
    private void Stage2()
    {
        Managers.Scene.LoadScene("SingleGame");
        Stage.currentStage = 2;
    }
    private void Stage3()
    {
        Managers.Scene.LoadScene("SingleGame");
        Stage.currentStage = 3;
    }
    private void GoToPlayModeScene()
    {
        Managers.Scene.LoadScene("PlayMode");
    }
    private void GoToLobbyScene()
    {
        Managers.Scene.LoadScene("Lobby");
    }
}
public static class Stage
{
    public static int currentStage = 0;
    public static int maxStage = 0;
}