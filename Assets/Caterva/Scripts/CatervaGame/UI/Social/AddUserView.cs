using CatervaClient;
using TMPro;
using UnityEngine;

namespace CatervaGame.UI.Social
{
    public class AddUserView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;

        public async void Add()
        {
            if (string.IsNullOrEmpty(inputField.text)) return;

            await CatervaNetworkManager.Instance.friendControllerClient.AddFriendAsync(new ReferenceFriendDto
            {
                Id = inputField.text
            });
        }
    }
}