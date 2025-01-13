namespace pdtapp.PrecipitationDataImporter
{
    public interface IPrecipitationFileImporter
    {
        bool ImportFile(string precipitationFileFolder);
    }
}