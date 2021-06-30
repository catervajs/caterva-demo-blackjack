using System.Globalization;
using CatervaClient;
using CatervaGame.UI.Social;
using TMPro;
using UnityEngine;

namespace CatervaGame.UI.Leaderboard
{
    public class LeaderboardItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI idText;
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI scoreText;

        private CatervaClient.Profile profile;

        public async void SetData(LeaderboardEntryDto entry)
        {
            try
            {
                profile = (await CatervaNetworkManager.Instance.profileControllerClient.FindOneAsync(entry.UserId)).Profile;
                idText.text = profile.DisplayName;
            }
            catch (ApiException e)
            {
                Debug.LogError("No user found with this id. Ex:\n" + e);
                idText.text = entry.UserId;
            }

            rankText.text = (entry.Rank + 1).ToString(CultureInfo.InvariantCulture);
            scoreText.text = entry.Score.ToString(CultureInfo.InvariantCulture);
        }

        public void OpenProfile()
        {
            UserDisplayManager.Instance.OpenProfile(profile);
        }
    }
}