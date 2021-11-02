# Spells

## Attack

| name                 | level | mana cost | cast time | potion image | wand image | staff image | potion type | Offer | description |
|----------------------|------:|----------:|----------:|--------------|------------|-------------|-------------|------:|-------------|
| Magic Arrow          |     1 |         1 |         5 |              | wand       |             |             |       |             |
| Cold Bolt            |     2 |         2 |         5 |              |            |             |             |       |             |
| Lightning Bolt       |     3 |         3 |         5 |              |            |             |             |       |             |
| Fire Bolt            |     3 |         3 |         5 |              |            |             |             |   720 |             |
| Cold Ball            |     3 |         4 |         5 |              |            |             |             |       |             |
| Ball Lightning       |     4 |         4 |         5 |              |            |             |             |       |             |
| Fireball             |     4 |         5 |         5 |              |            |             |             |       |             |
| Sleep Monster        |     3 |         4 |         5 |              |            |             |             |  2160 |             |
| Slow Monster         |     3 |         4 |         5 |              |            |             |             |       |             |
| Transmogrify Monster |     5 |         6 |         5 |              |            |             |             |       |             |

## Defense

| name             | level | mana cost | cast time | potion image | wand image | staff image | potion type | Offer | description |
|------------------|------:|----------:|----------:|--------------|------------|-------------|-------------|------:|-------------|
| Shield           |     1 |         1 |         5 |              |            |             |             |       |             |
| Resist Cold      |     3 |         3 |         5 |              |            |             |             |       |             |
| Resist Lightning |     3 |         3 |         5 |              |            |             |             |       |             |
| Resist Fire      |     3 |         3 |         5 |              |            |             |             |   900 |             |

## Healing

| name               | level | mana cost | cast time | potion image      | wand image | staff image  | potion type | Offer | description                                               |
|--------------------|------:|----------:|----------:|-------------------|------------|--------------|-------------|------:|-----------------------------------------------------------|
| Heal Minor Wounds  |     1 |         1 |         5 | potion_heal_minor |            |              | Elixir      |   240 | heals a small amount of the damage a character has taken. |
| Heal Medium Wounds |     3 |         3 |         5 |                   |            | staff_blue_1 |             |       |                                                           |
| Heal Major Wounds  |     4 |         5 |         5 |                   |            |              |             |       |                                                           |
| Neutralize Poison  |     2 |         3 |         5 | potion            |            |              | Elixir      |   900 | neutralize poison in the character                        |
| Healing            |     5 |         6 |         5 |                   |            |              |             |       |                                                           |

## Movement

| name           | level | mana cost | cast time | potion image | wand image | staff image | potion type | Offer | description                                        |
|----------------|------:|----------:|----------:|--------------|------------|-------------|-------------|------:|----------------------------------------------------|
| Phase Door     |     1 |         1 |         5 |              |            |             |             |       |                                                    |
| Levitation     |     2 |         2 |         5 | potion       |            |             | Potion      |   240 | makes the character levitate, for a period of time |
| Rune Of Return |     3 |         3 |         5 |              |            |             |             |       |                                                    |
| Teleport       |     3 |         3 |         5 |              |            |             |             |       |                                                    |

## Divination

| name            | level | mana cost | cast time | potion image  | wand image | staff image | potion type | Offer | description                                                  |
|-----------------|------:|----------:|----------:|---------------|------------|-------------|-------------|------:|--------------------------------------------------------------|
| Detect Objects  |     1 |         1 |        30 | potion_detect |            |             | Drought     |   600 | allows the character to detect objects on the current level. |
| Detect Monsters |     2 |         2 |        30 | potion_detect |            |             | Drought     |   600 |                                                              |
| Detect Traps    |     2 |         2 |        30 | potion_detect |            |             | Drought     |   600 | allows the character to detect traps close by.               |
| Identify        |     2 |         2 |        60 |               |            |             |             |       |                                                              |
| Clairvoyance    |     2 |         3 |        30 |               |            |             |             |   900 |                                                              |
| Map Quadrant    |     2 |         3 |        30 |               |            |             |             |   900 | fills the characters map of the current quadrant             |

## Miscellaneous

| name         | level | mana cost | cast time | potion image | wand image | staff image | potion type | Offer | description |
|--------------|------:|----------:|----------:|--------------|------------|-------------|-------------|------:|-------------|
| Light        |     1 |         1 |         5 |              | wand_light |             |             |   240 |             |
| Remove Curse |     3 |         3 |        60 |              |            |             |             |       |             |

## Non-character

| name          | level | mana cost | cast time | potion image | wand image | staff image | potion type | Offer | description |
|---------------|------:|----------:|----------:|--------------|------------|-------------|-------------|------:|-------------|
| Haste Monster |    NA |        NA |        NA |              |            |             |             |       |             |
| Clone Monster |    NA |        NA |        NA |              |            |             |             |       |             |
| Teleport Away |    NA |        NA |        NA |              |            |             |             |       |             |
| Create Traps  |    NA |        NA |        NA |              |            |             |             |       |             |


# Notes

> Theodis - 2:57 PM - Aug 07, 2010
> Formula is
> (InitialSpellCost * 3 / 2) - (PlayerLevel / 4)
> Math is done with integer values so for the division and such any remainder is truncated and if the resulting value is less than 1 it's set to one. The calculation is performed when learning the spell and stored. I think I posted a list of the base spell costs > elsewhere.

## Save File Spells block

```
02 01 01 01 02 32 00 08 00 0B 00 02  -  HealMinorWounds
02 01 01 01 04 2C 01 10 00 00 00 A3  -  DetectObjects
02 01 01 01 05 32 00 00 00 00 00 FE  -  Light
01 01 01 01 00 32 00 00 00 03 00 01  -  MagicArrow
02 01 01 01 03 32 00 00 00 02 00 43  -  PhaseDoor
01 01 01 01 01 32 00 00 00 00 00 78  -  Shield
03 02 03 FF 04 2C 01 10 00 00 00 0A  -  Clairvoyance
02 02 02 FF 00 32 00 02 00 03 00 12  -  ColdBolt
02 02 02 01 04 2C 01 10 00 00 00 FF  -  DetectMonsters
01 02 02 FF 04 2C 01 10 00 00 00 4B  -  DetectTraps
02 02 02 FF 04 58 02 10 00 00 00 84  -  Identify
01 02 02 FF 03 32 00 00 00 00 00 A4  -  Levitation
02 02 03 FF 02 32 00 08 00 0B 00 0D  -  NeutralizePoison
02 03 04 FF 00 32 00 02 00 04 00 8B  -  ColdBall
02 03 03 FF 02 32 00 08 00 0B 00 0C  -  HealMedWounds
02 03 03 FF 00 32 00 01 00 03 00 0B  -  FireBolt
02 03 03 FF 00 32 00 04 00 11 00 8C  -  LightningBolt
02 03 03 02 05 58 02 00 00 00 00 86  -  RemoveCurse
02 03 03 FF 01 32 00 01 00 00 00 87  -  ResistFire
02 03 03 FF 01 32 00 02 00 00 00 88  -  ResistCold
02 03 03 FF 01 32 00 04 00 00 00 89  -  ResistLightning
02 03 03 FE 01 32 00 00 00 00 00 8A  -  ResistAcid
02 03 03 FE 01 32 00 00 00 00 00 31  -  ResistFear
03 03 04 FF 00 32 00 00 00 00 00 32  -  SleepMonster
03 03 04 FF 00 32 00 00 00 00 00 A6  -  SlowMonster
02 03 03 02 03 32 00 00 00 02 00 B6  -  Teleport
02 03 03 02 03 32 00 00 00 02 00 E0  -  RuneOfReturn
02 04 05 FF 02 32 00 08 00 0B 00 0F  -  HealMajorWounds
02 04 05 FF 00 32 00 01 00 04 00 0E  -  Fireball
02 04 04 FF 00 32 00 04 00 04 00 A7  -  BallLightning
01 05 06 FF 02 32 00 08 00 0B 00 36  -  Healing
03 05 06 FF 00 32 00 00 00 00 00 30  -  TransMonster
03 0A 06 FF 05 32 00 00 00 00 00 33  -  CreateTraps
03 0A 06 FF 05 32 00 00 00 00 00 34  -  HasteMonster
03 0A 06 FF 05 32 00 00 00 00 00 35  -  TeleportAway
03 0A 06 FF 05 32 00 00 00 00 00 00  -  CloneMonster
```



```
02 - Unknown
01 - byte, Spell Level
01 - sbyte, Default Mana Cost
01 - sbyte, Current Mana Cost, can't cast if negative
02 - byte, Category
32 - short, Casting time (* 0.1 sec) [32 00, 2C 01, 58 02] => [50, 300, 600]
00 - Unknown
08 - Unknown
00 - Unknown
0B - Unknown
00 - Unknown
02 - Unknown
```