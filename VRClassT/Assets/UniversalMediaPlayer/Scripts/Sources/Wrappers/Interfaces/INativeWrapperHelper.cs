using System;

namespace UMP.Wrappers
{
    interface INativeWrapperHelper
    {
        bool IsValid { get; }
        string LibraryName { get; }

        int NativeHelperInit();
        void NativeHelperUpdateIndex(IntPtr mpInstance);

        void NativeHelperSetTexture(IntPtr mpInstance, IntPtr texture);
        IntPtr NativeHelperGetTexture(IntPtr mpInstance);
        void NativeHelperUpdateTexture(IntPtr mpInstance, IntPtr texture);
        void NativeHelperSetPixelsBuffer(IntPtr mpInstance, IntPtr buffer, int width, int height);
        void NativeHelperUpdatePixelsBuffer(IntPtr mpInstance);
        IntPtr NativeHelperGetAudioSamples(IntPtr samples, int length);
        IntPtr NativeHelperGetUnityRenderCallback();
        void NativeHelperSetUnityLogMessageCallback(IntPtr callback);
    }
}