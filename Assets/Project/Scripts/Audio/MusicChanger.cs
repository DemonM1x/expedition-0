using System;
using System.Collections.Generic;
using UnityEngine;
using Expedition0.Save;

namespace Expedition0.Audio
{
    public class MusicChanger : MonoBehaviour
    {
        [Serializable]
        public class ConditionalTrack
        {
            public GameProgress progressConditions;
            public int priority;                 // higher = wins
            public MusicTrackAsset trackAsset;

            public bool IsSatisfiedBy(GameProgress p) =>
                (p & progressConditions) == progressConditions;
        }

        [SerializeField] private List<ConditionalTrack> conditionalTracks;
        [SerializeField] private MusicTrackAsset defaultTrack;

        private MusicTrackAsset _last;

        private void Start()
        {
            Reevaluate(SaveManager.LoadProgress());
        }

        // public so other systems can trigger it when flags change
        public void Reevaluate(GameProgress progress)
        {
            MusicTrackAsset best = defaultTrack;
            int bestPr = int.MinValue;

            foreach (var c in conditionalTracks)
            {
                if (c.trackAsset && c.IsSatisfiedBy(progress) && c.priority >= bestPr)
                {
                    best = c.trackAsset;
                    bestPr = c.priority;
                }
            }
            SetMusicTrack(best);
        }

        private void SetMusicTrack(MusicTrackAsset trackAsset)
        {
            if (_last == trackAsset) return;              // avoid needless restarts
            _last = trackAsset;
            if (MusicPlayer.Instance) MusicPlayer.Instance.Play(trackAsset);
        }
    }
}