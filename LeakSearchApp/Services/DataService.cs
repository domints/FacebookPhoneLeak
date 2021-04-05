using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeakSearchApp.Database;
using LeakSearchApp.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.Bulk;

namespace LeakSearchApp.Services
{
    public class DataService : IDataService
    {
        private readonly LeakContext _cx;
        private readonly IHashService _hashService;

        public DataService(
            LeakContext context,
            IHashService hashService)
        {
            _cx = context;
            _hashService = hashService;
        }

        public async Task<int> AddCollection(string name)
        {
            var cleanName = name.Trim();
            var existingColl = await _cx.Collections.FirstOrDefaultAsync(c => c.Name == cleanName);
            if (existingColl != null)
                throw new InvalidOperationException("Such collection already exists.");

            var newColl = new Collection
            {
                Name = name.Trim()
            };

            _cx.Collections.Add(newColl);
            await _cx.SaveChangesAsync();
            return newColl.Id;
        }

        public async Task AddEntries(int collectionId, IEnumerable<InsertEntry> entries)
        {
            var existingColl = await _cx.Collections.FindAsync(collectionId);
            if (existingColl == null)
                throw new InvalidOperationException($"Such collection (Id:{collectionId}) does not exist.");

            await BulkInserEntries(collectionId, entries);
        }

        private async Task BulkInserEntries(int collectionId, IEnumerable<InsertEntry> entries)
        {
            using (var conn = new NpgsqlConnection(_cx.Database.GetConnectionString()))
            {
                await conn.OpenAsync().ConfigureAwait(false);
                using var importer = conn.BeginBinaryImport(@"
            COPY entries (
                phone_hash,
                id_hash,
                has_name,
                has_gender,
                has_living_place,
                has_coming_place,
                has_relationship_status,
                has_workplace,
                has_email,
                has_birthdate,
                collection_id) FROM STDIN (FORMAT binary)");

                foreach (var entry in entries)
                {
                    var phonePrivateHash = _hashService.GeneratePrivateHash(entry.PhoneHash);
                    var idPrivateHash = _hashService.GeneratePrivateHash(entry.IdHash);
                    await importer.StartRowAsync();
                    await importer.WriteAsync(phonePrivateHash, NpgsqlTypes.NpgsqlDbType.Bytea);
                    await importer.WriteAsync(idPrivateHash, NpgsqlTypes.NpgsqlDbType.Bytea);
                    await importer.WriteAsync(entry.HasName);
                    await importer.WriteAsync(entry.HasGender);
                    await importer.WriteAsync(entry.HasLivingPlace);
                    await importer.WriteAsync(entry.HasComingPlace);
                    await importer.WriteAsync(entry.HasRelationshipStatus);
                    await importer.WriteAsync(entry.HasWorkplace);
                    await importer.WriteAsync(entry.HasEmail);
                    await importer.WriteAsync(entry.HasBirthdate);
                    await importer.WriteAsync(collectionId);
                }

                await importer.CompleteAsync();
            }
        }

        private Entry ModelToDb(InsertEntry entry, int collectionId)
        {
            return new Entry
            {
                CollectionId = collectionId,
                PhoneHash = _hashService.GeneratePrivateHash(entry.PhoneHash),
                IdHash = _hashService.GeneratePrivateHash(entry.IdHash),
                HasBirthdate = entry.HasBirthdate,
                HasComingPlace = entry.HasComingPlace,
                HasEmail = entry.HasEmail,
                HasGender = entry.HasGender,
                HasLivingPlace = entry.HasLivingPlace,
                HasName = entry.HasName,
                HasRelationshipStatus = entry.HasRelationshipStatus,
                HasWorkplace = entry.HasWorkplace
            };
        }
    }
}