## Localization instruction ##

- Feel free to translate to your language, by making a new folder with your language code and copy-pasting all the json files into the folder.
- Please maintain all the [gold][/gold] highlight tag to provide better card/tooltip description to players. Make sure there is no breakage when you enter Compendium to check all the cards/relics/potions in-game (`unlock all` console command would be helpful).
- Refer to the existing localization examples as much as possible, to know what to translate and what to not.
- IMPORTANT - refer to the main Slay the Spire 2 game cards and use consistent wording for the same interactions.
  - For example, if you translate "Exhaust" in your language, make sure to use the exact same wording that is being used in the main StS2 game.
  - Same for "Retain", "Vigor", "Steady" (enchantment name), etc.
- Often, "Rite" keyword is translated with the same word of "Ritual", which is already existing keyword in StS2 that gives you 1 Strength per turn. Please use distinct keyword different from the "Ritual"'s translation.
- In ancients.json, DO NOT translate `THE_ARCHITECT.talk.THECURSEDMOD-THE_CURSED_MOD.0-attack` and `THE_ARCHITECT.talk.THECURSEDMOD-THE_CURSED_MOD.1-attack`. They are not for displaying, they determine who will attack in the scene. Just leave them as-is.
- Once completed, either send email to solderist.park@gmail.com or make a pull request in GitHub.
  - If you don't follow the above instructions, I may change your translation by myself without notice.