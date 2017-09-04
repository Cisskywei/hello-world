using System;

namespace UMP.Wrappers
{
    interface IPlayerWrapperAudio
    {
        int PlayerAudioGetTrackCount(IntPtr mpInstance);
        IntPtr PlayerAudioGetTrackDescription(IntPtr mpInstance);
        void PlayerAudioTrackDescriptionRelease(IntPtr trackDescription);
        int PlayerAudioGetTrack(IntPtr mpInstance);
        int PlayerAudioSetTrack(IntPtr mpInstance, int audioIndex);
    }
}
