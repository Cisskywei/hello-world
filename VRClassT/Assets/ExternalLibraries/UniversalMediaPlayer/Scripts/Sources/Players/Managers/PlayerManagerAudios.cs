using UnityEngine;

namespace UMP
{
    internal class PlayerManagerAudios
    {
        private AudioSource[] _audioSources;

        public PlayerManagerAudios(AudioSource[] audioSources)
        {
            _audioSources = audioSources;
        }

        public bool IsValid
        {
            get
            {
                foreach (var source in _audioSources)
                {
                    if (source != null)
                        return true;
                }

                return false;
            }
        }

        public AudioSource[] AudioSources
        {
            get
            {
                return _audioSources;
            }
        }

        public AudioClip Clip
        {
            get
            {
                foreach (var source in _audioSources)
                {
                    if (source != null)
                        return source.clip;
                }

                return null;
            }
            set
            {
                foreach (var source in _audioSources)
                {
                    if (source != null)
                        source.clip = value;
                }
            }
        }

        public bool Mute
        {
            get
            {
                foreach (var source in _audioSources)
                {
                    if (source != null)
                        return source.mute;
                }

                return false;
            }
            set
            {
                foreach (var source in _audioSources)
                {
                    if (source != null)
                        source.mute = value;
                }
            }
        }

        public bool Loop
        {
            get
            {
                foreach (var source in _audioSources)
                {
                    if (source != null)
                        return source.loop;
                }

                return false;
            }
            set
            {
                foreach (var source in _audioSources)
                {
                    if (source != null)
                        source.loop = value;
                }
            }
        }

        public float Time
        {
            get
            {
                foreach (var source in _audioSources)
                {
                    if (source != null)
                        return source.time;
                }

                return 1;
            }
            set
            {
                foreach (var source in _audioSources)
                {
                    if (source != null)
                        source.time = value;
                }
            }
        }

        public float Volume
        {
            get
            {
                foreach (var source in _audioSources)
                {
                    if (source != null)
                        return source.volume;
                }

                return 1;
            }
            set
            {
                foreach (var source in _audioSources)
                {
                    if (source != null)
                        source.volume = value;
                }
            }
        }

        public float Pitch
        {
            get
            {
                foreach (var source in _audioSources)
                {
                    if (source != null)
                        return source.pitch;
                }

                return 1;
            }
            set
            {
                foreach (var source in _audioSources)
                {
                    if (source != null)
                        source.pitch = value;

                }
            }
        }

        public bool IsPlaying
        {
            get
            {
                foreach (var source in _audioSources)
                {
                    if (source != null)
                        return source.isPlaying;
                }

                return false;
            }
        }

        public void Play()
        {
            foreach (var source in _audioSources)
            {
                if (source != null)
                    source.Play();
            }
        }

        public void Pause()
        {
            foreach (var source in _audioSources)
            {
                if (source != null)
                    source.Pause();
            }
        }

        public void Stop()
        {
            foreach (var source in _audioSources)
            {
                if (source != null)
                    source.Stop();
            }
        }
    }
}
