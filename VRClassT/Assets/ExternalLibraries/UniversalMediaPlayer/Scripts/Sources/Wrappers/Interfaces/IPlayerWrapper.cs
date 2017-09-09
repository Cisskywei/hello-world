using System;

namespace UMP.Wrappers
{
    interface IPlayerWrapper
    {
        void PlayerSetDataSource(IntPtr mpInstance, string path);
        bool PlayerPlay(IntPtr mpInstance);
        void PlayerPause(IntPtr mpInstance);
        void PlayerStop(IntPtr mpInstance);
        void PlayerRelease(IntPtr mpInstance);
        bool PlayerIsPlaying(IntPtr mpInstance);
        bool PlayerWillPlay(IntPtr mpInstance);
        long PlayerGetLength(IntPtr mpInstance);
        long PlayerGetTime(IntPtr mpInstance);
        void PlayerSetTime(IntPtr mpInstance, long time);
        float PlayerGetPosition(IntPtr mpInstance);
        void PlayerSetPosition(IntPtr mpInstance, float pos);
        float PlayerGetRate(IntPtr mpInstance);
        bool PlayerSetRate(IntPtr mpInstance, float rate);
        int PlayerGetVolume(IntPtr mpInstance);
        int PlayerSetVolume(IntPtr mpInstance, int volume);
        bool PlayerGetMute(IntPtr mpInstance);
        void PlayerSetMute(IntPtr mpInstance, bool mute);
        int PlayerVideoWidth(IntPtr mpInstance);
        int PlayerVideoHeight(IntPtr mpInstance);
        float PlayerVideoGetScale(IntPtr mpInstance);
        void PlayerVideoSetScale(IntPtr mpInstance, float factor);
        void PlayerVideoTakeSnapshot(IntPtr mpInstance, uint stream, string filePath, uint width, uint height);
        float PlayerVideoFrameRate(IntPtr mpInstance);
        int PlayerVideoFrameCount(IntPtr mpInstance);
        PlayerState PlayerGetState(IntPtr mpInstance);
        object PlayerGetStateValue(IntPtr mpInstance);
    }
}
