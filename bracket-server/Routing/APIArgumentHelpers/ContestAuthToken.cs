﻿namespace bracket_server.Routing.APIArgumentHelpers
{
    public class ContestAuthToken
    {
        public AuthToken Token { get; set; } = new AuthToken();
        public string TournamentName { get; set; } = string.Empty;
    }
}
