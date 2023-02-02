# Video Player Example

This is a simple example for a video player that plays radom videos from a folder you define in the inspector and automatically fades between them. When all videos were played, the folder is reloaded and the videos are played again in a new random order.

# How to use
1) Put your videos in one or more subfolders in the Resources folder.
2) Grab a "Window" prefab from the Prefabs folder and attach it to the Canvas Object as a child.
3) Setup the window in the inspector:
- "Bitdepth" controlls the amount of colors the RenderTexture can display which is used to render the video to ( usually 8 is sufficent )
- "VidLayerPrefab" should be preconfigured and not changed. If it is empty attach the "VideoLayer" prefab from the Prefabs folder here.
- "Video Path" corresponds to the subfolder in the Resources folder where you saved your videos. If you distributed them into more than one folder you can have multiple windows showing the content of different folders on the canvas.
- "Fade Duration" controls how long the fade between videos takes in seconds. If this is too long compared to the length of your shortest video you will get an error and the application might stop working correctly. As a rule of thumb the longest fadeDuration you can use is a bit less than half of the length of your shortest video file.
- "Fade Curve" allows you to control more precisely how the fade behaves. You can expirement with what looks the best to you here, however the curve must always start at ( 0 , 0 ) and end at ( 1 , 1 ).
