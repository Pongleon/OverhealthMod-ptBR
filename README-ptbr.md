# OverhealthMod
[English](README.md) | **Português brasileiro**

OverhealthMod é um mod de Terraria feito para o tModLoader que introduz uma mecânica de **sobrevida**, permitindo a vida atual de um jogador (`statLife`) ultrapassar a sua vida máxima (`statLifeMax2`), mostrando ela como uma sobrevida temporária, que é perdida (ex: `480/400 HP`).

---

## Como funciona

Em seu princípio, **esse mod funciona removendo as verificações e as definições de limite de vida** (como `player.statLife < player.statLifeMax2`, `player.statLife = player.statLifeMax2`) ao longo do código do jogo.

### Limitações

1. **Porque NÃO modificar `statLifeMax2` diretamente:**
   - **Escala e balanceamento imprecisos.** Muitos sistemas, items e accessórios no Terraria vanilla e em outros grandes mods escalam suas estatísticas baseado no `statLifeMax2`. Mudar esse valor quebraria essas mecânicas de escala.
   - **Renderização da interface indesejado.** Se aumentarmos `statLifeMax2`, a interface iria simplesmente mostrar a vida base do jogador como maior em vez de representá-la como uma *sobrevida* temporária acima da verdadeira vida máxima deles (ex: sem a distinção `480/400 HP`, ela seria `480/480 HP`).
   - **Problemas na regeneração de vida.** A regeneração de vida natural aumenta nativamente e depende da vida máxima, e ela não deve regenerar o jogador depois de seu limite de vida verdadeiro.
  
2. **Porque NÃO usar uma variável de sobrevida customizada e interceptar todos os eventos de dano/regeneração.**
   - **Um inferno para suportar outros mods.** Tentar pegar, interceptar e reescrever cada dano, regeneração, roubo de vida e efeito de projéteis é uma tortura para desenvolvedores e cria problemas. 

### A Solução

A melhor forma para permitir a vida do jogador passar do máximo é **remover os próprios limites de vida**.
- **Preservar limites de regeneração de vida**, garantindo que a regeneração natural ainda respeite o limite de vida máxima padrão (`statLifeMax2`) e não redefine a sobrevida.
- Para outros métodos de cura (poções, roubo de vida, curas de projéteis/armaduras especiais), nós removemos os limites `if (... statLife > statLifeMax2 ...)` e `statLife = statLifeMax2;` padrões por meio da edição IL (olhe as edições do common IL no **`CommonIL.cs`**).
- Um sistema passivo de perda drena gradualmente a sobrevida de volta para a vida máxima do jogador com o tempo.
- Sem netcode, a sincronização da vida é controlada de forma vanilla.
- Toda a renderização customizada é controlada em **`OverhealthUI.cs`**.

## Estrutura de Arquivos

- **`OverhealthPlayer.cs`**: Monitora o estado de sobrevida do jogador individual, calcula e aplica índices de perda e conecta nas atualizações principais do jogador.
- **`OverhealthUI.cs`**: Controla a renderização do indicador de sobrevida na barra de vida do jogador.
- **`Utils/`**:
  - **`QuickIL.cs`**: Uma classe utilitária que envolve edição de métodos via `MonoModHooks.Modify` para ganchos de método de uma única linha.
  - **`CommonIL.cs`**: Contém métodos de manipulação IL compartilhados para achar, remover ou substituir as verificações do limite de vida vanilla.
- **`Common/Crossmod/`**: Contém classes de compatibilidade crossmod (ex: `ThoriumCrossmodSystem.cs`) que aplicam edições IL para limites de vida customizados e comportamentos de fixação de outros mods.

## Como contribuir

Para adicionar suporte à um novo mod:

1. Decompile o mod-alvo para achar referências de `statLife` e `statLifeMax2` onde limites de vida ou tarefas são verificadas (você pode fazer isso facilmente com o [dnSpy](https://github.com/dnSpyEx/dnSpy)).
2. Crie uma nova classe crossmod system dentro do diretório `Common/Crossmod/`, marcado com o atributo `[ExtendsFromMod("NomeDoMod")] do mod-alvo.
3. Registre edições IL usando `QuickIL.EditMethod` para remover limites de vida específicos de mods. Você pode usar as edições common IL definidas em `CommonIL.cs` para instruções padrões.
4. Atualize arquivos do projeto:
   - Atualize `build.txt` para incluir o mod-alvo em `weakReferences` se apropriado.
   - Referencie o mod em `OverhealthMod.csproj` para compilação se necessário.
   - Atualize a descrição de compatibilidade em `description_workshop.txt`.
