using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace UMP.Wrappers
{
    [SuppressUnmanagedCodeSecurity]
    internal class WrapperStandalone : INativeWrapperHelper, IPlayerWrapperExpanded, IPlayerWrapperSpu, IPlayerWrapperAudio
    {
        public const string LIBRARY_NAME = "UniversalMediaPlayer";

        private IntPtr _libVLCCoreHandler = IntPtr.Zero;
        private IntPtr _libVLCHandler = IntPtr.Zero;
        private IntPtr _libUMPHandler = IntPtr.Zero;

        private delegate void ManageLogCallback(string msg);
        private ManageLogCallback _manageLogCallback;

        #region UMP Imports
        [DllImport(LIBRARY_NAME)]
        private static extern int UMPNativeInit();

        [DllImport(LIBRARY_NAME)]
        private static extern void UMPNativeUpdateIndex(int mpInstance);

        [DllImport(LIBRARY_NAME)]
        private static extern void UMPNativeSetTexture(int mpInstance, IntPtr texture);

        [DllImport(LIBRARY_NAME)]
        private static extern void UMPNativeSetPixelsBuffer(int mpInstance, IntPtr buffer, int width, int height);

        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr UMPNativeGetPixelsBuffer(int mpInstance);

        [DllImport(LIBRARY_NAME)]
        private static extern int UMPNativeGetPixelsBufferWidth(int mpInstance);

        [DllImport(LIBRARY_NAME)]
        private static extern int UMPNativeGetPixelsBufferHeight(int mpInstance);

        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr UMPNativeGetLogMessage(int mpInstance);

        [DllImport(LIBRARY_NAME)]
        private static extern int UMPNativeGetLogLevel(int mpInstance);

        [DllImport(LIBRARY_NAME)]
        private static extern int UMPNativeGetState(int mpInstance);

        [DllImport(LIBRARY_NAME)]
        private static extern float UMPNativeGetStateFloatValue(int mpInstance);

        [DllImport(LIBRARY_NAME)]
        private static extern long UMPNativeGetStateLongValue(int mpInstance);

        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr UMPNativeGetStateStringValue(int mpInstance);

        [DllImport(LIBRARY_NAME)]
        private static extern int UMPNativeSetPixelsVerticalFlip(int mpInstance, bool flip);

        [DllImport(LIBRARY_NAME)]
        internal static extern IntPtr UMPNativeGetAudioSamples(IntPtr samples, int length);

        [DllImport(LIBRARY_NAME)]
        internal static extern void UMPNativeDirectRender(int mpInstance);

        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr UMPNativeGetUnityRenderCallback();

        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr UMPNativeGetVideoLockCallback();

        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr UMPNativeGetVideoDisplayCallback();

        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr UMPNativeGetVideoFormatCallback();

        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr UMPNativeGetVideoCleanupCallback();

        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr UMPNativeGetAudioPlayCallback();

        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr UMPNativeMediaPlayerEventCallback();

        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr UMPNativeGetLogMessageCallback();

        [DllImport(LIBRARY_NAME)]
        private static extern void UMPNativeSetUnityLogMessageCallback(IntPtr callback);

        [DllImport(LIBRARY_NAME)]
        private static extern void UMPNativeUpdateFramesCounter(int mpInstance, int counter);

        [DllImport(LIBRARY_NAME)]
        private static extern int UMPNativeGetFramesCounter(int mpInstance);
        #endregion

        #region UMP Windows, Linux delegates
        [InteropFunction("UMPNativeInit")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeInit();

        [InteropFunction("UMPNativeUpdateIndex")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void NativeUpdateIndex(int mpInstance);

        [InteropFunction("UMPNativeSetTexture")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void NativeSetTexture(int mpInstance, IntPtr texture);

        [InteropFunction("UMPNativeSetPixelsBuffer")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void NativeSetPixelsBuffer(int mpInstance, IntPtr buffer, int width, int height);

        [InteropFunction("UMPNativeGetPixeslBuffer")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr NativeGetPixelsBuffer(int mpInstance);

        [InteropFunction("UMPNativeGetPixelsBufferWidth")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeGetPixelsBufferWidth(int mpInstance);

        [InteropFunction("UMPNativeGetPixelsBufferHeight")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeGetPixelsBufferHeight(int mpInstance);

        [InteropFunction("UMPNativeGetLogMessage")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr NativeGetLogMessage(int mpInstance);

        [InteropFunction("UMPNativeGetLogLevel")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeGetLogLevel(int mpInstance);

        [InteropFunction("UMPNativeGetState")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeGetState(int mpInstance);

        [InteropFunction("UMPNativeGetStateFloatValue")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate float NativeGetStateFloatValue(int mpInstance);

        [InteropFunction("UMPNativeGetStateLongValue")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate long NativeGetStateLongValue(int mpInstance);

        [InteropFunction("UMPNativeGetStateStringValue")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr NativeGetStateStringValue(int mpInstance);

        [InteropFunction("UMPNativeSetPixelsVerticalFlip")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void NativeSetPixelsVerticalFlip(int mpInstance, bool flip);

        [InteropFunction("UMPNativeDecodeAudioSamples")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr NativeGetAudioSamples(IntPtr samples, int length);

        [InteropFunction("UMPNativeDirectRender")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void NativeDirectRender(int mpInstance);

        [InteropFunction("UMPNativeGetUnityRenderCallback")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr NativeGetUnityRenderCallback();

        [InteropFunction("UMPNativeGetVideoLockCallback")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr NativeGetVideoLockCallback();

        [InteropFunction("UMPNativeGetVideoDisplayCallback")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr NativeGetVideoDisplayCallback();

        [InteropFunction("UMPNativeGetVideoFormatCallback")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr NativeGetVideoFormatCallback();

        [InteropFunction("UMPNativeGetVideoCleanupCallback")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr NativeGetVideoCleanupCallback();

        [InteropFunction("UMPNativeGetAudioPlayCallback")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr NativeGetAudioPlayCallback();

        [InteropFunction("UMPNativeMediaPlayerEventCallback")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr NativeMediaPlayerEventCallback();

        [InteropFunction("UMPNativeGetLogMessageCallback")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr NativeGetLogMessageCallback();

        /*[InteropFunction("UMPNativeSetLibLogMessageCallback")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void NativeSetLibLogMessageCallback(int mpInstance, IntPtr callback);*/

        [InteropFunction("UMPNativeSetUnityLogMessageCallback")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void NativeSetUnityLogMessageCallback(IntPtr callback);

        [InteropFunction("UMPNativeUpdateFramesCounter")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void NativeUpdateFramesCounterCallback(int mpInstance, int counter);

        [InteropFunction("UMPNativeGetFramesCounter")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeGetFramesCounterCallback(int mpInstance);
        #endregion

        #region LibVLC Windows, Mac & Linux delegates
        /// <summary>
        /// Set current crop filter geometry.
        /// </summary>
        [InteropFunction("libvlc_video_set_aspect_ratio")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetVideoAspectRatio(IntPtr mpInstance, string cropGeometry);

        /// <summary>
        /// Unset libvlc log instance.
        /// </summary>
        [InteropFunction("libvlc_log_unset")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void LogUnset(IntPtr instance);

        /// <summary>
        /// Unset libvlc log instance.
        /// </summary>
        [InteropFunction("libvlc_log_set")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void LogSet(IntPtr instance, IntPtr callback, IntPtr data);

        /// <summary>
        /// Create and initialize a libvlc instance.
        /// </summary>
        /// <returns>Return the libvlc instance or NULL in case of error.</returns>
        [InteropFunction("libvlc_new")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr CreateNewInstance(int argc, string[] argv);

        /// <summary>
        /// Destroy libvlc instance.
        /// </summary>
        [InteropFunction("libvlc_release")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ReleaseInstance(IntPtr instance);

        /// <summary>
        /// Frees an heap allocation returned by a LibVLC function.
        /// </summary>
        /// <returns>Return the libvlc instance or NULL in case of error.</returns>
        [InteropFunction("libvlc_free")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void Free(IntPtr ptr);

        /// <summary>
        /// CCallback function notification.
        /// </summary>
        [InteropFunction("libvlc_callback_t")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void EventCallback(IntPtr args);

        /// <summary>
        /// Register for an event notification.
        /// </summary>
        /// <param name="eventManagerInstance">Event manager to which you want to attach to.</param>
        /// <param name="eventType">The desired event to which we want to listen.</param>
        /// <param name="callback">The function to call when i_event_type occurs.</param>
        /// <param name="userData">User provided data to carry with the event.</param>
        /// <returns>Return 0 on success, ENOMEM on error.</returns>
        [InteropFunction("libvlc_event_attach")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int AttachEvent(IntPtr eventManagerInstance, EventTypes eventType, IntPtr callback, IntPtr userData);

        /// <summary>
        /// Unregister an event notification.
        /// </summary>
        /// <param name="eventManagerInstance">Event manager to which you want to attach to.</param>
        /// <param name="eventType">The desired event to which we want to listen.</param>
        /// <param name="callback">The function to call when i_event_type occurs.</param>
        /// <param name="userData">User provided data to carry with the event.</param>
        /// <returns>Return 0 on success, ENOMEM on error.</returns>
        [InteropFunction("libvlc_event_detach")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int DetachEvent(IntPtr eventManagerInstance, EventTypes eventType, IntPtr callback, IntPtr userData);

        /// <summary>
        /// Create a media with a certain given media resource location, for instance a valid URL.
        /// </summary>
        /// <returns>Return the newly created media or NULL on error.</returns>
        [InteropFunction("libvlc_media_new_location")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr CreateNewMediaFromLocation(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string path);

        /// <summary>
        /// Create a media for a certain file path.
        /// </summary>
        /// <returns>Return the newly created media or NULL on error.</returns>
        [InteropFunction("libvlc_media_new_path")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr CreateNewMediaFromPath(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string path);

        /// <summary>
        /// Add an option to the media.
        /// </summary>
        [InteropFunction("libvlc_media_add_option")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void AddOptionToMedia(IntPtr mediaInstance, [MarshalAs(UnmanagedType.LPStr)] string option);

        /// <summary>
        /// It will release the media descriptor object. It will send out an libvlc_MediaFreed event to all listeners. If the media descriptor object has been released it should not be used again.
        /// </summary>
        [InteropFunction("libvlc_media_release")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ReleaseMedia(IntPtr mediaInstance);

        /// <summary>
        /// Get the media resource locator (mrl) from a media descriptor object.
        /// </summary>
        [InteropFunction("libvlc_media_get_mrl")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr GetMediaMrl(IntPtr mediaInstance);

        /// <summary>
        /// Read a meta of the media.
        /// </summary>
        [InteropFunction("libvlc_media_get_meta")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr GetMediaMetadata(IntPtr mediaInstance, MediaMetadatas meta);

        /// <summary>
        /// Parse a media meta data and tracks information. 
        /// </summary>
        [InteropFunction("libvlc_media_parse")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ParseMedia(IntPtr mediaInstance);

        /// <summary>
        /// Parse a media meta data and tracks information async. 
        /// </summary>
        [InteropFunction("libvlc_media_parse_async")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ParseMediaAsync(IntPtr mediaInstance);

        /// <summary>
        /// Get Parsed status for media descriptor object.
        /// </summary>
        [InteropFunction("libvlc_media_is_parsed")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int IsParsedMedia(IntPtr mediaInstance);

        /// <summary>
        /// Get media descriptor's elementary streams description.
        /// </summary>
        [InteropFunction("libvlc_media_get_tracks_info")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GetMediaTracksInformations(IntPtr mediaInstance, out IntPtr tracksInformationsPointer);

        /// <summary>
        /// Create an empty Media Player object.
        /// </summary>
        /// <returns>Return a new media player object, or NULL on error.</returns>
        [InteropFunction("libvlc_media_player_new")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr CreateMediaPlayer(IntPtr instance);

        /// <summary>
        /// It will release the media player object. If the media player object has been released, then it should not be used again.
        /// </summary>
        [InteropFunction("libvlc_media_player_release")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ReleaseMediaPlayer(IntPtr mpInstance);

        /// <summary>
        /// Set the media that will be used by the media_player. If any, previous media will be released.
        /// </summary>
        /// <returns>Return a new media player object, or NULL on error.</returns>
        [InteropFunction("libvlc_media_player_set_media")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetMediaToMediaPlayer(IntPtr mpInstance, IntPtr mediaInstance);

        /// <summary>
        /// Get the Event Manager from which the media player send event.
        /// </summary>
        /// <returns>Return the event manager associated with media player.</returns>
        [InteropFunction("libvlc_media_player_event_manager")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr GetMediaPlayerEventManager(IntPtr mpInstance);

        /// <summary>
        /// Check if media player is playing.
        /// </summary>
        [InteropFunction("libvlc_media_player_is_playing")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int IsPlaying(IntPtr mpInstance);

        /// <summary>
        /// Play.
        /// </summary>
        /// <returns>Return 0 if playback started (and was already started), or -1 on error.</returns>
        [InteropFunction("libvlc_media_player_play")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int Play(IntPtr mpInstance);

        /// <summary>
        /// Toggle pause (no effect if there is no media).
        /// </summary>
        [InteropFunction("libvlc_media_player_pause")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void Pause(IntPtr mpInstance);

        /// <summary>
        /// Stop.
        /// </summary>
        [InteropFunction("libvlc_media_player_stop")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void Stop(IntPtr mpInstance);

        /// <summary>
        /// Set video callbacks.
        /// </summary>
        [InteropFunction("libvlc_video_set_callbacks")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetVideoCallbacks(IntPtr mpInstance, IntPtr @lock, IntPtr unlock, IntPtr display, IntPtr opaque);

        /// <summary>
        /// Set video format.
        /// </summary>
        [InteropFunction("libvlc_video_set_format")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetVideoFormat(IntPtr mpInstance, [MarshalAs(UnmanagedType.LPStr)] string chroma, uint width, uint height, uint pitch);

        /// <summary>
        /// Set video format callbacks.
        /// </summary>
        [InteropFunction("libvlc_video_set_format_callbacks")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetVideoFormatCallbacks(IntPtr mpInstance, IntPtr setup, IntPtr cleanup);

        /// <summary>
        /// Get length of movie playing
        /// </summary>
        /// <returns>Get the requested movie play rate.</returns>
        [InteropFunction("libvlc_media_player_get_length")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate long GetLength(IntPtr mpInstance);

        /// <summary>
        /// Get Rate at which movie is playing.
        /// </summary>
        /// <returns>Get the requested movie play rate.</returns>
        [InteropFunction("libvlc_media_player_get_time")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate long GetTime(IntPtr mpInstance);

        /// <summary>
        /// Get time at which movie is playing.
        /// </summary>
        /// <returns>Get the requested movie play time.</returns>
        [InteropFunction("libvlc_media_player_set_time")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetTime(IntPtr mpInstance, long time);

        /// <summary>
        /// Get media position.
        /// </summary>
        [InteropFunction("libvlc_media_player_get_position")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate float GetMediaPosition(IntPtr mpInstance);

        /// <summary>
        /// Get media position.
        /// </summary>
        [InteropFunction("libvlc_media_player_set_position")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

        private delegate void SetMediaPosition(IntPtr mpInstance, float position);

        /// <summary>
        /// Is the player able to play.
        /// </summary>
        [InteropFunction("libvlc_media_player_will_play")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CouldPlay(IntPtr mpInstance);

        /// <summary>
        /// Get the requested media play rate.
        /// </summary>
        [InteropFunction("libvlc_media_player_get_rate")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate float GetRate(IntPtr mpInstance);

        /// <summary>
        /// Set the requested media play rate.
        /// </summary>
        [InteropFunction("libvlc_media_player_set_rate")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SetRate(IntPtr mpInstance, float rate);

        /// <summary>
        /// Get the media state.
        /// </summary>
        [InteropFunction("libvlc_media_player_get_state")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate MediaStates GetMediaPlayerState(IntPtr mpInstance);

        /// <summary>
        /// Get media fps rate.
        /// </summary>
        [InteropFunction("libvlc_media_player_get_fps")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate float GetFramesPerSecond(IntPtr mpInstance);

        /// <summary>
        /// Get video size in pixels.
        /// </summary>
        [InteropFunction("libvlc_video_get_size")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GetVideoSize(IntPtr mpInstance, uint num, out uint px, out uint py);

        /// <summary>
        /// Get video scale.
        /// </summary>
        [InteropFunction("libvlc_video_get_scale")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate float GetVideoScale(IntPtr mpInstance);

        /// <summary>
        /// Set video scale.
        /// </summary>
        [InteropFunction("libvlc_video_set_scale")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate float SetVideoScale(IntPtr mpInstance, float f_factor);

        /// <summary>
        /// Take a snapshot of the current video window.
        /// </summary>
        /// <returns>Return 0 on success, -1 if the video was not found.</returns>
        [InteropFunction("libvlc_video_take_snapshot")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int TakeSnapshot(IntPtr mpInstance, uint num, string fileName, uint width, uint height);

        #region Spu setup
        /// <summary>
        /// Get the number of available video subtitles.
        /// </summary>
        [InteropFunction("libvlc_video_get_spu_count")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GetVideoSpuCount(IntPtr mpInstance);

        /// <summary>
        /// Get the description of available video subtitles.
        /// </summary>
        [InteropFunction("libvlc_video_get_spu_description")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr GetVideoSpuDescription(IntPtr mpInstance);

        /// <summary>
        /// Get current video subtitle.
        /// </summary>
        [InteropFunction("libvlc_video_get_spu")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

        private delegate int GetVideoSpu(IntPtr mpInstance);

        /// <summary>
        /// Set new video subtitle.
        /// </summary>
        [InteropFunction("libvlc_video_set_spu")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SetVideoSpu(IntPtr mpInstance, int spu);

        /// <summary>
        /// Set new video subtitle file.
        /// </summary>
        [InteropFunction("libvlc_video_set_subtitle_file")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SetVideoSubtitleFile(IntPtr mpInstance, [MarshalAs(UnmanagedType.LPStr)] string path);
        #endregion

        #region Audio setup
        /// <summary>
        /// Set audio format.
        /// </summary>
        [InteropFunction("libvlc_audio_set_format")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetAudioFormat(IntPtr mpInstance, [MarshalAs(UnmanagedType.LPStr)] string format, int rate, int channels);

        /// <summary>
        /// Set audio callbacks.
        /// </summary>
        [InteropFunction("libvlc_audio_set_callbacks")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetAudioCallbacks(IntPtr mpInstance, IntPtr play, IntPtr pause, IntPtr resume, IntPtr flush, IntPtr drain, IntPtr opaque);

        /// <summary>
        /// Set audio format callbacks.
        /// </summary>
        [InteropFunction("libvlc_audio_set_format_callbacks")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetAudioFormatCallbacks(IntPtr mpInstance, IntPtr setup, IntPtr cleanup);

        /// <summary>
        /// Get number of available audio tracks.
        /// </summary>
        [InteropFunction("libvlc_audio_get_track_count")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GetAudioTracksCount(IntPtr mpInstance);

        /// <summary>
        /// Get the description of available audio tracks.
        /// </summary>
        [InteropFunction("libvlc_audio_get_track_description")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr GetAudioTracksDescriptions(IntPtr mpInstance);

        /// <summary>
        /// Relase the description of available track.
        /// </summary>
        [InteropFunction("libvlc_track_description_release")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr ReleaseTracksDescriptions(IntPtr trackDescription);

        /// <summary>
        /// Get current audio track.
        /// </summary>
        [InteropFunction("libvlc_audio_get_track")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GetAudioTrack(IntPtr mpInstance);

        /// <summary>
        /// Set audio track.
        /// </summary>
        [InteropFunction("libvlc_audio_set_track")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SetAudioTrack(IntPtr mpInstance, int trackId);

        /// <summary>
        /// Get current audio delay.
        /// </summary>
        [InteropFunction("libvlc_audio_get_delay")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate long GetAudioDelay(IntPtr mpInstance);

        /// <summary>
        /// Set current audio delay. The audio delay will be reset to zero each time the media changes.
        /// </summary>
        [InteropFunction("libvlc_audio_set_delay")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetAudioDelay(IntPtr mpInstance, long channel);

        /// <summary>
        /// Get current software audio volume.
        /// </summary>
        [InteropFunction("libvlc_audio_get_volume")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GetVolume(IntPtr mpInstance);

        /// <summary>
        /// Set current software audio volume.
        /// </summary>
        [InteropFunction("libvlc_audio_set_volume")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SetVolume(IntPtr mpInstance, int volume);

        /// <summary>
        /// Set current mute status.
        /// </summary>
        [InteropFunction("libvlc_audio_set_mute")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetMute(IntPtr mpInstance, int status);

        /// <summary>
        /// Get current mute status.
        /// </summary>
        [InteropFunction("libvlc_audio_get_mute")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int IsMute(IntPtr mpInstance);

        /// <summary>
        /// Get the list of available audio outputs.
        /// </summary>
        [InteropFunction("libvlc_audio_output_list_get")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr GetAudioOutputsDescriptions(IntPtr instance);

        /// <summary>
        /// It will release the list of available audio outputs.
        /// </summary>
        [InteropFunction("libvlc_audio_output_list_release")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ReleaseAudioOutputDescription(IntPtr audioOutput);

        /// <summary>
        /// Set the audio output. Change will be applied after stop and play.
        /// </summary>
        [InteropFunction("libvlc_audio_output_set")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SetAudioOutput(IntPtr mpInstance, IntPtr audioOutputName);

        /// <summary>
        ///  Set audio output device. Changes are only effective after stop and play.
        /// </summary>
        [InteropFunction("libvlc_audio_output_device_set")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetAudioOutputDevice(IntPtr mpInstance, [MarshalAs(UnmanagedType.LPStr)] string audioOutputName, [MarshalAs(UnmanagedType.LPStr)] string deviceName);

        /// <summary>
        ///  Get audio output devices list.
        /// </summary>
        [InteropFunction("libvlc_audio_output_device_list_get")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr GetAudioOutputDeviceList(IntPtr mpInstance, [MarshalAs(UnmanagedType.LPStr)] string aout);

        /// <summary>
        ///  Release audio output devices list.
        /// </summary>
        [InteropFunction("libvlc_audio_output_device_list_release")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr ReleaseAudioOutputDeviceList(IntPtr p_list);
        #endregion
        #endregion

        public WrapperStandalone()
        {
            var libraryExtension = string.Empty;
            var settings = UMPSettings.Instance;

            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux && settings.UseExternalLibs)
                libraryExtension = ".8";

            _libVLCCoreHandler = InteropLibraryLoader.Load(Wrapper.LibraryVLCCoreName, settings.UseExternalLibs, settings.AdditionalLibsPath, libraryExtension);

            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux && settings.UseExternalLibs)
                libraryExtension = ".5";

            _libVLCHandler = InteropLibraryLoader.Load(Wrapper.LibraryVLCName, settings.UseExternalLibs, settings.AdditionalLibsPath, libraryExtension);

            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Mac)
                return;

            _libUMPHandler = InteropLibraryLoader.Load(LIBRARY_NAME, false, string.Empty, string.Empty);

            if (_libUMPHandler != IntPtr.Zero)
            {
                _manageLogCallback = DebugLogHandler;
                NativeHelperSetUnityLogMessageCallback(Marshal.GetFunctionPointerForDelegate(_manageLogCallback));
            }
        }

        private void DebugLogHandler(string msg)
        {
            UnityEngine.Debug.LogError(msg);
        }

        #region Native Helper
        public bool IsValid
        {
            get
            {
                return (_libVLCCoreHandler != IntPtr.Zero && 
                    _libVLCHandler != IntPtr.Zero);
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
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeInit>(_libUMPHandler).Invoke();

            return UMPNativeInit();
        }

        public void NativeHelperUpdateIndex(IntPtr mpInstance)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                InteropLibraryLoader.GetInteropDelegate<NativeUpdateIndex>(_libUMPHandler).Invoke((int)mpInstance);
            else
                UMPNativeUpdateIndex((int)mpInstance);
        }

        public void NativeHelperSetTexture(IntPtr mpInstance, IntPtr texture)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                InteropLibraryLoader.GetInteropDelegate<NativeSetTexture>(_libUMPHandler).Invoke((int)mpInstance, texture);
            else
                UMPNativeSetTexture((int)mpInstance, texture);
        }

        public IntPtr NativeHelperGetTexture(IntPtr mpInstance)
        {
            return IntPtr.Zero;
        }

        public void NativeHelperUpdateTexture(IntPtr mpInstance, IntPtr texture)
        {
        }

        public void NativeHelperSetPixelsBuffer(IntPtr mpInstance, IntPtr buffer, int width, int height)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                InteropLibraryLoader.GetInteropDelegate<NativeSetPixelsBuffer>(_libUMPHandler).Invoke((int)mpInstance, buffer, width, height);
            else
                UMPNativeSetPixelsBuffer((int)mpInstance, buffer, width, height);
        }

        public IntPtr NativeHelperGetPixelsBuffer(IntPtr mpInstance)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetPixelsBuffer>(_libUMPHandler).Invoke((int)mpInstance);

            return UMPNativeGetPixelsBuffer((int)mpInstance);
        }

        public int NativeHelperGetPixelsBufferWidth(IntPtr mpInstance)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetPixelsBufferWidth>(_libUMPHandler).Invoke((int)mpInstance);

            return UMPNativeGetPixelsBufferWidth((int)mpInstance);
        }

        public int NativeHelperGetPixelsBufferHeight(IntPtr mpInstance)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetPixelsBufferHeight>(_libUMPHandler).Invoke((int)mpInstance);

            return UMPNativeGetPixelsBufferHeight((int)mpInstance);
        }

        public string NativeHelperGetLogMessage(IntPtr mpInstance)
        {
            IntPtr value = IntPtr.Zero;

            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                value = InteropLibraryLoader.GetInteropDelegate<NativeGetLogMessage>(_libUMPHandler).Invoke((int)mpInstance);
            else
                value = UMPNativeGetLogMessage((int)mpInstance);

            return value != IntPtr.Zero ? Marshal.PtrToStringAnsi(value) : null;
        }

        public int NativeHelperGetLogLevel(IntPtr mpInstance)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetLogLevel>(_libUMPHandler).Invoke((int)mpInstance);

            return UMPNativeGetLogLevel((int)mpInstance);
        }

        public int NativeHelperGetState(IntPtr mpInstance)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetState>(_libUMPHandler).Invoke((int)mpInstance);

            return UMPNativeGetState((int)mpInstance);
        }

        private float NativeHelperGetStateFloatValue(IntPtr mpInstance)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetStateFloatValue>(_libUMPHandler).Invoke((int)mpInstance);

            return UMPNativeGetStateFloatValue((int)mpInstance);
        }

        private long NativeHelperGetStateLongValue(IntPtr mpInstance)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetStateLongValue>(_libUMPHandler).Invoke((int)mpInstance);

            return UMPNativeGetStateLongValue((int)mpInstance);
        }

        private string NativeHelperGetStateStringValue(IntPtr mpInstance)
        {
            IntPtr value = IntPtr.Zero;

            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                value = InteropLibraryLoader.GetInteropDelegate<NativeGetStateStringValue>(_libUMPHandler).Invoke((int)mpInstance);
            else
                value = UMPNativeGetStateStringValue((int)mpInstance);

            return value != IntPtr.Zero ? Marshal.PtrToStringAnsi(value) : null;
        }

        public void NativeHelperUpdatePixelsBuffer(IntPtr mpInstance)
        {
        }

        public void NativeHelperSetPixelsVerticalFlip(IntPtr mpInstance, bool flip)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                InteropLibraryLoader.GetInteropDelegate<NativeSetPixelsVerticalFlip>(_libUMPHandler).Invoke((int)mpInstance, flip);
            else
                UMPNativeSetPixelsVerticalFlip((int)mpInstance, flip);
        }

        public IntPtr NativeHelperGetAudioSamples(IntPtr samples, int length)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetAudioSamples>(_libUMPHandler).Invoke(samples, length);

            return UMPNativeGetAudioSamples(samples, length);
        }

        public IntPtr NativeHelperGetUnityRenderCallback()
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetUnityRenderCallback>(_libUMPHandler).Invoke();

            return UMPNativeGetUnityRenderCallback();
        }

        public IntPtr NativeHelperGetVideoLockCallback()
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetVideoLockCallback>(_libUMPHandler).Invoke();

            return UMPNativeGetVideoLockCallback();
        }

        public IntPtr NativeHelperGetVideoDisplayCallback()
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetVideoDisplayCallback>(_libUMPHandler).Invoke();

            return UMPNativeGetVideoDisplayCallback();
        }

        public IntPtr NativeHelperGetVideoFormatCallback()
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetVideoFormatCallback>(_libUMPHandler).Invoke();

            return UMPNativeGetVideoFormatCallback();
        }

        public IntPtr NativeHelperGetVideoCleanupCallback()
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetVideoCleanupCallback>(_libUMPHandler).Invoke();

            return UMPNativeGetVideoCleanupCallback();
        }

        public IntPtr NativeHelperGetAudioPlayCallback()
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetAudioPlayCallback>(_libUMPHandler).Invoke();

            return UMPNativeGetAudioPlayCallback();
        }

        public IntPtr NativeHelperMediaPlayerEventCallback()
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeMediaPlayerEventCallback>(_libUMPHandler).Invoke();

            return UMPNativeMediaPlayerEventCallback();
        }

        public IntPtr NativeHelperGetLogMessageCallback()
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetLogMessageCallback>(_libUMPHandler).Invoke();

            return UMPNativeGetLogMessageCallback();
        }

        public void NativeHelperSetUnityLogMessageCallback(IntPtr callback)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                InteropLibraryLoader.GetInteropDelegate<NativeSetUnityLogMessageCallback>(_libUMPHandler).Invoke(callback);
            else
                UMPNativeSetUnityLogMessageCallback(callback);
        }

        public void NativeHelperUpdateFramesCounter(IntPtr mpInstance, int counter)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                InteropLibraryLoader.GetInteropDelegate<NativeUpdateFramesCounterCallback>(_libUMPHandler).Invoke((int)mpInstance, counter);
            else
                UMPNativeUpdateFramesCounter((int)mpInstance, counter);
        }

        public int NativeHelperGetFramesCounter(IntPtr mpInstance)
        {
            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                return InteropLibraryLoader.GetInteropDelegate<NativeGetFramesCounterCallback>(_libUMPHandler).Invoke((int)mpInstance);

            return UMPNativeGetFramesCounter((int)mpInstance);
        }
        #endregion

        #region Player
        public void PlayerSetDataSource(IntPtr mpInstance, string path)
        {
        }

        public bool PlayerPlay(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<Play>(_libVLCHandler).Invoke(mpInstance) == 0;
        }

        public void PlayerPause(IntPtr mpInstance)
        {
            InteropLibraryLoader.GetInteropDelegate<Pause>(_libVLCHandler).Invoke(mpInstance);
        }

        public void PlayerStop(IntPtr mpInstance)
        {
            InteropLibraryLoader.GetInteropDelegate<Stop>(_libVLCHandler).Invoke(mpInstance);
        }

        public void PlayerRelease(IntPtr mpInstance)
        {
            InteropLibraryLoader.GetInteropDelegate<ReleaseMediaPlayer>(_libVLCHandler).Invoke(mpInstance);
        }

        public bool PlayerIsPlaying(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<IsPlaying>(_libVLCHandler).Invoke(mpInstance) == 1;
        }

        public bool PlayerWillPlay(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<CouldPlay>(_libVLCHandler).Invoke(mpInstance) == 1;
        }

        public long PlayerGetLength(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetLength>(_libVLCHandler).Invoke(mpInstance);
        }

        public float PlayerGetBufferingPercentage(IntPtr mpInstance)
        {
            return 0;
        }

        public long PlayerGetTime(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetTime>(_libVLCHandler).Invoke(mpInstance);
        }

        public void PlayerSetTime(IntPtr mpInstance, long time)
        {
            InteropLibraryLoader.GetInteropDelegate<SetTime>(_libVLCHandler).Invoke(mpInstance, time);
        }

        public float PlayerGetPosition(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetMediaPosition>(_libVLCHandler).Invoke(mpInstance);
        }

        public void PlayerSetPosition(IntPtr mpInstance, float pos)
        {
            InteropLibraryLoader.GetInteropDelegate<SetMediaPosition>(_libVLCHandler).Invoke(mpInstance, pos);
        }

        public float PlayerGetRate(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetRate>(_libVLCHandler).Invoke(mpInstance);
        }

        public bool PlayerSetRate(IntPtr mpInstance, float rate)
        {
            return InteropLibraryLoader.GetInteropDelegate<SetRate>(_libVLCHandler).Invoke(mpInstance, rate) == 0;
        }

        public int PlayerGetVolume(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetVolume>(_libVLCHandler).Invoke(mpInstance);
        }

        public int PlayerSetVolume(IntPtr mpInstance, int volume)
        {
            return InteropLibraryLoader.GetInteropDelegate<SetVolume>(_libVLCHandler).Invoke(mpInstance, volume);
        }

        public bool PlayerGetMute(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<IsMute>(_libVLCHandler).Invoke(mpInstance) == 1;
        }

        public void PlayerSetMute(IntPtr mpInstance, bool mute)
        {
            InteropLibraryLoader.GetInteropDelegate<SetMute>(_libVLCHandler).Invoke(mpInstance, mute ? 1 : 0);
        }

        public int PlayerVideoWidth(IntPtr mpInstance)
        {
            uint width = 0, height = 0;
            InteropLibraryLoader.GetInteropDelegate<GetVideoSize>(_libVLCHandler).Invoke(mpInstance, 0, out width, out height);
            return (int)width;
        }

        public int PlayerVideoHeight(IntPtr mpInstance)
        {
            uint width = 0, height = 0;
            InteropLibraryLoader.GetInteropDelegate<GetVideoSize>(_libVLCHandler).Invoke(mpInstance, 0, out width, out height);
            return (int)height;
        }

        public float PlayerVideoGetScale(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetVideoScale>(_libVLCHandler).Invoke(mpInstance);
        }

        public void PlayerVideoSetScale(IntPtr mpInstance, float factor)
        {
            InteropLibraryLoader.GetInteropDelegate<SetVideoScale>(_libVLCHandler).Invoke(mpInstance, factor);
        }

        public void PlayerVideoTakeSnapshot(IntPtr mpInstance, uint stream, string filePath, uint width, uint height)
        {
            InteropLibraryLoader.GetInteropDelegate<TakeSnapshot>(_libVLCHandler).Invoke(mpInstance, stream, filePath, width, height);
        }

        public float PlayerVideoFrameRate(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetFramesPerSecond>(_libVLCHandler).Invoke(mpInstance);
        }

        public int PlayerVideoFrameCount(IntPtr mpInstance)
        {
            return 0;
        }
        
        public PlayerState PlayerGetState(IntPtr mpInstance)
        {
            int state = 0;

            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                state = InteropLibraryLoader.GetInteropDelegate<NativeGetState>(_libUMPHandler).Invoke((int)mpInstance);
            else
                state = UMPNativeGetState((int)mpInstance);

            return (PlayerState)state;
        }

        public object PlayerGetStateValue(IntPtr mpInstance)
        {
            object value = NativeHelperGetStateFloatValue(mpInstance);

            if ((float)value < 0)
            {
                value = NativeHelperGetStateLongValue(mpInstance);
                if ((long)value < 0)
                    value = NativeHelperGetStateStringValue(mpInstance);
            }

            return value;
        }
        #endregion

        #region Player Expanded
        public IntPtr PlayerExpandedLibVLCNew(string[] args)
        {
            if (args == null)
                args = new string[0];

            return InteropLibraryLoader.GetInteropDelegate<CreateNewInstance>(_libVLCHandler).Invoke(args.Length, args);
        }

        public void PlayerExpandedLibVLCRelease(IntPtr libVLCInst)
        {
            InteropLibraryLoader.GetInteropDelegate<ReleaseInstance>(_libVLCHandler).Invoke(libVLCInst);
        }

        public IntPtr PlayerExpandedMediaNewLocation(IntPtr libVLCInst, string path)
        {
            return InteropLibraryLoader.GetInteropDelegate<CreateNewMediaFromLocation>(_libVLCHandler).Invoke(libVLCInst, path);
        }

        public void PlayerExpandedSetMedia(IntPtr mpInstance, IntPtr libVLCMediaInst)
        {
            InteropLibraryLoader.GetInteropDelegate<SetMediaToMediaPlayer>(_libVLCHandler).Invoke(mpInstance, libVLCMediaInst);
        }

        public void PlayerExpandedAddOption(IntPtr libVLCMediaInst, string option)
        {
            InteropLibraryLoader.GetInteropDelegate<AddOptionToMedia>(_libVLCHandler).Invoke(libVLCMediaInst, option);
        }

        // TODO Move to IPlayerWrapper
        public TrackInfo[] PlayerExpandedMediaGetTracksInfo(IntPtr mpInstance)
        {
            IntPtr result_buffer = new IntPtr();
            int trackCount = InteropLibraryLoader.GetInteropDelegate<GetMediaTracksInformations>(_libVLCHandler).Invoke(mpInstance, out result_buffer);

            if (trackCount < 0)
                return null;

            IntPtr buffer = result_buffer;
            var tracks = new TrackInfo[trackCount];

            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[i] = (TrackInfo)Marshal.PtrToStructure(buffer, typeof(TrackInfo));
                buffer = new IntPtr(buffer.ToInt64() + Marshal.SizeOf(typeof(TrackInfo)));
            }
            PlayerExpandedFree(result_buffer);
            return tracks;
        }

        public void PlayerExpandedMediaRelease(IntPtr libVLCMediaInst)
        {
            InteropLibraryLoader.GetInteropDelegate<ReleaseMedia>(_libVLCHandler).Invoke(libVLCMediaInst);
        }

        public IntPtr PlayerExpandedMediaPlayerNew(IntPtr libVLCInst)
        {
            return InteropLibraryLoader.GetInteropDelegate<CreateMediaPlayer>(_libVLCHandler).Invoke(libVLCInst);
        }

        public void PlayerExpandedFree(IntPtr instance)
        {
            InteropLibraryLoader.GetInteropDelegate<Free>(_libVLCHandler).Invoke(instance);
        }

        public IntPtr PlayerExpandedEventManager(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetMediaPlayerEventManager>(_libVLCHandler).Invoke(mpInstance);
        }

        public int PlayerExpandedEventAttach(IntPtr eventManagerInst, EventTypes eventType, IntPtr callback, IntPtr userData)
        {
            return InteropLibraryLoader.GetInteropDelegate<AttachEvent>(_libVLCHandler).Invoke(eventManagerInst, eventType, callback, userData);
        }

        public void PlayerExpandedEventDetach(IntPtr eventManagerInst, EventTypes eventType, IntPtr callback, IntPtr userData)
        {
            InteropLibraryLoader.GetInteropDelegate<DetachEvent>(_libVLCHandler).Invoke(eventManagerInst, eventType, callback, userData);
        }

        public void PlayerExpandedLogSet(IntPtr libVLC, IntPtr callback, IntPtr data)
        {
            InteropLibraryLoader.GetInteropDelegate<LogSet>(_libVLCHandler).Invoke(libVLC, callback, data);
        }

        public void PlayerExpandedLogUnset(IntPtr libVLC)
        {
            InteropLibraryLoader.GetInteropDelegate<LogUnset>(_libVLCHandler).Invoke(libVLC);
        }

        public void PlayerExpandedVideoSetCallbacks(IntPtr mpInstance, IntPtr @lock, IntPtr unlock, IntPtr display, IntPtr opaque)
        {
            InteropLibraryLoader.GetInteropDelegate<SetVideoCallbacks>(_libVLCHandler).Invoke(mpInstance, @lock, unlock, display, opaque);
        }

        public void PlayerExpandedVideoSetFormatCallbacks(IntPtr mpInstance, IntPtr setup, IntPtr cleanup)
        {
            InteropLibraryLoader.GetInteropDelegate<SetVideoFormatCallbacks>(_libVLCHandler).Invoke(mpInstance, setup, cleanup);
        }

        public void PlayerExpandedVideoSetFormat(IntPtr mpInstance, string chroma, uint width, uint height, uint pitch)
        {
            InteropLibraryLoader.GetInteropDelegate<SetVideoFormat>(_libVLCHandler).Invoke(mpInstance, chroma, width, height, pitch);
        }

        public void PlayerExpandedAudioSetCallbacks(IntPtr mpInstance, IntPtr play, IntPtr pause, IntPtr resume, IntPtr flush, IntPtr drain, IntPtr opaque)
        {
            InteropLibraryLoader.GetInteropDelegate<SetAudioCallbacks>(_libVLCHandler).Invoke(mpInstance, play, pause, resume, flush, drain, opaque);
        }

        public void PlayerExpandedAudioSetFormatCallbacks(IntPtr mpInstance, IntPtr setup, IntPtr cleanup)
        {
            InteropLibraryLoader.GetInteropDelegate<SetAudioFormatCallbacks>(_libVLCHandler).Invoke(mpInstance, setup, cleanup);
        }

        public void PlayerExpandedAudioSetFormat(IntPtr mpInstance, string format, int rate, int channels)
        {
            InteropLibraryLoader.GetInteropDelegate<SetAudioFormat>(_libVLCHandler).Invoke(mpInstance, format, rate, channels);
        }

        public long PlayerExpandedGetAudioDelay(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetAudioDelay>(_libVLCHandler).Invoke(mpInstance);
        }

        public void PlayerExpandedSetAudioDelay(IntPtr mpInstance, long channel)
        {
            InteropLibraryLoader.GetInteropDelegate<SetAudioDelay>(_libVLCHandler).Invoke(mpInstance, channel);
        }

        public int PlayerExpandedAudioOutputSet(IntPtr mpInstance, string psz_name)
        {
            return InteropLibraryLoader.GetInteropDelegate<SetAudioOutput>(_libVLCHandler).Invoke(mpInstance, Marshal.StringToHGlobalAnsi(psz_name));
        }

        public IntPtr PlayerExpandedAudioOutputListGet(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetAudioOutputsDescriptions>(_libVLCHandler).Invoke(mpInstance);
        }

        public void PlayerExpandedAudioOutputListRelease(IntPtr outputListInst)
        {
            InteropLibraryLoader.GetInteropDelegate<ReleaseAudioOutputDescription>(_libVLCHandler).Invoke(outputListInst);
        }

        public void PlayerExpandedAudioOutputDeviceSet(IntPtr mpInstance, string psz_audio_output, string psz_device_id)
        {
            InteropLibraryLoader.GetInteropDelegate<SetAudioOutputDevice>(_libVLCHandler).Invoke(mpInstance, psz_audio_output, psz_device_id);
        }

        public IntPtr PlayerExpandedAudioOutputDeviceListGet(IntPtr mpInstance, string aout)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetAudioOutputDeviceList>(_libVLCHandler).Invoke(mpInstance, aout);
        }

        public void PlayerExpandedAudioOutputDeviceListRelease(IntPtr deviceListInst)
        {
            InteropLibraryLoader.GetInteropDelegate<ReleaseAudioOutputDeviceList>(_libVLCHandler).Invoke(deviceListInst);
        }
        #endregion

        #region Player Spu
        public int PlayerSpuGetCount(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetVideoSpuCount>(_libVLCHandler).Invoke(mpInstance);
        }

        public IntPtr PlayerSpuGetDescription(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetVideoSpuDescription>(_libVLCHandler).Invoke(mpInstance);
        }

        public int PlayerSpuGet(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetVideoSpu>(_libVLCHandler).Invoke(mpInstance);
        }

        public int PlayerSpuSet(IntPtr mpInstance, int spuIndex)
        {
            return InteropLibraryLoader.GetInteropDelegate<SetVideoSpu>(_libVLCHandler).Invoke(mpInstance, spuIndex);
        }

        public int PlayerSpuSetFile(IntPtr mpInstance, string path)
        {
            return InteropLibraryLoader.GetInteropDelegate<SetVideoSubtitleFile>(_libVLCHandler).Invoke(mpInstance, path);
        }
        #endregion

        #region Player Audio
        public int PlayerAudioGetTrackCount(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetAudioTracksCount>(_libVLCHandler).Invoke(mpInstance);
        }

        public IntPtr PlayerAudioGetTrackDescription(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetAudioTracksDescriptions>(_libVLCHandler).Invoke(mpInstance);
        }

        public void PlayerAudioTrackDescriptionRelease(IntPtr trackDescription)
        {
            InteropLibraryLoader.GetInteropDelegate<ReleaseTracksDescriptions>(_libVLCHandler).Invoke(trackDescription);
        }

        public int PlayerAudioGetTrack(IntPtr mpInstance)
        {
            return InteropLibraryLoader.GetInteropDelegate<GetAudioTrack>(_libVLCHandler).Invoke(mpInstance);
        }

        public int PlayerAudioSetTrack(IntPtr mpInstance, int audioIndex)
        {
            return InteropLibraryLoader.GetInteropDelegate<SetAudioTrack>(_libVLCHandler).Invoke(mpInstance, audioIndex);
        }
        #endregion
    }
}
