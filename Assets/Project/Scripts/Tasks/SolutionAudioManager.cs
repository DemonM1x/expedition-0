using UnityEngine;

namespace Expedition0.Tasks
{
    public class SolutionAudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource correctSolutionAudio;
        [SerializeField] private AudioSource incorrectSolutionAudio;
        [SerializeField] private AudioSource nthIncorrectAudio;
        
        [Header("Audio Clips")]
        [SerializeField] private AudioClip correctSolutionClip;
        [SerializeField] private AudioClip incorrectSolutionClip;
        [SerializeField] private AudioClip nthIncorrectClip;
        
        [Header("Volume Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float correctVolume = 0.7f;
        [Range(0f, 1f)]
        [SerializeField] private float incorrectVolume = 0.5f;
        [Range(0f, 1f)]
        [SerializeField] private float nthIncorrectVolume = 0.8f;
        
        /// <summary>
        /// Воспроизводит звук правильного ответа - вызывается из UnityEvent
        /// </summary>
        public void PlayCorrectSolutionSound()
        {
            PlaySound(correctSolutionAudio, correctSolutionClip, correctVolume, "correct solution");
        }
        
        /// <summary>
        /// Воспроизводит звук неправильного ответа - вызывается из UnityEvent
        /// </summary>
        public void PlayIncorrectSolutionSound()
        {
            PlaySound(incorrectSolutionAudio, incorrectSolutionClip, incorrectVolume, "incorrect solution");
        }
        
        /// <summary>
        /// Воспроизводит звук при n-ом неправильном ответе - вызывается из UnityEvent
        /// </summary>
        public void PlayNthIncorrectSound()
        {
            PlaySound(nthIncorrectAudio, nthIncorrectClip, nthIncorrectVolume, "nth incorrect solution");
        }
        
        private void PlaySound(AudioSource audioSource, AudioClip clip, float volume, string soundType)
        {
            if (audioSource == null)
            {
                Debug.LogWarning($"SolutionAudioManager: AudioSource for {soundType} is not assigned!");
                return;
            }
            
            if (clip != null)
            {
                audioSource.clip = clip;
            }
            
            if (audioSource.clip == null)
            {
                Debug.LogWarning($"SolutionAudioManager: No audio clip assigned for {soundType}!");
                return;
            }
            
            audioSource.volume = volume;
            audioSource.Play();
            
            Debug.Log($"SolutionAudioManager: Playing {soundType} sound");
        }
        
        /// <summary>
        /// Останавливает все звуки
        /// </summary>
        public void StopAllSounds()
        {
            if (correctSolutionAudio != null) correctSolutionAudio.Stop();
            if (incorrectSolutionAudio != null) incorrectSolutionAudio.Stop();
            if (nthIncorrectAudio != null) nthIncorrectAudio.Stop();
            
            Debug.Log("SolutionAudioManager: All sounds stopped");
        }
        
        // Методы для тестирования в инспекторе
        [ContextMenu("Test Correct Sound")]
        public void TestCorrectSound()
        {
            PlayCorrectSolutionSound();
        }
        
        [ContextMenu("Test Incorrect Sound")]
        public void TestIncorrectSound()
        {
            PlayIncorrectSolutionSound();
        }
        
        [ContextMenu("Test Nth Incorrect Sound")]
        public void TestNthIncorrectSound()
        {
            PlayNthIncorrectSound();
        }
    }
}