using CatervaClient;
using TMPro;
using UnityEngine;

namespace CatervaGame.UI.Groups
{
    public class GroupListItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI groupNameText;
        [SerializeField] private GameObject adminImage;

        private Group group;
        
        public void SetData(GroupMember member)
        {
            adminImage.SetActive(member.MembershipStatus == GroupMemberMembershipStatus.Groupadmin);
            groupNameText.text = member.Group.Name;
            group = member.Group;
        }

        public void OpenGroup()
        {
            GroupDisplayManager.Instance.OpenGroup(group);
        }
    }
}