namespace VeteranAnalyticsSystem.Contracts;

public interface IGoogleFormsImporterService
{
    Task<int> ImportForms();
}
