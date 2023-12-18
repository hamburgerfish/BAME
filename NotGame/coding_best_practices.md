# Coding rules
- when naming variables, functions, gameObjects, folders, always use camel case (first letter not caps, first letter of every following word is caps, no spaces eg. playerMovement, mainMenu)
- variables should be public for testing, debugging and fine tuning, like player speed, dash speed, enemy health. Not things like flags
- scripts will be divided by categories. Each category will have one script containing separate sections for each thing. For example player movement script will contain horizontal movement, jump, dash
- scripts, like with everything else, can only be changed in your own branches, not in dev or main. Changes will be added to dev and main using merges
- comment your code so anyone reading can understand what is happening
- any gameobject that appears in multiple instances, like the player, should be saved as a prefab in the prefab folder so any changes made to the prefab will be applied to all its instances
-