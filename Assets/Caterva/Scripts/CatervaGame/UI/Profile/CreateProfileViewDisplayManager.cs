using UnityEngine;
using UnityEngine.UI;

namespace CatervaGame.UI.Profile
{
    public class CreateProfileViewDisplayManager : MonoBehaviour
    {
        public static CreateProfileViewDisplayManager Instance;

        [SerializeField] private CreateProfileView createProfileView;
        [SerializeField] private CanvasGroup windows;
        [SerializeField] private GameObject groupView;
        [SerializeField] private Button[] buttonsToBlock;

        private void Awake()
        {
            Instance = this;
        }

        public void BlockButtons()
        {
            foreach (var button in buttonsToBlock)
            {
                button.interactable = false;
            }
        }

        public void UnblockButtons()
        {
            foreach (var button in buttonsToBlock)
            {
                button.interactable = true;
            }
        }

        public void OpenCreateProfilePage()
        {
            createProfileView.gameObject.SetActive(true);
            windows.alpha = 0;
            windows.blocksRaycasts = false;
            windows.interactable = false;
        }

        public void CloseCreateProfilePage()
        {
            createProfileView.gameObject.SetActive(false);
            windows.alpha = 1;
            windows.blocksRaycasts = true;
            windows.interactable = true;
        }
    }
}