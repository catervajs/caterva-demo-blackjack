using CatervaClient;
using TMPro;
using UnityEngine;

namespace CatervaGame.UI.Social
{
    public class UserListItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI userNameText;
        [SerializeField] private GameObject adminImage;

        private CatervaClient.Profile user;

        public void SetData(CatervaClient.Profile profile)
        {
            user = profile;
            userNameText.text = user.DisplayName;
        }

        public void OpenProfile()
        {
            UserDisplayManager.Instance.OpenProfile(user);
        }

        public void ToggleAdminImage(bool status)
        {
            adminImage.SetActive(status);
        }
    }
}