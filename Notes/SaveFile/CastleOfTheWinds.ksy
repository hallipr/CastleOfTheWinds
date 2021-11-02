meta:
  id: castle_of_the_winds
  file-extension: cwg
  endian: le
  bit-endian: le
seq:
  - size: 8
  - id: name_offset
    type: s4
  - id: unknown_offset_1
    type: s4
  - id: unknown_offset_2
    type: s4
  - id: shops_offset
    type: s4
  - id: unknown_offset_3
    type: s4
  - id: unknown_offset_4
    type: s4
  - id: file_end_offset
    type: s4
  - size: 92
  - id: strength
    type: s1
  - id: intelligence
    type: s1
  - id: constitution
    type: s1
  - id: dexterity
    type: s1
  - id: strength_base
    type: s1
  - id: intelligence_base
    type: s1
  - id: constitution_base
    type: s1
  - id: dexterity_base
    type: s1
  - id: strength_penalty
    type: s1
  - id: intelligence_penalty
    type: s1
  - id: constitution_penalty
    type: s1
  - id: dexterity_penalty
    type: s1
  - size: 4
  - id: strength_bonus
    type: s1
  - id: intelligence_bonus
    type: s1
  - id: constitution_bonus
    type: s1
  - id: dexterity_bonus
    type: s1
  - id: hitpoints
    type: s2
  - id: hitpoints_max
    type: s2
  - id: mana
    type: s2
  - id: mana_max
    type: s2
  - id: level
    type: s2
  - id: experience
    type: u4
  - id: experience_max
    type: u4
  - id: armor
    type: s2
  - id: to_hit
    type: s2
  - id: to_damage
    type: s2
  - id: speed
    type: s2
  - id: speed_base
    type: s2
  - id: speed_burden
    type: s2
  - id: game_version
    type: s2
  - id: player_position
    type: position
  - id: player_position_prev
    type: position
  - size: 2
  - id: gender
    type: u1
  - size: 12
  - id: story_progression
    type: story_progression
  - size: 42
  - id:  player_name
    type: str
    encoding: ascii
    terminator: 0
    size: 80
  - id: resist_fire
    type: s2
  - id: resist_cold
    type: s2
  - id: resist_lightning
    type: s2
  - id: resist_acid
    type: s2
  - id: resist_fear
    type: s2
  - id: resist_drain_life
    type: s2
  - size: 83
  - id: spells
    type: spell_book
  - size: 47
  - id: spell_slots
    type: u2
    repeat: expr
    repeat-expr: 10
  - size: 92
  - id: weight
    type: s4
  - id: bulk
    type: s4
  - id: weight_max
    type: s4
  - id: bulk_max
    type: s4
  - size: 8
  - id: inventory
    type: inventory
  - id: current_map_id
    type: s2
types:
  inventory:
    seq:
    - id: slot_count
      type: u2
    - id: allocated_slot_count
      type: u2
    - id: slots
      type: slot
      repeat: expr
      repeat-expr: allocated_slot_count
  shop:
    seq:
    - id: name_length
      type: u1
    - id: name
      type: str
      encoding: ascii
      size: name_length
    - size: 36
    - id: slot_count
      type: u2
    - id: allocated_slot_count
      type: u2
    - id: slots
      type: slot
      repeat: expr
      repeat-expr: allocated_slot_count
    - id: items
      type: item
      repeat: expr
      repeat-expr: allocated_slot_count
  slot:
    seq: 
    - id: required_type
      type: s2
    - id: required_subtype
      type: s1
    - id: id
      type: u2
  item:
    seq: 
    - id: type
      type: u1
    - id: subtype
      type: u1
    - id: value
      type: u4
    - id: name_offset
      type: u2
    - id: wield_actions
      type: b1
    - id: unwield_actions
      type: b1
    - id: activate_actions
      type: b1
    - id: use_actions
      type: b1
    - id: delete_on_activate
      type: b1
    - id: charged
      type: b1
    - id: identify
      type: b2
    - id: identitied
      type: b1
    - id: enchantment 
      type: b2
    - id: multiple
      type: b1
    - id: has_fixed_weight
      type: b1
    - id: has_fixed_bulk
      type: b1
    - id: no_expand
      type: b1
    - id: objlist
      type: b1
    - id: attr_count
      type: b4
      if: objlist == false
    - id: attr_alloc_count
      type: b4
      if: objlist == false
    - id: description_type
      type: b3
      if: objlist == false
    - id: unknown_0
      type: b5
      if: objlist == false
    - id: attributes
      type: item_attribute
      repeat: expr
      repeat-expr: attr_alloc_count
      if: objlist == false
    - id: handle_to_parent_object_list
      type: u2
      if: objlist == true
    - id: weight
      type: s4
      if: objlist == true
    - id: bulk
      type: s4
      if: objlist == true
    - id: weight_max
      type: s4
      if: objlist == true
    - id: bulk_max
      type: s4
      if: objlist == true
    - id: weight_fixed
      type: s4
      if: objlist == true
    - id: bulk_fixed
      type: s4
      if: objlist == true
    - id: slot_count
      type: u2
      if: objlist == true
    - id: allocated_slot_count
      type: u2
      if: objlist == true
    - id: item_slots
      type: slot
      if: objlist == true
      repeat: expr
      repeat-expr: allocated_slot_count
    - id: items
      type: item
      if: objlist == true
      repeat: expr
      repeat-expr: allocated_slot_count
  item_attribute:
    seq:
    - id: attrib
      type: b10
    - id: wield
      type: b1
    - id: unwield
      type: b1
    - id: activate
      type: b1
    - id: use
      type: b1
    - id: time_activate
      type: b1
    - id: fuse
      type: b1
    - id: count
      type: b14
    - id: countx
      type: b2
    - id: w_param
      size: 2
    - id: l_param
      size: 4
  position:
    seq:
    - id: y
      type: u1
    - id: x
      type: u1
  story_progression:
    seq:
    - id: burining_farm
      type: u1
    - id: parchment
      type: u1
    - id: burining_hamlet
      type: u1
    - id: page_gap_0
      type: u1
    - id: patrol_orders
      type: u1
    - id: bandit_chest
      type: u1
    - id: hrugnir_first
      type: u1
    - id: page_gap_1
      type: u1
    - id: hrugnir_spell
      type: u1
    - id: hrugnir_final
      type: u1
    - id: page_gap_2
      type: u1
    - id: father_speach
      type: u1
    - id: page_gap_3
      type: u1
    - id: page_gap_4
      type: u1
    - id: end_game_1
      type: u1
  spell_book:
    seq:
    - id: heal_minor_wounds
      type: spell
    - id: detect_objects
      type: spell
    - id: light
      type: spell
    - id: magic_arrow
      type: spell
    - id: phase_door
      type: spell
    - id: shield
      type: spell
    - id: clairvoyance
      type: spell
    - id: cold_bolt
      type: spell
    - id: detect_monsters
      type: spell
    - id: detect_traps
      type: spell
    - id: identify
      type: spell
    - id: levitation
      type: spell
    - id: neutralize_poison
      type: spell
    - id: cold_ball
      type: spell
    - id: heal_med_wounds
      type: spell
    - id: fire_bolt
      type: spell
    - id: lightning_bolt
      type: spell
    - id: remove_curse
      type: spell
    - id: resist_fire
      type: spell
    - id: resist_cold
      type: spell
    - id: resist_lightning
      type: spell
    - id: resist_acid
      type: spell
    - id: resist_fear
      type: spell
    - id: sleep_monster
      type: spell
    - id: slow_monster
      type: spell
    - id: teleport
      type: spell
    - id: rune_of_return
      type: spell
    - id: heal_major_wounds
      type: spell
    - id: fireball
      type: spell
    - id: ball_lightning
      type: spell
    - id: healing
      type: spell
    - id: trans_monster
      type: spell
    - id: create_traps
      type: spell
    - id: haste_monster
      type: spell
    - id: teleport_away
      type: spell
    - id: clone_monster
      type: spell
  spell:
    seq:
    - id: level
      type: s1
    - size: 1
    - id: learned_cost
      type: s1
    - id: cost
      type: s1
    - id: category
      type: s1
    - id: cast_time
      type: s2
    - size: 5
