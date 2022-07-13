# SFX Explorer

## What?
This tool will help you quickly browse through sound effects to pinpoint the ones you need.

### For who?
Anyone who needs to find specific audio files from huge collections. Game developers, video editors,..

### Limitations
Right now it only supports wav file. No mp3, no ogg, no flac...

### What does it do?
It lets you add a folder containing subfolders and audio files.
You can then browse files using keyboard navigation. If you have autoplay checked, it will play each audio file immediately.

![1-keyboard nav](https://user-images.githubusercontent.com/1586515/178749559-d33252af-234d-4e51-bc5e-df94abed1302.gif)

You can filter files based on their full path.

![3-search](https://user-images.githubusercontent.com/1586515/178749594-383121db-e052-4a96-9616-22b3bf4cc2a6.gif)

Once you've found your perfect SFX, just drag it to Unity/Audacity/whatever.

![2-drag-drop](https://user-images.githubusercontent.com/1586515/178748573-a0704856-f3f6-4c43-bc9f-cfe6317c21df.gif)


### Roadmap?
Many things actually :

- search/filter (mvvm + non-hammer + diacritics + several words)
- show file icons (lazy load)
- show file duration (lazy load)
- auto open last folder
- file > open recent folders
- volume control
- show only audio files checkbox
- right click (open folder, open file, copy path, copy file, open with... (add softwares to list))
- copy to... (add folders to list)
- improve audio file compatibility (more wav, mp3, ogg, flac,..)
- save folders to local storage
- panel with sound preview, play button and other data (mono/stereo, file size, etc..)
- short list feature

