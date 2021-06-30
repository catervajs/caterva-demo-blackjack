using CatervaClient;
using UnityEngine;

namespace CatervaGame.UI.Groups
{
    public class GroupDisplayManager : MonoBehaviour
    {
        public static GroupDisplayManager Instance;
        [SerializeField] private GroupProfileView groupProfileView;
        [SerializeField] private GameObject userView;
        [SerializeField] private CanvasGroup windows;

        private void Awake()
        {
            Instance = this;
        }

        public async void OpenGroup(Group group)
        {
            windows.alpha = 0;
            windows.blocksRaycasts = false;
            windows.interactable = false;
            userView.SetActive(false);
            group = (await CatervaNetworkManager.Instance.groupControllerClient.FindGroupAsync(group.Id)).Group;
            group.Members = (await CatervaNetworkManager.Instance.groupControllerClient.GetMembersOfGroupAsync(group.Id)).Memberships;
            groupProfileView.SetData(group);
            groupProfileView.gameObject.SetActive(true);
        }

        public void CloseGroup()
        {
            groupProfileView.gameObject.SetActive(false);
            windows.alpha = 1;
            windows.blocksRaycasts = true;
            windows.interactable = true;
        }
    }
}