﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace miniproject2
{
    class DetermineIfLikelyToBuy
    {
        public DetermineIfLikelyToBuy(Clusterer clusterMachine, Classifier classifier, List<List<int>> communities, List<int> userIndexes, List<string> userNames)
        {
            ClusterMachine = clusterMachine;
            ClassifierMachine = classifier;
            Communities = communities;
            UserIndexes = userIndexes;
            //UserNames = userNames;

            UserNames = ClusterMachine.People.Select(p => p.name).ToList();
            Persons = txtParser.Instance.parseTxt(@"../../../friendships.reviews.txt");
        }

        private Clusterer ClusterMachine { get; set; }
        private Classifier ClassifierMachine { get; set; }
        private List<List<int>> Communities { get; set; }
        private List<int> UserIndexes { get; set; }
        private List<string> UserNames { get; set; }
        private List<Person> Persons { get; set; }

        private int GetCommunityIndex(int userID)
        {
            for (int i = 0; i < Communities.Count; i++)
            {
                if (Communities[i].Contains(userID))
                {
                    return i;
                }
            }

            throw new Exception();
        }

        public string BoolToOneOrFive(bool val)
        {
            return val ? "5" : "1";
        }

        public List<Tuple<string, string, string>> WillUsersBuy()
        {
            // classifier.learn

            var result = new List<Tuple<string, string, string>>();

            foreach (var review in Persons)
            {
                var userID = ClusterMachine.PersonNameIndex[review.name];
                int communityIndex = GetCommunityIndex(userID);

                // has not bought
                if (review.review != "*")
                {
                    // spørg venner!
                    bool IsRecommended = IsRecommendedFromFriends(userID, communityIndex);
                }
                else // has bought
                {
                    var sentBool = ClassifierMachine.reteReview(GetReviewFromString(review.review));

                    var res = new Tuple<string, string, string>(review.name, BoolToOneOrFive(sentBool), "*");
                    result.Add(res);
                }
            }

            return result;
        }

        private bool IsRecommendedFromFriends(int userID, int communityIndex)
        {
            int friendSentiments = 0;
            var friends = ClusterMachine.NeighBours[userID];

            foreach (var friend in friends)
            {
                var friendReview = GetReviewFromName(UserNames[friend]);
                var sentBool = ClassifierMachine.reteReview(friendReview);
                var friendCommunity = GetCommunityIndex(friend);

                if (sentBool)
                {
                    friendSentiments += (communityIndex != friendCommunity || UserNames[friend] == "kyle" ? 10 : 1);
                }
                else
                {
                    friendSentiments -= (communityIndex != friendCommunity || UserNames[friend] == "kyle" ? 10 : 1);
                }
            }

            return friendSentiments > 0;
        }

        private Review GetReviewFromString(string str)
        {
            return new Review() { review = str };
        }

        private Review GetReviewFromName(string name)
        {
            return Persons
                .Where(p => p.name == name)
                .Select(p => GetReviewFromString(p.review))
                .First();
        }
    }
}
