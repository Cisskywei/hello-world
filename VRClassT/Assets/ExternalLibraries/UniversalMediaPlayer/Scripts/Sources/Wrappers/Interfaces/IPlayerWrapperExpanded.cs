using System;
using System.Runtime.InteropServices;

namespace UMP.Wrappers
{
    interface IPlayerWrapperExpanded : IPlayerWrapper
    {
        IntPtr PlayerExpandedLibVLCNew(string[] args);
        void PlayerExpandedLibVLCRelease(IntPtr libVLCInst);
        IntPtr PlayerExpandedMediaNewLocation(IntPtr libVLCInst, string path);
        void PlayerExpandedSetMedia(IntPtr mpInstance, IntPtr libVLCMediaInst);
        void PlayerExpandedAddOption(IntPtr libVLCMediaInst, string option);
        // TODO Move to IPlayerWrapper
        TrackInfo[] PlayerExpandedMediaGetTracksInfo(IntPtr mpInstance);
        void PlayerExpandedMediaRelease(IntPtr libVLCMediaInst);
        IntPtr PlayerExpandedMediaPlayerNew(IntPtr libVLCInst);

        void PlayerExpandedFree(IntPtr inst);
        IntPtr PlayerExpandedEventManager(IntPtr mpInstance);
        int PlayerExpandedEventAttach(IntPtr eventManagerInst, EventTypes eventType, IntPtr callback, IntPtr userData);
        void PlayerExpandedEventDetach(IntPtr eventManagerInst, EventTypes eventType, IntPtr callback, IntPtr userData);

        void PlayerExpandedLogSet(IntPtr libVLC, IntPtr callback, IntPtr data);
        void PlayerExpandedLogUnset(IntPtr libVLC);

        void PlayerExpandedVideoSetCallbacks(IntPtr mpInstance, IntPtr @lock, IntPtr unlock, IntPtr display, IntPtr opaque);
        void PlayerExpandedVideoSetFormatCallbacks(IntPtr mpInstance, IntPtr setup, IntPtr cleanup);
        void PlayerExpandedVideoSetFormat(IntPtr mpInstance, string chroma, uint width, uint height, uint pitch);

        void PlayerExpandedAudioSetCallbacks(IntPtr mpInstance, IntPtr play, IntPtr pause, IntPtr resume, IntPtr flush, IntPtr drain, IntPtr opaque);
        void PlayerExpandedAudioSetFormatCallbacks(IntPtr mpInstance, IntPtr setup, IntPtr cleanup);
        void PlayerExpandedAudioSetFormat(IntPtr mpInstance, string format, int rate, int channels);

        long PlayerExpandedGetAudioDelay(IntPtr mpInstance);
        void PlayerExpandedSetAudioDelay(IntPtr mpInstance, long channel);

        int PlayerExpandedAudioOutputSet(IntPtr mpInstance, string psz_name);
        IntPtr PlayerExpandedAudioOutputListGet(IntPtr mpInstance);
        void PlayerExpandedAudioOutputListRelease(IntPtr outputListInst);

        void PlayerExpandedAudioOutputDeviceSet(IntPtr mpInstance, string psz_audio_output, string psz_device_id);
        IntPtr PlayerExpandedAudioOutputDeviceListGet(IntPtr mpInstance, string aout);
        void PlayerExpandedAudioOutputDeviceListRelease(IntPtr deviceListInst);
    }
}
