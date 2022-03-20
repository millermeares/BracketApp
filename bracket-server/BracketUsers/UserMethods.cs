using bracket_server.Tournaments;

namespace bracket_server.BracketUsers
{
    public class UserMethods
    {
        public static MenuOptions GetMenuOptionsForTournamentUser(ITournamentDAL dal)
        {
            MenuOptions options = new MenuOptions();
            ConditionalAddFillOutBracketMenuOption(dal, ref options);
            // add completed bracket, 
            options.Add(new MenuOption("/completedbrackets", "Completed Brackets"));
            options.Add(new MenuOption("/userexposurereport", "Exposure Report"));
            return options;
        }

        private static void ConditionalAddFillOutBracketMenuOption(ITournamentDAL dal, ref MenuOptions options)
        {
            return; // todo. probably look up the active active tournament id and make sure that it hasn't started yet.
        }
    }
}
