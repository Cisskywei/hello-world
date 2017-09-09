using System;

namespace UMP.Wrappers
{
    interface IPlayerWrapperSpu
    {
        int PlayerSpuGetCount(IntPtr mpInstance);
        IntPtr PlayerSpuGetDescription(IntPtr mpInstance);
        int PlayerSpuGet(IntPtr mpInstance);
        int PlayerSpuSet(IntPtr mpInstance, int spuIndex);
        int PlayerSpuSetFile(IntPtr mpInstance, string path);
    }
}
