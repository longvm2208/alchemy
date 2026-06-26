using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] ItemDatabaseConfig itemDatabaseConfig;
    public ItemDatabaseConfig ItemDatabaseConfig => itemDatabaseConfig;
}
