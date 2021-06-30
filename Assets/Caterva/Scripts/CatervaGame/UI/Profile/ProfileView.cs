using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CatervaClient;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;

namespace CatervaGame.UI.Profile
{
    public class ProfileView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI idText;
        [SerializeField] private TextMeshProUGUI createdText;
        [SerializeField] private TextMeshProUGUI usernameText;
        [SerializeField] private TextMeshProUGUI locationText;
        [SerializeField] private TextMeshProUGUI languageText;
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private CustomDropdown locationDropdown;
        [SerializeField] private CustomDropdown languageDropdown;

        [Header("Buttons")]
        [SerializeField] private ButtonManagerBasic saveButton;
        [SerializeField] private ButtonManagerBasic addButton;
        [SerializeField] private ButtonManagerBasic removeButton;
        [SerializeField] private ButtonManagerBasic blockButton;
        [SerializeField] private ButtonManagerBasic unblockButton;
        [SerializeField] private CustomDropdown groupDropdown;
        [SerializeField] private ButtonManagerBasic inviteButton;
        [SerializeField] private ButtonManagerBasic banButton;
        [SerializeField] private ButtonManagerBasic kickButton;
        [SerializeField] private ButtonManagerBasic promoteButton;

        private readonly List<Group> groupList = new List<Group>();
        private int activeGroupIndex;

        private CatervaClient.Profile currentProfile;

        private void OnEnable()
        {
            activeGroupIndex = 0;
        }

        public void SetData()
        {
            languageDropdown.dropdownItems.Clear();
            languageDropdown.CreateNewItemFast("Turkish", null);
            languageDropdown.CreateNewItemFast("English", null);
            languageDropdown.CreateNewItemFast("German", null);
            languageDropdown.CreateNewItemFast("French", null);
            languageDropdown.CreateNewItemFast("Chinese", null);
            languageDropdown.SetupDropdown();
            languageDropdown.UpdateValues();

            locationDropdown.dropdownItems.Clear();
            locationDropdown.CreateNewItemFast("Turkey", null);
            locationDropdown.CreateNewItemFast("United Kingdom", null);
            locationDropdown.CreateNewItemFast("Germany", null);
            locationDropdown.CreateNewItemFast("France", null);
            locationDropdown.CreateNewItemFast("China", null);
            locationDropdown.SetupDropdown();
            locationDropdown.UpdateValues();

            SetData(CatervaNetworkManager.Instance.currentProfile);
        }

        public async void SetData(CatervaClient.Profile profile)
        {
            currentProfile = profile;

            var isOwnProfile = CatervaNetworkManager.Instance.currentProfile.UserId == profile.UserId;

            usernameInputField.transform.parent.gameObject.SetActive(isOwnProfile);
            locationDropdown.transform.parent.gameObject.SetActive(isOwnProfile);
            languageDropdown.transform.parent.gameObject.SetActive(isOwnProfile);
            usernameText.transform.parent.gameObject.SetActive(!isOwnProfile);
            locationText.transform.parent.gameObject.SetActive(!isOwnProfile);
            languageText.transform.parent.gameObject.SetActive(!isOwnProfile);

            idText.text = profile.UserId;
            createdText.text = profile.CreatedAt.ToString(CultureInfo.InvariantCulture);

            saveButton.transform.parent.gameObject.SetActive(isOwnProfile);

            FriendshipTwoWayRelationStatusDtoStatus relation;

            try
            {
                var friendshipRelation =
                    await CatervaNetworkManager.Instance.friendControllerClient.FindRelationOfMeAndOtherAsync(profile.UserId);
                relation = friendshipRelation.Status;
            }
            catch (Exception)
            {
                relation = (FriendshipTwoWayRelationStatusDtoStatus) (-1);
            }

            switch (relation)
            {
                case FriendshipTwoWayRelationStatusDtoStatus.Friends:
                    addButton.transform.parent.gameObject.SetActive(false);
                    removeButton.transform.parent.gameObject.SetActive(true);
                    removeButton.buttonText = "REMOVE";
                    removeButton.normalText.text = "REMOVE";
                    blockButton.transform.parent.gameObject.SetActive(true);
                    blockButton.buttonText = "BLOCK";
                    blockButton.normalText.text = "BLOCK";
                    unblockButton.transform.parent.gameObject.SetActive(false);
                    break;
                case FriendshipTwoWayRelationStatusDtoStatus.Sentrequest:
                    addButton.transform.parent.gameObject.SetActive(false);
                    removeButton.transform.parent.gameObject.SetActive(true);
                    removeButton.buttonText = "CANCEL REQUEST";
                    removeButton.normalText.text = "CANCEL REQUEST";
                    blockButton.transform.parent.gameObject.SetActive(true);
                    blockButton.buttonText = "BLOCK";
                    blockButton.normalText.text = "BLOCK";
                    unblockButton.transform.parent.gameObject.SetActive(false);
                    break;
                case FriendshipTwoWayRelationStatusDtoStatus.Receivedrequest:
                    addButton.transform.parent.gameObject.SetActive(true);
                    addButton.buttonText = "ACCEPT";
                    addButton.normalText.text = "ACCEPT";
                    removeButton.transform.parent.gameObject.SetActive(true);
                    removeButton.buttonText = "IGNORE";
                    removeButton.normalText.text = "IGNORE";
                    blockButton.transform.parent.gameObject.SetActive(true);
                    blockButton.buttonText = "BLOCK";
                    blockButton.normalText.text = "BLOCK";
                    unblockButton.transform.parent.gameObject.SetActive(false);
                    break;
                case FriendshipTwoWayRelationStatusDtoStatus.Blockedbyother:
                    addButton.transform.parent.gameObject.SetActive(false);
                    removeButton.transform.parent.gameObject.SetActive(false);
                    blockButton.transform.parent.gameObject.SetActive(false);
                    unblockButton.transform.parent.gameObject.SetActive(false);
                    break;
                case FriendshipTwoWayRelationStatusDtoStatus.Blockedbyyou:
                    addButton.transform.parent.gameObject.SetActive(false);
                    removeButton.transform.parent.gameObject.SetActive(false);
                    blockButton.transform.parent.gameObject.SetActive(false);
                    unblockButton.transform.parent.gameObject.SetActive(true);
                    unblockButton.buttonText = "UNBLOCK";
                    break;
                default:
                    addButton.transform.parent.gameObject.SetActive(true);
                    addButton.buttonText = "ADD";
                    removeButton.transform.parent.gameObject.SetActive(false);
                    blockButton.transform.parent.gameObject.SetActive(true);
                    blockButton.buttonText = "BLOCK";
                    unblockButton.transform.parent.gameObject.SetActive(false);
                    break;
            }

            if (isOwnProfile)
            {
                headerText.text = "My Profile";
                usernameInputField.text = profile.DisplayName;
                addButton.transform.parent.gameObject.SetActive(false);
                removeButton.transform.parent.gameObject.SetActive(false);
                blockButton.transform.parent.gameObject.SetActive(false);
                unblockButton.transform.parent.gameObject.SetActive(false);
                groupDropdown.transform.parent.gameObject.SetActive(false);
                inviteButton.transform.parent.gameObject.SetActive(false);
                banButton.transform.parent.gameObject.SetActive(false);
                kickButton.transform.parent.gameObject.SetActive(false);
                promoteButton.transform.parent.gameObject.SetActive(false);
                locationDropdown.selectedItemIndex = Utils.GetDropdownItemIndex(profile.Location);
                locationDropdown.SetupDropdown();
                locationDropdown.UpdateValues();
                languageDropdown.selectedItemIndex = Utils.GetDropdownItemIndex(profile.Language);
                languageDropdown.SetupDropdown();
                languageDropdown.UpdateValues();
            }
            else
            {
                headerText.text = "Profile of " + profile.DisplayName;
                usernameText.text = profile.DisplayName;
                locationText.text = profile.Location;
                languageText.text = profile.Language;

                var membershipsOfMe = await CatervaNetworkManager.Instance.groupControllerClient.GetMembershipsOfMeAsync();

                var adminGroups = new List<Group>();
                foreach (var groupMember in membershipsOfMe.Memberships)
                {
                    if (groupMember.MembershipStatus == GroupMemberMembershipStatus.Groupadmin)
                        adminGroups.Add((await CatervaNetworkManager.Instance.groupControllerClient.FindGroupAsync(groupMember.GroupId)).Group);
                }

                groupDropdown.dropdownItems.Clear();
                groupList.Clear();

                Group activeGroup = null;

                if (adminGroups.Count > 0)
                {
                    foreach (var group in adminGroups)
                    {
                        groupDropdown.CreateNewItemFast(group.Name, null);
                        groupList.Add(group);
                    }

                    groupDropdown.SetupDropdown();
                    activeGroup = groupList[activeGroupIndex];
                    activeGroup.Members = (await CatervaNetworkManager.Instance.groupControllerClient.GetMembersOfGroupAsync(activeGroup.Id)).Memberships;
                }

                groupDropdown.transform.parent.gameObject.SetActive(false);

                if (activeGroup != null)
                {
                    var member = activeGroup.Members.FirstOrDefault(m => m.MemberId == profile.UserId);

                    if (member != null)
                    {
                        switch (member.MembershipStatus)
                        {
                            case GroupMemberMembershipStatus.Groupadmin:
                                inviteButton.transform.parent.gameObject.SetActive(false);
                                banButton.transform.parent.gameObject.SetActive(false);
                                kickButton.transform.parent.gameObject.SetActive(false);
                                promoteButton.transform.parent.gameObject.SetActive(false);
                                break;
                            case GroupMemberMembershipStatus.Groupmember:
                                inviteButton.transform.parent.gameObject.SetActive(false);
                                banButton.transform.parent.gameObject.SetActive(true);
                                kickButton.normalText.text = "KICK";
                                kickButton.buttonText = "KICK";
                                kickButton.transform.parent.gameObject.SetActive(true);
                                promoteButton.transform.parent.gameObject.SetActive(true);
                                groupDropdown.transform.parent.gameObject.SetActive(true);
                                break;
                            case GroupMemberMembershipStatus.Groupinvited:
                                inviteButton.transform.parent.gameObject.SetActive(false);
                                banButton.transform.parent.gameObject.SetActive(false);
                                kickButton.normalText.text = "CANCEL INVITE";
                                kickButton.buttonText = "CANCEL INVITE";
                                kickButton.transform.parent.gameObject.SetActive(true);
                                promoteButton.transform.parent.gameObject.SetActive(false);
                                groupDropdown.transform.parent.gameObject.SetActive(true);
                                break;
                            case GroupMemberMembershipStatus.Groupbanned:
                                inviteButton.transform.parent.gameObject.SetActive(false);
                                banButton.transform.parent.gameObject.SetActive(false);
                                kickButton.normalText.text = "UNBAN";
                                kickButton.buttonText = "UNBAN";
                                kickButton.transform.parent.gameObject.SetActive(true);
                                promoteButton.transform.parent.gameObject.SetActive(false);
                                groupDropdown.transform.parent.gameObject.SetActive(true);
                                break;
                            default:
                                groupDropdown.transform.parent.gameObject.SetActive(true);
                                break;
                        }
                    }
                    else
                    {
                        groupDropdown.transform.parent.gameObject.SetActive(true);
                        inviteButton.transform.parent.gameObject.SetActive(true);
                        banButton.transform.parent.gameObject.SetActive(true);
                        kickButton.transform.parent.gameObject.SetActive(false);
                        promoteButton.transform.parent.gameObject.SetActive(false);
                    }
                }
                else
                {
                    groupDropdown.transform.parent.gameObject.SetActive(false);
                    inviteButton.transform.parent.gameObject.SetActive(false);
                    banButton.transform.parent.gameObject.SetActive(false);
                    kickButton.transform.parent.gameObject.SetActive(false);
                    promoteButton.transform.parent.gameObject.SetActive(false);
                }
            }
        }

        public async void Save()
        {
            var username = usernameInputField.text;
            var location = locationDropdown.selectedText.text;
            var language = languageDropdown.selectedText.text;

            if (string.IsNullOrEmpty(username))
                username = CatervaNetworkManager.Instance.currentProfile.DisplayName;

            var profileDto = await CatervaNetworkManager.Instance.profileControllerClient.UpdateAsync(new UpdateProfileDto
            {
                DisplayName = username,
                Location = location,
                Language = language
            });

            CatervaNetworkManager.Instance.currentProfile = profileDto.Profile;
        }

        public void Refresh()
        {
            activeGroupIndex = groupDropdown.selectedItemIndex;
            SetData(currentProfile);
        }

        public async void Add()
        {
            var id = currentProfile.UserId;
            await CatervaNetworkManager.Instance.friendControllerClient.AddFriendAsync(new ReferenceFriendDto
            {
                Id = id
            });
            var newProfile = await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(id);
            SetData(newProfile.Profile);
        }

        public async void Remove()
        {
            var id = currentProfile.UserId;
            try
            {
                await CatervaNetworkManager.Instance.friendControllerClient.RemoveFriendAsync(new ReferenceFriendDto
                {
                    Id = id
                });
            }
            catch
            {
                // ignored
            }

            var newProfile = await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(id);
            SetData(newProfile.Profile);
        }

        public async void Block()
        {
            var id = currentProfile.UserId;
            await CatervaNetworkManager.Instance.friendControllerClient.BlockAsync(new ReferenceFriendDto
            {
                Id = id
            });
            var newProfile = await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(id);
            SetData(newProfile.Profile);
        }

        public async void Unblock()
        {
            var id = currentProfile.UserId;
            try
            {
                await CatervaNetworkManager.Instance.friendControllerClient.UnblockAsync(new ReferenceFriendDto
                {
                    Id = id
                });
            }
            catch
            {
                // ignored
            }

            var newProfile = await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(id);
            SetData(newProfile.Profile);
        }

        public async void Invite()
        {
            var id = currentProfile.UserId;
            await CatervaNetworkManager.Instance.groupControllerClient.InviteToGroupAsync(groupList[activeGroupIndex].Id, id);
            var newProfile = await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(id);
            SetData(newProfile.Profile);
        }

        public async void Ban()
        {
            var id = currentProfile.UserId;
            await CatervaNetworkManager.Instance.groupControllerClient.BanFromGroupAsync(groupList[activeGroupIndex].Id, id);
            var newProfile = await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(id);
            SetData(newProfile.Profile);
        }

        public async void Kick()
        {
            var id = currentProfile.UserId;
            await CatervaNetworkManager.Instance.groupControllerClient.RemoveMemberOfGroupAsync(groupList[activeGroupIndex].Id, id);
            var newProfile = await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(id);
            SetData(newProfile.Profile);
        }

        public async void Promote()
        {
            var id = currentProfile.UserId;
            await CatervaNetworkManager.Instance.groupControllerClient.PromoteAsync(groupList[activeGroupIndex].Id, id);
            var newProfile = await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(id);
            SetData(newProfile.Profile);
        }
    }
}