# OverhealthMod
[English](README.md) | **Português brasileiro**

OverhealthMod é um mod de Terraria feito para o tModLoader que introduz uma mecânica de **sobrevida**, permitindo a vida atual de um jogador (`statLife`) ultrapassar a sua vida máxima (`statLifeMax2`), mostrando ela como uma sobrevida temporária, que é perdida (ex: `480/400 HP`).

---

## Como funciona

Em seu núcleo, **esse mod funciona removendo as verificações e as definições de limite de vida** (como `player.statLife < player.statLifeMax2`, `player.statLife = player.statLifeMax2`) ao longo do código do jogo.

### Limitações

1. **Porque NÃO modificar `statLifeMax2` diretamente:**
   - **Escala e balanceamento imprecisos.** Muitos sistemas, items e accessórios no Terraria vanilla e em outros grandes mods escalam suas estatísticas baseado no `statLifeMax2`. Mudar esse valor quebraria essas mecânicas de escala.
   - **Renderização da interface indesejado.** Se aumentarmos `statLifeMax2`, a interface iria simplesmente mostrar a vida base do jogador como maior em vez de representá-la como uma *sobrevida* temporária acima da verdadeira vida máxima deles (ex: sem a distinção `480/400 HP`, ela seria `480/480 HP`).
   - **Problemas na regeneração de vida.** A regeneração de vida natural aumenta nativamente e depende da vida máxima, e ela não deve regenerar o jogador depois de seu limite de vida verdadeiro.
  
2. **Porque NÃO usar uma variável de sobrevida customizada e interceptar todos os eventos de dano/regeneração.**
   - **Um inferno para suportar outros mods.** Tentar pegar, interceptar e reescrever cada dano, regeneração, roubo de vida e efeito de projéteis é uma tortura para desenvolvedores e cria problemas. 

### A Solução

A melhor forma para permitir a vida do jogador passar do máximo é **remover os próprios limites de vida**.
- **Preserve health regeneration caps**, ensuring natural life regen still respects the standard max health cap (`statLifeMax2`) and not resets overhealth.
- For other healing methods (potions, lifesteal, special projectile/armor heals), we remove the standard `if (... statLife > statLifeMax2 ...)` and `statLife = statLifeMax2;` caps via IL editing (check out common IL edits in **`CommonIL.cs`**).
- A passive decay system gradually drains the overhealth back down to the player's max health over time.
- No netcode, health sync is handled vanilla way.
- All custom rendering is handled in **`OverhealthUI.cs`**.

## File Structure

- **`OverhealthPlayer.cs`**: Tracks individual player overhealth state, calculates and applies passive decay rates, and hooks into core player updates.
- **`OverhealthUI.cs`**: Handles drawing/rendering the overhealth indicator on the player's health bar.
- **`Utils/`**:
  - **`QuickIL.cs`**: A utility helper class that wraps method editing via `MonoModHooks.Modify` for one-line method hooks.
  - **`CommonIL.cs`**: Contains shared IL manipulation methods to find, remove, or replace vanilla health cap checks.
- **`Common/Crossmod/`**: Contains crossmod compatibility classes (e.g., `ThoriumCrossmodSystem.cs`) that apply IL edits to other mods' custom healing cap and clamping behaviors.

## How to contribute

To add support for a new mod:

1. Decompile the target mod to find references to `statLife` and `statLifeMax2` where health caps or assignments are checked (you can easily do it in [dnSpy](https://github.com/dnSpyEx/dnSpy)).
2. Create a new crossmod system class under the `Common/Crossmod/` directory, marked with the target mod's `[ExtendsFromMod("ModName")]` attribute.
3. Register IL edits using `QuickIL.EditMethod` to remove mod-specific health caps. You can use the common IL edits defined in `CommonIL.cs` for standard instructions.
4. Update project files:
   - Update `build.txt` to include the target mod in `weakReferences` if appropriate.
   - Reference the mod in `OverhealthMod.csproj` for compilation if necessary.
   - Update the description compatability table in `description_workshop.txt`.
