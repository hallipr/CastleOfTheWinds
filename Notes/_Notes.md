# External Sites

## Other rewrites
- https://github.com/mordrax/cotwelm

## Notes on the original game
- Wiki - https://en.wikibooks.org/wiki/Castle_of_the_Winds
- Creatures - http://lkbm.ecritters.biz/cotw/enemies.html
- Battle Mechanics - https://strategywiki.org/wiki/Castle_of_the_Winds/Battle_Mechanics
- Tables - http://web.archive.org/web/20200920070928/http://www.cheatbook.de/files/castlewinds.htm
- Cheats/Bugs? - http://web.archive.org/web/20210722090015/http://lkbm.ecritters.biz/cotw/
- Forum - https://www.tapatalk.com/groups/cotwfr/
- Tables - http://web.archive.org/web/20050829194230/http://vengeance.et.tudelft.nl/cow/


## Save file
- Save File general: http://web.archive.org/web/20001005051616/http://www.csonline.net:80/pagint/cowsvg.htm
- Item hex codes: http://web.archive.org/web/20001031194644/http://www.csonline.net/pagint/itemnum.txt
- Save file hacking: https://strategywiki.org/wiki/Castle_of_the_Winds/Hacks
- structure: https://www.tapatalk.com/groups/cotwfr/viewtopic.php?p=1343#p1343

# Open questions



## At higher character levels, some spells cost less than their normal amount of mana, for example cold bolt costing only 1 mana at level 8. What is the exact formula for this reduction?

> Formula is
  (InitialSpellCost * 3 / 2) - (PlayerLevel / 4)
  Math is done with integer values so for the division and such any remainder is truncated and if the resulting value is less than 1 it's set to one. The calculation is performed when learning the spell and stored. I think I posted a list of the base spell costs elsewhere.

https://www.tapatalk.com/groups/cotwfr/viewtopic.php?p=1514#p1514


> Damage is calculated as such
>   1) Initialize the damage to the number of dice plus the damage bonus of the weapon
>   2) Loop over the number of dice and add a random amount up to the die type to the damage
>   3) Add any to damage modifiers to the damage
>   4) If the damage is less than 0 set it to 1
> 
> There's some bit before 4 about looping over player special attacks and adding that to the damage.  It's
> referencing the monster table and making a call to some monster function.  Was there a polymorph spell in Castle of
> the Winds?
> 
> The to damage modifier is increased by the gauntlets of slaying and strength bonuses.
> For every 4 points of strength above 60, to damage is increased by one.  So at 64 it's +1, 68 it's +2, etc.
> For every 4 points of strength below 32, to damage is reduced by one.  So at 28 it's -1, 24 it's -2, etc.

## New Character Stats
Gil:
> Can anyone explain to me the range of the attributes strength, dexterity, intelligence and constitution, and how far you're allowed to set them at the beginning?

Theodis: 
>All defaulted to 45 points with a max of 100 to spend. Minimum allowed value is 20 and max is 75. Bar range visually ranges from 0 to 100.

https://strategywiki.org/wiki/Castle_of_the_Winds/Character_Attributes

```
Every player starts Part I at Level 1, with 0 experience.
20 experience points are needed to reach Level 2.
For n > 2, the experience requirement for Level n are those for Level n – 1 plus 20 points at Easy difficulty, 40 at Intermediate, 60 at Difficult, and 80 at Experts Only.
The maximum level, 30, requires 10,695,475,180 xp at Easy, 16,106,127,320 at Intermediate, 21,558,722,500 at Difficult and 27,011,317,680 at Experts Only.
Experience rewards for killing monsters range from 1 xp for a Giant Rat or Goblin to 344 xp for Surtur. (Reaching Level 30 by repeatedly killing Surtur would mean doing so 31,091,498, 46,820,138, 62,670,705 or 78,521,272 times, depending on the difficulty level.) Disarming a trap yields 1 to 4 xp.
```

