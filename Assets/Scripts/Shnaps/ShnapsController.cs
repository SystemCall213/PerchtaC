using DefaultNamespace.Shnaps.Interfaces;

namespace DefaultNamespace.Shnaps
{
    public class ShnapsController : IShnapsController
    {
        private int shnaps;
        
        public void AddShnaps()
        {
            shnaps++;
        }

        public void RemoveShnaps()
        {
            shnaps--;
        }

        public void ClearShnaps()
        {
            shnaps = 0;
        }

        public bool IsShnapsAvailable()
        {
            return shnaps > 0;
        }
        
        public int GetShnaps()
        {
            return shnaps;
        }
    }
}