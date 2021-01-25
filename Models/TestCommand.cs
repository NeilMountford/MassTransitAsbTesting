namespace Models
{
    public class TestCommand : ITestCommand
    {
        public string TheThingToDo { get; set; }

        public TestCommand()
        {
            TheThingToDo = "Dance!";
        }
    }
}