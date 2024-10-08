﻿using Core.Dto.Response;
using Core.Entities.NameEntry;
using Core.Enums;
using Core.Events;
using Core.Repositories;
using Core.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.MongoDB.Repositories;

public class NameEntryRepository : MongoDBRepository, INameEntryRepository
{
    private readonly IMongoCollection<NameEntry> _nameEntryCollection;
    private readonly IEventPubService _eventPubService;

    public NameEntryRepository(
        IMongoDatabase database,
        IEventPubService eventPubService)
    {
        _nameEntryCollection = database.GetCollection<NameEntry>("NameEntries");
        _eventPubService = eventPubService;

        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var indexKeys = Builders<NameEntry>.IndexKeys.Ascending(x => x.Name);
        var indexOptions = new CreateIndexOptions
        {
            Unique = true,
            Name = "IX_NameEntries_Name_Unique", 
            Background = true
        };
        _nameEntryCollection.Indexes.CreateOne(new CreateIndexModel<NameEntry>(indexKeys, indexOptions));
    }

    public async Task<NameEntry> FindById(string id)
    {
        return await _nameEntryCollection.Find(x => x.Id == id).SingleOrDefaultAsync();
    }

    public async Task<bool> DeleteAll()
    {
        var deleteResult = await _nameEntryCollection.DeleteManyAsync(FilterDefinition<NameEntry>.Empty);
        return deleteResult.DeletedCount > 0;
    }


    public async Task Create(NameEntry entry)
    {
        entry.Id = ObjectId.GenerateNewId().ToString();
        await _nameEntryCollection.InsertOneAsync(entry);
    }

    public async Task Create(List<NameEntry> entries)
    {
        entries.ForEach(entry => entry.Id = ObjectId.GenerateNewId().ToString()!);
        await _nameEntryCollection.InsertManyAsync(entries);
    }

    public async Task<int> CountByState(State state)
    {
        return await CountWhere(ne => ne.State == state);
    }

    public async Task Delete(string name)
    {
        var filter = Builders<NameEntry>.Filter.Eq("Name", name);
        var options = SetCollationPrimary<DeleteOptions>(new DeleteOptions());

        await _nameEntryCollection.DeleteOneAsync(filter, options);
    }
    public async Task DeleteMany(string[] names)
    {
        var filter = Builders<NameEntry>.Filter.In("Name", names);
        var options = SetCollationPrimary<DeleteOptions>(new DeleteOptions());

        await _nameEntryCollection.DeleteManyAsync(filter, options);
    }

    public async Task<bool> DeleteByNameAndState(string name, State state)
    {
        var filter = Builders<NameEntry>
            .Filter
            .And(Builders<NameEntry>.Filter.Eq("Name", name), Builders<NameEntry>.Filter.Eq("State", state));

        var options = SetCollationPrimary<DeleteOptions>(new DeleteOptions());

        var deleteResult = await _nameEntryCollection.DeleteOneAsync(filter, options);

        return deleteResult.DeletedCount > 0;
    }

    // TODO Hafiz (Later): This is pulling too much data. We should eventually get rid of it.
    public async Task<HashSet<NameEntry>> ListAll()
    {
        var allEntries = await _nameEntryCollection.Find(_ => true).ToListAsync();
        return new HashSet<NameEntry>(allEntries);
    }

    public async Task<NameEntry?> FindByName(string name)
    {
        var filter = Builders<NameEntry>.Filter.Eq("Name", name);
        var options = SetCollationPrimary<FindOptions>(new FindOptions());

        return await _nameEntryCollection.Find(filter, options).SingleOrDefaultAsync();
    }

    public async Task<List<NameEntry>> FindByNames(string[] names)
    {
        var filter = Builders<NameEntry>.Filter.In("Name", names);
        var options = SetCollationPrimary<FindOptions>(new FindOptions());

        return await _nameEntryCollection.Find(filter, options).ToListAsync();
    }

    public async Task<NameEntry?> FindByNameAndState(string name, State state)
    {
        var options = SetCollationPrimary<FindOptions>(new FindOptions());
        return await _nameEntryCollection
                            .Find(ne => ne.Name == name && ne.State == state, options)
                            .SingleOrDefaultAsync();
    }

    public async Task<HashSet<NameEntry>> FindByNameStartingWithAndState(string searchTerm, State state)
    {
        return await FindByNameStartingWithAnyAndState(new string[] { searchTerm }, state);
    }

    public async Task<HashSet<NameEntry>> FindByNameStartingWithAnyAndState(IEnumerable<string> searchTerms, State state)
    {
        var regexFilters = searchTerms.Select(term =>
        {
            return Builders<NameEntry>.Filter.Regex(ne => ne.Name, 
                new BsonRegularExpression($"^{term.ReplaceYorubaVowelsWithPattern()}", "i"));
        });

        var namesFilter = Builders<NameEntry>.Filter.Or(regexFilters);
        var stateFilter = Builders<NameEntry>.Filter.Eq(ne => ne.State, state);
        var combinedFilter = Builders<NameEntry>.Filter.And(namesFilter, stateFilter);

        var result = await _nameEntryCollection.Find(combinedFilter).ToListAsync();
        return new HashSet<NameEntry>(result);
    }


    public async Task<List<NameEntry>> FindByState(State state)
    {
        return await _nameEntryCollection.Find(ne => ne.State == state).ToListAsync();
    }

    public async Task<HashSet<NameEntry>> FindNameEntryByExtendedMeaningContainingAndState(string name, State state)
    {
        var filter = Builders<NameEntry>.Filter.Regex(ne => ne.ExtendedMeaning, 
            new BsonRegularExpression(name.ReplaceYorubaVowelsWithPattern(), "i")) &
            Builders<NameEntry>.Filter.Eq(ne => ne.State, state);
        var result = await _nameEntryCollection.Find(filter).ToListAsync();
        return new HashSet<NameEntry>(result);
    }

    public async Task<HashSet<NameEntry>> FindNameEntryByMeaningContainingAndState(string name, State state)
    {
        var filter = Builders<NameEntry>.Filter.Regex(ne => ne.Name, 
            new BsonRegularExpression(name.ReplaceYorubaVowelsWithPattern(), "i"))
                                    & Builders<NameEntry>.Filter.Eq(ne => ne.State, state);
        var result = await _nameEntryCollection.Find(filter).ToListAsync();
        return new HashSet<NameEntry>(result);
    }

    public async Task<HashSet<NameEntry>> FindNameEntryByNameContainingAndState(string name, State state)
    {
        var filter = Builders<NameEntry>.Filter.Regex(ne => ne.Name, 
            new BsonRegularExpression(name.ReplaceYorubaVowelsWithPattern(), "i")) &
             Builders<NameEntry>.Filter.Eq(ne => ne.State, state);
        var result = await _nameEntryCollection.Find(filter).ToListAsync();
        return new HashSet<NameEntry>(result);
    }

    public async Task<HashSet<NameEntry>> FindNameEntryByVariantsContainingAndState(string name, State state)
    {
        var regex = new BsonRegularExpression(name.ReplaceYorubaVowelsWithPattern(), "i");
        var filter = Builders<NameEntry>.Filter.Regex(ne => ne.Variants, regex) & Builders<NameEntry>.Filter.Eq(ne => ne.State, state);
        var result = await _nameEntryCollection.Find(filter).ToListAsync();
        return new HashSet<NameEntry>(result);
    }

    public async Task<NameEntry?> Update(string originalName, NameEntry newEntry)
    {
        var filter = Builders<NameEntry>.Filter.Eq(ne => ne.Name, originalName);
        var updateStatement = GenerateUpdateStatement(newEntry);

        var options = new FindOneAndUpdateOptions<NameEntry>
        {
            ReturnDocument = ReturnDocument.After
        };

        var updated = await _nameEntryCollection.FindOneAndUpdateAsync(filter, updateStatement, options);

        if (updated == null)
        {
            await _eventPubService.PublishEvent(new NonExistingNameUpdateAttempted(originalName));
        }
        else if (originalName != newEntry.Name)
        {
            await _eventPubService.PublishEvent(new NameEntryNameUpdated(originalName, newEntry.Name));
        }

        return updated;
    }

    public async Task<int> CountWhere(Expression<Func<NameEntry, bool>> filter)
    {
        var count = await _nameEntryCollection.CountDocumentsAsync(filter);
        return (int)count;
    }

    public async Task<IEnumerable<NameEntry>> GetAllNames(State? state, string? submittedBy)
    {
        var filterBuilder = Builders<NameEntry>.Filter;
        var filter = filterBuilder.Empty;

        if (state.HasValue)
        {
            filter &= filterBuilder.Eq(ne => ne.State, state.Value);
        }

        if (!string.IsNullOrWhiteSpace(submittedBy))
        {
            filter &= filterBuilder.Eq(ne => ne.CreatedBy, submittedBy.Trim());
        }

        var projection = Builders<NameEntry>.Projection.Include(ne => ne.Name).Exclude(ne => ne.Id);

        var names = await _nameEntryCollection
                            .Find(filter)
                            .Project<NameEntry>(projection)
                            .Sort(Builders<NameEntry>.Sort.Ascending(ne => ne.CreatedAt))
                            .ToListAsync();

        return names;
    }

    public async Task<List<NameEntry>> List(int pageNumber, int pageSize, State? state, string? submittedBy)
    {
        var filterBuilder = Builders<NameEntry>.Filter;
        var filter = filterBuilder.Empty;

        if (state.HasValue)
        {
            filter &= filterBuilder.Eq(ne => ne.State, state.Value);
        }

        if (!string.IsNullOrWhiteSpace(submittedBy))
        {
            filter &= filterBuilder.Eq(ne => ne.CreatedBy, submittedBy.Trim());
        }

        int skip = (pageNumber - 1) * pageSize;
        var names = await _nameEntryCollection
                            .Find(filter)
                            .Skip(skip)
                            .Limit(pageSize)
                            .Sort(Builders<NameEntry>.Sort.Ascending(ne => ne.CreatedAt))
                            .ToListAsync();
        return names;
    }

    public async Task<NamesMetadataDto> GetMetadata()
    {
        return new NamesMetadataDto
        {
            TotalNames = await _nameEntryCollection.CountDocumentsAsync(FilterDefinition<NameEntry>.Empty),
            TotalNewNames = await _nameEntryCollection.CountDocumentsAsync(Builders<NameEntry>.Filter.Eq(x => x.State, State.NEW)),
            TotalModifiedNames = await _nameEntryCollection.CountDocumentsAsync(Builders<NameEntry>.Filter.Eq(x => x.State, State.MODIFIED)),
            TotalPublishedNames = await _nameEntryCollection.CountDocumentsAsync(Builders<NameEntry>.Filter.Eq(x => x.State, State.PUBLISHED))
        };
    }

    private static UpdateDefinition<NameEntry> GenerateUpdateStatement(NameEntry newEntry)
    {
        var statement = Builders<NameEntry>.Update
                    .Set(ne => ne.Name, newEntry.Name)
                    .Set(ne => ne.State, newEntry.State)
                    .Set(ne => ne.Pronunciation, newEntry.Pronunciation)
                    .Set(ne => ne.IpaNotation, newEntry.IpaNotation)
                    .Set(ne => ne.Meaning, newEntry.Meaning)
                    .Set(ne => ne.ExtendedMeaning, newEntry.ExtendedMeaning)
                    .Set(ne => ne.Morphology, newEntry.Morphology)
                    .Set(ne => ne.Media, newEntry.Media)
                    .Set(ne => ne.State, newEntry.State)
                    .Set(ne => ne.Etymology, newEntry.Etymology)
                    .Set(ne => ne.Videos, newEntry.Videos)
                    .Set(ne => ne.GeoLocation, newEntry.GeoLocation)
                    .Set(ne => ne.FamousPeople, newEntry.FamousPeople)
                    .Set(ne => ne.Syllables, newEntry.Syllables)
                    .Set(ne => ne.Variants, newEntry.Variants)
                    .Set(ne => ne.Modified, newEntry.Modified)
                    .Set(ne => ne.Duplicates, newEntry.Duplicates)
                    .CurrentDate(ne => ne.UpdatedAt);

        if (!string.IsNullOrWhiteSpace(newEntry.UpdatedBy))
        {
            statement = statement.Set(ne => ne.UpdatedBy, newEntry.UpdatedBy);
        }

        if (newEntry.Duplicates.Any())
        {
            statement = statement.Set(ne => ne.Duplicates, newEntry.Duplicates);
        }

        return statement;
    }
}