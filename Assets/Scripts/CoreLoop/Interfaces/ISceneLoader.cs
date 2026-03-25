namespace CoreLoop.Interfaces
{
    public interface ISceneLoader
    {
        void LoadNextLevel();
        void LoadMainMenu();
        void LoadCombatScene(string levelName);
        void UnloadCombatScene();
        void LoadCreditsScene();
        void LoadCinematicScene(string scene);
        void ResetPlaythrough();
    }
}