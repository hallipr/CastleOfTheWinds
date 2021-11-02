
# Initial stats

Characters start with 25 points in each of Strength, Intelligence, Constitution, and Dexterity.
You get 100 points to add across those stats.
Each stat has a starting range of 20 to 72.

From those stats, max weight, starting HP and starting Mana are derived using a standard pattern:
```cs
if (BaseStat < WeakAt)
  // The lower you go below WeakAt, the lower the derived stat's value
  DerivedStat = BaseAmount - (WeakAt - BaseStat) * Reduction
else if (Strength > StrongAt)
  // The higher you go above StrongAt, the higher the derived stat's value
  DerivedStat = BaseAmount + (BaseStat - StrongAt) * Increase
else
  DerivedStat = BaseAmount
```

| Derived Stat | Base Stat    | Base  | Weak At | Strong At | Weak Reduction | Strong Increase |
|--------------|--------------|-------|--------:|----------:|---------------:|----------------:|
| MaxWeight    | Strength     | 25000 |      32 |        48 |            500 |            1000 |
| HP           | Constitution | 10    |      32 |        59 |           0.25 |            0.25 |
| Mana         | Intelligence | 5     |      32 |        66 |           0.25 |            0.33 |

# Levels
| Level |        Easy | Intermediate |   Difficult |      Expert |
|------:|------------:|-------------:|------------:|------------:|
|     1 |           0 |            0 |           0 |           0 |
|     2 |          10 |           20 |          30 |          40 |
|     3 |          50 |           80 |         110 |         140 |
|     4 |         130 |          200 |         270 |         340 |
|     5 |         290 |          440 |         590 |         740 |
|     6 |         610 |          920 |        1230 |        1540 |
|     7 |        1250 |         1880 |        2510 |        3140 |
|     8 |        2530 |         3800 |        5070 |        6340 |
|     9 |        5090 |         7640 |       10190 |       12740 |
|    10 |       10210 |        15320 |       20430 |       25540 |
|    11 |       20450 |        30680 |       40910 |       51140 |
|    12 |       40930 |        61400 |       81870 |      102340 |
|    13 |       81890 |       122840 |      163790 |      204740 |
|    14 |      163810 |       245720 |      327630 |      409540 |
|    15 |      327650 |       491480 |      655310 |      819140 |
|    16 |      655330 |       983000 |     1310670 |     1638340 |
|    17 |     1310690 |      1966040 |     2621390 |     3276740 |
|    18 |     2621410 |      3932120 |     5242830 |     6553540 |
|    19 |     5242850 |      7864280 |    10485710 |    13107140 |
|    20 |    10485730 |     15728600 |    20971470 |    26214340 |
|    21 |    20971490 |     31457240 |    41942990 |    52428740 |
|    22 |    41943010 |     62914520 |    83886030 |   104857540 |
|    23 |    83886050 |    125829080 |   167772110 |   209715140 |
|    24 |   167772130 |    251658200 |   335544270 |   419430340 |
|    25 |   335544290 |    503316440 |   671088590 |   838860740 |
|    26 |   671088610 |   1006632920 |  1342177230 |  1677721540 |
|    27 |  1342177250 |   2013265880 |  2684354510 |  3355443140 |
|    28 |  2684354530 |   4026531800 |  5368709070 |  6710886340 |
|    29 |  5368709090 |   8053063640 | 10737418190 | 13421772740 |
|    30 | 10737418210 |  16106127320 | 21474836430 | 26843545540 |

## Formula
There are 4 difficulty levels, each with a different experience-to-level table. Each difficulty requires a different amount of experience for level 1, but there's a formula for all the other levels.

```
(2^(N-2)-1)*(2X+20)+X
```

where N = Level, and X = Base (Level 1 experience)

## Bases
|              |    |
|--------------|---:|
| Easy         | 10 |
| Intermediate | 20 |
| Difficult    | 30 |
| Expert       | 40 |


## Examples
For Easy difficulty, X == 10

At level 2:
```
(2^(N-2)-1)*(2X+20)+X
(2^(2-2)-1)*40+10
(2^0-1)*40+10
(1-1)*40+10
0*40+10
10
```

At level 3:
```
(2^(N-2)-1)*(2X+20)+X
(2^(3-2)-1)*40+10
(2^1-1)*40+10
(2-1)*40+10
1*40+10
40+10
50
```

At level 8:
```
(2^(N-2)-1)*(2X+20)+X
(2^(8-2)-1)*40+10
(2^6-1)*40+10
(64-1)*40+10
63*40+10
2520+10
2530
```

# Difficulty
> Difficulty affects
> - Score position in high score list
> - Number of objects randomly generated on levels
> - Number of monsters generated on levels
> - Number of traps generated on levels
> - Special rooms contain more monsters on harder difficulties
> - Exp requirements
> - Enemies have a better chance to hit you on higher difficulty levels
> - Less exp from killing stuff on harder difficulty levels
> - Reduced chance to hit for the player on higher difficulty levels
> - Less treasure from enemies on harder difficulty levels
> - You sell stuff to shops for less money on harder difficulty levels   (This doesn't seem to be correct)
> - Cost increase in shops at harder difficulties   (This doesn't seem to be correct)

## How is HP and mana increase determined when you level up
> You gain 3+d5+(con modifier) HP<br/>
  You gain 1+d4+(int modifier) MP<br/>
  The modifiers will be zero if your CON/INT is anywhere near the midrange, but can go as low as -3 and as high as 4.75 (CON) or 2.25 (INT).

https://www.tapatalk.com/groups/cotwfr/how-is-hp-and-mana-increase-determined-when-you-le-t133.html

```
//For the purpose of this psuedo code rand() returns a random number
//between the two parameters including the parameters in the range
hpGain = rand(4,8)
manaGain = rand(2,5)

if(con > 56) hpGain += (con - 56) / 4
else if(con < 32) hpGain -= (32 - con) / 4
if(hpGain < 1) hpGain = 1

if(int > 64) manaGain += (int - 64) / 3
else if(int < 32) manaGain -= (32 - int) / 4
if(manaGain < 1) manaGain = 1

The game does not keep track of gains at level up so if you lose a level the losses are also random. Oddly enough the loss ranges aren't quite the same.
hpLoss = rand(4,
manaLoss = rand(1,4)
if(con > 56) hpLoss += (con - 56) / 4
if(int > 64) manaLoss += (int - 64) / 4

So from what it looks like you have a smaller mana loss random range on deleveling than the gain on a level. So if you level and delevel your mana is likely to go up especially with a high int since the gain is divided by 3 for high int and the loss is divided by 4 on high int. Probably an artifact from the author changing the gains but forgetting to update the losses .
And for the stat bonuses. factor is a param that is either 1, or -1 depending on if the game is (sic)
str > 60 : todam += (str - 60) / 4
str < 32 : todam -= (32 - str) / 4

dex > 56 : ac += dex - 56
dex < 32 : tohit -= 32-dex
dex > 60 : tohit += dex - 60
dex < 32 : ac -= 32-dex

base weight max = 25000
str < 32 : weightMax -= (32 - str) * 500;
str > 48 : weightMax += (str - 48) * 1000;
weight < weightMax/2 : movespeed = 50;
weight >= weightMax/2 : movespeed = 50 + (weight - weight     

//  maybe it was + (weight - weightMax/2)?
```