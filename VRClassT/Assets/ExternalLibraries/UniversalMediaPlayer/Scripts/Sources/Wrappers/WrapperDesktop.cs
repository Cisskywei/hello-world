using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UMP.Wrappers
{
    internal class Wrapper : INativeWrapperHelper, IPlayerWrapper, IPlayerWrapperExpanded, IPlayerWrapperAudio, IPlayerWrapperSpu
    {
        public const string LIBRARY_VLC_NAME = "libvlc";
        public const string LIBRARY_VLC_CORE_NAME = "libvlccore";

        private static Wrapper _instance;
        private object _wrapperObject;
        private INativeWrapperHelper _nativeWrapper;
        private IPlayerWrapper _playerWrapper;
        private IPlayerWrapperExpanded _playerWrapperExpanded;
        private IPlayerWrapperAudio _playerWrapperAudio;
        private IPlayerWrapperSpu _playerWrapperSpu;

        private Wrapper()
        {
            switch ((int)Application.platform)
            {
                case (int)RuntimePlatform.WindowsPlayer:
                case (int)RuntimePlatform.WindowsEditor:
                case (int)RuntimePlatform.OSXPlayer:
                case (int)RuntimePlatform.OSXEditor:
                case (int)RuntimePlatform.LinuxPlayer:
                case 16:
                    _wrapperObject = new WrapperStandalone();
                    break;

#if UNITY_IOS || UNITY_WEBGL
                case (int)RuntimePlatform.WebGLPlayer:
                    _wrapperObject = new WrapperInternal();
                    break;
#endif
            }

            if (_wrapperObject is INativeWrapperHelper)
                _nativeWrapper = (_wrapperObject as INativeWrapperHelper);

            if (_wrapperObject is IPlayerWrapper)
                _playerWrapper = (_wrapperObject as IPlayerWrapper);

            if (_wrapperObject is IPlayerWrapperExpanded)
                _playerWrapperExpanded = (_wrapperObject as IPlayerWrapperExpanded);

            if (_wrapperObject is IPlayerWrapperAudio)
                _playerWrapperAudio = (_wrapperObject as IPlayerWrapperAudio);

            if (_wrapperObject is IPlayerWrapperSpu)
                _playerWrapperSpu = (_wrapperObject as IPlayerWrapperSpu);
        }

        public static string LibraryVLCName
        {
            get
            {
                var libraryName = LIBRARY_VLC_NAME;

                if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Mac)
                    libraryName += ".5";
                return libraryName;
            }
        }

        public static string LibraryVLCCoreName
        {
            get
            {
                var libraryName = LIBRARY_VLC_CORE_NAME;

                if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Mac)
                    libraryName += ".8";
                return libraryName;
            }
        }

        public static Wrapper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Wrapper();
                return _instance;
            }
        }

        public object PlatformWrapper
        {
            get
            {
                return _wrapperObject;
            }
        }

        #region Native Helper
        public bool IsValid
        {
            get
            {
                return _nativeWrapper.IsValid;
            }
        }

        public string LibraryName
        {
            get
            {
                return _nativeWrapper.LibraryName;
            }
        }

        public int NativeHelperInit()
        {
            return _nativeWrapper.NativeHelperInit();
        }

        public void NativeHelperUpdateIndex(IntPtr mpInstance)
        {
            _nativeWrapper.NativeHelperUpdateIndex(mpInstance);
        }

        public void NativeHelperSetTexture(IntPtr mpInstance, IntPtr texture)
        {
            _nativeWrapper.NativeHelperSetTexture(mpInstance, texture);
        }

        public IntPtr NativeHelperGetTexture(IntPtr mpInstance)
        {
            return _nativeWrapper.NativeHelperGetTexture(mpInstance);
        }

        public void NativeHelperUpdateTexture(IntPtr mpInstance, IntPtr texture)
        {
            _nativeWrapper.NativeHelperUpdateTexture(mpInstance, texture);
        }

        public void NativeHelperSetPixelsBuffer(IntPtr mpInstance, IntPtr buffer, int width, int height)
        {
            _nativeWrapper.NativeHelperSetPixelsBuffer(mpInstance, buffer, width, height);
        }

        public void NativeHelperUpdatePixelsBuffer(IntPtr mpInstance)
        {
            _nativeWrapper.NativeHelperUpdatePixelsBuffer(mpInstance);
        }

        public IntPtr NativeHelperGetAudioSamples(IntPtr samples, int length)
        {
            return _nativeWrapper.NativeHelperGetAudioSamples(samples, length);
        }

        public IntPtr NativeHelperGetUnityRenderCallback()
        {
            return _nativeWrapper.NativeHelperGetUnityRenderCallback();
        }

        public void NativeHelperSetUnityLogMessageCallback(IntPtr callback)
        {
            _nativeWrapper.NativeHelperSetUnityLogMessageCallback(callback);
        }
        #endregion

        #region Player
        public void PlayerSetDataSource(IntPtr mpInstance, string path)
        {
            _playerWrapper.PlayerSetDataSource(mpInstance, path);
        }

        public bool PlayerPlay(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerPlay(mpInstance);
        }

        public void PlayerPause(IntPtr mpInstance)
        {
            _playerWrapper.PlayerPause(mpInstance);
        }

        public void PlayerStop(IntPtr mpInstance)
        {
            _playerWrapper.PlayerStop(mpInstance);
        }

        public void PlayerRelease(IntPtr mpInstance)
        {
            _playerWrapper.PlayerRelease(mpInstance);
        }

        public bool PlayerIsPlaying(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerIsPlaying(mpInstance);
        }

        public bool PlayerWillPlay(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerWillPlay(mpInstance);
        }

        public long PlayerGetLength(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerGetLength(mpInstance);
        }

        public long PlayerGetTime(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerGetTime(mpInstance);
        }

        public void PlayerSetTime(IntPtr mpInstance, long time)
        {
            _playerWrapper.PlayerSetTime(mpInstance, (int)time);
        }

        public float PlayerGetPosition(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerGetPosition(mpInstance);
        }

        public void PlayerSetPosition(IntPtr mpInstance, float pos)
        {
            _playerWrapper.PlayerSetPosition(mpInstance, pos);
        }

        public float PlayerGetRate(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerGetRate(mpInstance);
        }

        public bool PlayerSetRate(IntPtr mpInstance, float rate)
        {
            return _playerWrapper.PlayerSetRate(mpInstance, rate);
        }

        public int PlayerGetVolume(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerGetVolume(mpInstance);
        }

        public int PlayerSetVolume(IntPtr mpInstance, int volume)
        {
            return _playerWrapper.PlayerSetVolume(mpInstance, volume);
        }

        public bool PlayerGetMute(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerGetMute(mpInstance);
        }

        public void PlayerSetMute(IntPtr mpInstance, bool mute)
        {
            _playerWrapper.PlayerSetMute(mpInstance, mute);
        }

        public int PlayerVideoWidth(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerVideoWidth(mpInstance);
        }

        public int PlayerVideoHeight(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerVideoHeight(mpInstance);
        }

        public float PlayerVideoGetScale(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerVideoGetScale(mpInstance);
        }

        public void PlayerVideoSetScale(IntPtr mpInstance, float factor)
        {
            _playerWrapper.PlayerVideoSetScale(mpInstance, factor);
        }

        public void PlayerVideoTakeSnapshot(IntPtr mpInstance, uint stream, string filePath, uint width, uint height)
        {
            _playerWrapper.PlayerVideoTakeSnapshot(mpInstance, stream, filePath, width, height);
        }

        public float PlayerVideoFrameRate(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerVideoFrameRate(mpInstance);
        }

        public int PlayerVideoFrameCount(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerVideoFrameCount(mpInstance);
        }

        public PlayerState PlayerGetState(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerGetState(mpInstance);
        }

        public object PlayerGetStateValue(IntPtr mpInstance)
        {
            return _playerWrapper.PlayerGetStateValue(mpInstance);
        }
        #endregion

        #region Player Expanded
        public IntPtr PlayerExpandedLibVLCNew(string[] args)
        {
            if (_playerWrapperExpanded != null)
                return _playerWrapperExpanded.PlayerExpandedLibVLCNew(args);

            return IntPtr.Zero;
        }

        public void PlayerExpandedLibVLCRelease(IntPtr libVLCInst)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedLibVLCRelease(libVLCInst);
        }

        public IntPtr PlayerExpandedMediaNewLocation(IntPtr libVLCInst, string path)
        {
            if (_playerWrapperExpanded != null)
                return _playerWrapperExpanded.PlayerExpandedMediaNewLocation(libVLCInst, path);

            return IntPtr.Zero;
        }

        public void PlayerExpandedSetMedia(IntPtr mpInstance, IntPtr libVLCMediaInst)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedSetMedia(mpInstance, libVLCMediaInst);
        }

        public void PlayerExpandedAddOption(IntPtr libVLCMediaInstm, string option)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedAddOption(libVLCMediaInstm, option);
        }

        public TrackInfo[] PlayerExpandedMediaGetTracksInfo(IntPtr mpInstance)
        {
            if (_playerWrapperExpanded != null)
                return _playerWrapperExpanded.PlayerExpandedMediaGetTracksInfo(mpInstance);

            return null;
        }

        public void PlayerExpandedMediaRelease(IntPtr libVLCMediaInst)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedMediaRelease(libVLCMediaInst);
        }

        public IntPtr PlayerExpandedMediaPlayerNew(IntPtr libVLCInst)
        {
            if (_playerWrapperExpanded != null)
                return _playerWrapperExpanded.PlayerExpandedMediaPlayerNew(libVLCInst);

            return IntPtr.Zero;
        }

        public void PlayerExpandedFree(IntPtr inst)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedFree(inst);
        }

        public IntPtr PlayerExpandedEventManager(IntPtr mpInstance)
        {
            if (_playerWrapperExpanded != null)
                return _playerWrapperExpanded.PlayerExpandedEventManager(mpInstance);

            return IntPtr.Zero;
        }

        public int PlayerExpandedEventAttach(IntPtr eventManagerInst, EventTypes eventType, IntPtr callback, IntPtr userData)
        {
            if (_playerWrapperExpanded != null)
                return _playerWrapperExpanded.PlayerExpandedEventAttach(eventManagerInst, eventType, callback, userData);

            return -1;
        }

        public void PlayerExpandedEventDetach(IntPtr eventManagerInst, EventTypes eventType, IntPtr callback, IntPtr userData)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedEventDetach(eventManagerInst, eventType, callback, userData);
        }

        public void PlayerExpandedLogSet(IntPtr libVLC, IntPtr callback, IntPtr data)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedLogSet(libVLC, callback, data);
        }

        public void PlayerExpandedLogUnset(IntPtr libVLC)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedLogUnset(libVLC);
        }

        public void PlayerExpandedVideoSetCallbacks(IntPtr mpInstance, IntPtr @lock, IntPtr unlock, IntPtr display, IntPtr opaque)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedVideoSetCallbacks(mpInstance, @lock, unlock, display, opaque);
        }

        public void PlayerExpandedVideoSetFormatCallbacks(IntPtr mpInstance, IntPtr setup, IntPtr cleanup)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedVideoSetFormatCallbacks(mpInstance, setup, cleanup);
        }

        public void PlayerExpandedVideoSetFormat(IntPtr mpInstance, string chroma, uint width, uint height, uint pitch)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedVideoSetFormat(mpInstance, chroma, width, height, pitch);
        }

        public void PlayerExpandedAudioSetCallbacks(IntPtr mpInstance, IntPtr play, IntPtr pause, IntPtr resume, IntPtr flush, IntPtr drain, IntPtr opaque)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedAudioSetCallbacks(mpInstance, play, pause, resume, flush, drain, opaque);
        }

        public void PlayerExpandedAudioSetFormatCallbacks(IntPtr mpInstance, IntPtr setup, IntPtr cleanup)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedAudioSetFormatCallbacks(mpInstance, setup, cleanup);
        }

        public void PlayerExpandedAudioSetFormat(IntPtr mpInstance, string format, int rate, int channels)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedAudioSetFormat(mpInstance, format, rate, channels);
        }

        public long PlayerExpandedGetAudioDelay(IntPtr mpInstance)
        {
            if (_playerWrapperExpanded != null)
                return _playerWrapperExpanded.PlayerExpandedGetAudioDelay(mpInstance);

            return 0;
        }

        public void PlayerExpandedSetAudioDelay(IntPtr mpInstance, long channel)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedSetAudioDelay(mpInstance, channel);
        }

        public int PlayerExpandedAudioOutputSet(IntPtr mpInstance, [MarshalAs(UnmanagedType.LPStr)] string psz_name)
        {
            if (_playerWrapperExpanded != null)
                return _playerWrapperExpanded.PlayerExpandedAudioOutputSet(mpInstance, psz_name);

            return -1;
        }

        public IntPtr PlayerExpandedAudioOutputListGet(IntPtr mpInstance)
        {
            if (_playerWrapperExpanded != null)
                return _playerWrapperExpanded.PlayerExpandedAudioOutputListGet(mpInstance);

            return IntPtr.Zero;
        }

        public void PlayerExpandedAudioOutputListRelease(IntPtr outputListInst)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedAudioOutputListRelease(outputListInst);
        }

        public void PlayerExpandedAudioOutputDeviceSet(IntPtr mpInstance, [MarshalAs(UnmanagedType.LPStr)] string psz_audio_output, [MarshalAs(UnmanagedType.LPStr)] string psz_device_id)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedAudioOutputDeviceSet(mpInstance, psz_audio_output, psz_device_id);
        }

        public IntPtr PlayerExpandedAudioOutputDeviceListGet(IntPtr mpInstance, [MarshalAs(UnmanagedType.LPStr)] string aout)
        {
            if (_playerWrapperExpanded != null)
                return _playerWrapperExpanded.PlayerExpandedAudioOutputDeviceListGet(mpInstance, aout);

            return IntPtr.Zero;
        }

        public void PlayerExpandedAudioOutputDeviceListRelease(IntPtr deviceListInst)
        {
            if (_playerWrapperExpanded != null)
                _playerWrapperExpanded.PlayerExpandedAudioOutputDeviceListRelease(deviceListInst);
        }
        #endregion

        #region Player Audio
        public int PlayerAudioGetTrackCount(IntPtr mpInstance)
        {
            if (_playerWrapperAudio != null)
                return _playerWrapperAudio.PlayerAudioGetTrackCount(mpInstance);

            return 0;
        }

        public IntPtr PlayerAudioGetTrackDescription(IntPtr mpInstance)
        {
            if (_playerWrapperAudio != null)
                return _playerWrapperAudio.PlayerAudioGetTrackDescription(mpInstance);

            return IntPtr.Zero;
        }

        public void PlayerAudioTrackDescriptionRelease(IntPtr trackDescription)
        {
            if (_playerWrapperAudio != null)
                _playerWrapperAudio.PlayerAudioTrackDescriptionRelease(trackDescription);
        }

        public int PlayerAudioGetTrack(IntPtr mpInstance)
        {
            if (_playerWrapperAudio != null)
                return _playerWrapperAudio.PlayerAudioGetTrack(mpInstance);

            return 0;
        }

        public int PlayerAudioSetTrack(IntPtr mpInstance, int audioIndex)
        {
            if (_playerWrapperAudio != null)
                return _playerWrapperAudio.PlayerAudioSetTrack(mpInstance, audioIndex);

            return 0;
        }
        #endregion

        #region Player Spu
        public int PlayerSpuGetCount(IntPtr mpInstance)
        {
            if (_playerWrapperSpu != null)
                return _playerWrapperSpu.PlayerSpuGetCount(mpInstance);

            return 0;
        }

        public IntPtr PlayerSpuGetDescription(IntPtr mpInstance)
        {
            if (_playerWrapperSpu != null)
                return _playerWrapperSpu.PlayerSpuGetDescription(mpInstance);

            return IntPtr.Zero;
        }

        public int PlayerSpuGet(IntPtr mpInstance)
        {
            if (_playerWrapperSpu != null)
                return _playerWrapperSpu.PlayerSpuGet(mpInstance);

            return 0;
        }

        public int PlayerSpuSet(IntPtr mpInstance, int spuIndex)
        {
            if (_playerWrapperSpu != null)
                return _playerWrapperSpu.PlayerSpuSet(mpInstance, spuIndex);

            return 0;
        }

        public int PlayerSpuSetFile(IntPtr mpInstance, string path)
        {
            if (_playerWrapperSpu != null)
                return _playerWrapperSpu.PlayerSpuSetFile(mpInstance, path);

            return 0;
        }
        #endregion
    }
}
