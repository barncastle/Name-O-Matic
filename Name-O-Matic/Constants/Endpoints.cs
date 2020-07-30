namespace NameOMatic.Constants
{
    internal static class Endpoints
    {
        public const string ListFile = "https://wow.tools/casc/listfile/download/csv/unverified";

        public const string FilesAPI = "https://wow.tools/files/scripts/api.php?start={0}&length=500&search[value]={1}&_={2}";

        public const string DiffAPI = "https://wow.tools/casc/root/diff_api?from={0}&to={1}&_={2}";
    }
}
