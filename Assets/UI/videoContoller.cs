using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        // 비디오 재생이 끝났을 때의 이벤트에 함수를 연결합니다.
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // 비디오가 끝났을 때 해당 게임 오브젝트를 비활성화합니다.
        gameObject.SetActive(false);
    }

    void Update()
    {
        // 화면을 클릭(터치)했을 때 해당 게임 오브젝트를 비활성화합니다.
        if (Input.GetMouseButtonDown(0))
        {
            gameObject.SetActive(false);
        }
    }
}