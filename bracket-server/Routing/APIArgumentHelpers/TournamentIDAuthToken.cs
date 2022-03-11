namespace bracket_server.Routing.APIArgumentHelpers
{
    public class TournamentIDAuthToken
    {
        public string TournamentID { get; set; } = string.Empty;
        public AuthToken Token { get; set; } = new AuthToken();
    }
}
