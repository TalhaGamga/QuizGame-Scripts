public class SubmitAnswer : ICommand
{
    PlayerPanelDTO _playerPanelDTO;
    string _answer;
    IPlayerManager _playerManager;

    public SubmitAnswer(PlayerPanelDTO playerPanelDTO, string Answer, IPlayerManager playerManager)
    {
        _playerPanelDTO = playerPanelDTO;
        _answer = Answer;
        _playerManager = playerManager;
    }

    public void Execute()
    {
        _playerManager.SubmitAnswerToQuestion(_playerPanelDTO.Id, _answer);
    }

    public void Undo()
    {
    }
}