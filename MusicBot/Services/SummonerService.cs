using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.SummonerV4;

namespace MusicBot.Services
{
    public class SummonerService
    {
        //DUNNO
        private readonly RiotApi _api;

        public SummonerService(RiotApi api)
        {
            _api = api ?? RiotApi.NewInstance(new RiotApiConfig.Builder("RGAPI-9a9db252-dbea-4ec9-8792-70b0c59e4d1d")
                {
                    MaxConcurrentRequests = 200,
                    Retries = 10,
                }.Build()
            );
        }
    }
}
