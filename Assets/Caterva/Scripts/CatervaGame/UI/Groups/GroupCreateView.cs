using CatervaClient;
using Michsky.UI.ModernUIPack;
using UnityEngine;

namespace CatervaGame.UI.Groups
{
    public class GroupCreateView : MonoBehaviour
    {
        [SerializeField] private CustomInputField groupNameInputField;
        [SerializeField] private CustomToggle inviteOnlyToggle;

        public async void Create()
        {
            var group = await CatervaNetworkManager.Instance.groupControllerClient.CreateGroupAsync(new CreateGroupDto
            {
                Name = groupNameInputField.inputText.text,
                InviteOnly = inviteOnlyToggle.toggleObject.isOn
            });

            GroupDisplayManager.Instance.OpenGroup(group.Group);
        }
    }
}