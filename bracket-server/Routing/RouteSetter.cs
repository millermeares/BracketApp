namespace bracket_server.Routing
{
    public class RouteSetter
    {
        public enum HttpRequestType { Post, Get }
        private Dictionary<HttpRequestType, Dictionary<string, Delegate>> Routes = new Dictionary<HttpRequestType, Dictionary<string, Delegate>>();
        public RouteSetter()
        {
            
        }

        private void RegisterRoute(HttpRequestType requestType, string route, Delegate method)
        {
            if(!Routes.ContainsKey(requestType))
            {
                Routes.Add(requestType, new Dictionary<string, Delegate>());
            }
            Routes[requestType].Add(route, method);
        }

        public void RegisterRoutes(WebApplication app)
        {
            
        }



        

    }
}
