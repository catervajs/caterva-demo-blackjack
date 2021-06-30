using System.Collections.Generic;
using System.Linq;
using CatervaClient;
using UnityEngine;

namespace CatervaGame.UI.Social
{
    public class UserListView : MonoBehaviour
    {
        [SerializeField] private UserListItemView userListItemPrefab;
        [SerializeField] private Transform itemParent;
        [SerializeField] private FriendshipTwoWayRelationStatusDtoStatus[] statusToShow;

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

            if (statusToShow.Length > 0)
                GetYourRelations();
        }

        private async void GetYourRelations()
        {
            await Utils.WaitWhile(() => !CatervaNetworkManager.Instance || string.IsNullOrEmpty(CatervaNetworkManager.Instance.GetToken()) || !CatervaNetworkManager.Instance.IsInitialized);
            var relations = await CatervaNetworkManager.Instance.friendControllerClient.FindRelationsOfMeAsync();
            var users = new List<CatervaClient.Profile>();
            if (statusToShow.Contains(FriendshipTwoWayRelationStatusDtoStatus.Friends))
            {
                foreach (var friendId in relations.Friends)
                {
                    var user = await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(friendId);
                    users.Add(user.Profile);
                }
            }

            if (statusToShow.Contains(FriendshipTwoWayRelationStatusDtoStatus.Receivedrequest))
            {
                foreach (var friendId in relations.ReceivedRequests)
                {
                    var user = await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(friendId);
                    users.Add(user.Profile);
                }
            }

            if (statusToShow.Contains(FriendshipTwoWayRelationStatusDtoStatus.Blockedbyother))
            {
                foreach (var friendId in relations.BlockedByOther)
                {
                    var user = await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(friendId);
                    users.Add(user.Profile);
                }
            }

            if (statusToShow.Contains(FriendshipTwoWayRelationStatusDtoStatus.Blockedbyyou))
            {
                foreach (var friendId in relations.BlockedByYou)
                {
                    var user = await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(friendId);
                    users.Add(user.Profile);
                }
            }

            if (statusToShow.Contains(FriendshipTwoWayRelationStatusDtoStatus.Sentrequest))
            {
                foreach (var friendId in relations.SentRequests)
                {
                    var user = await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(friendId);
                    users.Add(user.Profile);
                }
            }

            SetData(users);
        }

        public List<UserListItemView> SetData(IEnumerable<CatervaClient.Profile> users)
        {
            var data = new List<UserListItemView>();
            
            for (var i = 0; i < itemParent.childCount; i++)
            {
                var child = itemParent.GetChild(i);
                Destroy(child.gameObject);
            }

            foreach (var user in users)
            {
                var item = Instantiate(userListItemPrefab, itemParent);
                item.SetData(user);
                data.Add(item);
            }

            return data;
        }
    }
}