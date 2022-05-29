using BikeService.Sonic.Const;
using BikeService.Sonic.Dtos;
using BikeService.Sonic.Extensions;
using BikeService.Sonic.Services.Interfaces;
using Nest;

namespace BikeService.Sonic.Services.Implementation;

public class ElasticSearchService : IElasticSearchService
{
    private readonly IElasticClient _elasticClient;

    public ElasticSearchService(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }
    
    public async Task AddBikeStationToIndex(BikeStationSearchDto bikeStationSearchDto)
    {
        await _elasticClient.IndexDocumentAsync(bikeStationSearchDto);
    }

    public async Task UpdateBikeStationRecord(int id, BikeStationSearchDto bikeStationSearchDto)
    {
        await _elasticClient.UpdateAsync<BikeStationSearchDto>(
            bikeStationSearchDto, 
            u => u.Index(ElasticSearchIndex.BikeStationIndex).Doc(bikeStationSearchDto));
    }

    public async Task DeleteBikeStationRecord(int id)
    {
        await _elasticClient.DeleteAsync(new DeleteRequest(ElasticSearchIndex.BikeStationIndex, new Id(id)));
    }

    public async Task<List<BikeStationSearchDto>> SearchBikeStationRecord(string queryString)
    {
        var searchResponse = queryString.Length <= ElasticSearchQuery.ElasticSearchLengthForFuzzySearch ? 
            await GetSearchResultWhenQueryStringLengthIsNotGreaterThanThree(queryString) : 
            await GetSearchResultWhenQueryStringLengthIsGreaterThanThree(queryString);
        
        return searchResponse.Documents.ToList();
    }

    private async Task<ISearchResponse<BikeStationSearchDto>> 
        GetSearchResultWhenQueryStringLengthIsNotGreaterThanThree(string query)
    {
        return await _elasticClient.SearchAsync<BikeStationSearchDto>(s => s
            .Query(q => q.MultiMatch(mm => mm
                .Fields(f => f
                    .Field(ff => ff.NameNormalize)
                    .Field(ff => ff.DescriptionNormalize)
                    .Field(ff => ff.AddressNormalize)
                )
                .Type(TextQueryType.PhrasePrefix)
                .Query(query.ConvertToUnSign())
            ))
        );
    }
    
    private async Task<ISearchResponse<BikeStationSearchDto>> 
        GetSearchResultWhenQueryStringLengthIsGreaterThanThree(string query)
    {
        return await _elasticClient.SearchAsync<BikeStationSearchDto>(s => s
            .Query(q => q.MultiMatch(mm => mm
                .Fields(f => f
                    .Field(ff => ff.NameNormalize)
                    .Field(ff => ff.DescriptionNormalize)
                    .Field(ff => ff.AddressNormalize)
                )
                .Fuzziness(Fuzziness.Auto)
                .Query(query.ConvertToUnSign())
            ))
        );
    }
}
