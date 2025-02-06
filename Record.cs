using System;

public class Record
{
    public int Id { get; set; }
    public string ImageName { get; set; }
    public string DifficultyLevel { get; set; }
    public int Moves { get; set; }
    public TimeSpan BestTime { get; set; }
}
