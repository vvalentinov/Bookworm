﻿namespace Bookworm.Web.ViewModels.Ratings
{
    public class RatingResponseModel
    {
        public double AverageVote { get; set; }

        public int? UserVote { get; set; }

        public int VotesCount { get; set; }
    }
}
