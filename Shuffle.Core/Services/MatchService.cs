﻿using System.Linq;
using System.Collections.Generic;
using AutoMapper.QueryableExtensions;
using Shuffle.Data;
using Shuffle.Data.Entities;
using Shuffle.Core.Models;

namespace Shuffle.Core.Services
{
    public class MatchService : IMatchService
    {
        private readonly ShuffleDbContext _db;

        enum GameOutcome
        {
            Win = 1,
            Loss = 0
        }

        public MatchService(ShuffleDbContext context)
        {
            _db = context;
        }

        public Match GetMatch(int Id)
        {
            var match = _db. Matches.Where(x => x.Id == Id).ProjectTo<Match>().FirstOrDefault();

            return match;
        }

        public List<Match> GetMatches()
        {
            var matches = _db.Matches.ProjectTo<Match>().ToList();

            return matches;
        }

        public Match CreateMatch(Match matchToCreate)
        {
            var newMatch = new MatchEntity
            {
                ChallengerId = matchToCreate.ChallengerId,
                OppositionId = matchToCreate.OppositionId,
                ChallengerScore = null,
                OppositionScore = null,
                MatchDate = matchToCreate.MatchDate,
                RulesetId = matchToCreate.RulesetId
            };

            _db.Matches.Add(newMatch);

            var result = _db.SaveChanges();

            return new Match
            {
                ChallengerId = matchToCreate.ChallengerId,
                OppositionId = matchToCreate.OppositionId,
                MatchDate = matchToCreate.MatchDate,
                RulesetId = matchToCreate.RulesetId,
                Id = result
            };
        }

        public void CompleteMatch(int Id, Score finalScore)
        {
            var match = GetMatch(Id);

            var challenger = _db.TeamRecords.FirstOrDefault(x => x.TeamId == match.ChallengerId && x.RulesetId == match.RulesetId);
            var opposition = _db.TeamRecords.FirstOrDefault(x => x.TeamId == match.OppositionId && x.RulesetId == match.RulesetId);

            var outcome = DetermineWinner(finalScore);

            var eloChange = CalculateELO(challenger.Elo, opposition.Elo, outcome);
            
            challenger.Elo += eloChange;
            opposition.Elo -= eloChange;

            _db.SaveChanges();
        }

        static double ExpectationToWin(int playerOneRating, int playerTwoRating)
        {
            return 1 / (1 + System.Math.Pow(10, (playerTwoRating - playerOneRating) / 400.0));
        }

        static int CalculateELO(int playerOneRating, int playerTwoRating, GameOutcome outcome)
        {
            int eloK = 32;

            int delta = (int)(eloK * ((int)outcome - ExpectationToWin(playerOneRating, playerTwoRating)));

            return delta;
        }

        static GameOutcome DetermineWinner(Score score) {
            if (score.ChallengerScore > score.OppositionScore) return GameOutcome.Win;

            return GameOutcome.Loss;
        }
    }
}