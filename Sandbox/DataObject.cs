namespace Sandbox
{
    public class DataObject
    {
        public int HitCount { get; private set; }

        public void OnSomeCommandInvoked(int overrideCounter)
        {
            HitCount = overrideCounter;
        }

        public DataObject()
        {
        }

        public DataObject(int hitCount)
        {
            HitCount = hitCount;
        }
    }
}
