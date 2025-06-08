namespace Service.ExportService
{
    public interface IExport<T>
    {
        string Export(IEnumerable<T> items);
    }
}