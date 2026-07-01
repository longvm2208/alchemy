public class GamePref : PrefBase<GamePref>
{
    public bool IsMergeTut;
    public int LevelIndex;
    public SerializableHashSet<ItemId> DiscoveredItems;
    public SerializableHashSet<int> DiscoveredRecipes;
    public int CoinCount;
    public int HintCount;
    public int ExtraTimeCount;

    public override void Init()
    {
        base.Init();

        IsMergeTut = true;
        LevelIndex = 0;
        DiscoveredItems = new SerializableHashSet<ItemId>()
        {
            ItemId.Air,
            ItemId.Earth,
            ItemId.Fire,
            ItemId.Water,
            ItemId.Time,
        };
        DiscoveredRecipes = new SerializableHashSet<int>();
        CoinCount = 0;
        HintCount = 1;
        ExtraTimeCount = 1;
    }

    public void AddCoin(int count, string placement, bool isTriggerEvent = false)
    {
        Add(ref CoinCount, count);
        //FirebaseManager.Ins.resource_earn("coin", placement, count);
        if (isTriggerEvent) EventBus.Publish<ChangeCoinCount>(default);
    }

    public void RemoveCoin(int count, string placement, string spend_type, bool isTriggerEvent = true)
    {
        Remove(ref CoinCount, count);
        //FirebaseManager.Ins.resource_spend("coin", placement, spend_type, count);
        if (isTriggerEvent) EventBus.Publish<ChangeCoinCount>(default);
    }

    public void AddHint(int count, string placement, bool isTriggerEvent = false)
    {
        Add(ref HintCount, count);
        //FirebaseManager.Ins.booster_earn(placement, "wand", count);
        if (isTriggerEvent) EventBus.Publish<ChangeHintCount>(default);
    }

    public void RemoveHint(int count, string placement, bool isTriggerEvent = false)
    {
        Remove(ref HintCount, count);
        //FirebaseManager.Ins.booster_spend(placement, "wand");
        if (isTriggerEvent) EventBus.Publish<ChangeHintCount>(default);
    }

    public void AddExtraTime(int count, string placement, bool isTriggerEvent = false)
    {
        Add(ref ExtraTimeCount, count);
        //FirebaseManager.Ins.booster_earn(placement, "Booster2", count);
        if (isTriggerEvent) EventBus.Publish<ChangeExtraTimeCount>(default);
    }

    public void RemoveExtraTime(int count, string placement, bool isTriggerEvent = false)
    {
        Remove(ref ExtraTimeCount, count);
        //FirebaseManager.Ins.booster_spend(placement, "Booster2");
        if (isTriggerEvent) EventBus.Publish<ChangeExtraTimeCount>(default);
    }

    void Add(ref int current, int amount)
    {
        if (amount < 0) return;
        current += amount;
    }

    void Remove(ref int current, int amount)
    {
        if (amount < 0 || current - amount < 0) return;
        current -= amount;
    }
}

public struct ChangeCoinCount { }
public struct ChangeHintCount { }
public struct ChangeExtraTimeCount { }
