﻿using System.Linq;
using AutoMapper;
using Shuffle.Data.Entities;
using Shuffle.Core.Models;

namespace shuffleboard.core.Models.Mappings
{
    public class TeamRecordMap : Profile
    {
        public TeamRecordMap()
        {
            CreateMap<TeamRecordEntity, TeamRecord>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Team.Name))
                .ForMember(x => x.TeamId, opt => opt.MapFrom(x => x.TeamId))
                .ForMember(x => x.Elo, opt => opt.MapFrom(x => x.Elo))
                .ForMember(x => x.Losses, opt => opt.MapFrom(x => x.Losses))
                .ForMember(x => x.Wins, opt => opt.MapFrom(x => x.Wins))
                .ForMember(x => x.RulesetId, opt => opt.MapFrom(x => x.RulesetId))
                .ForMember(x => x.Color, opt => opt.MapFrom(x => x.Team.Color));
        }
    }
}