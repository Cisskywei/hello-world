using System;
using UnityEngine;

namespace UMP
{
    public class MediaPlayer : IPlayer, IPlayerAudio, IPlayerSpu
    {
        private object _playerObject;
        private IPlayer _player;
        private IPlayerAudio _playerAudio;
        private IPlayerSpu _playerSpu;

        #region Constructors
        /// <summary>
        ///  Create new instance of MediaPlayer object
        /// </summary>
        /// <param name="monoObject">MonoBehaviour instanse</param>
        /// <param name="videoOutputObjects">Objects that will be rendering video output</param>
        public MediaPlayer(MonoBehaviour monoObject, GameObject[] videoOutputObjects) : this(monoObject, videoOutputObjects, null)
        {
        }

        /// <summary>
        ///  Create instance of MediaPlayer object with additional arguments
        /// </summary>
        /// <param name="monoObject">MonoBehaviour instanse</param>
        /// <param name="videoOutputObjects">Objects that will be rendering video output</param>
        /// <param name="options">Additional player options</param>
        public MediaPlayer(MonoBehaviour monoObject, GameObject[] videoOutputObjects, PlayerOptions options)
        {
            var supportedPlatform = UMPSettings.SupportedPlatform;

            switch (supportedPlatform)
            {
                case UMPSettings.Platforms.Win:
                case UMPSettings.Platforms.Mac:
                case UMPSettings.Platforms.Linux:
                    PlayerOptionsStandalone standaloneOptions = null;
                    if (options is PlayerOptionsStandalone)
                        standaloneOptions = options as PlayerOptionsStandalone;

                    _playerObject = new MediaPlayerStandalone(monoObject, videoOutputObjects, standaloneOptions);
                    break;

                case UMPSettings.Platforms.WebGL:
                    _playerObject = new MediaPlayerWebGL(monoObject, videoOutputObjects, options);
                    break;
            }

            if (_playerObject is IPlayer)
                _player = (_playerObject as IPlayer);

            if (_playerObject is IPlayerAudio)
                _playerAudio = (_playerObject as IPlayerAudio);

            if (_playerObject is IPlayerSpu)
                _playerSpu = (_playerObject as IPlayerSpu);

        }

        /// <summary>
        ///  Create new instance of MediaPlayer object from another MediaPlayer instance
        /// </summary>
        /// <param name="monoObject">MonoBehaviour instanse</param>
        /// <param name="basedPlayer">Based on MediaPlayer instance</param>
        public MediaPlayer(MonoBehaviour monoObject, MediaPlayer basedPlayer) : this(monoObject, basedPlayer.VideoOutputObjects, null)
        {
            if (basedPlayer.DataSource != null && string.IsNullOrEmpty(basedPlayer.DataSource.ToString()))
                _player.DataSource = basedPlayer.DataSource;

            _player.EventManager.CopyPlayerEvents(basedPlayer.EventManager);
            _player.Mute = basedPlayer.Mute;
            _player.Volume = basedPlayer.Volume;
            _player.PlaybackRate = basedPlayer.PlaybackRate;
        }
        #endregion

        public object PlatformPlayer
        {
            get
            {
                return _playerObject;
            }
        }

        public GameObject[] VideoOutputObjects
        {
            get
            {
                return _player.VideoOutputObjects;
            }
            set
            {
                _player.VideoOutputObjects = value;
            }
        }

        public PlayerManagerEvents EventManager
        {
            get
            {
                return _player.EventManager;
            }
        }

        public PlayerOptions Arguments
        {
            get
            {
                return _player.Arguments;
            }
        }

        public PlayerState State
        {
            get
            {
                return _player.State;
            }
        }

        public object StateValue
        {
            get
            {
                return _player.StateValue;
            }
        }


        public void AddMediaListener(IMediaListener listener)
        {
            _player.AddMediaListener(listener);
        }

        public void RemoveMediaListener(IMediaListener listener)
        {
            _player.RemoveMediaListener(listener);
        }

        public void Prepare()
        {
            _player.Prepare();
        }

        /// <summary>
        /// Play or resume (True if playback started (and was already started), or False on error.
        /// </summary>
        /// <returns></returns>
        public bool Play()
        {
            return _player.Play();
        }

        public void Pause()
        {
            _player.Pause();
        }

        public void Stop(bool resetTexture)
        {
            _player.Stop(resetTexture);
        }

        public void Stop()
        {
            Stop(true);
        }

        public void Release()
        {
            _player.Release();
        }

        public Uri DataSource
        {
            get
            {
                return _player.DataSource;
            }
            set
            {
                _player.DataSource = value;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _player.IsPlaying;
            }
        }

        public bool IsReady
        {
            get
            {
                return _player.IsReady;
            }
        }

        public bool AbleToPlay
        {
            get
            {
                return _player.AbleToPlay;
            }
        }

        /// <summary>
        /// Get the current movie length (in ms).
        /// </summary>
        /// <returns></returns>
        public long Length
        {
            get
            {
                return _player.Length;
            }
        }

        /// <summary>
        /// Get the current movie formatted length (hh:mm:ss[:ms]).
        /// </summary>
        /// <param name="detail">True: formatted length will be with [:ms]</param>
        /// <returns></returns>
        public string GetFormattedLength(bool detail)
        {
            return _player.GetFormattedLength(detail);
        }

        public float FrameRate
        {
            get { return _player.FrameRate; }
        }

        public int FramesCounter
        {
            get { return _player.FramesCounter; }
        }

        public byte[] FramePixels
        {
            get
            {
                return _player.FramePixels;
            }
        }

        public long Time
        {
            get
            {
                return _player.Time;
            }
            set
            {
                _player.Time = value;
            }
        }

        public float Position
        {
            get
            {
                return _player.Position;
            }
            set
            {
                _player.Position = value;
            }
        }

        public float PlaybackRate
        {
            get
            {
                return _player.PlaybackRate;
            }
            set
            {
                _player.PlaybackRate = value;
            }
        }

        public int Volume
        {
            get
            {
                return _player.Volume;
            }
            set
            {
                _player.Volume = value;
            }
        }

        public bool Mute
        {
            get
            {
                return _player.Mute;
            }
            set
            {
                _player.Mute = value;
            }
        }

        public int VideoWidth
        {
            get
            {
                return _player.VideoWidth;
            }
        }

        public int VideoHeight
        {
            get
            {
                return _player.VideoHeight;
            }
        }

        public Vector2 VideoSize
        {
            get
            {
                return new Vector2(VideoWidth, VideoHeight);
            }
        }

        /// <summary>
        /// Get available audio tracks.
        /// </summary>
        public MediaTrackInfo[] AudioTracks
        {
            get
            {
                if (_playerAudio != null)
                    return _playerAudio.AudioTracks;

                return null;
            }
        }

        /// <summary>
        /// Get/Set current audio track.
        /// </summary>
        public MediaTrackInfo AudioTrack
        {
            get
            {
                if (_playerAudio != null)
                    return _playerAudio.AudioTrack;

                return null;
            }
            set
            {
                if (_playerAudio != null)
                    _playerAudio.AudioTrack = value;
            }
        }

        /// <summary>
        /// Get available spu tracks.
        /// </summary>
        public MediaTrackInfo[] SpuTracks
        {
            get
            {
                if (_playerSpu != null)
                    return _playerSpu.SpuTracks;

                return null;
            }
        }

        /// <summary>
        /// Get/Set current spu track.
        /// </summary>
        public MediaTrackInfo SpuTrack
        {
            get
            {
                if (_playerSpu != null)
                    return _playerSpu.SpuTrack;

                return null;
            }
            set
            {
                if (_playerSpu != null)
                    _playerSpu.SpuTrack = value;
            }
        }

        /// <summary>
        /// Set new video subtitle file
        /// </summary>
        /// <param name="path">Path to the new video subtitle file</param>
        /// <returns></returns>
        public bool SetSubtitleFile(Uri path)
        {
            if (_playerSpu != null)
                return _playerSpu.SetSubtitleFile(path);

            return false;
        }
    }
}
