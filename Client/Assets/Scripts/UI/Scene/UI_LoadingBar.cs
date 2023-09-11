using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_LoadingBar : UI_Scene
{
    GameObject fillArea;
    Slider LoadSlider;
    enum Buttons
    {
        LoadButton,
        SceneLoadButton,
    }
    enum Sliders
    {
        LoadSlider,
    }

    void Start()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));

        GetButton((int)Buttons.LoadButton).onClick.AddListener(LoadAllAsync);
        //GetButton((int)Buttons.SceneLoadButton).onClick.AddListener(GoToLobbyScene);
        LoadSlider = GetSlider((int)Sliders.LoadSlider);
        fillArea = LoadSlider.transform.Find("Fill Area").gameObject;

        // 슬라이더 값 0일 땐 안보이게 비활성화
        if (LoadSlider.value == 0f)
            fillArea.SetActive(false);
        //Managers.Resource.InstantiateResources("Bush");
    }
    private void Update()
    {
        // 슬라이더 값 0이 아닐 땐 안보이게 비활성화
        if (LoadSlider.value != 0f)
            fillArea.SetActive(true);
    }
    void LoadAllAsync()
    {
        GetButton((int)Buttons.LoadButton).interactable = false;
        Managers.Resource.LoadAllAsync<GameObject>("Prefab", (key, count, totalCount) =>
        {
            // 로딩바 사용 가능
            Debug.Log($"{key} {count}/{totalCount}");
            GetSlider((int)Sliders.LoadSlider).value = (float)count / (float)totalCount;
            if (count == totalCount)
            {
                // 다 완료되고 나서 실행할 함수
                Debug.Log("Load All Prefabs");
                // TODO
                Object loadingScene = null;
                if (Managers.Resource.Res.TryGetValue("LoadingScene", out loadingScene) == false)
                {
                    loadingScene = Managers.Resource.Load<GameObject>("UI/Scene/LoadingScene");
                    Managers.Resource.Res.Add("LoadingScene", loadingScene);
                    Debug.Log("Temp");
                }
                Managers.Scene.LoadScene("Lobby");
                //Managers.Scene.LoadScene("Lobby");

                //#if UNITY_EDITOR
                //                Managers.Scene.LoadScene("Lobby");
                //#elif UNITY_STANDALONE
                //                Managers.Scene.LoadScene("SampleScene");
                //#elif UNITY_ANDROID
                //                Managers.Scene.LoadScene("Lobby");
                //#endif
            }
        });
    }
    public void GoToLobbyScene()
    {
        Managers.Scene.LoadScene("Lobby");
    }
}
