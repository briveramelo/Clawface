// LevelManagerPlayback.cs
// Author: Aaron

namespace Turing.LevelEditor
{
    /// <summary>
    /// Holds functionality for level playback through LM.
    /// </summary>
    public partial class LevelManager : EditorSingleton<LevelManager>
    {
        #region Private Fields

        /// <summary>
        /// Is the level currently being played?
        /// </summary>
        bool playingLevel = false;

        #endregion
        #region Public Methods

        /// <summary>
        /// Starts playing the currently loaded level.
        /// </summary>
        public void PlayCurrentLevel()
        {
            //Debug.Log("Play");

            // Activate all spawners
            foreach (ObjectSpawner spawner in loadedSpawners)
                spawner.Play();

            playingLevel = true;
        }

        /// <summary>
        /// Stops playing the currently loaded level.
        /// </summary>
        public void StopPlayingLevel()
        {
            //Debug.Log("Stop");

            // Reset all spawners
            foreach (var spawner in loadedSpawners)
                spawner.ResetSpawner();

            playingLevel = false;
        }

        #endregion
    }
}