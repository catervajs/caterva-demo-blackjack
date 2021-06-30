using System.Collections.Generic;
using System.Linq;
using CatervaClient;
using TMPro;
using UnityEngine;

namespace CatervaGame.UI.Groups
{
    public class GroupListView : MonoBehaviour
    {
        [SerializeField] private GroupListItemView groupListItemPrefab;
        [SerializeField] private Transform itemParent;
        [SerializeField] private GroupMemberMembershipStatus[] statusToShow;
        [SerializeField] private TMP_InputField searchInputField;

        private void Start()
        {
            Load();
        }

        public void Load()
        {
            for (var i = 0; i < itemParent.childCount; i++)
            {
                var child = itemParent.GetChild(i);
                Destroy(child.gameObject);
            }

            if (!searchInputField)
                GetYourGroups();
        }

        private async void GetYourGroups()
        {
            await Utils.WaitWhile(() => !CatervaNetworkManager.Instance || string.IsNullOrEmpty(CatervaNetworkManager.Instance.GetToken()) || !CatervaNetworkManager.Instance.IsInitialized);
            var membershipsDto = await CatervaNetworkManager.Instance.groupControllerClient.GetMembershipsOfMeAsync();
            var yourGroupsList = membershipsDto.Memberships.Where(membership => statusToShow.Length == 0 || statusToShow.Contains(membership.MembershipStatus));
            SetData(yourGroupsList);
        }

        public void Search()
        {
            GetSearchResults(searchInputField.text);
        }

        private async void GetSearchResults(string groupName)
        {
            await Utils.WaitWhile(() => !CatervaNetworkManager.Instance || string.IsNullOrEmpty(CatervaNetworkManager.Instance.GetToken()) || !CatervaNetworkManager.Instance.IsInitialized);

            var groups = await CatervaNetworkManager.Instance.groupControllerClient.FindGroupsAsync(groupName, false);
            var groupMembers = groups.Groups.Select(group => new GroupMember {Group = group, MembershipStatus = GroupMemberMembershipStatus.Groupmember});
            SetData(groupMembers);
        }

        public void SetData(IEnumerable<GroupMember> memberships)
        {
            for (var i = 0; i < itemParent.childCount; i++)
            {
                var child = itemParent.GetChild(i);
                Destroy(child.gameObject);
            }

            foreach (var groupMember in memberships)
            {
                var item = Instantiate(groupListItemPrefab, itemParent);
                item.SetData(groupMember);
            }
        }
    }
}