using UnityEngine;
public struct UnoptimisedStruct1 // 128 bytes
{
    public UnoptimizedStruct2 mainFriend; // 56 : 56
    public float velocity; // 4 : 60
    public Vector3 position; // 12 : 72
    public double range; // 8 : 80
    public float[] distancesFromObjectives; // 8 : 88
    public UnoptimizedStruct2[] otherFriends; // 8 : 96
    public float size; // 4 : 100
    public int baseHP; // 4 : 104
    public int nbAllies; // 4 : 108
    public int currentHp; // 4 : 112
    public bool isVisible; // 4 : 116
    public bool isStanding; // 4 : 120
    public bool canJump; // 4 : 124
    public byte colorAlpha; // 1 : 125
    // + 3 bytes of padding to 128 bytes

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

public struct UnoptimizedStruct2 // 56 bytes
{
    public Vector3 position; // 12 : 12
    public float height; // 4 : 16
    public float velocity; // 4 : 20
    public float acceleration; // 4 : 24
    public float maxVelocity; // 4 : 28
    public int currentObjective; // 4 : 32
    public double maxSight; // 8 : 40
    public FriendState currentState; // 4 : 44
    public bool isAlive; // 4 : 48
    public bool canJump; // 4 : 52
    // + 4 bytes of padding to 56 bytes

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
