using UnityEngine;

public interface IHitReaction
{
    void Begin();
    bool IsComplete { get; }
    void End();
}
