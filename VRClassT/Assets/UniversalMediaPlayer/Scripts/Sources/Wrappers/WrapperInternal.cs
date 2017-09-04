using System;
using System.Runtime.InteropServices;

namespace UMP.Wrappers
{
    internal class WrapperInternal : INativeWrapperHelper, IPlayerWrapper
    {
        public const string LIBRARY_NAME = "UniversalMediaPlayer";

        #region iOS/WebGL Imports
        [DllImport("__Internal")]
        private static extern IntPtr UMPNativeInit();

        [DllImport("__Internal")]
        private static extern void UMPNativeInitPlayer(IntPtr index, string options);

        [DllImport("__Internal")]
        private static extern void UMPNativeUpdateTexture(IntPtr index, IntPtr texture);

        [DllImport("__Internal")]
        private static extern IntPtr UMPNativeGetTexturePointer(IntPtr index);

        [DllImport("__Internal")]
        private static extern void UMPNativeSetPixelsBuffer(IntPtr index, IntPtr buffer, int width, int height);

        [DllImport("__Internal")]
        private static extern void UMPNativeUpdateFrameBuffer(IntPtr index);

        [DllImport("__Internal")]
        private static extern void UMPSetDataSource(IntPtr index, string path);

        [DllImport("__Internal")]
        private static extern bool UMPPlay(IntPtr index);

        [DllImport("__Internal")]
        private static extern void UMPPause(IntPtr index);

        [DllImport("__Internal")]
        private static extern void UMPStop(IntPtr index);

        [DllImport("__Internal")]
        private static extern void UMPRelease(IntPtr index);

        [DllImport("__Internal")]
        private static extern bool UMPIsPlaying(IntPtr index);

        [DllImport("__Internal")]
        private static extern bool UMPIsReady(IntPtr index);

        [DllImport("__Internal")]
        private static extern int UMPGetLength(IntPtr index);

        [DllImport("__Internal")]
        private static extern int UMPGetTime(IntPtr index);

        [DllImport("__Internal")]
        private static extern void UMPSetTime(IntPtr index, int time);

        [DllImport("__Internal")]
        private static extern float UMPGetPosition(IntPtr index);

        [DllImport("__Internal")]
        private static extern void UMPSetPosition(IntPtr index, float position);

        [DllImport("__Internal")]
        private static extern float UMPGetRate(IntPtr index);

        [DllImport("__Internal")]
        private static extern void UMPSetRate(IntPtr index, float rate);

        [DllImport("__Internal")]
        private static extern int UMPGetVolume(IntPtr index);

        [DllImport("__Internal")]
        private static extern void UMPSetVolume(IntPtr index, int volume);

        [DllImport("__Internal")]
        private static extern bool UMPGetMute(IntPtr index);

        [DllImport("__Internal")]
        private static extern void UMPSetMute(IntPtr index, bool state);

        [DllImport("__Internal")]
        private static extern int UMPVideoWidth(IntPtr index);

        [DllImport("__Internal")]
        private static extern int UMPVideoHeight(IntPtr index);

        [DllImport("__Internal")]
        private static extern int UMPVideoFrameCount(IntPtr index);

        [DllImport("__Internal")]
        private static extern int UMPGetState(IntPtr index);

        [DllImport("__Internal")]
        private static extern float UMPGetStateFloatValue(IntPtr index);

        [DllImport("__Internal")]
        private static extern long UMPGetStateLongValue(IntPtr index);

        [DllImport("__Internal")]
        private static extern IntPtr UMPGetStateStringValue(IntPtr index);
        #endregion

        #region Native Helper
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public string LibraryName
        {
            get
            {
                return LIBRARY_NAME;
            }
        }

        public int NativeHelperInit()
        {
            return (int)UMPNativeInit();
        }

        public void NativeHelperInitPlayer(IntPtr index, string options)
        {
            UMPNativeInitPlayer(index, options);
        }

        public void NativeHelperUpdateIndex(IntPtr index)
        {
        }

        public void NativeHelperSetTexture(IntPtr index, IntPtr texture)
        {
        }

        public IntPtr NativeHelperGetTexture(IntPtr index)
        {
            return UMPNativeGetTexturePointer(index);
        }

        public void NativeHelperUpdateTexture(IntPtr index, IntPtr texture)
        {
            UMPNativeUpdateTexture(index, texture);
        }

        public void NativeHelperSetPixelsBuffer(IntPtr mpInstance, IntPtr buffer, int width, int height)
        {
            UMPNativeSetPixelsBuffer(mpInstance, buffer, width, height);
        }

        public void NativeHelperUpdatePixelsBuffer(IntPtr mpInstance)
        {
            UMPNativeUpdateFrameBuffer(mpInstance);
        }

        public IntPtr NativeHelperGetAudioSamples(IntPtr samples, int length)
        {
            return IntPtr.Zero;
        }

        public IntPtr NativeHelperGetUnityRenderCallback()
        {
            return IntPtr.Zero;
        }

        public void NativeHelperSetUnityLogMessageCallback(IntPtr callback)
        {
        }
        #endregion

        #region Player
        public void PlayerSetDataSource(IntPtr index, string path)
        {
            UMPSetDataSource(index, path);
        }

        public bool PlayerPlay(IntPtr index)
        {
            return UMPPlay(index);
        }

        public void PlayerPause(IntPtr index)
        {
            UMPPause(index);
        }

        public void PlayerStop(IntPtr index)
        {
            UMPStop(index);
        }

        public void PlayerRelease(IntPtr index)
        {
            UMPRelease(index);
        }

        public bool PlayerIsPlaying(IntPtr index)
        {
            return UMPIsPlaying(index);
        }

        public bool PlayerWillPlay(IntPtr index)
        {
            return false;
        }

        public long PlayerGetLength(IntPtr index)
        {
            return UMPGetLength(index);
        }

        public long PlayerGetTime(IntPtr index)
        {
            return UMPGetTime(index);
        }

        public void PlayerSetTime(IntPtr index, long time)
        {
            UMPSetTime(index, (int)time);
        }

        public float PlayerGetPosition(IntPtr index)
        {
            return UMPGetPosition(index);
        }

        public void PlayerSetPosition(IntPtr index, float pos)
        {
            UMPSetPosition(index, pos);
        }

        public float PlayerGetRate(IntPtr index)
        {
            return UMPGetRate(index);
        }

        public bool PlayerSetRate(IntPtr index, float rate)
        {
            UMPSetRate(index, rate);
            return true;
        }

        public int PlayerGetVolume(IntPtr index)
        {
            return UMPGetVolume(index);
        }

        public int PlayerSetVolume(IntPtr index, int volume)
        {
            UMPSetVolume(index, volume);
            return 0;
        }

        public bool PlayerGetMute(IntPtr index)
        {
            return UMPGetMute(index);
        }

        public void PlayerSetMute(IntPtr index, bool mute)
        {
            UMPSetMute(index, mute);
        }

        public int PlayerVideoWidth(IntPtr index)
        {
            return UMPVideoWidth(index);
        }

        public int PlayerVideoHeight(IntPtr index)
        {
            return UMPVideoHeight(index);
        }

        public float PlayerVideoGetScale(IntPtr index)
        {
            return -1;
        }

        public void PlayerVideoSetScale(IntPtr index, float factor)
        {
        }

        public void PlayerVideoTakeSnapshot(IntPtr index, uint stream, string filePath, uint width, uint height)
        {
        }

        public float PlayerVideoFrameRate(IntPtr mpInstance)
        {
            return 0;
        }

        public int PlayerVideoFrameCount(IntPtr index)
        {
            return UMPVideoFrameCount(index);
        }

        public PlayerState PlayerGetState(IntPtr index)
        {
            return (PlayerState)UMPGetState(index);
        }

        public object PlayerGetStateValue(IntPtr mpInstance)
        {
            object value = UMPGetStateFloatValue(mpInstance);

            if ((float)value < 0)
            {
                value = UMPGetStateLongValue(mpInstance);
                if ((long)value < 0)
                    value = UMPGetStateStringValue(mpInstance);
            }

            return value;
        }
        #endregion
    }
}
