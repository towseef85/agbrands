using System.Diagnostics;

namespace AGBrand.Packages.Contracts
{
    public interface IContextLogger
    {
        void Commit(EventLogEntryType eventLogEntryType, string id);

        void Log(string message);
    }
}
