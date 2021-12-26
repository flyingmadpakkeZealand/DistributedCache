namespace Sandbox
{
    public class Person
    {
        public short Age { get; set; }

        public string Name { get; set; }

        public float Height { get; set; }

        public float Weight { get; set; }

        public long Atoms { get; set; }

        public Person()
        {
        }

        public Person(string name)
        {
            Name = name;
        }
    }
}
