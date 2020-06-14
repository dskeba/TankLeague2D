
using UnityEngine;
using UnityEngine.UI;

namespace TankGame
{
    public class MenuSystem : MonoBehaviour
    {
        public Dropdown resolutionDropdown;
        public Toggle fullScreenToggle;
        public Slider volumeSlider;

        private void Start()
        {
            SceneLoadManager.Instance.SetActiveScene("Menu");
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(GraphicsManager.Instance.GetResolutionStrings());
            resolutionDropdown.value = GraphicsManager.Instance.GetIndexOfCurrentResolution();
            resolutionDropdown.RefreshShownValue();
            fullScreenToggle.isOn = GraphicsManager.Instance.GetFullScreen();
            volumeSlider.value = SoundManager.Instance.GetVolume(MixerGroup.Master);
            SetupCursor();
        }

        private void SetupCursor()
        {
            Texture2D crosshair = Resources.Load<Texture2D>("Cursors/Crosshair");
            Vector2 hotspot = new Vector2(25, 25);
            Cursor.SetCursor(crosshair, hotspot, CursorMode.Auto);
        }

        public void SetResolution(int index)
        {
            GraphicsManager.Instance.SetResolutionByString(resolutionDropdown.options[index].text);
        }

        public void PlaySoccerDesertOasis()
        {
            PlaySoccer("SoccerDesertOasis");
        }

        public void PlaySoccerMuddyMeadows()
        {
            PlaySoccer("SoccerMuddyMeadows");
        }

        private void PlaySoccer(string sceneName)
        {
            LoadingSceneOptions options = new LoadingSceneOptions(LoadingSceneBehavior.AnyKey);
            SceneLoadManager.Instance.LoadSceneWithLoadingScene(sceneName, options, SpawnManager.Instance.PreloadPools);
        }

        public void PlaySurvival()
        {
            LoadingSceneOptions options = new LoadingSceneOptions(LoadingSceneBehavior.AnyKey);
            SceneLoadManager.Instance.LoadSceneWithLoadingScene("Survival", options, SpawnManager.Instance.PreloadPools);
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void ChangeMasterVolume(float level)
        {
            SoundManager.Instance.SetVolume(MixerGroup.Master, level);
        }

        public void SetFullscreen(bool isFullScreen)
        {
            GraphicsManager.Instance.SetFullScreen(isFullScreen);
        }

        public void SetDifficulty(int difficulty)
        {
            PlayerPrefs.SetInt("difficulty", difficulty);
        }
    }
}
