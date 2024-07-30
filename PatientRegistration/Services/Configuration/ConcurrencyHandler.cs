using Microsoft.EntityFrameworkCore;
using PatientRegistration.Infrastructure.Configuration;
using PatientRegistration.Infrastructure.DbModels;

namespace PatientRegistration.Services.Configuration
{
    public static class ConcurrencyHandler
    {
        public static async Task SaveChangesWithConcurrencyHandlingAsync(PatientDbContext context)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is PatientDetail || entry.Entity is PatientLabVisit ||
                            entry.Entity is PatientMedication || entry.Entity is PatientVaccinationDatum)
                        {
                            var databaseEntity = await entry.GetDatabaseValuesAsync();
                            if (databaseEntity != null)
                            {
                                entry.OriginalValues.SetValues(databaseEntity);
                            }
                        }
                        else
                        {
                            throw new NotSupportedException($"There is concurrency conflicts for {entry.Metadata.Name}");
                        }
                    }
                }
            } while (saveFailed);
        }
    }
}
