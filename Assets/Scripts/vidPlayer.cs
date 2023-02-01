using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class vidPlayer : MonoBehaviour
{
    // List<VideoClip> sources = new List<VideoClip>();
    // Start is called before the first frame update

    // 
    public List<Object> videos;

    // VideoPlayer object to control
    public VideoPlayer player;

    void Start()
    {
        // Check wether a video player is attached to the gameObject and if not create one and attach it to player
        if (GetComponent<VideoPlayer>() != null)
        {

            player = GetComponent<VideoPlayer>();
        }
        else
        {
            player = gameObject.AddComponent<VideoPlayer>();
        }

        //Debug.Log("Start");
        videos.AddRange(loadVideos());
        //Debug.Log(videos.Count);
        foreach (var video in videos)
        {
            Debug.Log(video.name);
        }
        //Debug.Log("Passed");

        playRandomVideo();

        player.loopPointReached += OnLoopPointReached;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnLoopPointReached(UnityEngine.Video.VideoPlayer vp)
    {
        playRandomVideo();
        //Debug.Log("next video playing");
    }

    public void playRandomVideo()
    {
        // reload all videos after playing all videos
        if (videos.Count == 0)
        {
            videos.AddRange(loadVideos());
        }

        // select a random video from the list
        int index = Random.Range(0, videos.Count);
        player.clip = (VideoClip)videos[index];
        player.Play();

        // remove current playing video from the video list
        videos.RemoveAt(index);
    }

    List<Object> loadVideos()
    {
        List<Object> videos = new List<Object>();
        videos.AddRange(Resources.LoadAll("Videos", typeof(VideoClip)));
        return videos;
    }
}
