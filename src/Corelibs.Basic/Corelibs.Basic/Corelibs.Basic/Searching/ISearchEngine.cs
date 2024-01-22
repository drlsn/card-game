namespace Corelibs.Basic.Searching
{
    public interface ISearchEngine<T>
    {
        bool Add(SearchIndexData data);
        bool Add(IEnumerable<SearchIndexData> data);
        bool Update(SearchIndexData data, string newName);
        bool Delete(SearchIndexData data);

        SearchIndexData[] Search(string name, SearchType searchType = SearchType.Full);
    }

    public record SearchIndexData(string ID, string Name);
    
    public enum SearchType
    {
        Substring,
        Full,
        Start,
        End
    }

    public static class SearchEngineExtensions
    {
        public static bool Add<T>(this ISearchEngine<T> engine, string id, string name) =>
            engine.Add(new SearchIndexData(id, name));
    }
}
