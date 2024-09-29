using Microsoft.Extensions.Configuration;

namespace dynamics365accelerator.Support.Utils
{
    ///<Summary>
    //A static interface to provide uniform access to the environment variables
    ///</Summary>

    public static class EnvConfig
    {
        private static IConfiguration? config;

        public static IConfiguration Get()
        {
            if(config is null) ConstructConfig();
            return config!;
        }

        private static void ConstructConfig()
        {
            config = new ConfigurationBuilder()
                .AddJsonFile("env.json", optional:true)
                .AddEnvironmentVariables()
                .Build();
        }

        //Completed
    }
}