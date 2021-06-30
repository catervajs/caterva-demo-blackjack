using CatervaClient;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;

namespace CatervaGame.UI.Profile
{
    public class CreateProfileView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private CustomDropdown locationDropdown;
        [SerializeField] private CustomDropdown languageDropdown;

        public async void CreateAccount()
        {
            try
            {
                CatervaNetworkManager.Instance.currentProfile = (await CatervaNetworkManager.Instance.profileControllerClient.CreateAsync(new CreateProfileDto
                {
                    DisplayName = inputField.text,
                    Language = languageDropdown.selectedText.text,
                    Location = locationDropdown.selectedText.text
                })).Profile;
                CreateProfileViewDisplayManager.Instance.CloseCreateProfilePage();
                CreateProfileViewDisplayManager.Instance.UnblockButtons();
            }
            catch (ApiException e)
            {
                Debug.LogError($"Error while creating profile.\n{e}");
            }
        }
    }
}