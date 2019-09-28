using System;

namespace Version
{
    public interface IEnvironmentWrapper
    {
        string GetEnvironmentVariable(string variableName);
    }

    public class EnvironmentWrapper : IEnvironmentWrapper
    {
        public string GetEnvironmentVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName);
        }
    }
}