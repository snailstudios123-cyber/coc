═══════════════════════════════════════════════════════════════
  STORY PROGRESSION SYSTEM - COMPLETE DOCUMENTATION
═══════════════════════════════════════════════════════════════

CREATED BY: Bezi AI Assistant
FOR: Metroidvania Tutorial Project
UNITY VERSION: 2021.3

═══════════════════════════════════════════════════════════════
  OVERVIEW
═══════════════════════════════════════════════════════════════

This story progression system implements your complete tutorial
sequence as described:

1. Wake up in bedroom
2. Grandpa calls you downstairs
3. Talk to Grandpa, receive instructions
4. Complete 3 chores (combat + spell training)
5. Travel to outer village ruins
6. Witness Botanica statue activation
7. Fall through breaking ground
8. Meet Botanica's astral projection
9. Receive vine spell
10. Explore underground ruins
11. Enter boss room (door locks)
12. Defeat boss, gain mana storage
13. Botanica appears again
14. Get teleported back to village

═══════════════════════════════════════════════════════════════
  FILES CREATED
═══════════════════════════════════════════════════════════════

CORE SYSTEMS:
─────────────
✓ StoryProgressionManager.cs     - Tracks story state & progress
✓ SceneTransitionManager.cs      - Handles scene transitions
✓ DialogueManager.cs              - Displays dialogue on screen

TRIGGER SYSTEMS:
────────────────
✓ GrandpaDialogueTrigger.cs      - Grandpa call & dialogue
✓ ChoreObject.cs                  - Individual chore objects
✓ OuterRuinsTrigger.cs            - Transition to ruins
✓ StatueTrigger.cs                - Statue activation sequence
✓ BotanicaAstralProjection.cs    - Botanica appearances
✓ BossRoomDoor.cs                 - Boss room door control
✓ BossDefeatedHandler.cs          - Post-boss events

UTILITIES:
──────────
✓ StoryDebugUI.cs                 - Debug overlay (F1/F2/F3)
✓ QuickStorySetup.cs              - Editor quick setup tool

DOCUMENTATION:
──────────────
✓ StorySetupGuide.txt             - Step-by-step setup guide
✓ StoryFlowReference.txt          - Flow chart & reference
✓ STORY_SYSTEM_README.txt         - This file

UPDATED FILES:
──────────────
✓ BossController.cs               - Added story integration

═══════════════════════════════════════════════════════════════
  QUICK START GUIDE
═══════════════════════════════════════════════════════════════

STEP 1: USE THE QUICK SETUP TOOL
─────────────────────────────────
In Unity Editor:
1. Go to Tools > Story Progression > Quick Setup
2. Click "Create Story Progression Manager"
3. Click "Create Scene Transition Manager"
4. Click "Create Dialogue Manager"
5. Click "Create Story Debug UI" (optional but helpful)

STEP 2: CREATE YOUR SCENES
───────────────────────────
Create these scenes in Unity:
- Bedroom.unity (wake up scene)
- Downstairs.unity (grandpa scene)
- Outside.unity (chores scene)
- OuterRuins.unity (statue scene)
- UndergroundRuins.unity (Botanica scene)
- BossRoom.unity (boss fight scene)
- Village.unity (return scene)

Add ALL scenes to Build Settings (File > Build Settings)

STEP 3: SETUP EACH SCENE
─────────────────────────
Use the Quick Setup tool in each scene:
- In Bedroom: Already has manager, add trigger if needed
- In Downstairs: Create Grandpa Dialogue Trigger
- In Outside: Create 3 Chore Objects
- In OuterRuins: Create Statue Trigger
- In UndergroundRuins: Create Botanica Projection
- In BossRoom: Create Boss Room Door + Boss Defeated Handler

STEP 4: CONFIGURE IN INSPECTOR
───────────────────────────────
For each component you created:
- Set scene names (exact match with your scenes)
- Set spawn positions
- Write dialogue text
- Assign references

STEP 5: CREATE VINE SPELL DATA
───────────────────────────────
1. Right-click in Project > Create > Spell System > Spell Data
2. Name it "VineSpell"
3. Configure the spell properties
4. Assign to Botanica's "Vine Spell Data" field

STEP 6: TEST!
─────────────
1. Start in Bedroom scene
2. Watch console for story state changes
3. Press F1 to see debug UI overlay
4. Press F2 to reset story (if needed)
5. Press F3 to skip to next state (testing only)

═══════════════════════════════════════════════════════════════
  SYSTEM ARCHITECTURE
═══════════════════════════════════════════════════════════════

SINGLETON MANAGERS:
───────────────────
All managers use the Singleton pattern and persist across scenes.
Access them via: ManagerName.Instance

Example:
  StoryProgressionManager.Instance.SetStoryState(newState);
  DialogueManager.Instance.StartDialogue(lines, callback);
  SceneTransitionManager.Instance.TransitionToScene(name, pos);

EVENT SYSTEM:
─────────────
StoryProgressionManager provides events you can subscribe to:

  OnStoryStateChanged    - When story state changes
  OnChoreCompleted       - When a chore is completed
  OnAllChoresCompleted   - When all chores are done
  OnVineSpellReceived    - When vine spell is granted
  OnManaStorageIncreased - When mana increases

Example usage in your own scripts:
  void Start()
  {
      StoryProgressionManager.Instance.OnStoryStateChanged += 
          HandleStateChange;
  }

  void HandleStateChange(StoryProgressionManager.StoryState state)
  {
      Debug.Log($"Story changed to: {state}");
  }

STORY STATES:
─────────────
The system tracks 20 different story states in order:

1.  WakeUpInBedroom          - Initial state
2.  GrandpaCallsYouDownstairs - Transition triggered
3.  TalkingWithGrandpa        - Dialogue active
4.  DoingChores               - Chore phase active
5.  ChoresCompleted           - All chores done
6.  GoingToOuterRuins         - Traveling to ruins
7.  LookingAtStatue           - Player near statue
8.  StatueGlowing             - Visual effect phase
9.  GroundBreaking            - Earthquake effect
10. FallingUnderground        - Transition to underground
11. UndergroundRuins          - Arrived underground
12. MeetingBotanica           - First Botanica appearance
13. BotanicaGavePower         - Spell granted
14. ExploringRuins            - Free exploration
15. BossRoomEntered           - Entered boss arena
16. FightingBoss              - Combat active
17. BossDefeated              - Boss defeated
18. BotanicaFinalDialogue     - Final conversation
19. TeleportedBackToVillage   - Returned to village
20. TutorialComplete          - Tutorial finished

═══════════════════════════════════════════════════════════════
  CUSTOMIZATION
═══════════════════════════════════════════════════════════════

DIALOGUE:
─────────
Edit dialogue in the Inspector for each trigger component.
Each DialogueLine has:
- Speaker Name (string)
- Dialogue Text (multi-line string)
- Display Duration (float, seconds)

TIMING:
───────
Adjust these values in the Inspector:
- Wakeup Delay (GrandpaDialogueTrigger)
- Look At Duration (StatueTrigger)
- Glow Duration (StatueTrigger)
- Appear Duration (BotanicaAstralProjection)
- Transition Duration (SceneTransitionManager)
- Text Speed (DialogueManager)

CHORES:
───────
For each ChoreObject, set:
- Chore Name (display name)
- Chore Type (Combat/Spell/Mixed)
- Hits Required (how many hits to complete)
- Tutorial Text (hint for player)
- Completion Effect (particle effect prefab)

REWARDS:
────────
In BossDefeatedHandler:
- Mana Storage Increase (default: 0.3)

VISUAL EFFECTS:
───────────────
Assign particle effects in Inspector:
- Statue glow effect (StatueTrigger)
- Ground breaking effect (StatueTrigger)
- Botanica appear effect (BotanicaAstralProjection)
- Chore completion effect (ChoreObject)

═══════════════════════════════════════════════════════════════
  INTEGRATION WITH EXISTING SYSTEMS
═══════════════════════════════════════════════════════════════

PLAYER CONTROLLER:
──────────────────
✓ Already integrated
✓ Mana system works with rewards
✓ No changes needed

SPELL MANAGER:
──────────────
✓ Already integrated
✓ Vine spell can be added to equipped spells
✓ No changes needed

BOSS CONTROLLER:
────────────────
✓ Updated to call BossDefeatedHandler
✓ Automatically triggers story events on death
✓ No additional changes needed

PAUSE MENU:
───────────
✓ Compatible with existing PauseMenuManager
✓ Dialogue system respects pause state
✓ No changes needed

═══════════════════════════════════════════════════════════════
  DEBUG TOOLS
═══════════════════════════════════════════════════════════════

KEYBOARD SHORTCUTS (when StoryDebugUI is active):
──────────────────────────────────────────────────
F1 - Toggle debug UI visibility
F2 - Reset story progress to beginning
F3 - Skip to next story state (testing only)

CONSOLE LOGGING:
────────────────
All story events log to console automatically.
Watch for messages like:
- "Story State Changed: [state]"
- "Chore Completed: X/3"
- "[Component] completed!"

DEBUG UI DISPLAY:
─────────────────
Shows in top-left corner:
- Current story state
- Chores completed (X/3)

═══════════════════════════════════════════════════════════════
  COMMON ISSUES & SOLUTIONS
═══════════════════════════════════════════════════════════════

ISSUE: Scene won't load
SOLUTION: 
- Check scene name exactly matches in Build Settings
- Verify scene is added to Build Settings list
- Check spelling in Inspector fields

ISSUE: Dialogue doesn't appear
SOLUTION:
- Ensure DialogueManager is in the scene
- Check all UI references are assigned
- Verify Canvas is set to Screen Space - Overlay
- Make sure dialogue panel GameObject is assigned

ISSUE: Chores don't complete
SOLUTION:
- Player must have "Player" tag
- ChoreObject must have Trigger collider
- Check Hits Required value
- Verify StoryProgressionManager state is "DoingChores"

ISSUE: Statue doesn't activate
SOLUTION:
- Complete all 3 chores first
- Story state must be "GoingToOuterRuins"
- Player must enter statue's trigger zone
- Check StatueTrigger references are assigned

ISSUE: Boss defeat doesn't trigger events
SOLUTION:
- BossDefeatedHandler must be in scene
- Botanica and door references must be set
- Boss must have updated BossController.cs

ISSUE: Scene transitions are instant/no fade
SOLUTION:
- SceneTransitionManager must have FadeCanvasGroup assigned
- Canvas must be set to Don't Destroy On Load
- Check transition duration > 0

═══════════════════════════════════════════════════════════════
  EXTENDING THE SYSTEM
═══════════════════════════════════════════════════════════════

ADD NEW STORY STATES:
─────────────────────
1. Open StoryProgressionManager.cs
2. Add new state to StoryState enum
3. Call SetStoryState() when appropriate
4. Subscribe to OnStoryStateChanged if needed

ADD NEW DIALOGUE:
─────────────────
1. Add DialogueLine list to any script
2. Call DialogueManager.Instance.StartDialogue()
3. Pass callback function for completion

ADD SAVE/LOAD:
──────────────
1. Store current state index in PlayerPrefs
2. Save chores completed count
3. Restore on game start:
   int savedState = PlayerPrefs.GetInt("StoryState", 0);
   StoryProgressionManager.Instance.SetStoryState((StoryState)savedState);

ADD MORE CHORES:
────────────────
1. Change TOTAL_CHORES in StoryProgressionManager.cs
2. Add more ChoreObject instances in scene
3. System will automatically track completion

═══════════════════════════════════════════════════════════════
  PERFORMANCE NOTES
═══════════════════════════════════════════════════════════════

✓ Managers use Singleton pattern (one instance only)
✓ Event system prevents polling overhead
✓ Dialogue typing can be skipped (no wasted frames)
✓ Scene transitions use async loading
✓ All timers use Time.deltaTime (frame-rate independent)
✓ No Update() loops in trigger scripts (event-driven)

═══════════════════════════════════════════════════════════════
  SUPPORT
═══════════════════════════════════════════════════════════════

For detailed setup instructions:
→ See: StorySetupGuide.txt

For flow charts and reference:
→ See: StoryFlowReference.txt

For quick object creation:
→ Use: Tools > Story Progression > Quick Setup

═══════════════════════════════════════════════════════════════
  FINAL NOTES
═══════════════════════════════════════════════════════════════

This system is designed to be:
✓ Modular - Each component works independently
✓ Flexible - Easy to modify dialogue and timing
✓ Extensible - Add new states and events easily
✓ Debuggable - Clear logging and debug UI
✓ Maintainable - Well-documented and organized

The story progression follows your exact specifications while
remaining flexible for future changes.

All scripts follow Unity best practices and your project's
coding guidelines (self-explanatory names, comments on public
methods, constants instead of magic numbers).

Good luck with your Metroidvania game!

═══════════════════════════════════════════════════════════════
