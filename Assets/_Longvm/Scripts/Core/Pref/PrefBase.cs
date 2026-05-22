public abstract class PrefBase<T> : IPref where T : PrefBase<T>, new()
{
    static T ins;
    public static T Ins
    {
        get
        {
            if (ins == null)
            {
                ins = PrefUtils.Load<T>();

                if (ins != null && PrefManager.Ins != null)
                {
                    PrefManager.Ins.OnPrefLoaded(ins);
                }
            }

            return ins;
        }
    }

    public virtual void Init() { }

    public virtual void OnAppQuit() { }

    public virtual void Save()
    {
        PrefUtils.Save(ins);
    }
}
