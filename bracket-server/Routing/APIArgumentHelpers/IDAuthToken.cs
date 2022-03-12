namespace bracket_server.Routing.APIArgumentHelpers
{
    public class IDAuthToken
    {
        public AuthToken Token { get; set; } = new AuthToken();
        public string ID { get; set; } = string.Empty;
    }
}
