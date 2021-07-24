using eCommerce.Common;

namespace eCommerce.Service
{
    public interface ISystemService
    {
        /// <summary>
        /// Check if the system is in valid state
        /// </summary>
        /// <returns>True if the system is valid</returns>
        public bool IsSystemValid();

        /// <summary>
        /// Return the error message if the system is not valid
        /// </summary>
        /// <param name="message">string to set message in</param>
        /// <returns>True if the err message was set</returns>
        public bool GetErrMessageIfValidSystem(out string message);

        /// <summary>
        /// Initialize the system, must be called before system use.
        /// </summary>
        public bool InitSystem(string[] args);
        
        /// <summary>
        /// Start the server
        /// </summary>
        /// <param name="args">Args</param>
        public void Start(string[] args);

        /// <summary>
        /// Return service instances
        /// </summary>
        /// <param name="configFile">The config file</param>
        /// <returns>Services</returns>
        public Result<Services> GetInstanceForTests(string configFile);
    }
}