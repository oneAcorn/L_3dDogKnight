using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>, IEndGameObserver
{
    public GameObject playerPrefab;
    public SceneFader sceneFaderPrefab;

    private bool isFadeFinished;
    private GameObject player;
    private NavMeshAgent playerAgent;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        isFadeFinished = true;
    }

    public void TransitionToDestination(TransitionPoint transPoint)
    {
        switch (transPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                TransSameScene(transPoint);
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                TransDefferentScene(transPoint);
                break;
        }
    }

    private void TransSameScene(TransitionPoint transPoint)
    {
        StartCoroutine(Transition(SceneManager.GetActiveScene().name, transPoint.destinationTag));
    }

    private void TransDefferentScene(TransitionPoint transPoint)
    {
        StartCoroutine(Transition(transPoint.sceneName, transPoint.destinationTag));
    }

    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        SaveManager.Instance.SavePlayerData();
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            //不同场景的传送
            yield return SceneManager.LoadSceneAsync(sceneName);
            var destinationTf = GetDestination(destinationTag).transform;
            yield return Instantiate(playerPrefab, destinationTf.position, destinationTf.rotation);
            SaveManager.Instance.LoadPlayerData();
        }
        else
        {
            //相同场景的传送
            player = GameManager.Instance.playerState.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            var destinationTf = GetDestination(destinationTag).transform;
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(destinationTf.position, destinationTf.rotation);
            playerAgent.enabled = true;
            yield return null;
        }
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        foreach (var entrance in entrances)
        {
            if (entrance.destinationTag == destinationTag)
                return entrance;
        }

        return null;
    }

    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("Main"));
    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    public void TransitionToMainMenu()
    {
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadLevel(string scene)
    {
        if (string.IsNullOrEmpty(scene))
            yield break;
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2.5f));
        yield return SceneManager.LoadSceneAsync(scene);
        yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position,
            GameManager.Instance.GetEntrance().rotation);
        //保存数据
        SaveManager.Instance.SavePlayerData();
        yield return StartCoroutine(fade.FadeIn(2.5f));
    }

    IEnumerator LoadMainMenu()
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2.5f));
        yield return SceneManager.LoadSceneAsync("UI");
        yield return StartCoroutine(fade.FadeIn(2.5f));
    }

    public void EndNotify()
    {
        if (isFadeFinished)
        {
            isFadeFinished = false;
            StartCoroutine(LoadMainMenu());
        }
    }
}