# Built-in Views

The game's MVVM system has built in support for these Views, which have their own ViewModel pre-defined. You can use these as "controls" in larger UIs:

* [ButtonView](/Assets/Scripts/_ShinyUnity/UI/Views/ButtonView.cs)
  * Binds to label string and an OnClick action
* [StringView](/Assets/Scripts/_ShinyUnity/UI/Views/StringView.cs)
  * Binds to a string 
* [ImageView](/Assets/Scripts/_ShinyUnity/UI/Views/ImageView.cs)
  * Binds to a Sprite asset
* [ListView](/Assets/Scripts/_ShinyUnity/UI/Views/ListView.cs)
  * Binds to an ObservableCollection, and takes a ViewModel type and a View prefab set in the inspector for its items
  * Automatically handles populating/adding/removing item views when the backing ObservableCollection is changed
* [MessageBoxView](/Assets/Scripts/_ShinyUnity/UI/Views/MessageBoxView.cs)
  * Binds to a title and message string, and an OK and Cancel ButtonViewModel (each having a label string and OnClick action)
* [SliderView](/Assets/Scripts/_ShinyUnity/UI/Views/SliderView.cs)
  * Binds to a float value to drive the UGUI slider component
