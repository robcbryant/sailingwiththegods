# UI System

## Core
* [Registry...](registry.md)
  * Stores an instance in a dictionary keyed by a class name.
* [Owner...](owner.md)
  * Tracks a bunch of different "resources" that are all cleaned up automatically when the Owner is Disposed, and are also all turned on/off together when the owner is enabled/disabled.
  * EventOwner
    * Similar to Owner, but only for events. Uses DelegateHandle.
  * DelegateHandle
    * Used by EventOwner, OwnerBehavior, ViewModel, and some project specific views and models to enable/disable/cleanup event handlers.
  * OwnerBehaviour
    * A MonoBehaviour that uses the Owner class internally to automatically enable/disable its owned objects in Unity's OnEnable/OnDisable functions, and automatically Dispose the Owner in OnDestroy.

## [MVVM System](view-model.md)
* UISystem
  * This is a top level API which provides an interface to turn Views on and off (see Generic Views section)
* ViewBehaviour
  * This is the base class for Views. It has a reference to a ViewModel, and can automatically update itself when its ViewModel changes. It uses callbacks on the ViewModel to communicates back to the game (Model) layer.
* ViewModel
  * Dispatches events when its properties changed (INotifyPropertyChanged) and contains callbacks that the View can use to report user input back to the game.
* InteractableBehaviour
  * This catches Unity's IPointer_______Event interface callbacks and turns them into UnityEvents so they can be listened to in other Views. Used on the market screen to select a cargo cell (CargoItemTradeView) even though it's not a button (so doesn't have an onClick UnityEvent).

## [Generic Views...](built-in-views.md)
These are built in "controls" that can be nested in other Views to build up a UI.

* StringView
* ButtonView
* ImageView
* MessageBoxView
* SliderView
* ListView

# [Game Specific Views...](game-views/main.md)

There's a few pieces of UI that are done in a simpler way, but most of the in game UI is built using the above MVVM Framework.

The main game UI can be broken up into these main sections:
* Dashboard (the menu button, captain's log, ship controls, etc)
* Cargo
* Crew
* Port
