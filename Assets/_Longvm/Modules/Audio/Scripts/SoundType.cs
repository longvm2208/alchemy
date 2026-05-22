using UnityEngine;

public enum SoundType
{
    [HideInInspector]
    None = -1,
    sfx_game_block_correct,
    sfx_game_block_down_1,
    sfx_game_block_down_2,
    sfx_game_block_down_3,
    sfx_game_block_down_4,
    sfx_game_block_down_5,
    sfx_game_block_up_1,
    sfx_game_block_up_2,
    sfx_game_block_up_3,
    sfx_game_block_up_4,
    sfx_game_block_up_5,
    sfx_game_claim_booster,
    sfx_game_item_correct,
    sfx_game_lose,
    sfx_game_transfer,
    sfx_game_win,
    sfx_ui_button_click,
    sfx_ui_coin,
    sfx_ui_coin_claim,
    sfx_ui_energy_claim,
    sfx_ui_energy_fly,
    sfx_ui_popup_disappear,
    sfx_ui_popup_show,
    sfx_ui_rewards_claim,
    sfx_ui_star,
    sfx_ui_star_claim,
    sfx_ui_upgrade,
    sfx_game_block_add,
    sfx_game_booster_broom,
    sfx_game_booster_magnet,
    sfx_game_booster_wand,
    sfx_game_level_complete
}

public static class SoundTypeUtils
{
    public static SoundType GetBlockUp(int index)
    {
        int soundIndex = (int)SoundType.sfx_game_block_up_1 + index % 5;
        return (SoundType)soundIndex;
    }

    public static SoundType GetBlockDown(int index)
    {
        int soundIndex = (int)SoundType.sfx_game_block_down_1 + index % 5;
        return (SoundType)soundIndex;
    }
}
