public class PlayerCreateDTO
{
    public string Nickname { get; set; }
}

public class PlayerPanelDTO
{
    public int Id { get; set; }
    public int Score { get; set; } = 0;
    public string Nickname { get; set; }
}

public class PlayerUpdateDTO
{
    public int Id { get; set; }
    public int Nickname { get; set; }
}

public class PlayerRacingDTO
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public string Answer { get; set; }
    public string FiredAnswer { get; set; }
    public int Score { get; set; } = 0;
    public bool IsReady { get; set; }
    public bool HasAnswered { get; set; } = false;
}