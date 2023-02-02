using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
public class vidPlayer : MonoBehaviour
{
    // Bitdepth option to select the bitdepth of the input video
    // Detail: The bitdepth controlls the amount of colors the RenderTextures used to display the videos can display
    public enum Bitdepth { int8, int10, int12, int16, int24, int32 };
    public Bitdepth bitdepth;

    public GameObject vidLayerPrefab;

    // subpath in the Resources folder where the videos are stored
    public string videoPath = "Videos";

    // fade duration in seconds
    public float fadeDuration = 5f;

    // Animation Curve to control the fade
    public AnimationCurve fadeCurve;

    // List to store references to the video files that the player can play
    public List<Object> videos;

    // VideoPlayers to render the video files
    private VideoPlayer playerA;
    private VideoPlayer playerB;

    // RenderTextures to display the video files
    private RenderTexture videoA;
    private RenderTexture videoB;

    // Resolution of the input video
    int width;
    int height;

    // Variables to store the video layers
    GameObject vidLayerA;
    GameObject vidLayerB;

    // fade duration in frames
    private int fadeDurationFrames;

    // fading state
    private bool faded = true;

    void Start()
    {
        // Create video layers
        vidLayerA = Instantiate(vidLayerPrefab, transform);
        vidLayerA.transform.SetParent(transform, false);
        vidLayerA.name = "VideoLayerA";
        vidLayerB = Instantiate(vidLayerPrefab, transform);
        vidLayerB.transform.SetParent(transform, false);
        vidLayerB.name = "VideoLayerB";

        // Load all video files from the Resources/videoPath (defined in Inspector) folder
        videos.AddRange(loadVideos(videoPath));

        // Check that the fadeDuration is less than half as long as the shortest video
        videos.FindAll(x => ((VideoClip)x).length < fadeDuration * 2.5f).ForEach(x => Debug.LogError("The fadeDuration is longer than half the length of the video " + x.name + ". This will cause flickering and can break the application."));

        // Create VideoPlayers
        playerA = gameObject.AddComponent<VideoPlayer>();
        playerB = gameObject.AddComponent<VideoPlayer>();

        // Get the resolution from one of the videos
        // Detail: (int) is needed to convert uint to int for compatibility with the RenderTexture
        width = (int)((VideoClip)videos[0]).width;
        height = (int)((VideoClip)videos[0]).height;

        // Apply resolution to the VideoLayers
        vidLayerA.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        vidLayerB.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        // Create RenderTextures
        videoA = new RenderTexture(width, height, ((int)bitdepth));
        videoB = new RenderTexture(width, height, ((int)bitdepth));

        // Apply RenderTextures to the VideoLayers
        vidLayerA.GetComponent<RawImage>().texture = videoA;
        vidLayerB.GetComponent<RawImage>().texture = videoB;

        // Apply RenderTextures to the VideoPlayers
        playerA.targetTexture = videoA;
        playerB.targetTexture = videoB;

        // Calculate the fade duration in frames
        fadeDurationFrames = (int)(fadeDuration * ((VideoClip)videos[0]).frameRate);

        // Start playing the first video
        playNextVideo(playerA);

        // Add a listener to the VideoPlayer to detect when the video has finished playing
        playerA.loopPointReached += OnLoopPointReached;
        playerB.loopPointReached += OnLoopPointReached;
    }

    // Update is called once per frame
    void Update()
    {
        // check wether the fade should start
        if (faded && playerA.isPlaying && playerA.frame >= (int)playerA.frameCount - fadeDurationFrames)
        {
            faded = false;
            playNextVideo(playerB);
            StartCoroutine(fadeVideo(vidLayerA, vidLayerB, 1f / playerA.frameRate));
        }
        else if (faded && playerB.isPlaying && playerB.frame >= (int)playerB.frameCount - fadeDurationFrames)
        {
            faded = false;
            playNextVideo(playerA);
            StartCoroutine(fadeVideo(vidLayerB, vidLayerA, 1f / playerA.frameRate));
        }
    }

    void OnLoopPointReached(UnityEngine.Video.VideoPlayer vp)
    {
        faded = true;
    }

    // Plays the next video in the list picked randomly
    public void playNextVideo(VideoPlayer vp)
    {
        // reload all videos after playing all videos
        if (videos.Count == 0)
        {
            videos.AddRange(loadVideos(videoPath));
        }

        // select a random video from the list and play it
        int index = Random.Range(0, videos.Count);
        vp.clip = (VideoClip)videos[index];
        vp.Play();

        // remove current playing video from the video list
        videos.RemoveAt(index);
    }

    // Load all video files from the Resources/Videos folder
    List<Object> loadVideos(string path)
    {
        List<Object> videos = new List<Object>();
        videos.AddRange(Resources.LoadAll(path, typeof(VideoClip)));
        return videos;
    }

    // Coroutine to fade between two videos
    IEnumerator fadeVideo(GameObject layerA, GameObject layerB, float frameDuration)
    {
        // set the inital alpha values of the video layers
        layerA.GetComponent<CanvasGroup>().alpha = 1;
        layerB.GetComponent<CanvasGroup>().alpha = 0;

        // fade the video layer in
        for (int i = 0; i < fadeDurationFrames; i++)
        {
            layerA.GetComponent<CanvasGroup>().alpha = 1 - fadeCurve.Evaluate((float)i / fadeDurationFrames);
            layerB.GetComponent<CanvasGroup>().alpha = fadeCurve.Evaluate((float)i / fadeDurationFrames);
            yield return new WaitForSeconds(frameDuration);
        }

        // set the final alpha values of the video layers
        layerA.GetComponent<CanvasGroup>().alpha = 0;
        layerB.GetComponent<CanvasGroup>().alpha = 1;
    }
}
