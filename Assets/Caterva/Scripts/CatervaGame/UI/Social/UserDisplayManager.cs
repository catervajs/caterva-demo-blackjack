using CatervaClient;
using CatervaGame.UI.Profile;
using UnityEngine;

namespace CatervaGame.UI.Social
{
    public class UserDisplayManager : MonoBehaviour
    {
        public static UserDisplayManager Instance;
        [SerializeField] private ProfileView profileView;
        [SerializeField] private GameObject groupView;
        [SerializeField] private CanvasGroup windows;

        private void Awake()
        {
            Instance = this;
        }

        public async void OpenProfile(CatervaClient.Profile profile)
        {
            windows.alpha = 0;
            windows.blocksRaycasts = false;
            windows.interactable = false;
            groupView.SetActive(false);
            profile = (await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(profile.UserId)).Profile;
            profileView.gameObject.SetActive(true);
            profileView.SetData(profile);
        }

        public void CloseProfile()
        {
            profileView.gameObject.SetActive(false);
            windows.alpha = 1;
            windows.blocksRaycasts = true;
            windows.interactable = true;
        }
    }
}