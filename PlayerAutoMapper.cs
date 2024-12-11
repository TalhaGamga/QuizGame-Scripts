using System.Collections.Generic;

public static class PlayerAutoMapper
{
    public static Player MapPlayerCreateDtoToPlayer(PlayerCreateDTO playerCreateDTO)
    {
        return new Player
        {
            NickName = playerCreateDTO.Nickname
        };
    }

    public static Player MapPlayerUpdateDtoToPlayer(PlayerUpdateDTO playerUpdateDTO)
    {
        return new Player
        {
            Score = playerUpdateDTO.Nickname
        };
    }

    public static PlayerPanelDTO MapPlayerToPlayerPanelDto(Player player)
    {
        return new PlayerPanelDTO
        {
            Id = player.Id,
            Nickname = player.NickName
        };
    }

    public static PlayerRacingDTO MapPlayerToPlayerRacingDto(Player player)
    {
        return new PlayerRacingDTO
        {
            Id = player.Id,
            Nickname = player.NickName,
        };
    }

    public static List<PlayerRacingDTO> MapPlayersToPlayerRacingDtos(List<Player> players)
    {
        List<PlayerRacingDTO> racingPlayers = new List<PlayerRacingDTO>();

        foreach (var player in players)
        {
            PlayerRacingDTO playerRacingDTO = MapPlayerToPlayerRacingDto(player);
            racingPlayers.Add(playerRacingDTO);
        }

        return racingPlayers;
    }
}