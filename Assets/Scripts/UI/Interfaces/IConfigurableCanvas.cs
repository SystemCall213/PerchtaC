namespace UI.Interfaces
{
    public interface IConfigurableCanvas
    {
        void Open();
        void Close();
        
        bool ClosableWithEscape();
        bool PausesTime();
    }
}