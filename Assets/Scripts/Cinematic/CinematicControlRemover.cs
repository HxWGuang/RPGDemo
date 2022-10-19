using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicControlRemover : MonoBehaviour
{
    public GameObject player;

    private PlayableDirector _playableDirector = null;

    private void Awake()
    {
        // 获取PlayableDirector组件
        _playableDirector = GetComponent<PlayableDirector>();
    }

    // 使用PlayableDirector组件自带的两个委托
    // played 和 stopped 来控制PlayController
    private void OnEnable()
    {
        _playableDirector.played += DisableControl;
        _playableDirector.stopped += EnableControl;
    }

    private void OnDisable()
    {
        _playableDirector.played -= DisableControl;
        _playableDirector.stopped -= EnableControl;
    }

    void EnableControl(PlayableDirector pd)
    {
        Debug.Log("启用Controller");
        // GetComponent<PlayerController>().enabled = true;
        player.GetComponent<PlayerController>().enabled = true;
    }

    void DisableControl(PlayableDirector pd)
    {
        Debug.Log("禁用Controller");
        //取消当前的所有行动
        player.GetComponent<ActionScheduler>().CancelCurrentAction();
        
        player.GetComponent<PlayerController>().enabled = false;
    }
}
