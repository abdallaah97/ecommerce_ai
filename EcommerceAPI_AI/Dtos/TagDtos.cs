namespace EcommerceAPI_AI.Dtos
{
    public class CreateTagInput
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TagOutput
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class AssignTagsInput
    {
        public List<int> TagIds { get; set; } = new();
    }
}

