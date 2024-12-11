using UnityEngine;
using DG.Tweening; // Ensure you have DOTween imported and installed

public class JumpToAnswer : IState
{
    private Transform _target;
    private Transform _player;
    private float _jumpPower; // Height of the jump
    private int _numJumps;    // Number of jumps before reaching the target
    private float _duration; // Duration of the jump animation

    public JumpToAnswer(Transform target, Transform player, float jumpPower = 2f, int numJumps = 1, float duration = 1f)
    {
        _target = target;
        _player = player;
        _jumpPower = jumpPower;
        _numJumps = numJumps;
        _duration = duration;
    }

    public void Enter()
    {
        // Start the jump tween
        _player.DOJump(
            _target.position,   // Target position to jump to
            _jumpPower,         // Jump height
            _numJumps,          // Number of jumps
            _duration           // Duration of the tween
        ).SetEase(Ease.OutQuad); // Optional: Set easing for smoother animation
    }

    public void Exit()
    {
        // Kill any running tweens associated with the player (cleanup)
        _player.DOKill();
    }

    public void Update()
    {
        // If needed, you can add logic here for updating during the jump
    }
}
