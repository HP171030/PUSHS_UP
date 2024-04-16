using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        // ���� ����� ������ ���� �̺�Ʈ�� �Լ��� �����մϴ�.
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // ������ ������ �� �ش� ���� ������Ʈ�� ��Ȱ��ȭ�մϴ�.
        gameObject.SetActive(false);
    }

    void Update()
    {
        // ȭ���� Ŭ��(��ġ)���� �� �ش� ���� ������Ʈ�� ��Ȱ��ȭ�մϴ�.
        if (Input.GetMouseButtonDown(0))
        {
            gameObject.SetActive(false);
        }
    }
}