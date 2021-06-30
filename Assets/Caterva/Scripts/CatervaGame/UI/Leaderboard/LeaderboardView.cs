using CatervaClient;
using UnityEngine;

namespace CatervaGame.UI.Leaderboard
{
    public class LeaderboardView : MonoBehaviour
    {
        [SerializeField] private LeaderboardItemView viewPrefab;
        [SerializeField] private Transform viewParent;

        private void Start()
        {
            Load();
        }

        public async void Load()
        {
            await Utils.WaitWhile(() => !CatervaNetworkManager.Instance || string.IsNullOrEmpty(CatervaNetworkManager.Instance.GetToken()) || !CatervaNetworkManager.Instance.IsInitialized);
            var entries = await CatervaNetworkManager.Instance.leaderboardControllerClient.GetEntriesAsync("default", "0", "100");
            SetData(entries);
        }

        public void SetData(LeaderboardEntriesDto leaderboard)
        {
            for (var i = 1; i < viewParent.childCount; i++)
            {
                var child = viewParent.GetChild(i);
                Destroy(child.gameObject);
            }

            foreach (var entry in leaderboard.Entries)
            {
                var item = Instantiate(viewPrefab, viewParent);
                item.SetData(entry);
            }
        }
    }
}