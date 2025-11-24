#Persistent
SetTimer, CheckAEAudio, 200
LastState := 0

CheckAEAudio:
    FileRead, state, C:\temp\ae_audio.txt
    state := Trim(state)

    if (state = "1" and LastState = "0")
    {
        ; AE started playing audio → pause Spotify
        SendMediaKey("pause")
    }
    else if (state = "0" and LastState = "1")
    {
        ; AE stopped playing audio → resume Spotify
        SendMediaKey("play")
    }

    LastState := state
return


SendMediaKey(key)
{
    ; Use Spotify keyboard media keys
    if (key = "pause" or key = "play")
        Send {Media_Play_Pause}
}
