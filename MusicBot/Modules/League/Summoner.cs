using System;
using System.Collections.Generic;
using System.Text;

namespace MusicBot.Modules.League
{
    public struct Summoner
    {
        public string SummonerName { get; private set; }
        public int LeaguePoints { get; private set; }
        public int Wins { get; private set; }
        public int Losses { get; private set; }

        public Summoner(string summonerName, int leaguePoints, int wins, int losses)
        {
            SummonerName = summonerName;
            LeaguePoints = leaguePoints;
            Wins = wins;
            Losses = losses;
        }
    }
}
