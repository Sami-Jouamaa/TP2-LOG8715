using UnityEngine;
public struct UnoptimisedStruct1
{
    public UnoptimizedStruct2 mainFriend; // 52
    public float velocity; // 4
    public Vector3 position; // 12
    public float size; // 4
    public double range; // 8
    public int baseHP; // 4
    public int nbAllies; // 4
    public int currentHp; // 4
    public bool isVisible; // 4
    public bool isStanding; // 4
    public bool canJump; // 4
    public float[] distancesFromObjectives; // multiple of 4
    public UnoptimizedStruct2[] otherFriends; // multiple of 52
    public byte colorAlpha; // 1

    public UnoptimisedStruct1(float velocity, bool canJump, int baseHP, int nbAllies, Vector3 position, int currentHp, float[] distancesFromObjectives, byte colorAlpha, double range, UnoptimizedStruct2 mainFriend, bool isVisible, UnoptimizedStruct2[] otherFriends, bool isStanding, float size)
    {
        this.velocity = velocity;
        this.size = size;
        this.baseHP = baseHP;
        this.nbAllies = nbAllies;
        this.currentHp = currentHp;
        this.position = position;
        this.distancesFromObjectives = distancesFromObjectives;
        this.range = range;
        this.mainFriend = mainFriend;
        this.otherFriends = otherFriends;
        this.canJump = canJump;
        this.isVisible = isVisible;
        this.isStanding = isStanding;
        this.colorAlpha = colorAlpha;
    }
}

public enum FriendState
{
    isFolowing,
    isSearching,
    isPatrolling,
    isGuarding,
    isJumping,
}

public struct UnoptimizedStruct2 // 52 or 46 bytes
{
    public Vector3 position; // 12
    public float height; // 4
    public float velocity; // 4
    public float acceleration; // 4
    public float maxVelocity; // 4
    public int currentObjective; // 4
    public double maxSight; // 8
    public FriendState currentState; // 4
    public bool isAlive; // 4 or 1 
    public bool canJump; // 4 or 1

    public UnoptimizedStruct2(bool isAlive, float height, FriendState currentState, float velocity, int currentObjective, double maxSight, bool canJump, float acceleration, Vector3 position, float maxVelocity)
    {
        this.height = height;
        this.velocity = velocity;
        this.maxVelocity = maxVelocity;
        this.acceleration = acceleration;
        this.maxSight = maxSight;
        this.currentObjective = currentObjective;
        this.position = position;
        this.currentState = currentState;
        this.isAlive = isAlive;
        this.canJump = canJump;
    }
}
