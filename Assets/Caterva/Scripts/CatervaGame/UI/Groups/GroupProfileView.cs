using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CatervaClient;
using CatervaGame.UI.Social;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;

namespace CatervaGame.UI.Groups
{
    public class GroupProfileView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI groupNameText;
        [SerializeField] private TextMeshProUGUI createdText;
        [SerializeField] private ButtonManagerBasic joinButton;
        [SerializeField] private ButtonManagerBasic leaveButton;
        [SerializeField] private GameObject banText;
        [SerializeField] private UserListView userListView;

        private Group group;

        public async void SetData(Group group)
        {
            this.group = group;

            headerText.text = $"Page of {group.Name}";
            groupNameText.text = group.Name;
            createdText.text = group.CreatedAt.ToString(CultureInfo.InvariantCulture);

            var membershipStatuses = await CatervaNetworkManager.Instance.groupControllerClient.GetMembershipsOfMeAsync();
            var membership = membershipStatuses.Memberships.FirstOrDefault(otherMembership => otherMembership.GroupId == group.Id);
            if (membership == null)
            {
                joinButton.transform.parent.gameObject.SetActive(!group.InviteOnly);
                joinButton.buttonText = "JOIN";
                leaveButton.transform.parent.gameObject.SetActive(false);
                banText.SetActive(false);
            }
            else
            {
                switch (membership.MembershipStatus)
                {
                    case GroupMemberMembershipStatus.Groupadmin:
                    case GroupMemberMembershipStatus.Groupmember:
                        joinButton.transform.parent.gameObject.SetActive(false);
                        leaveButton.transform.parent.gameObject.SetActive(true);
                        leaveButton.buttonText = "LEAVE";
                        banText.SetActive(false);
                        break;
                    case GroupMemberMembershipStatus.Groupinvited:
                        joinButton.transform.parent.gameObject.SetActive(true);
                        joinButton.buttonText = "JOIN";
                        leaveButton.transform.parent.gameObject.SetActive(false);
                        banText.SetActive(false);
                        break;
                    case GroupMemberMembershipStatus.Groupbanned:
                        joinButton.transform.parent.gameObject.SetActive(false);
                        leaveButton.transform.parent.gameObject.SetActive(false);
                        banText.SetActive(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var users = new List<CatervaClient.Profile>();
            var adminIndexes = new List<int>();
            if (group.Members != null)
            {
                var groupMembers = group.Members.ToList();
                for (var i = 0; i < groupMembers.Count; i++)
                {
                    var groupMember = groupMembers[i];
                    switch (groupMember.MembershipStatus)
                    {
                        case GroupMemberMembershipStatus.Groupadmin:
                            if (groupMember.MembershipStatus == GroupMemberMembershipStatus.Groupadmin)
                            {
                                adminIndexes.Add(i);
                            }

                            goto case GroupMemberMembershipStatus.Groupmember;
                        case GroupMemberMembershipStatus.Groupmember:
                        {
                            var profile = await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(groupMember.MemberId);
                            users.Add(profile.Profile);
                            break;
                        }
                    }
                }
            }

            var userViewList = userListView.SetData(users);
            for (var i = 0; i < userViewList.Count; i++)
            {
                var view = userViewList[i];
                view.ToggleAdminImage(adminIndexes.Contains(i));
            }
        }

        public async void JoinGroup()
        {
            var membershipDto = await CatervaNetworkManager.Instance.groupControllerClient.JoinGroupAsync(this.group.Id);
            var group = await CatervaNetworkManager.Instance.groupControllerClient.FindGroupAsync(membershipDto.Membership.GroupId);
            group.Group.Members = (await CatervaNetworkManager.Instance.groupControllerClient.GetMembersOfGroupAsync(group.Group.Id)).Memberships;
            SetData(group.Group);
        }

        public async void LeaveGroup()
        {
            await CatervaNetworkManager.Instance.groupControllerClient.LeaveGroupAsync(this.group.Id);
            var group = await CatervaNetworkManager.Instance.groupControllerClient.FindGroupAsync(this.group.Id);
            group.Group.Members = (await CatervaNetworkManager.Instance.groupControllerClient.GetMembersOfGroupAsync(group.Group.Id)).Memberships;
            SetData(group.Group);
        }
    }
}