namespace bracket_server.Routing.APIArgumentHelpers
{
    public class TournamentAuthToken
    {
        public string TournamentID { get; set; } = string.Empty;
        public AuthToken Token { get; set; } = new AuthToken();
    }
}
