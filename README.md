Multi Scene Manager
=========================== 

The idea of this project is to improve Unity's Multi-Scene editing workflow.

Unity is currently lacking support for saving a group of scenes as a unit, so that you can later load the full group in one step.

The plan is to support:

* Favorites: Being able to name a group of scenes and be able to load them all with one click (Normal or Additive)
* Favorites should be able to inherit another Favorite so you can make a specialized version that has some common group of scenes
	* For example (common "Gameplay" Favorite with "Level1" specialization)
		* "Gameplay" Favorite
			* Gameplay Logic scene
			* Hud scene
		* "Level 1" Favorite
			* inherits from "Gameplay" Favorite
			* adds the Level1 content scene

* It should be easy to have different contexts for favorites, project favorites, developer favorites, test favorites
* It should be easy to manage the favorites definitions

# Right now this is just super temporal barebones work in progress